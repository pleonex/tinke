// -----------------------------------------------------------------------
// <copyright file="NitroTextureCompressor.cs" company="NII">
//
//   Copyright (C) 2016 MetLob
//   Used NVidia optimization methods of end-points
//   
//      This program is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//   
//      This program is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//   
//      You should have received a copy of the GNU General Public License
//      along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// </copyright>
// -----------------------------------------------------------------------

namespace Ekona.Images
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Text;

    using Mathematics;

    /// <summary>
    /// Fast texture compressor for Nintendo DS format
    /// </summary>
    public class NitroTextureCompressor
    {
        #region Math

        private static double Clamp(double a)
        {
            if (a > 255) a = 255;
            if (a < 0) a = 0;
            return a;
        }

        #endregion

        #region Error

        private static double ColorLengthSquared(Vector3 v)
        {
            return Math.Truncate(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
        }

        private static double Error(Vector3[] inputPoints, int[] mask, Vector3 a, Vector3 b, bool hasAlpha)
        {
            Vector3[] dxtFourPointsTemp = new Vector3[4];
            dxtFourPointsTemp[0] = a;
            dxtFourPointsTemp[1] = b;

            // Calculate inside DXT colors
            if (!hasAlpha)
            {
                dxtFourPointsTemp[2] = (5 * dxtFourPointsTemp[0] + 3* dxtFourPointsTemp[1]) / 8;
                dxtFourPointsTemp[3] = (5 * dxtFourPointsTemp[1] + 3* dxtFourPointsTemp[0]) / 8;
            }
            else
            {
                dxtFourPointsTemp[2] = (dxtFourPointsTemp[0] + dxtFourPointsTemp[1]) / 2;
                dxtFourPointsTemp[3] = new Vector3(double.MaxValue, double.MaxValue, double.MaxValue);
            }

            double error = 0;
            for (int i = 0; i < inputPoints.Length; i++)
            {
                if (mask[i] > 0)
                {
                    double dist0 = ColorLengthSquared(inputPoints[i] - dxtFourPointsTemp[0]);
                    double dist1 = ColorLengthSquared(inputPoints[i] - dxtFourPointsTemp[1]);
                    double dist2 = ColorLengthSquared(inputPoints[i] - dxtFourPointsTemp[2]);
                    double dist3 = ColorLengthSquared(inputPoints[i] - dxtFourPointsTemp[3]);
                    if (dist0 < dist2 && dist0 < dist3)
                    {
                        error += dist0;
                    }
                    else if (dist1 < dist2 && dist1 < dist3)
                    {
                        error += dist1;
                    }
                    else error += Math.Min(dist2, dist3);
                }
            }

            return error;
        }

        #endregion

        #region Colors manipulations

        private static uint ColorSquaredDistance(Color c1, Color c2)
        {
            int r = c1.R - c2.R;
            int g = c1.G - c2.G;
            int b = c1.B - c2.B;
            return (uint)(r * r + g * g + b * b);
        }

        private static Color VectorToColor(Vector3 v)
        {
            return Color.FromArgb(255, 
                        (byte)Clamp(v.X /* 255 / 31*/), 
                        (byte)Clamp(v.Y /* 255 / 63*/), 
                        (byte)Clamp(v.Z /* 255 / 31*/));
        }

        private static Vector3 ColorToVector(Color c)
        {
            return new Vector3(c.R, c.G, c.B);
        }

        private static Color SumColors(Color a, Color b, int wa, int wb)
        {
            return Color.FromArgb(
                (a.R * wa + b.R * wb) / (wa + wb), 
                (a.G * wa + b.G * wb) / (wa + wb), 
                (a.B * wa + b.B * wb) / (wa + wb));
        }

        #endregion

        #region Compressed block processing

        private static Color CalcSecondBoundColor(Color c, Color boundColor)
        {
            int r = 2 * c.R - boundColor.R;
            int g = 2 * c.G - boundColor.G;
            int b = 2 * c.B - boundColor.B;
            if (r < 0 || r > 255 || g < 0 || g > 255 || b < 0 || b > 255) return c;
            return Color.FromArgb(255, r, g, b);
        }

        private static bool IsCompressedBlock(Color[] colors, bool[] uniqueFlags, int count, int aIndex, int bIndex, out Color[] dxtBoundColors, ref bool hasAlpha)
        {
            Color[] uniqueColors = new Color[count];
            Vector3[] inputPoints = new Vector3[count];
            if (count > 0)
            {
                uniqueColors[0] = colors[aIndex];
                uniqueColors[count - 1] = colors[bIndex];
                for (int i = 0, j = 1; i < colors.Length && j < count - 1; i++)
                    if (uniqueFlags[i] && i != aIndex && i != bIndex) uniqueColors[j++] = colors[i];
                for (int i = 0; i < count; i++)
                    inputPoints[i] = new Vector3(uniqueColors[i].R / 8, uniqueColors[i].G / 8, uniqueColors[i].B / 8);
            }

            dxtBoundColors = new Color[2];
            if (count == 0)
            {
                #region Transparent block
                dxtBoundColors[0] = Color.Black;
                dxtBoundColors[1] = dxtBoundColors[0];
                hasAlpha = true;
                #endregion
            }
            else if (count == 1)
            {
                #region Single color
                int r = (byte)(inputPoints[0].X * 255 / 31);
                int g = (byte)(inputPoints[0].Y * 255 / 31);
                int b = (byte)(inputPoints[0].Z * 255 / 31);
                if (uniqueColors[0].R == r && uniqueColors[0].G == g && uniqueColors[0].B == b)
                {
                    dxtBoundColors[0] = uniqueColors[0];
                    dxtBoundColors[1] = dxtBoundColors[0];
                    hasAlpha = true;
                }
                else
                {
                    Vector3[] dxtPoints = new Vector3[2];
                    dxtPoints[0] = Vector3.Zero;
                    dxtPoints[1] = 2 * new Vector3(uniqueColors[0].R, uniqueColors[0].G, uniqueColors[0].B);
                    if (dxtPoints[1].X > 255)
                    {
                        dxtPoints[0].X = dxtPoints[1].X - 255;
                        dxtPoints[1].X -= dxtPoints[0].X;
                    }
                    if (dxtPoints[1].Y > 255)
                    {
                        dxtPoints[0].Y = dxtPoints[1].Y - 255;
                        dxtPoints[1].Y -= dxtPoints[0].Y;
                    }
                    if (dxtPoints[1].Z > 255)
                    {
                        dxtPoints[0].Z = dxtPoints[1].Z - 255;
                        dxtPoints[1].Z -= dxtPoints[0].Z;
                    }

                    dxtBoundColors[0] = Color.FromArgb(255, (byte)dxtPoints[0].X, (byte)dxtPoints[0].Y, (byte)dxtPoints[0].Z);
                    dxtBoundColors[1] = Color.FromArgb(255, (byte)dxtPoints[1].X, (byte)dxtPoints[1].Y, (byte)dxtPoints[1].Z);
                    hasAlpha = true;
                }
                #endregion
            }
            else if (count == 2)
            {
                #region Two colors block
                byte r0 = (byte)(inputPoints[0].X * 255 / 31);
                byte g0 = (byte)(inputPoints[0].Y * 255 / 31);
                byte b0 = (byte)(inputPoints[0].Z * 255 / 31);
                byte r1 = (byte)(inputPoints[1].X * 255 / 31);
                byte g1 = (byte)(inputPoints[1].Y * 255 / 31);
                byte b1 = (byte)(inputPoints[1].Z * 255 / 31);
                bool color0 = uniqueColors[0].R == r0 && uniqueColors[0].G == g0 && uniqueColors[0].B == b0;
                bool color1 = uniqueColors[1].R == r1 && uniqueColors[1].G == g1 && uniqueColors[1].B == b1;
                if (color0 && color1)
                {
                    dxtBoundColors[0] = uniqueColors[0];
                    dxtBoundColors[1] = uniqueColors[1];
                    hasAlpha = true;
                }
                else
                {
                    if (color0)
                    {
                        dxtBoundColors[0] = uniqueColors[0];
                        dxtBoundColors[1] = CalcSecondBoundColor(uniqueColors[1], uniqueColors[0]);
                        hasAlpha = true;
                    }
                    else if (color1 || hasAlpha)
                    {
                        dxtBoundColors[0] = CalcSecondBoundColor(uniqueColors[0], uniqueColors[1]);
                        dxtBoundColors[1] = uniqueColors[1];
                        hasAlpha = true;
                    }
                    else
                    {
                        dxtBoundColors[0] = CalcSecondBoundColor(uniqueColors[0], uniqueColors[1]);
                        dxtBoundColors[1] = CalcSecondBoundColor(uniqueColors[1], uniqueColors[0]);
                        if (dxtBoundColors[0] == uniqueColors[0] || dxtBoundColors[1] == uniqueColors[1])
                            hasAlpha = true;
                    }
                }
                #endregion
            }
            else if (count == 3)
            {
                #region Three colors block

                Color middle = SumColors(uniqueColors[0], uniqueColors[2], 1, 1);
                if (middle.R == uniqueColors[1].R && middle.G == uniqueColors[1].G && middle.B == uniqueColors[1].B)
                {
                    byte r0 = (byte)(inputPoints[0].X * 255 / 31);
                    byte g0 = (byte)(inputPoints[0].Y * 255 / 31);
                    byte b0 = (byte)(inputPoints[0].Z * 255 / 31);
                    byte r2 = (byte)(inputPoints[2].X * 255 / 31);
                    byte g2 = (byte)(inputPoints[2].Y * 255 / 31);
                    byte b2 = (byte)(inputPoints[2].Z * 255 / 31);
                    bool color0 = uniqueColors[0].R == r0 && uniqueColors[0].G == g0 && uniqueColors[0].B == b0;
                    bool color2 = uniqueColors[2].R == r2 && uniqueColors[2].G == g2 && uniqueColors[2].B == b2;
                    if ((color0 && color2) || hasAlpha)
                    {
                        dxtBoundColors[0] = uniqueColors[0];
                        dxtBoundColors[1] = uniqueColors[2];
                        hasAlpha = true;
                    }
                    else if (color0)
                    {
                        dxtBoundColors[0] = uniqueColors[0];
                        dxtBoundColors[1] = CalcSecondBoundColor(uniqueColors[2], uniqueColors[1]);
                        hasAlpha = (dxtBoundColors[1] == uniqueColors[2]);
                    }
                    else
                    {
                        dxtBoundColors[0] = CalcSecondBoundColor(uniqueColors[0], uniqueColors[1]);
                        dxtBoundColors[1] = uniqueColors[2];
                        hasAlpha = (dxtBoundColors[0] == uniqueColors[0]);
                    }
                }
                else if (!hasAlpha)
                {
                    Color m1 = SumColors(uniqueColors[0], uniqueColors[2], 5, 3);
                    Color m2 = SumColors(uniqueColors[0], uniqueColors[2], 3, 5);
                    if ((uniqueColors[1].R == m1.R && uniqueColors[1].G == m1.G && uniqueColors[1].B == m1.B)
                        || (uniqueColors[1].R == m2.R && uniqueColors[1].G == m2.G && uniqueColors[1].B == m2.B))
                    {
                        dxtBoundColors[0] = uniqueColors[0];
                        dxtBoundColors[1] = uniqueColors[2];
                    }
                    else return false;

                }
                else return false;

                #endregion
            }
            else if (count == 4 && !hasAlpha)
            {
                #region Four colors block

                Color m1 = SumColors(uniqueColors[0], uniqueColors[3], 5, 3);
                Color m2 = SumColors(uniqueColors[0], uniqueColors[3], 3, 5);
                if ((uniqueColors[1].R == m1.R && uniqueColors[1].G == m1.G && uniqueColors[1].B == m1.B
                     && uniqueColors[2].R == m2.R && uniqueColors[2].G == m2.G && uniqueColors[2].B == m2.B)
                    || (uniqueColors[2].R == m1.R && uniqueColors[2].G == m1.G && uniqueColors[2].B == m1.B
                        && uniqueColors[1].R == m2.R && uniqueColors[1].G == m2.G && uniqueColors[1].B == m2.B))
                {
                    dxtBoundColors[0] = uniqueColors[0];
                    dxtBoundColors[1] = uniqueColors[3];
                }
                else return false;

                #endregion
            }
            else return false;

            return true;
        }

        #endregion

        #region NVidia optimization

        static void OptimizeEndPoints3(Vector3[] block, ref Vector3[] dxtPoints, ref uint indices)
        {
            float alpha2_sum = 0.0f;
            float beta2_sum = 0.0f;
            float alphabeta_sum = 0.0f;
            Vector3 alphax_sum = Vector3.Zero;
            Vector3 betax_sum = Vector3.Zero;

            for (int i = 0; i < 16; ++i)
            {
                uint bits = indices >> (2 * i);

                float beta = (float)(bits & 1);
                if ((bits & 2) > 0) beta = 0.5f;
                float alpha = 1.0f - beta;

                alpha2_sum += alpha * alpha;
                beta2_sum += beta * beta;
                alphabeta_sum += alpha * beta;
                alphax_sum += alpha * block[i];
                betax_sum += beta * block[i];
            }

            float denom = alpha2_sum * beta2_sum - alphabeta_sum * alphabeta_sum;
            if (NvMath.Equal(denom, 0.0f)) return;

            float factor = 1.0f / denom;

            Vector3 a = (alphax_sum * beta2_sum - betax_sum * alphabeta_sum) * factor;
            Vector3 b = (betax_sum * alpha2_sum - alphax_sum * alphabeta_sum) * factor;

            a = Vector3.Clamp(a, 0, 255);
            b = Vector3.Clamp(b, 0, 255);

            //UInt16 color0 = roundAndExpand(ref a);
            //UInt16 color1 = roundAndExpand(ref b);

            //if (color0 < color1)
            //{
            //    NvMath.swap(ref a, ref b);
            //    NvMath.swap(ref color0, ref color1);
            //}

            //indices = computeIndices3(block, a, b);
            dxtPoints[0] = b;
            dxtPoints[1] = a;
        }

        static void OptimizeEndPoints4(Vector3[] block, ref Vector3[] dxtPoints, ref uint indices)
        {
            float alpha2_sum = 0.0f;
            float beta2_sum = 0.0f;
            float alphabeta_sum = 0.0f;
            Vector3 alphax_sum = Vector3.Zero;
            Vector3 betax_sum = Vector3.Zero;

            for (int i = 0; i < 16; ++i)
            {
                uint bits = indices >> (2 * i);

                float beta = (float)(bits & 1);
                if ((bits & 2) > 0) beta = (1 + beta) / 3.0f;
                float alpha = 1.0f - beta;

                alpha2_sum += alpha * alpha;
                beta2_sum += beta * beta;
                alphabeta_sum += alpha * beta;
                alphax_sum += alpha * block[i];
                betax_sum += beta * block[i];
            }

            float denom = alpha2_sum * beta2_sum - alphabeta_sum * alphabeta_sum;
            if (NvMath.Equal(denom, 0.0f)) return;

            float factor = 1.0f / denom;

            Vector3 a = (alphax_sum * beta2_sum - betax_sum * alphabeta_sum) * factor;
            Vector3 b = (betax_sum * alpha2_sum - alphax_sum * alphabeta_sum) * factor;

            a = Vector3.Clamp(a, 0, 255);
            b = Vector3.Clamp(b, 0, 255);

            //UInt16 color0 = roundAndExpand(ref a);
            //UInt16 color1 = roundAndExpand(ref b);

            //if (color0 < color1)
            //{
            //    NvMath.swap(ref a, ref b);
            //    NvMath.swap(ref color0, ref color1);
            //}

            //indices = computeIndices4(block, a, b);
            dxtPoints[0] = a;
            dxtPoints[1] = b;
        }

        static uint ComputeIndices3(Vector3[] block, Vector3 maxColor, Vector3 minColor)
        {
            Vector3[] palette = new Vector3[4];
            palette[0] = minColor;
            palette[1] = maxColor;
            palette[2] = (palette[0] + palette[1]) * 0.5f;

            uint indices = 0;
            for (int i = 0; i < 16; i++)
            {
                double d0 = ColorDistance(palette[0], block[i]);
                double d1 = ColorDistance(palette[1], block[i]);
                double d2 = ColorDistance(palette[2], block[i]);

                uint index;
                if (d0 < d1 && d0 < d2) index = 0;
                else if (d1 < d2) index = 1;
                else index = 2;

                indices |= index << (2 * i);
            }

            return indices;
        }

        static uint ComputeIndices4(Vector3[] block, Vector3 maxColor, Vector3 minColor)
        {
            Vector3[] palette = new Vector3[4];
            palette[0] = maxColor;
            palette[1] = minColor;
            palette[2] = Vector3.Lerp(palette[0], palette[1], 3.0f / 8.0f);
            palette[3] = Vector3.Lerp(palette[0], palette[1], 5.0f / 8.0f);

            uint indices = 0;
            for (int i = 0; i < 16; i++)
            {
                double d0 = ColorDistance(palette[0], block[i]);
                double d1 = ColorDistance(palette[1], block[i]);
                double d2 = ColorDistance(palette[2], block[i]);
                double d3 = ColorDistance(palette[3], block[i]);

                uint b0 = d0 > d3 ? (uint)1 : 0;
                uint b1 = d1 > d2 ? (uint)1 : 0;
                uint b2 = d0 > d2 ? (uint)1 : 0;
                uint b3 = d1 > d3 ? (uint)1 : 0;
                uint b4 = d2 > d3 ? (uint)1 : 0;

                uint x0 = b1 & b2;
                uint x1 = b0 & b3;
                uint x2 = b0 & b4;

                indices |= (x2 | ((x0 | x1) << 1)) << (2 * i);
            }

            return indices;
        }

        static double ColorDistance(Vector3 c0, Vector3 c1)
        {
            return (c0 - c1).LengthSquared();
        }

        // Takes a normalized color in [0, 255] range and returns 
        static ushort RoundAndExpand(ref Vector3 v)
        {
            uint r = (uint)Math.Floor(NvMath.Clamp(v.X * (31.0f / 255.0f), 0.0f, 31.0f));
            uint g = (uint)Math.Floor(NvMath.Clamp(v.Y * (31.0f / 255.0f), 0.0f, 31.0f));
            uint b = (uint)Math.Floor(NvMath.Clamp(v.Z * (31.0f / 255.0f), 0.0f, 31.0f));

            float r0 = (float)(((r + 0) << 3) | ((r + 0) >> 2));
            float r1 = (float)(((r + 1) << 3) | ((r + 1) >> 2));
            if (Math.Abs(v.X - r1) < Math.Abs(v.X - r0)) r = Math.Min(r + 1, 31U);

            float g0 = (float)(((g + 0) << 3) | ((g + 0) >> 2));
            float g1 = (float)(((g + 1) << 3) | ((g + 1) >> 2));
            if (Math.Abs(v.Y - g1) < Math.Abs(v.Y - g0)) g = Math.Min(g + 1, 31U);

            float b0 = (float)(((b + 0) << 3) | ((b + 0) >> 2));
            float b1 = (float)(((b + 1) << 3) | ((b + 1) >> 2));
            if (Math.Abs(v.Z - b1) < Math.Abs(v.Z - b0)) b = Math.Min(b + 1, 31U);


            ushort w = (ushort)((b << 10) | (g << 5) | r);

            r = (r << 3) | (r >> 2);
            g = (g << 3) | (g >> 2);
            b = (b << 3) | (b >> 2);
            v = new Vector3((float)r, (float)g, (float)b);

            return w;
        }

        #endregion

        #region Compress

        /// <summary>
        /// The compress 4x4 block.
        /// </summary>
        /// <param name="block">
        /// The block of 4x4 colors.
        /// </param>
        /// <param name="mask">
        /// The mask of 4x4 pixels (1 - visible, 0 - not visible).
        /// </param>
        /// <param name="compressColors">
        /// Four compressed colors.
        /// </param>
        /// <param name="c0">
        /// The color0.
        /// </param>
        /// <param name="c1">
        /// The color1.
        /// </param>
        /// <param name="texels">
        /// Texels.
        /// </param>
        /// <returns>
        /// The approximation error<see cref="double"/>.
        /// </returns>
        public static double CompressBlock(Color[] block, int[] mask, out Color[] compressColors, out ushort c0, out ushort c1, out uint texels)
        {
            #region Init
            bool hasAlpha = false;
            double bestError = 0;
            Vector3[] inputPoints = new Vector3[block.Length];
            bool[] uniqueFlags = new bool[block.Length];
            int[] lowIds = new int[block.Length];
            int[] highIds = new int[block.Length];

            int count = 0;
            int countWithmask = 0;
            double maxDistance = -1;
            double maxDistanceWithMask = -1;
            for (int i = 0; i < block.Length; i++)
            {
                inputPoints[i] = ColorToVector(block[i]);
                if (block[i].A >= 8)
                {
                    bool dubled = false;
                    bool dubledWithMask = false;
                    for (int j = i - 1; j >= 0 && (!dubled || !dubledWithMask); --j)
                    {
                        if (block[j].A >= 8)
                        {
                            double dist = (inputPoints[i] - inputPoints[j]).LengthSquared();
                            if (uniqueFlags[j])
                            {
                                dubled |= dist < 1e-6;
                                if (dist > maxDistance)
                                {
                                    maxDistance = dist;
                                    lowIds[0] = j;
                                    highIds[0] = i;
                                }
                            }

                            if (mask[j] > 0 && mask[i] > 0)
                            {
                                dubledWithMask |= dist < 1e-6;
                                if (dist > maxDistanceWithMask)
                                {
                                    maxDistanceWithMask = dist;
                                    lowIds[2] = j;
                                    highIds[2] = i;
                                }
                            }
                        }
                    }

                    if (!dubled)
                    {
                        count++;
                        uniqueFlags[i] = true;
                        if (maxDistance < 0) lowIds[0] = i;
                    }

                    if (!dubledWithMask && mask[i] > 0)
                    {
                        countWithmask++;
                        if (maxDistanceWithMask < 0) lowIds[2] = i;
                    }
                }
                else hasAlpha = true;
            }
            #endregion

            #region Compress
            bool compressed = false;
            bool hasAlpha0 = hasAlpha;
            Color[] dxtBoundColors = null;
            if (count <= 4)
                compressed = IsCompressedBlock(block, uniqueFlags, count, lowIds[0], highIds[0], out dxtBoundColors, ref hasAlpha);

            Vector3[] dxtBoundPoints = new Vector3[2];
            if (!compressed)
            {
                hasAlpha = hasAlpha0;
                if (countWithmask == 0)
                {
                    bestError = 0;
                    dxtBoundPoints[0] = inputPoints[lowIds[0]];
                    dxtBoundPoints[1] = inputPoints[highIds[0]];
                    hasAlpha = true;
                }
                else if (countWithmask == 1)
                {
                    bestError = Math.Max(0, maxDistance);
                    uint dist0 = ColorSquaredDistance(block[lowIds[2]], block[lowIds[0]]);
                    uint dist1 = ColorSquaredDistance(block[lowIds[2]], block[highIds[0]]);
                    if (dist0 > dist1)
                        dxtBoundPoints[1] = inputPoints[lowIds[0]];
                    else
                        dxtBoundPoints[1] = inputPoints[highIds[0]];
                    dxtBoundPoints[0] = inputPoints[lowIds[2]];
                    hasAlpha = true;
                }
                else
                {
                    Vector3 maxColor = inputPoints[lowIds[2]];
                    Vector3 minColor = inputPoints[highIds[2]];

                    UInt16 color0 = RoundAndExpand(ref maxColor);
                    UInt16 color1 = RoundAndExpand(ref minColor);
                    hasAlpha |= color0 == color1;

                    if (color0 < color1)
                    {
                        NvMath.Swap(ref maxColor, ref minColor);
                        NvMath.Swap(ref color0, ref color1);
                    }

                    dxtBoundPoints[0] = maxColor;
                    dxtBoundPoints[1] = minColor;
                    if (!hasAlpha)
                    {
                        uint indices = ComputeIndices4(inputPoints, maxColor, minColor);
                        OptimizeEndPoints4(inputPoints, ref dxtBoundPoints, ref indices);
                    }
                    //else
                    //{
                    //    uint indices = computeIndices3(inputPoints, maxColor, minColor);
                    //    optimizeEndPoints3(inputPoints, ref dxtBoundPoints, ref indices);
                    //}
                }
            }
            else
            {
                for (int i = 0; i < 2; i++) dxtBoundPoints[i] = ColorToVector(dxtBoundColors[i]);
            }
            #endregion

            #region Finilize

            c0 = RoundAndExpand(ref dxtBoundPoints[0]);
            c1 = RoundAndExpand(ref dxtBoundPoints[1]);

            if (c0 == c1) hasAlpha = true;
            if (c0 > c1 == hasAlpha)
            {
                NvMath.Swap(ref c0, ref c1);
                NvMath.Swap(ref dxtBoundPoints[0], ref dxtBoundPoints[1]);
            }

            if (c0 > c1) NvMath.Swap(ref dxtBoundPoints[0], ref dxtBoundPoints[1]);

            compressColors = new Color[4];
            for (int i = 0; i < 2; i++) compressColors[i] = VectorToColor(dxtBoundPoints[i]);
            if (hasAlpha)
            {
                compressColors[2] = SumColors(compressColors[0], compressColors[1], 1, 1);
                compressColors[3] = Color.FromArgb(0, 0, 0, 0);
            }
            else
            {
                compressColors[2] = SumColors(compressColors[0], compressColors[1], 5, 3);
                compressColors[3] = SumColors(compressColors[0], compressColors[1], 3, 5);
            }

            bestError = 0;
            texels = 0;
            for (int i = 0; i < 16; i++)
            {
                byte ci;
                if (block[i].A >= 8)
                {
                    double dist0 = ColorSquaredDistance(block[i], compressColors[0]);
                    double dist1 = ColorSquaredDistance(block[i], compressColors[1]);
                    double dist2 = ColorSquaredDistance(block[i], compressColors[2]);
                    double dist3 = ColorSquaredDistance(block[i], compressColors[3]);

                    if (dist0 < dist2)
                    {
                        ci = 0;
                        bestError += dist0;
                    }
                    else if (dist1 < dist2 && dist1 < dist3)
                    {
                        ci = 1;
                        bestError += dist1;
                    }
                    else if (dist2 < dist3)
                    {
                        ci = 2;
                        bestError += dist2;
                    }
                    else
                    {
                        ci = 3;
                        bestError += dist3;
                    }
                }
                else ci = 3;

                texels |= (uint)(ci << (2 * i));
            }

            #endregion

            return Math.Sqrt(bestError / 16);
        }

        /// <summary>
        /// The compress ARGB32 format image to 4x4 blocks.
        /// </summary>
        /// <param name="bgra">The bgra data.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="boundaryApprox">The boundary approx flag (if flag = 1 then pixels inside of the 4x4 block will be ignored).</param>
        /// <param name="onlyInterpolatedPalettes">Interpolated palettes using only flag (if flag = 0 then palettes each 4x4 block can be contain from 2 to 4 colors).</param>
        /// <param name="palette">
        /// The output palette.
        /// WARNING: If color pairs number (palette size / 4) > 0x3FFF then output data has error color indexes.  
        /// </param>
        /// <returns>
        /// The compressed data<see cref="byte[]"/>.
        /// </returns>
        public static byte[] Compress(byte[] bgra, uint width, uint height, bool boundaryApprox, bool onlyInterpolatedPalettes, out byte[] palette)
        {
            uint wt = width / 4;
            uint texelsCount = width * height / 16;

            byte[] result = new byte[texelsCount * 6];
            
            List<ulong> colorQuarts = new List<ulong>();
            ulong[] colorPairsOrQuarts = new ulong[texelsCount];
            byte[] palTypes = new byte[texelsCount];
            int[] colorPairIndexes = new int[texelsCount];
            
            for (uint y = 0; y < height; y += 4)
            {
                for (uint x = 0; x < width; x += 4)
                {
                    Color[] block = new Color[16];
                    for (int j = 0; j < 4; j++)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            uint pixelIndex = (uint)((y + i) * width + (x + j));
                            int pixelBlockIndex = i * 4 + j;
                            block[pixelBlockIndex] = Color.FromArgb(
                                bgra[4 * pixelIndex + 3], 
                                bgra[4 * pixelIndex + 2], 
                                bgra[4 * pixelIndex + 1], 
                                bgra[4 * pixelIndex + 0]);
                        }
                    }

                    int[] mask;
                    if (boundaryApprox)
                    {
                        mask = new[] {
                            1, 1, 1, 1,
                            1, 0, 0, 1,
                            1, 0, 0, 1,
                            1, 1, 1, 1
                        };
                    }
                    else
                    {
                        mask = new[] {
                            1, 1, 1, 1,
                            1, 1, 1, 1,
                            1, 1, 1, 1,
                            1, 1, 1, 1
                        };
                    }

                    // try
                    {
                        Color[] compressColors;
                        uint texels = 0;
                        ushort c0, c1;
                        double err = CompressBlock(block, mask, out compressColors, out c0, out c1, out texels);

                        uint texelIndex = y / 4 * wt + x / 4;
                        uint resIndex = texelIndex * 4;
                        result[resIndex + 0] = (byte)((texels >> 0) & 0xFF);
                        result[resIndex + 1] = (byte)((texels >> 8) & 0xFF);
                        result[resIndex + 2] = (byte)((texels >> 16) & 0xFF);
                        result[resIndex + 3] = (byte)((texels >> 24) & 0xFF);

                        palTypes[texelIndex] = (c0 <= c1) ? (byte)1 : (byte)3;
                        colorPairsOrQuarts[texelIndex] = (c0 <= c1) ? (uint)((c1 << 16) | c0) : (uint)((c0 << 16) | c1);

                        int colorIndex = -1;
                        if (!onlyInterpolatedPalettes && err >= 16)
                        {
                            Vector3[] cV = { Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero };
                            int[] cN = { 0, 0, 0, 0};
                            for (int i = 0; i < 16; i++)
                            {
                                uint ci = (texels >> (2 * i)) & 0x3;
                                cV[ci] += ColorToVector(block[i]);
                                cN[ci]++;
                            }

                            if (cN[2] != 0 || cN[3] != 0)
                            {
                                palTypes[texelIndex] -= 1;

                                ushort[] c = new ushort[4];
                                for (int ci = 0; ci < 4; ci++)
                                {
                                    if (cN[ci] > 0) cV[ci] /= cN[ci];
                                    c[ci] = RoundAndExpand(ref cV[ci]);
                                }

                                colorPairsOrQuarts[texelIndex] = ((ulong)((c[3] << 16) | c[2]) << 32) | (ulong)((c[1] << 16) | c[0]);
                                colorIndex = 2 * colorQuarts.IndexOf(colorPairsOrQuarts[texelIndex]);
                                if (colorIndex < 0)
                                {
                                    colorIndex = 2 * colorQuarts.Count;
                                    colorQuarts.Add(colorPairsOrQuarts[texelIndex]);
                                }
                            }
                        }

                        colorPairIndexes[texelIndex] = colorIndex;
                    }
                    // catch (Exception e)
                    // {
                    //    throw e;
                    // }
                }
            }

            // Generates pairs of colors. First writing pairs from quartets.
            List<uint> colorPairs = new List<uint>();
            for (int i = 0; i < colorQuarts.Count; i++)
            {
                UInt32 pair1 = (uint)(colorQuarts[i] & 0xFFFFFFFF);
                UInt32 pair2 = (uint)((colorQuarts[i] >> 32) & 0xFFFFFFFF);
                colorPairs.Add(pair1);
                colorPairs.Add(pair2);
            }

            // Extend pairs list and writing <palette index data> to result.
            for (uint i = 0; i < texelsCount; i++)
            {
                int colorIndex = colorPairIndexes[i];
                if (palTypes[i] % 2 != 0)
                {
                    UInt32 colorPair = (uint)colorPairsOrQuarts[i];
                    colorIndex = colorPairs.IndexOf(colorPair);
                    if (colorIndex < 0)
                    {
                        colorIndex = colorPairs.Count;
                        colorPairs.Add(colorPair);
                    }
                }

                if (colorIndex > 0x3FFF) colorIndex = 0;
                ushort palInfo = (ushort)((colorIndex & 0x3FFF) | (palTypes[i] << 14));
                uint resIndex = texelsCount * 4 + i * 2;
                result[resIndex + 0] = (byte)((palInfo >> 0) & 0xFF);
                result[resIndex + 1] = (byte)((palInfo >> 8) & 0xFF);
            }

            // Convert color pais to output palette data.
            palette = new byte[colorPairs.Count * 4];
            for (int i = 0; i < colorPairs.Count; i++)
            {
                byte[] colorsData = BitConverter.GetBytes(colorPairs[i]);
                Array.Copy(colorsData, 0, palette, 4 * i, 4);
            }

            return result;
        }

        #endregion
    }
}
