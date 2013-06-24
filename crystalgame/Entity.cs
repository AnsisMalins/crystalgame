using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Utilities;

namespace crystalgame
{
    public class Entity : ViewModel
    {
        private FrameworkElement view;

        public Entity(FrameworkElement view)
        {
            Guard.ArgumentNotNull(view, "view");

            Size = new Vector(view.Width, view.Height);
            BoundingCircleRadiusSquared = Math.Pow(Math.Max(Size.X, Size.Y) / 2, 2);
            double left = Canvas.GetLeft(view);
            if (double.IsNaN(left)) left = 0;
            double top = Canvas.GetTop(view);
            if (double.IsNaN(top)) top = 0;
            Position = new Vector(left + Size.X / 2, top + Size.Y / 2);
            Velocity = GetVelocity(view);

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
            if (rotateTransform == null) rotateTransform = new RotateTransform();
            view.RenderTransform = rotateTransform;
            view.RenderTransformOrigin = new Point(0.5, 0.5);
            Angle = -rotateTransform.Angle * Math.PI / 180;
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

        public static double AngleBetween(Vector v1, Vector v2)
        {
            return Vector.AngleBetween(v1, v2) * Math.PI / 180;
        }

        public static bool AreNearby(Entity a, Entity b)
        {
            if (a == null || b == null) return false;
            return (b.Position - a.Position).LengthSquared
                - a.BoundingCircleRadiusSquared - b.BoundingCircleRadiusSquared <= 0;
        }

        public FrameworkElement CreateView()
        {
            var attr = Attribute.GetCustomAttribute(
                GetType(), typeof(DefaultViewAttribute)) as DefaultViewAttribute;
            if (attr == null) return null;
            view = Activator.CreateInstance(attr.ViewType) as FrameworkElement;
            if (view == null) return null;

            view.Width = Size.X;
            view.Height = Size.Y;
            Canvas.SetLeft(view, Left);
            Canvas.SetTop(view, Top);
            view.RenderTransform = new RotateTransform(-Angle * 180 / Math.PI);
            view.RenderTransformOrigin = new Point(0.5, 0.5);
            view.DataContext = this;

            return view;
        }
        
        public static double Direction(Vector v)
        {
            double direction = -Math.Atan2(v.Y, v.X);
            if (double.IsNaN(direction)) direction = 0;
            return direction;
        }

        public static double Distance(Entity a, Entity b)
        {
            if (a == null || b == null) return double.PositiveInfinity;
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

        public static Type GetType(FrameworkElement view)
        {
            Guard.ArgumentNotNull(view, "view");
            return view.GetValue(TypeProperty) as Type;
        }

        public static Vector GetVelocity(FrameworkElement view)
        {
            Guard.ArgumentNotNull(view, "view");
            return (Vector)view.GetValue(VelocityProperty);
        }

        public static Vector Normal(Vector v)
        {
            double length = v.Length;
            return length > 0 ? v / length : v;
        }

        public void Render()
        {
            Render(view);
        }

        public static void SetType(FrameworkElement view, Type value)
        {
            Guard.ArgumentNotNull(view, "view");
            view.SetValue(TypeProperty, value);
        }

        public static void SetVelocity(FrameworkElement view, Vector value)
        {
            Guard.ArgumentNotNull(view, "view");
            view.SetValue(VelocityProperty, value);
        }

        public virtual void Simulate(World world)
        {
        }

        public static readonly DependencyProperty TypeProperty
            = DependencyProperty.RegisterAttached("Type", typeof(Type), typeof(Entity));

        public static readonly DependencyProperty VelocityProperty
            = DependencyProperty.RegisterAttached("Velocity", typeof(Vector), typeof(Entity));

        protected virtual void Render(FrameworkElement view)
        {
        }
    }
}