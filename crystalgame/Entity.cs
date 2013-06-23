using System;
using System.Windows;

namespace crystalgame
{
    public class Entity
    {
        public double Angle { get; set; }

        public Vector Position { get; set; }

        public Vector Size { get; set; }

        public Vector Velocity { get; set; }

        public void Run(World world)
        {
        }

        public static double EllipseRadiusAtAngle(Vector size, double angle)
        {
            double x = Math.Cos(angle) * size.X;
            double y = Math.Sin(angle) * size.Y;
            return new Vector(x, y).Length;
        }
    }
}