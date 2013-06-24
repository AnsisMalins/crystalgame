using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Utilities;

namespace crystalgame
{
    public class Entity
    {
        private FrameworkElement view;

        public Entity(FrameworkElement view)
        {
            Guard.ArgumentNotNull(view, "view");
            this.view = view;
            Size = new Vector(view.Width, view.Height);
            BoundingCircleRadiusSquared = Math.Pow(Math.Max(Size.X, Size.Y) / 2, 2);
            double left = Canvas.GetLeft(view);
            if (double.IsNaN(left)) left = 0;
            double top = Canvas.GetTop(view);
            if (double.IsNaN(top)) top = 0;
            Position = new Vector(left + Size.X / 2, top + Size.Y / 2);

            var rotateTransform = view.RenderTransform as RotateTransform;
            if (rotateTransform == null)
            {
                var transformGroup = view.RenderTransform as TransformGroup;
                if (transformGroup != null)
                {
                    foreach (Transform i in transformGroup.Children)
                    {
                        rotateTransform = i as RotateTransform;
                        if (rotateTransform != null) break;
                    }
                }
            }
            if (rotateTransform == null) rotateTransform = new RotateTransform(0, 0, 0);
            view.RenderTransform = rotateTransform;
            view.RenderTransformOrigin = new Point(0.5, 0.5);
            Angle = rotateTransform.Angle * Math.PI / 180;
        }

        public double Angle { get; set; }

        public double BoundingCircleRadiusSquared { get; private set; }

        public double Left
        {
            get { return Position.X - Size.X / 2; }
        }

        public Vector Position { get; set; }

        public Vector Size { get; set; }

        public double Top
        {
            get { return Position.Y - Size.Y / 2; }
        }

        public Vector Velocity { get; set; }

        public static bool AreNearby(Entity a, Entity b)
        {
            return (b.Position - a.Position).LengthSquared
                - a.BoundingCircleRadiusSquared - b.BoundingCircleRadiusSquared <= 0;
        }
        
        public static double Direction(Vector v)
        {
            double direction = -Math.Atan2(v.Y, v.X);
            if (double.IsNaN(direction)) direction = 0;
            return direction;
        }

        public static double Distance(Entity a, Entity b)
        {
            double angle = Direction((b.Position - a.Position));
            double ra = EllipseRadius(a.Size, angle - a.Angle);
            double rb = EllipseRadius(b.Size, angle - b.Angle);
            return (a.Position - b.Position).Length - ra - rb;
        }

        public static double EllipseRadius(Vector size, double angle)
        {
            double x = Math.Cos(angle) * size.X / 2;
            double y = Math.Sin(angle) * size.Y / 2;
            return new Vector(x, y).Length;
        }

        public static Vector GetVelocity(FrameworkElement view)
        {
            Guard.ArgumentNotNull(view, "view");
            return (Vector)view.GetValue(VelocityProperty);
        }

        public void Render()
        {
            Render(view);
        }

        public virtual void Simulate(World world)
        {
        }

        public static void SetVelocity(FrameworkElement view, Vector value)
        {
            Guard.ArgumentNotNull(view, "view");
            view.SetValue(VelocityProperty, value);
        }

        public static readonly DependencyProperty VelocityProperty
            = DependencyProperty.RegisterAttached("Velocity", typeof(Vector), typeof(Entity));

        protected virtual void Render(FrameworkElement view)
        {
        }
    }
}