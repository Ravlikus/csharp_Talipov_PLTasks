using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Task2
{
    class Program
    {


        static string numberPattern = "([-]?[0-9]*[.]?[0-9]+)";
        static string arrayPattern = $@"\[{numberPattern}, {numberPattern}, {numberPattern}\]";
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Необходимо задать имя файла первым аргументом");
                return;
            }
            var fileName = args[0];
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            if (!File.Exists(fileName))
            {
                Console.WriteLine($"Файл не найден");
                return;
            }
            var dataStr = File.ReadAllText(fileName);
            var center = GetSphereCenter(dataStr);
            var radius = GetSphereRadius(dataStr);
            var linePoints = GetLinePoints(dataStr);
            var crossPositions = GetCrossPositions(center, radius, linePoints.Item1, linePoints.Item2);
            if (crossPositions.Length == 0)
            {
                Console.WriteLine("Коллизий не найдено");
            }
            else
            {
                foreach(var position in crossPositions)
                {
                    Console.WriteLine($"{position.X}, {position.Y}, {position.Z}");
                }
            }
        }

        static Position GetSphereCenter(string dataStr)
        {
            var regex = new Regex($"center: {arrayPattern}");
            var centerData = regex.Match(dataStr);
            return new Position(
                double.Parse(centerData.Groups[1].Value),
                double.Parse(centerData.Groups[2].Value),
                double.Parse(centerData.Groups[3].Value));
        }

        static Tuple<Position, Position> GetLinePoints(string dataStr)
        {
            var regex = new Regex($"line: \\{{{arrayPattern}, {arrayPattern}\\}}");
            var lineData = regex.Match(dataStr);
            var a = new Position(
                double.Parse(lineData.Groups[1].Value),
                double.Parse(lineData.Groups[2].Value),
                double.Parse(lineData.Groups[3].Value));
            var b = new Position(
                double.Parse(lineData.Groups[4].Value),
                double.Parse(lineData.Groups[5].Value),
                double.Parse(lineData.Groups[6].Value));
            return new Tuple<Position, Position>(a,b);
        }

        static double GetSphereRadius(string dataStr)
        {
            var regex = new Regex($"radius: {numberPattern}");
            return double.Parse(regex.Match(dataStr).Groups[1].Value);
        }

        /// <summary>
        /// Находит точки пересечения прямой с окружностью
        /// </summary>
        /// <param name="c">центр окружности</param>
        /// <param name="r">радиус</param>
        /// <param name="a">первая точки на прямой</param>
        /// <param name="b">вторая точка на прямой</param>
        /// <returns></returns>
        static Position[] GetCrossPositions(Position c, double r, Position a, Position b)
        {
            // Вычисляем длины проекций
            var vx = b.X - a.X;
            var vy = b.Y - a.Y;
            var vz = b.Z - a.Z;

            // вычисляем коэф. огибающей
            var A = vx * vx + vy * vy + vz * vz;
            var B = 2.0 * (a.X * vx + a.Y * vy + a.Z * vz - vx * c.X - vy * c.Y - vz * c.Z);
            var C = a.X * a.X - 2 * a.X * c.X + c.X * c.X + a.Y * a.Y - 2 * a.Y * c.Y + c.Y * c.Y +
                   a.Z * a.Z - 2 * a.Z * c.Z + c.Z * c.Z - r * r;

            // дискриминант
            var D = B * B - 4 * A * C;

            if (D < 0)
            {
                return new Position[0];
            }

            double t1 = (-B - Math.Sqrt(D)) / (2.0 * A);

            var t1Solution = new Position(a.X * (1 - t1) + t1 * b.X,
                                         a.Y * (1 - t1) + t1 * b.Y,
                                         a.Z * (1 - t1) + t1 * b.Z);

            if (D == 0)
            {
                return new Position[1] { t1Solution };
            }

            double t2 = (-B + Math.Sqrt(D)) / (2.0 * A);

            var t2Solution = new Position(a.X * (1 - t2) + t2 * b.X,
                                         a.Y * (1 - t2) + t2 * b.Y,
                                         a.Z * (1 - t2) + t2 * b.Z);

            return new Position[] { t1Solution, t2Solution };
        }

        struct Position
        {
            public double X;
            public double Y;
            public double Z;

            public Position(double x, double y, double z)
            {
                X = x;
                Y = y;
                Z = z;
            }
        }


    }
}
