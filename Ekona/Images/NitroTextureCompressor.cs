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

    /// <summary>
    /// Fast texture compressor for Nintendo DS format
    /// </summary>
    public class NitroTextureCompressor
    {
        #region Vector3

        /// <summary>
        /// Vector 3D
        /// </summary>
        struct Vector3
        {
            #region Атрибуты

            // Абсцисса
            double x;
            // Ордината
            double y;
            // Аппликата
            double z;

            #endregion

            #region Конструкторы

            /// <summary>
            /// Инициализация вектора путем задания его координат
            /// </summary>
            /// <param name="x">Абсцисса</param>
            /// <param name="y">Ордината</param>
            /// <param name="z">Аппликата</param>
            public Vector3(double x, double y, double z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            /// <summary>
            /// Инициализация вектора путем задания его компонет массивом
            /// </summary>
            /// <param name="coordinates">Массив компонент</param>
            public Vector3(double[] coordinates)
            {
                this.x = coordinates[0];
                this.y = coordinates[1];
                this.z = coordinates[2];
            }

            /// <summary>
            /// Конструктор копирования
            /// </summary>
            /// <param name="vector">Копируемый вектор</param>
            public Vector3(Vector3 vector)
            {
                x = vector.X;
                y = vector.Y;
                z = vector.Z;
            }

            #endregion

            #region Константы

            /// <summary>
            /// Нулевой вектор
            /// </summary>
            public static Vector3 Zero
            {
                get { return new Vector3(0.0, 0.0, 0.0); }
            }

            /// <summary>
            /// Ось абсцисс
            /// </summary>
            public static Vector3 XAxis
            {
                get { return new Vector3(1.0, 0.0, 0.0); }
            }

            /// <summary>
            /// Ось ординат
            /// </summary>
            public static Vector3 YAxis
            {
                get { return new Vector3(0.0, 1.0, 0.0); }
            }

            /// <summary>
            /// Ось аппликат
            /// </summary>
            public static Vector3 ZAxis
            {
                get { return new Vector3(0.0, 0.0, 1.0); }
            }

            #endregion

            #region Свойства

            /// <summary>
            /// Получение и установка абсциссы
            /// </summary>
            /// <value>Абсцисса этого вектора</value>
            public double X
            {
                get { return x; }
                set { x = value; }
            }

            /// <summary>
            /// Получение и установка ординаты
            /// </summary>
            /// <value>Ордината этого вектора</value>
            public double Y
            {
                get { return y; }
                set { y = value; }
            }

            /// <summary>
            /// Получение и установка аппликаты
            /// </summary>
            /// <value>Аппликата этого вектора</value>
            public double Z
            {
                get { return z; }
                set { z = value; }
            }

            #endregion

            #region Статические методы

            /// <summary>
            /// Сложение двух векторов Vector3
            /// </summary>
            /// <param name="v">Слагаемый вектор Vector3</param>
            /// <param name="w">Слагаемый вектор Vector3</param>
            /// <returns>Результирующий вектор суммы = v + w</returns>
            public static Vector3 Add(Vector3 v, Vector3 w)
            {
                return new Vector3(v.X + w.X, v.Y + w.Y, v.Z + w.Z);
            }

            /// <summary>
            /// Сложение вектора Vector3 и скаляра double
            /// </summary>
            /// <param name="v">Слагаемый вектор Vector3</param>
            /// <param name="s">Слагаемый скаляр double</param>
            /// <returns>Результирующий вектор суммы = v + s</returns>
            public static Vector3 Add(Vector3 v, double s)
            {
                return new Vector3(v.X + s, v.Y + s, v.Z + s);
            }

            /// <summary>
            /// Разница двух векторов
            /// </summary>
            /// <param name="v">Уменьшаемый вектор</param>
            /// <param name="w">Вычитаемый вектор</param>
            /// <returns>Разница двух векторов</returns>
            /// <remarks>
            ///	result[i] = v[i] - w[i].
            /// </remarks>
            public static Vector3 Subtract(Vector3 v, Vector3 w)
            {
                return new Vector3(v.X - w.X, v.Y - w.Y, v.Z - w.Z);
            }

            /// <summary>
            /// Вычитание скаляра из вектора
            /// </summary>
            /// <param name="v">Уменьшаемый вектор</param>
            /// <param name="s">Вычитаемый скаляр</param>
            /// <returns>Разница вектора и скаляра</returns>
            /// <remarks>
            /// result[i] = v[i] - s
            /// </remarks>
            public static Vector3 Subtract(Vector3 v, double s)
            {
                return new Vector3(v.X - s, v.Y - s, v.Z - s);
            }

            /// <summary>
            /// Вычитание вектора из скаляра  
            /// </summary>
            /// <param name="s">Уменьшаемый скаляр (по сути вектор (s,s,s))</param>
            /// <param name="v">Вычитаемый вектор</param>
            /// <returns>Разница скаляра и вектора</returns>
            /// <remarks>
            /// result[i] = s - v[i]
            /// </remarks>
            public static Vector3 Subtract(double s, Vector3 v)
            {
                return new Vector3(s - v.X, s - v.Y, s - v.Z);
            }

            /// <summary>
            /// Деление вектора на другой вектор
            /// </summary>
            /// <param name="u">Делимый вектор</param>
            /// <param name="v">Вектор делитель</param>
            /// <returns>Частное</returns>
            /// <remarks>
            ///	result[i] = u[i] / v[i].
            /// </remarks>
            public static Vector3 Divide(Vector3 u, Vector3 v)
            {
                return new Vector3(u.X / v.X, u.Y / v.Y, u.Z / v.Z);
            }

            /// <summary>
            /// Деление вектора на скаляр
            /// </summary>
            /// <param name="v">Делимый вектор</param>
            /// <param name="s">Делитель - скаляр</param>
            /// <returns>Частное</returns>
            /// <remarks>
            /// result[i] = v[i] / s;
            /// </remarks>
            public static Vector3 Divide(Vector3 v, double s)
            {
                return new Vector3(v.X / s, v.Y / s, v.Z / s);
            }

            /// <summary>
            /// Деление скаляра на вектор
            /// </summary>
            /// <param name="s">Делимое - скалар Vector(s,s,s)</param>
            /// <param name="v">Делитель вектор</param>
            /// <returns>Новый вектор, содержащий частное</returns>
            /// <remarks>
            /// result[i] = s / v[i]
            /// </remarks>
            public static Vector3 Divide(double s, Vector3 v)
            {
                return new Vector3(s / v.X, s / v.Y, s / v.Z);
            }

            /// <summary>
            /// Умножение вектора на скаляр
            /// </summary>
            /// <param name="u">Вектор</param>
            /// <param name="s">Скаляр</param>
            /// <returns>Вектор содержащий произведение</returns>
            public static Vector3 Multiply(Vector3 u, double s)
            {
                return new Vector3(u.X * s, u.Y * s, u.Z * s);
            }

            /// <summary>
            /// Вычисление скалярного произведения
            /// </summary>
            /// <param name="u">Вектор</param>
            /// <param name="v">Вектор</param>
            /// <returns>Скаляр, содержащий результат скалярного произведения</returns>
            public static double DotProduct(Vector3 u, Vector3 v)
            {
                return u.DotProduct(v);
            }

            /// <summary>
            /// Вычисление векторного произведения двух векторов
            /// </summary>
            /// <param name="u">Вектор</param>
            /// <param name="v">Вектор</param>
            /// <returns>Вектор, содержащий результат векторного произведения</returns>
            public static Vector3 CrossProduct(Vector3 u, Vector3 v)
            {
                return new Vector3(
                    u.Y * v.Z - u.Z * v.Y,
                    u.Z * v.X - u.X * v.Z,
                    u.X * v.Y - u.Y * v.X);
            }

            /// <summary>
            /// Отрицание вектора - обращение знаков его компонент
            /// </summary>
            /// <param name="v">Исходный вектор</param>
            /// <returns>Обращенный вектор</returns>
            public static Vector3 Negate(Vector3 v)
            {
                return new Vector3(-v.X, -v.Y, -v.Z);
            }

            /// <summary>
            /// Проверка на эквивалентность с использованием толерантности по умолчанию
            /// </summary>
            /// <param name="v">Вектор</param>
            /// <param name="u">Вектор</param>
            /// <returns>Истина если вектора эквивалентны, иначе ложь</returns>
            public static bool ApproxEqual(Vector3 v, Vector3 u)
            {
                return ApproxEqual(v, u, Double.Epsilon);
            }

            /// <summary>
            /// Проверка на эквивалентность
            /// </summary>
            /// <param name="v">Вектор</param>
            /// <param name="u">Вектор</param>
            /// <param name="tolerance">Толерантность (возможное отклонение)</param>
            /// <returns>Истина, если вектора эквивалентны, иначе ложь</returns>
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
            /// Нормализация вектора
            /// </summary>
            /// <param name="v">Вектор для нормализации</param>
            /// <returns>Нормализованный вектор</returns>
            public static Vector3 Normalize(Vector3 v)
            {
                v.Normalize();
                return v;
            }

            /// <summary>
            /// Вычисление угла между векторами
            /// </summary>
            /// <param name="vector1">вектор</param>
            /// <param name="vector2">вектор</param>
            /// <returns>угол в радианах</returns>
            public static double Angle(Vector3 vector1, Vector3 vector2)
            {
                return vector1.Angle(vector2);
            }

            #endregion

            #region Методы вектора

            /// <summary>
            /// Поворот вектора (точки вокруг начала координат) против часовой стрелки относительно вектора нормали
            /// </summary>
            /// <param name="angle">
            /// Угол поаорота
            /// </param>
            /// <param name="normal">
            /// Нормаль плоскости поворота
            /// </param>
            /// <returns>
            /// Повернутый вектор (точка)
            /// </returns>
            public Vector3 Rotate(double angle, Vector3 normal)
            {
                // Переход с системе координат плоскости нормали
                Vector3 axisZ = normal.Unit();
                Vector3 axisX = axisZ.Orthogonal();
                Vector3 axisY = Vector3.CrossProduct(axisZ, axisX);
                Vector3 thisVector = new Vector3(
                    this.X * axisX.X + this.Y * axisX.Y + this.Z * axisX.Z,
                    this.X * axisY.X + this.Y * axisY.Y + this.Z * axisY.Z,
                    this.X * axisZ.X + this.Y * axisZ.Y + this.Z * axisZ.Z);

                // Осуществляем поворот
                double c = Math.Cos(angle);
                double s = Math.Sin(angle);
                Vector3 rotated = new Vector3(
                    c * thisVector.X - s * thisVector.Y, s * thisVector.X + c * thisVector.Y, thisVector.Z);

                // Переводим повернутый вектор вновь в общую систему координат
                return new Vector3(
                   axisX.X * rotated.X + axisY.X * rotated.Y + axisZ.X * rotated.Z,
                   axisX.Y * rotated.X + axisY.Y * rotated.Y + axisZ.Y * rotated.Z,
                   axisX.Z * rotated.X + axisY.Z * rotated.Y + axisZ.Z * rotated.Z).Unit() * this.Length();
            }

            /// <summary>
            /// Поворот точки вокруг заданной против часовой стрелки относительно вектора нормали
            /// </summary>
            /// <param name="origin">
            /// Начало координат
            /// </param>
            /// <param name="angle">
            /// Угол поворота
            /// </param>
            /// <param name="normal">
            /// Нормаль плоскости поворота
            /// </param>
            /// <returns>
            /// Повернутая точка
            /// </returns>
            public Vector3 Rotate(Vector3 origin, double angle, Vector3 normal)
            {
                return origin + (this - origin).Rotate(angle, normal);
            }

            /// <summary>
            /// Нормализация вектора (приведение его к единичному)
            /// </summary>
            /// <exception cref="System.DivideByZeroException">
            /// Генерируется при попытке нормализовать нулевой вектор
            /// </exception>
            public void Normalize()
            {
                double length = Length();
                if (length == 0.0f)
                {
                    throw new Exception("Попытка нормализовать нулевой вектор");
                }

                x /= length;
                y /= length;
                z /= length;
            }

            /// <summary>
            /// Возвращает единичный вектор, произведенный из этого
            /// </summary>
            /// <returns>Новый вектор (нормализованный клон)</returns>
            /// <exception cref="System.DivideByZeroException">
            /// Генерируется при попытке нормализовать нулевой вектор
            /// </exception>
            public Vector3 Unit()
            {
                Vector3 result = new Vector3(this);
                if (result == Vector3.Zero) return result;
                result.Normalize();
                return result;
            }

            /// <summary>
            /// Ортональный вектор
            /// </summary>
            /// <returns>Ортогональный вектор</returns>
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
            /// Получение длины вектора
            /// </summary>
            /// <returns>Длина вектора (Sqrt(X*X + Y*Y + Z*Z))</returns>
            public double Length()
            {
                double lengthSquared = LengthSquared();
                return lengthSquared == 1.0 ? 1.0 : (double)Math.Sqrt(lengthSquared);
            }
            /// <summary>
            /// Получение квадрата длины вектора
            /// </summary>
            /// <returns>Квадрат длины вектора. (X*X + Y*Y + Z*Z)</returns>
            public double LengthSquared()
            {
                return (x * x + y * y + z * z);
            }
            /// <summary>
            /// Закрепить значения вектора в нуле используя данную толерантность
            /// </summary>
            /// <param name="tolerance">Толерантность</param>
            public void ClampZero(double tolerance)
            {
                x = (tolerance > Math.Abs(x)) ? 0.0f : x;
                y = (tolerance > Math.Abs(y)) ? 0.0f : y;
                z = (tolerance > Math.Abs(z)) ? 0.0f : z;
            }

            /// <summary>
            /// Закрепить значения вектора в нуле используя толерантность по умолчанию
            /// </summary>
            /// <remarks>
            /// Толерантное значение в этом случае - самое маленькое значимое для
            /// вещественного двойной точности Double.Epsilon
            /// </remarks>
            public void ClampZero()
            {
                ClampZero(Double.Epsilon);
            }

            /// <summary>
            /// Склярное произведение векторов
            /// </summary>
            /// <param name="vector">Второй вектор для скалярного произведения</param>
            /// <returns></returns>
            public double DotProduct(Vector3 vector)
            {
                return this.x * vector.X + this.y * vector.Y + this.z * vector.Z;
            }

            /// <summary>
            /// Вычисление векторного произведения двух векторов
            /// </summary>
            /// <param name="vector">Вектор</param>
            /// <returns>Вектор, содержащий результат векторного произведения</returns>
            public Vector3 CrossProduct(Vector3 vector)
            {
                return new Vector3(
                    this.Y * vector.Z - this.Z * vector.Y,
                    this.Z * vector.X - this.X * vector.Z,
                    this.X * vector.Y - this.Y * vector.X);
            }

            /// <summary>
            /// Вычисление угла между векторами
            /// </summary>
            /// <param name="vector">вектор</param>
            /// <returns>угол в радианах</returns>
            public double Angle(Vector3 vector)
            {
                double cosResult = this.DotProduct(vector) / (this.Length() * vector.Length());
                cosResult = (Math.Abs(cosResult) < 1.0) ? cosResult : (1.0 * Math.Sign(cosResult));
                return Math.Acos(cosResult);
            }

            /// <summary>
            /// Угол ПРОТИВ часовой стрелки от первого вектора ко второму
            /// </summary>
            /// <param name="beginVector">Вектор, от которого отсчитываем</param>
            /// <param name="endVector">Конечный вектор</param>
            /// <returns>Угол ПРОТИВ часовой стрелки от первого вектора ко второму</returns>
            public static double CounterclockwiseAngleBetween(Vector3 beginVector, Vector3 endVector)
            {
                // Определяет, большой или маленький угол
                double factor = beginVector.X * endVector.Y - endVector.X * beginVector.Y;

                // Угол дуги
                double betweenAngle = Vector3.Angle(beginVector, endVector);
                // Корректируем угол
                betweenAngle = factor > 0 ? betweenAngle : 2.0 * Math.PI - betweenAngle;
                return betweenAngle;
            }

            /// <summary>
            /// Вычисляем угол в радианах между двумя векторами 
            /// (от первого ко второму против часовой стрелки, если смотреть с вершины вектора directionAxe)
            /// </summary>
            /// <param name="firstVector">Первый вектор</param>
            /// <param name="secondVector">Второй вектор</param>
            /// <param name="directionAxe">Вектор определяет направление против часовой стрелки</param>
            /// <returns>Угол</returns>
            public static double CounterclockwiseAngleBetween(Vector3 firstVector, Vector3 secondVector, Vector3 directionAxe)
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
                double det = Vector3.DotProduct(z, directionAxe);
                if (det < 0) angle = 2 * Math.PI - angle;

                return angle;

            }

            /// <summary>
            /// Получаю геометрический центр массива векторов
            /// </summary>
            /// <param name="points">Двумерный вектор</param>
            /// <returns>Геометрический центр</returns>
            public static Vector3 GetCenter(Vector3[] points)
            {
                Vector3 center = new Vector3(0, 0, 0);
                foreach (Vector3 point in points)
                    center = center + point;
                return center / points.Length;
            }

            /// <summary>
            /// Меняет y и z местами
            /// </summary>
            /// <param name="vector">Вектор у которого меняются координаты</param>
            /// <returns>Вектор с измененными координатами</returns>
            static public Vector3 SwapYZ(Vector3 vector)
            {
                return new Vector3(vector.X, vector.Z, vector.Y);
            }

            #endregion

            #region Переопределенные методы

            /// <summary>
            /// Возвращает хасп-ключ для этого объекта
            /// </summary>
            /// <returns>32-bit хасп-ключ</returns>
            public override int GetHashCode()
            {
                return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
            }

            /// <summary>
            /// Проверка эквивалентности с другим объектом
            /// </summary>
            /// <param name="obj">Объект для сравнения</param>
            /// <returns>Истина, если объект эквивалентный, иначе ложь</returns>
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
            /// Проверка эквивалентности с другим вектором
            /// </summary>
            /// <param name="vector">Вектор для сравнения</param>
            /// <param name="tolerance">Толерантность</param>
            /// <returns>Истина, если объект эквивалентный, иначе ложь</returns>
            public bool Equals(Vector3 vector, double tolerance)
            {
                return ((Math.Abs(x - vector.X) < tolerance) && (Math.Abs(y - vector.Y) < tolerance) && (Math.Abs(z - vector.Z) < tolerance));
            }

            /// <summary>
            /// Строковое представление объекта
            /// </summary>
            /// <returns>Строка - представление объекта</returns>
            public override string ToString()
            {
                return string.Format(CultureInfo.InvariantCulture, "({0}; {1}; {2})", x, y, z);
            }

            #endregion

            #region Операторы сравнения

            /// <summary>
            /// Проверяет эквивалентность двух векторов
            /// </summary>
            /// <param name="u">Левосторонний операнд - вектор</param>
            /// <param name="v">Правосторонний операнд - вектор</param>
            /// <returns>Истина, если два вектора эквивалентны, иначе ложь</returns>
            public static bool operator ==(Vector3 u, Vector3 v)
            {
                return u.Equals((object)v);
            }

            /// <summary>
            /// Проверяет на отличность двух векторов
            /// </summary>
            /// <param name="u">Левосторонний операнд - вектор</param>
            /// <param name="v">Правосторонний операнд - вектор</param>
            /// <returns>Истина, если два вектора отличны, иначе ложь</returns>
            public static bool operator !=(Vector3 u, Vector3 v)
            {
                return !u.Equals((object)v);
            }

            /// <summary>
            /// Проверяет больше ли все компоненты одного вектора всех компонент другого
            /// </summary>
            /// <param name="u">Левосторонний операнд - вектор</param>
            /// <param name="v">Правосторонний операнд - вектор</param>
            /// <returns>
            /// Истина, если все компоненты левого операнда 
            /// окажутся больше другого, иначе ложь
            /// </returns>
            public static bool operator >(Vector3 u, Vector3 v)
            {
                return (
                    (u.x > v.x) &&
                    (u.y > v.y) &&
                    (u.z > v.z));
            }

            /// <summary>
            /// Проверяет меньше ли все компоненты одного вектора всех компонент другого
            /// </summary>
            /// <param name="u">Левосторонний операнд - вектор</param>
            /// <param name="v">Правосторонний операнд - вектор</param>
            /// <returns>
            /// Истина, если все компоненты левого операнда 
            /// окажутся меньше другого, иначе ложь
            /// </returns>
            public static bool operator <(Vector3 u, Vector3 v)
            {
                return (
                    (u.x < v.x) &&
                    (u.y < v.y) &&
                    (u.z < v.z));
            }

            /// <summary>
            /// Проверяет больше или равны компоненты одного вектора компонентам другого
            /// </summary>
            /// <param name="u">Левосторонний операнд - вектор</param>
            /// <param name="v">Правосторонний операнд - вектор</param>
            /// <returns>
            /// Истина, если все компоненты левого операнда 
            /// окажутся больше или равны другому, иначе ложь
            /// </returns>
            public static bool operator >=(Vector3 u, Vector3 v)
            {
                return (
                    (u.x >= v.x) &&
                    (u.y >= v.y) &&
                    (u.z >= v.z));
            }

            /// <summary>
            /// Проверяет меньше или равны компоненты одного вектора компонентам другого
            /// </summary>
            /// <param name="u">Левосторонний операнд - вектор</param>
            /// <param name="v">Правосторонний операнд - вектор</param>
            /// <returns>
            /// Истина, если все компоненты левого операнда 
            /// окажутся меньше или равны другому, иначе ложь
            /// </returns>
            public static bool operator <=(Vector3 u, Vector3 v)
            {
                return (
                    (u.x <= v.x) &&
                    (u.y <= v.y) &&
                    (u.z <= v.z));
            }

            #endregion

            #region Унарные операторы

            /// <summary>
            /// Обращение знака компонент вектора
            /// </summary>
            /// <param name="v">Вектор</param>
            /// <returns>Новый объект с обращенными значениями</returns>
            public static Vector3 operator -(Vector3 v)
            {
                return Vector3.Negate(v);
            }

            #endregion

            #region Бинарные операторы

            /// <summary>
            /// Сложение двух векторов
            /// </summary>
            /// <param name="u">Вектор, левое слагаемое</param>
            /// <param name="v">Вектор, правое слагаемое</param>
            /// <returns>Вектор, содержит сумму</returns>
            public static Vector3 operator +(Vector3 u, Vector3 v)
            {
                return Vector3.Add(u, v);
            }

            /// <summary>
            /// Складывает вектор и скаляр
            /// </summary>
            /// <param name="v">Вектор, левон слагаемое</param>
            /// <param name="s">Скаляр, правое слагаемое</param>
            /// <returns>Вектор, содержит сумму</returns>
            public static Vector3 operator +(Vector3 v, double s)
            {
                return Vector3.Add(v, s);
            }

            /// <summary>
            /// Складывает скаляр и вектор 
            /// </summary>
            /// <param name="s">Скаляр, левое слагаемое</param>
            /// <param name="v">Вектор, правое слагаемое</param>
            /// <returns>Вектор, содержит сумму</returns>
            public static Vector3 operator +(double s, Vector3 v)
            {
                return Vector3.Add(v, s);
            }

            /// <summary>
            /// Разница двух векторов
            /// </summary>
            /// <param name="u">Вектор, уменьшаемое</param>
            /// <param name="v">Вектор, вычитаемое</param>
            /// <returns>Вектор, содержит разницу</returns>
            /// <remarks>
            ///	result[i] = v[i] - w[i].
            /// </remarks>
            public static Vector3 operator -(Vector3 u, Vector3 v)
            {
                return Vector3.Subtract(u, v);
            }

            /// <summary>
            /// Вычитает скаляр из вектора
            /// </summary>
            /// <param name="v">Вектор, уменьшаемое</param>
            /// <param name="s">Скаляр, вычитаемое</param>
            /// <returns>Вектор, содержит разницу</returns>
            /// <remarks>
            /// result[i] = v[i] - s
            /// </remarks>
            public static Vector3 operator -(Vector3 v, double s)
            {
                return Vector3.Subtract(v, s);
            }

            /// <summary>
            /// Вычитает вектор из скаляра, который задает вектор (s,s,s)
            /// </summary>
            /// <param name="s">Скаляр, уменьшаемое</param>
            /// <param name="v">Вектор, вычитаемое</param>
            /// <returns>Вектор, содержит разницу</returns>
            /// <remarks>
            /// result[i] = s - v[i]
            /// </remarks>
            public static Vector3 operator -(double s, Vector3 v)
            {
                return Vector3.Subtract(s, v);
            }

            /// <summary>
            /// Умножение вектора на скаляр
            /// </summary>
            /// <param name="v">Вектор</param>
            /// <param name="s">Скаляр</param>
            /// <returns>Вектор, содержит произведение</returns>
            public static Vector3 operator *(Vector3 v, double s)
            {
                return Vector3.Multiply(v, s);
            }

            /// <summary>
            /// Умножение вектора на вектор
            /// </summary>
            /// <param name="v1">Вектор</param>
            /// <param name="v2">Вектор</param>
            /// <returns>Вектор, содержит попарное произведение координат</returns>
            public static Vector3 operator *(Vector3 v1, Vector3 v2)
            {
                return new Vector3(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
            }

            /// <summary>
            /// Умножение скаляра на вектор
            /// </summary>
            /// <param name="s">Скаляр</param>
            /// <param name="v">Вектор</param>
            /// <returns>Вектор, содержит произведение</returns>
            public static Vector3 operator *(double s, Vector3 v)
            {
                return Vector3.Multiply(v, s);
            }

            /// <summary>
            /// Деление вектора на скаляр
            /// </summary>
            /// <param name="v">Вектор, делимое</param>
            /// <param name="s">Скаляр, делитель</param>
            /// <returns>Вектор, содержит частное</returns>
            /// <remarks>
            /// result[i] = v[i] / s;
            /// </remarks>
            public static Vector3 operator /(Vector3 v, double s)
            {
                return Vector3.Divide(v, s);
            }

            /// <summary>
            /// Деление скаляра на вектор
            /// </summary>
            /// <param name="s">Скаляр, делимое</param>        
            /// <param name="v">Вектор, делитель</param>
            /// <returns>Вектор, содержит частное</returns>
            /// <remarks>
            /// result[i] = v[i] / s;
            /// </remarks>
            public static Vector3 operator /(double s, Vector3 v)
            {
                return Vector3.Divide(s, v);
            }

            #endregion

            #region Операторы доступа через индексы

            /// <summary>
            /// Индексы ( [x, y] ).
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

            public static Vector3 max(Vector3 a, Vector3 b)
            {
                return new Vector3(Math.Max(a.x, b.x), Math.Max(a.y, b.y), Math.Max(a.z, b.z));
            }

            public static Vector3 min(Vector3 a, Vector3 b)
            {
                return new Vector3(Math.Min(a.x, b.x), Math.Min(a.y, b.y), Math.Min(a.z, b.z));
            }

            public static Vector3 clamp(Vector3 v, float min, float max)
            {
                return new Vector3(NvMath.clamp(v.x, min, max), NvMath.clamp(v.y, min, max), NvMath.clamp(v.z, min, max));
            }

            public static Vector3 saturate(Vector3 v)
            {
                return new Vector3(NvMath.saturate((float)v.x), NvMath.saturate((float)v.y), NvMath.saturate((float)v.z));
            }

            public static Vector3 floor(Vector3 v)
            {
                return new Vector3(Math.Floor(v.x), Math.Floor(v.y), Math.Floor(v.z));
            }

            public static Vector3 ceil(Vector3 v)
            {
                return new Vector3(Math.Ceiling(v.x), Math.Ceiling(v.y), Math.Ceiling(v.z));
            }

            public static Vector3 lerp(Vector3 v1, Vector3 v2, float t)
            {
                float s = 1.0f - t;
                return new Vector3(v1.x * s + t * v2.x, v1.y * s + t * v2.y, v1.z * s + t * v2.z);
            }

            #endregion
        }

        #endregion

        #region Math

        private static class NvMath
        {
            public const float NV_EPSILON = 0.0001f;

            public static float saturate(float f)
            {
                return (float)clamp(f, 0.0f, 1.0f);
            }

            /// Clamp between two values.
            public static double clamp(double x, double a, double b)
            {
                return Math.Min(Math.Max(x, a), b);
            }

            /// Return the maximum of the three arguments.
            public static double max3(double x, double a, double b)
            {
                return Math.Max(Math.Max(x, a), b);
            }

            /// Return the maximum of the three arguments.
            public static double min3(double x, double a, double b)
            {
                return Math.Min(Math.Min(x, a), b);
            }

            public static void swap<T>(ref T o1, ref T o2)
            {
                var o3 = o1;
                o1 = o2;
                o2 = o3;
            }

            // Robust floating point comparisons:
            // http://realtimecollisiondetection.net/blog/?p=89
            public static bool equal(float f0, float f1, float epsilon = NV_EPSILON)
            {
                //return fabs(f0-f1) <= epsilon;
                return Math.Abs(f0 - f1) <= epsilon * max3(1.0f, (float)Math.Abs(f0), (float)Math.Abs(f1));
            }
        }

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

        static void optimizeEndPoints3(Vector3[] block, ref Vector3[] dxtPoints, ref uint indices)
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
            if (NvMath.equal(denom, 0.0f)) return;

            float factor = 1.0f / denom;

            Vector3 a = (alphax_sum * beta2_sum - betax_sum * alphabeta_sum) * factor;
            Vector3 b = (betax_sum * alpha2_sum - alphax_sum * alphabeta_sum) * factor;

            a = Vector3.clamp(a, 0, 255);
            b = Vector3.clamp(b, 0, 255);

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

        static void optimizeEndPoints4(Vector3[] block, ref Vector3[] dxtPoints, ref uint indices)
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
            if (NvMath.equal(denom, 0.0f)) return;

            float factor = 1.0f / denom;

            Vector3 a = (alphax_sum * beta2_sum - betax_sum * alphabeta_sum) * factor;
            Vector3 b = (betax_sum * alpha2_sum - alphax_sum * alphabeta_sum) * factor;

            a = Vector3.clamp(a, 0, 255);
            b = Vector3.clamp(b, 0, 255);

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

        static uint computeIndices3(Vector3[] block, Vector3 maxColor, Vector3 minColor)
        {
            Vector3[] palette = new Vector3[4];
            palette[0] = minColor;
            palette[1] = maxColor;
            palette[2] = (palette[0] + palette[1]) * 0.5f;

            uint indices = 0;
            for (int i = 0; i < 16; i++)
            {
                double d0 = colorDistance(palette[0], block[i]);
                double d1 = colorDistance(palette[1], block[i]);
                double d2 = colorDistance(palette[2], block[i]);

                uint index;
                if (d0 < d1 && d0 < d2) index = 0;
                else if (d1 < d2) index = 1;
                else index = 2;

                indices |= index << (2 * i);
            }

            return indices;
        }

        static uint computeIndices4(Vector3[] block, Vector3 maxColor, Vector3 minColor)
        {
            Vector3[] palette = new Vector3[4];
            palette[0] = maxColor;
            palette[1] = minColor;
            palette[2] = Vector3.lerp(palette[0], palette[1], 3.0f / 8.0f);
            palette[3] = Vector3.lerp(palette[0], palette[1], 5.0f / 8.0f);

            uint indices = 0;
            for (int i = 0; i < 16; i++)
            {
                double d0 = colorDistance(palette[0], block[i]);
                double d1 = colorDistance(palette[1], block[i]);
                double d2 = colorDistance(palette[2], block[i]);
                double d3 = colorDistance(palette[3], block[i]);

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

        static double colorDistance(Vector3 c0, Vector3 c1)
        {
            return (c0 - c1).LengthSquared();
        }

        // Takes a normalized color in [0, 255] range and returns 
        static ushort roundAndExpand(ref Vector3 v)
        {
            uint r = (uint)Math.Floor(NvMath.clamp(v.X * (31.0f / 255.0f), 0.0f, 31.0f));
            uint g = (uint)Math.Floor(NvMath.clamp(v.Y * (31.0f / 255.0f), 0.0f, 31.0f));
            uint b = (uint)Math.Floor(NvMath.clamp(v.Z * (31.0f / 255.0f), 0.0f, 31.0f));

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

                    UInt16 color0 = roundAndExpand(ref maxColor);
                    UInt16 color1 = roundAndExpand(ref minColor);
                    hasAlpha |= color0 == color1;

                    if (color0 < color1)
                    {
                        NvMath.swap(ref maxColor, ref minColor);
                        NvMath.swap(ref color0, ref color1);
                    }

                    dxtBoundPoints[0] = maxColor;
                    dxtBoundPoints[1] = minColor;
                    if (!hasAlpha)
                    {
                        uint indices = computeIndices4(inputPoints, maxColor, minColor);
                        optimizeEndPoints4(inputPoints, ref dxtBoundPoints, ref indices);
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

            c0 = roundAndExpand(ref dxtBoundPoints[0]);
            c1 = roundAndExpand(ref dxtBoundPoints[1]);

            if (c0 == c1) hasAlpha = true;
            if (c0 > c1 == hasAlpha)
            {
                NvMath.swap(ref c0, ref c1);
                NvMath.swap(ref dxtBoundPoints[0], ref dxtBoundPoints[1]);
            }

            if (c0 > c1) NvMath.swap(ref dxtBoundPoints[0], ref dxtBoundPoints[1]);

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
                                    c[ci] = roundAndExpand(ref cV[ci]);
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
