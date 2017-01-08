using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ekona.Mathematics
{
    internal static class NvMath
    {
        public const float NV_EPSILON = 0.0001f;

        public static float Saturate(float f)
        {
            return (float)Clamp(f, 0.0f, 1.0f);
        }

        /// Clamp between two values.
        public static double Clamp(double x, double a, double b)
        {
            return Math.Min(Math.Max(x, a), b);
        }

        /// Return the maximum of the three arguments.
        public static double Max3(double x, double a, double b)
        {
            return Math.Max(Math.Max(x, a), b);
        }

        /// Return the maximum of the three arguments.
        public static double Min3(double x, double a, double b)
        {
            return Math.Min(Math.Min(x, a), b);
        }

        public static void Swap<T>(ref T o1, ref T o2)
        {
            var o3 = o1;
            o1 = o2;
            o2 = o3;
        }

        // Robust floating point comparisons:
        // http://realtimecollisiondetection.net/blog/?p=89
        public static bool Equal(float f0, float f1, float epsilon = NV_EPSILON)
        {
            //return fabs(f0-f1) <= epsilon;
            return Math.Abs(f0 - f1) <= epsilon * Max3(1.0f, (float)Math.Abs(f0), (float)Math.Abs(f1));
        }
    }
}
