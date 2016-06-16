// -----------------------------------------------------------------------
// <copyright file="Vector3.cs" company="NII">
//
//   Copyright (C) 2016 MetLob
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

namespace Ekona.Mathematics
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Vector 3D
    /// </summary>
    public struct Vector3
    {
        #region Fields

        double x;
        double y;
        double z;

        #endregion

        #region Initialize

        /// <summary>
        /// Initialize the vector by coordinates
        /// </summary>
        /// <param name="x">Abscissa</param>
        /// <param name="y">Ordinate</param>
        /// <param name="z">Applicate</param>
        public Vector3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Initialize the vector by coordinates
        /// </summary>
        /// <param name="coordinates">Array of coodinates</param>
        public Vector3(double[] coordinates)
        {
            this.x = coordinates[0];
            this.y = coordinates[1];
            this.z = coordinates[2];
        }

        /// <summary>
        /// Initialize the vector as copy
        /// </summary>
        /// <param name="vector">Source vector</param>
        public Vector3(Vector3 vector)
        {
            x = vector.X;
            y = vector.Y;
            z = vector.Z;
        }

        #endregion

        #region Contants

        /// <summary>
        /// Null-vector
        /// </summary>
        public static Vector3 Zero
        {
            get { return new Vector3(0.0, 0.0, 0.0); }
        }

        /// <summary>
        /// X-axis
        /// </summary>
        public static Vector3 XAxis
        {
            get { return new Vector3(1.0, 0.0, 0.0); }
        }

        /// <summary>
        /// Y-axis
        /// </summary>
        public static Vector3 YAxis
        {
            get { return new Vector3(0.0, 1.0, 0.0); }
        }

        /// <summary>
        /// Z-axis
        /// </summary>
        public static Vector3 ZAxis
        {
            get { return new Vector3(0.0, 0.0, 1.0); }
        }

        #endregion

        #region Property

        /// <summary>
        /// Get or set the x coorinate (abscissa of the vector)
        /// </summary>
        /// <value>The abscissa of the vector</value>
        public double X
        {
            get { return x; }
            set { x = value; }
        }

        /// <summary>
        /// Get or set the x coorinate (ordinate of the vector)
        /// </summary>
        /// <value>The ordinate of the vector</value>
        public double Y
        {
            get { return y; }
            set { y = value; }
        }

        /// <summary>
        /// Get or set the x coorinate (applicate of the vector)
        /// </summary>
        /// <value>The applicate of the vector</value>
        public double Z
        {
            get { return z; }
            set { z = value; }
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Addition of two vectors Vector3
        /// </summary>
        /// <param name="v">First summand Vector3</param>
        /// <param name="w">Second summand Vector3</param>
        /// <returns>The result sum vector = v + w</returns>
        public static Vector3 Add(Vector3 v, Vector3 w)
        {
            return new Vector3(v.X + w.X, v.Y + w.Y, v.Z + w.Z);
        }

        /// <summary>
        /// Addition vector and scalar
        /// </summary>
        /// <param name="v">First summand Vector3</param>
        /// <param name="s">Second scalar summand double</param>
        /// <returns>The result sum vector = v + s</returns>
        public static Vector3 Add(Vector3 v, double s)
        {
            return new Vector3(v.X + s, v.Y + s, v.Z + s);
        }

        /// <summary>
        /// The subtract of two vectors
        /// </summary>
        /// <param name="v">Reduces vector</param>
        /// <param name="w">Subtracts the vector</param>
        /// <returns>The subtract of two vectors</returns>
        /// <remarks>
        ///	result[i] = v[i] - w[i].
        /// </remarks>
        public static Vector3 Subtract(Vector3 v, Vector3 w)
        {
            return new Vector3(v.X - w.X, v.Y - w.Y, v.Z - w.Z);
        }

        /// <summary>
        /// Subtraction scalar from a vector
        /// </summary>
        /// <param name="v">Reduces vector</param>
        /// <param name="s">Subtracts the scalar</param>
        /// <returns>The difference vector and scalar</returns>
        /// <remarks>
        /// result[i] = v[i] - s
        /// </remarks>
        public static Vector3 Subtract(Vector3 v, double s)
        {
            return new Vector3(v.X - s, v.Y - s, v.Z - s);
        }

        /// <summary>
        /// Subtraction vector from a scalar
        /// </summary>
        /// <param name="s">Reduces the scalar (essentially vector (s,s,s))</param>
        /// <param name="v">Subtracts the vector</param>
        /// <returns>The difference of the scalar and vector</returns>
        /// <remarks>
        /// result[i] = s - v[i]
        /// </remarks>
        public static Vector3 Subtract(double s, Vector3 v)
        {
            return new Vector3(s - v.X, s - v.Y, s - v.Z);
        }

        /// <summary>
        /// Dividing a vector to another vector
        /// </summary>
        /// <param name="u">Divisible vector</param>
        /// <param name="v">Vector divider</param>
        /// <returns>Quotient vector</returns>
        /// <remarks>
        ///	result[i] = u[i] / v[i].
        /// </remarks>
        public static Vector3 Divide(Vector3 u, Vector3 v)
        {
            return new Vector3(u.X / v.X, u.Y / v.Y, u.Z / v.Z);
        }

        /// <summary>
        /// The division of a vector by a scalar
        /// </summary>
        /// <param name="v">Divisible vector</param>
        /// <param name="s">Divider - scalar</param>
        /// <returns>Quotient vector</returns>
        /// <remarks>
        /// result[i] = v[i] / s;
        /// </remarks>
        public static Vector3 Divide(Vector3 v, double s)
        {
            return new Vector3(v.X / s, v.Y / s, v.Z / s);
        }

        /// <summary>
        /// Dividing a scalar to a vector
        /// </summary>
        /// <param name="s">Divisible scalar - Vector(s,s,s)</param>
        /// <param name="v">Vector divider</param>
        /// <returns>Quotient vector</returns>
        /// <remarks>
        /// result[i] = s / v[i]
        /// </remarks>
        public static Vector3 Divide(double s, Vector3 v)
        {
            return new Vector3(s / v.X, s / v.Y, s / v.Z);
        }

        /// <summary>
        /// Multiplication of a vector by a scalar
        /// </summary>
        /// <param name="u">Vector</param>
        /// <param name="s">Scalar</param>
        /// <returns>The vector containing the product</returns>
        public static Vector3 Multiply(Vector3 u, double s)
        {
            return new Vector3(u.X * s, u.Y * s, u.Z * s);
        }

        /// <summary>
        /// Calculating the scalar product
        /// </summary>
        /// <param name="u">Vector</param>
        /// <param name="v">Vector</param>
        /// <returns>Scalar containing the result of the scalar product</returns>
        public static double DotProduct(Vector3 u, Vector3 v)
        {
            return u.DotProduct(v);
        }

        /// <summary>
        /// Calculating the cross product of two vectors
        /// </summary>
        /// <param name="u">Vector</param>
        /// <param name="v">Vector</param>
        /// <returns>The vector containing the result of the cross product</returns>
        public static Vector3 CrossProduct(Vector3 u, Vector3 v)
        {
            return new Vector3(
                u.Y * v.Z - u.Z * v.Y,
                u.Z * v.X - u.X * v.Z,
                u.X * v.Y - u.Y * v.X);
        }

        /// <summary>
        /// The negate of the vector - an appeal of its component signs
        /// </summary>
        /// <param name="v">Source vector</param>
        /// <returns>Inverted vector</returns>
        public static Vector3 Negate(Vector3 v)
        {
            return new Vector3(-v.X, -v.Y, -v.Z);
        }

        /// <summary>
        /// Testing for equivalence using the default tolerance
        /// </summary>
        /// <param name="v">Vector</param>
        /// <param name="u">Vector</param>
        /// <returns>True if the vector equivalent, otherwise false</returns>
        public static bool ApproxEqual(Vector3 v, Vector3 u)
        {
            return ApproxEqual(v, u, Double.Epsilon);
        }

        /// <summary>
        /// Testing for equivalence
        /// </summary>
        /// <param name="v">Vector</param>
        /// <param name="u">Vector</param>
        /// <param name="tolerance">Tolerance (possible deviation)</param>
        /// <returns>True if the vector equivalent, otherwise false</returns>
        public static bool ApproxEqual(Vector3 v, Vector3 u, double tolerance)
        {
            return
                (
                (Math.Abs(v.X - u.X) <= tolerance) &&
                (Math.Abs(v.Y - u.Y) <= tolerance) &&
                (Math.Abs(v.Z - u.Z) <= tolerance)
                );
        }

        /// <summary>
        /// Normalizing the vector
        /// </summary>
        /// <param name="v">Vector for normalization</param>
        /// <returns>Normal vector</returns>
        public static Vector3 Normalize(Vector3 v)
        {
            v.Normalize();
            return v;
        }

        /// <summary>
        /// Computation of the angle between the vectors
        /// </summary>
        /// <param name="vector1">Vector</param>
        /// <param name="vector2">Vector</param>
        /// <returns>Angle in radians</returns>
        public static double Angle(Vector3 vector1, Vector3 vector2)
        {
            return vector1.Angle(vector2);
        }

        /// <summary>
        /// Angle counterclockwise from the first vector to the second
        /// </summary>
        /// <param name="beginVector">First vector</param>
        /// <param name="endVector">Second vector</param>
        /// <returns>Angle in radians</returns>
        public static double CounterclockwiseAngleBetween(Vector3 beginVector, Vector3 endVector)
        {
            // It defines a large or small angle
            double factor = beginVector.X * endVector.Y - endVector.X * beginVector.Y;

            // Arc angle
            double betweenAngle = Vector3.Angle(beginVector, endVector);
            // Adjust the angle
            betweenAngle = factor > 0 ? betweenAngle : 2.0 * Math.PI - betweenAngle;
            return betweenAngle;
        }

        /// <summary>
        /// Angle counterclockwise from the first vector to the second
        /// </summary>
        /// <param name="firstVector">First vector</param>
        /// <param name="secondVector">Second vector</param>
        /// <param name="direction">The vector defines the direction of counterclockwise</param>
        /// <returns>Угол</returns>
        public static double CounterclockwiseAngleBetween(Vector3 firstVector, Vector3 secondVector, Vector3 direction)
        {
            //Vector3d xVector = new Vector3d(firstVector);
            //xVector.Normalize();
            //Vector3d zVector = directionAxe;
            //zVector.Normalize();
            //Vector3d yVector = zVector.CrossProduct(xVector);
            //yVector.Normalize();
            //// xVector, yVector, zVector образовали правую декартову систему координат
            //// нахожу координаты endvector в этой системе
            //double x = xVector.DotProduct(secondVector);
            //double y = yVector.DotProduct(secondVector);
            //double angle = Math.Acos(Math.Abs(x) / secondVector.Length());
            //if (x > 0.0d)
            //{
            //    if (y < 0.0d)
            //        angle = 2 * Math.PI - angle;
            //}
            //else
            //    if (y > 0.0d)
            //        angle = Math.PI - angle;
            //    else
            //        angle = Math.PI + angle;

            double angle = firstVector.Angle(secondVector);
            Vector3 z = Vector3.CrossProduct(firstVector, secondVector);
            double det = Vector3.DotProduct(z, direction);
            if (det < 0) angle = 2 * Math.PI - angle;

            return angle;

        }

        /// <summary>
        /// Get a geometric center of a array of vectors
        /// </summary>
        /// <param name="points">Array of points</param>
        /// <returns>The geometric center</returns>
        public static Vector3 GetCenter(Vector3[] points)
        {
            Vector3 center = new Vector3(0, 0, 0);
            foreach (Vector3 point in points)
                center = center + point;
            return center / points.Length;
        }

        /// <summary>
        /// Changes y and z locations
        /// </summary>
        /// <param name="vector">The vector whose coordinates change</param>
        /// <returns>The vector with modified coordinates</returns>
        static public Vector3 SwapYZ(Vector3 vector)
        {
            return new Vector3(vector.X, vector.Z, vector.Y);
        }

        #endregion

        #region Methods of the vector

        /// <summary>
        /// Rotation vector (points around the origin) counterclockwise relative to the normal vector
        /// </summary>
        /// <param name="angle">
        /// Angle of rotation
        /// </param>
        /// <param name="normal">
        /// The normal of the rotation plane
        /// </param>
        /// <returns>
        /// Turned vector (point)
        /// </returns>
        public Vector3 Rotate(double angle, Vector3 normal)
        {
            // Switching to the normal plane coordinate system
            Vector3 axisZ = normal.Unit();
            Vector3 axisX = axisZ.Orthogonal();
            Vector3 axisY = Vector3.CrossProduct(axisZ, axisX);
            Vector3 thisVector = new Vector3(
                this.X * axisX.X + this.Y * axisX.Y + this.Z * axisX.Z,
                this.X * axisY.X + this.Y * axisY.Y + this.Z * axisY.Z,
                this.X * axisZ.X + this.Y * axisZ.Y + this.Z * axisZ.Z);

            // Rotating
            double c = Math.Cos(angle);
            double s = Math.Sin(angle);
            Vector3 rotated = new Vector3(
                c * thisVector.X - s * thisVector.Y, s * thisVector.X + c * thisVector.Y, thisVector.Z);

            // Translate rotated vector back into a common coordinate system
            return new Vector3(
               axisX.X * rotated.X + axisY.X * rotated.Y + axisZ.X * rotated.Z,
               axisX.Y * rotated.X + axisY.Y * rotated.Y + axisZ.Y * rotated.Z,
               axisX.Z * rotated.X + axisY.Z * rotated.Y + axisZ.Z * rotated.Z).Unit() * this.Length();
        }

        /// <summary>
        /// Rotation point around a origin point counterclockwise relative to the normal vector
        /// </summary>
        /// <param name="origin">
        /// Origin
        /// </param>
        /// <param name="angle">
        /// Angle of rotation
        /// </param>
        /// <param name="normal">
        /// The normal of the rotation plane
        /// </param>
        /// <returns>
        /// Turned point
        /// </returns>
        public Vector3 Rotate(Vector3 origin, double angle, Vector3 normal)
        {
            return origin + (this - origin).Rotate(angle, normal);
        }

        /// <summary>
        /// Normalizing the vector (bringing it to the unit)
        /// </summary>
        /// <exception cref="System.DivideByZeroException">
        /// It occurs when an attempt to normalize the zero vector
        /// </exception>
        public void Normalize()
        {
            double length = Length();
            if (length == 0.0f)
            {
                throw new Exception("Trying to normalize a zero vector.");
            }

            x /= length;
            y /= length;
            z /= length;
        }

        /// <summary>
        /// Returns a unit vector drawn from this
        /// </summary>
        /// <returns>The new vector (normalized clone)</returns>
        /// <exception cref="System.DivideByZeroException">
        /// It occurs when an attempt to normalize the zero vector
        /// </exception>
        public Vector3 Unit()
        {
            Vector3 result = new Vector3(this);
            if (result == Vector3.Zero) return result;
            result.Normalize();
            return result;
        }

        /// <summary>
        /// Get An orthogonal vector
        /// </summary>
        /// <returns>The orthogonal vector</returns>
        public Vector3 Orthogonal()
        {
            Vector3 result = Vector3.Zero;
            int maxCoordIndex = 0;
            for (int i = 1; i < 3; i++)
                if (Math.Abs(this[i]) > Math.Abs(this[maxCoordIndex]))
                    maxCoordIndex = i;
            result[(maxCoordIndex + 1) % 3] = 0;
            result[(maxCoordIndex + 2) % 3] = -this[maxCoordIndex];
            result[maxCoordIndex] = this[(maxCoordIndex + 2) % 3];
            return result.Unit() * this.Length();
        }

        /// <summary>
        /// Get the length of the vector
        /// </summary>
        /// <returns>The length of the vector (Sqrt(X*X + Y*Y + Z*Z))</returns>
        public double Length()
        {
            double lengthSquared = LengthSquared();
            return lengthSquared == 1.0 ? 1.0 : (double)Math.Sqrt(lengthSquared);
        }
        /// <summary>
        /// Get the length squared of the vector
        /// </summary>
        /// <returns>The length squared (X*X + Y*Y + Z*Z)</returns>
        public double LengthSquared()
        {
            return (x * x + y * y + z * z);
        }
        /// <summary>
        /// Clamp the values of the vector at the origin using the given tolerance
        /// </summary>
        /// <param name="tolerance">Tolerance</param>
        public void ClampZero(double tolerance)
        {
            x = (tolerance > Math.Abs(x)) ? 0.0f : x;
            y = (tolerance > Math.Abs(y)) ? 0.0f : y;
            z = (tolerance > Math.Abs(z)) ? 0.0f : z;
        }

        /// <summary>
        /// Clamp the values of the vector at the origin using the default tolerance
        /// </summary>
        /// <remarks>
        /// Tolerant value is Double.Epsilon.
        /// </remarks>
        public void ClampZero()
        {
            ClampZero(Double.Epsilon);
        }

        /// <summary>
        /// Calculating the scalar product of vectors
        /// </summary>
        /// <param name="vector">Second vector</param>
        /// <returns>The scalar product</returns>
        public double DotProduct(Vector3 vector)
        {
            return this.x * vector.X + this.y * vector.Y + this.z * vector.Z;
        }

        /// <summary>
        /// Calculating the cross product of two vectors
        /// </summary>
        /// <param name="vector">Second vector</param>
        /// <returns>The vector containing the result of the cross product</returns>
        public Vector3 CrossProduct(Vector3 vector)
        {
            return new Vector3(
                this.Y * vector.Z - this.Z * vector.Y,
                this.Z * vector.X - this.X * vector.Z,
                this.X * vector.Y - this.Y * vector.X);
        }

        /// <summary>
        /// Computation of the angle between the vectors
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <returns>Angle in radians</returns>
        public double Angle(Vector3 vector)
        {
            double cosResult = this.DotProduct(vector) / (this.Length() * vector.Length());
            cosResult = (Math.Abs(cosResult) < 1.0) ? cosResult : (1.0 * Math.Sign(cosResult));
            return Math.Acos(cosResult);
        }

        #endregion

        #region Overrided methods

        /// <summary>
        /// Returns HASP key for this property
        /// </summary>
        /// <returns>HASP 32-bit key</returns>
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }

        /// <summary>
        /// Equivalence Checking with another object
        /// </summary>
        /// <param name="obj">The object for comparison</param>
        /// <returns>True if an equivalent object, false otherwise</returns>
        public override bool Equals(object obj)
        {
            if (obj is Vector3)
            {
                Vector3 v = (Vector3)obj;
                return (x == v.X) && (y == v.Y) && (z == v.Z);
            }
            return false;
        }

        /// <summary>
        /// Check with another vector equivalence
        /// </summary>
        /// <param name="vector">The vector for comparison</param>
        /// <param name="tolerance">Tolerance</param>
        /// <returns>True if an equivalent object, false otherwise</returns>
        public bool Equals(Vector3 vector, double tolerance)
        {
            return ((Math.Abs(x - vector.X) < tolerance) && (Math.Abs(y - vector.Y) < tolerance) && (Math.Abs(z - vector.Z) < tolerance));
        }

        /// <summary>
        /// The string representation of a object
        /// </summary>
        /// <returns>String - representation of the object</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0}; {1}; {2})", x, y, z);
        }

        #endregion

        #region Comparison operators

        /// <summary>
        /// Checks the equivalence of two vectors
        /// </summary>
        /// <param name="u">Left operand - vector</param>
        /// <param name="v">The right operand - vector</param>
        /// <returns>True if the two vectors are equal, false otherwise</returns>
        public static bool operator ==(Vector3 u, Vector3 v)
        {
            return u.Equals((object)v);
        }

        /// <summary>
        /// Checks for equality of two vectors
        /// </summary>
        /// <param name="u">The left operand - vector</param>
        /// <param name="v">The right operand - vector</param>
        /// <returns>True if the two vectors are different, false otherwise</returns>
        public static bool operator !=(Vector3 u, Vector3 v)
        {
            return !u.Equals((object)v);
        }

        public static bool operator >(Vector3 u, Vector3 v)
        {
            return (
                (u.x > v.x) &&
                (u.y > v.y) &&
                (u.z > v.z));
        }

        public static bool operator <(Vector3 u, Vector3 v)
        {
            return (
                (u.x < v.x) &&
                (u.y < v.y) &&
                (u.z < v.z));
        }

        public static bool operator >=(Vector3 u, Vector3 v)
        {
            return (
                (u.x >= v.x) &&
                (u.y >= v.y) &&
                (u.z >= v.z));
        }

        public static bool operator <=(Vector3 u, Vector3 v)
        {
            return (
                (u.x <= v.x) &&
                (u.y <= v.y) &&
                (u.z <= v.z));
        }

        #endregion

        #region Unary operators

        /// <summary>
        /// Inverting sign vector components
        /// </summary>
        /// <param name="v">Vector</param>
        /// <returns>The new vector with inverted components</returns>
        public static Vector3 operator -(Vector3 v)
        {
            return Vector3.Negate(v);
        }

        #endregion

        #region Binary operators

        public static Vector3 operator +(Vector3 u, Vector3 v)
        {
            return Vector3.Add(u, v);
        }

        public static Vector3 operator +(Vector3 v, double s)
        {
            return Vector3.Add(v, s);
        }

        public static Vector3 operator +(double s, Vector3 v)
        {
            return Vector3.Add(v, s);
        }

        public static Vector3 operator -(Vector3 u, Vector3 v)
        {
            return Vector3.Subtract(u, v);
        }

        public static Vector3 operator -(Vector3 v, double s)
        {
            return Vector3.Subtract(v, s);
        }

        public static Vector3 operator -(double s, Vector3 v)
        {
            return Vector3.Subtract(s, v);
        }

        public static Vector3 operator *(Vector3 v, double s)
        {
            return Vector3.Multiply(v, s);
        }

        public static Vector3 operator *(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
        }

        public static Vector3 operator *(double s, Vector3 v)
        {
            return Vector3.Multiply(v, s);
        }

        public static Vector3 operator /(Vector3 v, double s)
        {
            return Vector3.Divide(v, s);
        }

        public static Vector3 operator /(double s, Vector3 v)
        {
            return Vector3.Divide(s, v);
        }

        #endregion

        #region Operators access via indexes

        /// <summary>
        /// Get or set component by index ( [x, y] ).
        /// </summary>
        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }

        }

        #endregion

        #region NvVector3

        public static Vector3 Max(Vector3 a, Vector3 b)
        {
            return new Vector3(Math.Max(a.x, b.x), Math.Max(a.y, b.y), Math.Max(a.z, b.z));
        }

        public static Vector3 Min(Vector3 a, Vector3 b)
        {
            return new Vector3(Math.Min(a.x, b.x), Math.Min(a.y, b.y), Math.Min(a.z, b.z));
        }

        public static Vector3 Clamp(Vector3 v, float min, float max)
        {
            return new Vector3(NvMath.Clamp(v.x, min, max), NvMath.Clamp(v.y, min, max), NvMath.Clamp(v.z, min, max));
        }

        public static Vector3 Saturate(Vector3 v)
        {
            return new Vector3(NvMath.Saturate((float)v.x), NvMath.Saturate((float)v.y), NvMath.Saturate((float)v.z));
        }

        public static Vector3 Floor(Vector3 v)
        {
            return new Vector3(Math.Floor(v.x), Math.Floor(v.y), Math.Floor(v.z));
        }

        public static Vector3 Ceil(Vector3 v)
        {
            return new Vector3(Math.Ceiling(v.x), Math.Ceiling(v.y), Math.Ceiling(v.z));
        }

        public static Vector3 Lerp(Vector3 v1, Vector3 v2, float t)
        {
            float s = 1.0f - t;
            return new Vector3(v1.x * s + t * v2.x, v1.y * s + t * v2.y, v1.z * s + t * v2.z);
        }

        #endregion
    }
}
