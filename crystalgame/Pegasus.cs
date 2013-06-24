using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Utilities;

namespace crystalgame
{
    [DefaultView(typeof(PegasusView))]
    public class Pegasus : Entity
    {
        public Pegasus(FrameworkElement view)
            : base(view)
        {
            Agility = GetAgility(view);
            Drag = GetDrag(view);
            Lift = GetLift(view);
            MaxSpeed = GetMaxSpeed(view);
        }

        public double Agility { get; set; }

        public Vector Drag { get; set; }

        private bool _IsFacingLeft;
        public bool IsFacingLeft
        {
            get { return _IsFacingLeft; }
            set { Set(ref _IsFacingLeft, value); }
        }

        public double Lift { get; set; }

        public double MaxSpeed { get; set; }

        public bool WingsSpread { get; set; }

        public static double GetAgility(FrameworkElement view)
        {
            Guard.ArgumentNotNull(view, "view");
            return (double)view.GetValue(AgilityProperty);
        }

        public static Vector GetDrag(FrameworkElement view)
        {
            Guard.ArgumentNotNull(view, "view");
            return (Vector)view.GetValue(DragProperty);
        }

        public static double GetLift(FrameworkElement view)
        {
            Guard.ArgumentNotNull(view, "view");
            return (double)view.GetValue(LiftProperty);
        }

        public static double GetMaxSpeed(FrameworkElement view)
        {
            Guard.ArgumentNotNull(view, "view");
            return (double)view.GetValue(MaxSpeedProperty);
        }

        public static void SetAgility(FrameworkElement view, double value)
        {
            Guard.ArgumentNotNull(view, "view");
            view.SetValue(AgilityProperty, value);
        }

        public static void SetDrag(FrameworkElement view, Vector value)
        {
            Guard.ArgumentNotNull(view, "view");
            view.SetValue(DragProperty, value);
        }

        public static void SetLift(FrameworkElement view, double value)
        {
            Guard.ArgumentNotNull(view, "view");
            view.SetValue(LiftProperty, value);
        }

        public static void SetMaxSpeed(FrameworkElement view, double value)
        {
            Guard.ArgumentNotNull(view, "view");
            view.SetValue(MaxSpeedProperty, value);
        }

        public override void Simulate(World world)
        {
            Vector uFw = new Vector(Math.Cos(Angle), -Math.Sin(Angle));
            double aUp = Angle + (IsFacingLeft ? -1 : 1) * Math.PI / 2;
            Vector uUp = new Vector(Math.Cos(aUp), -Math.Sin(aUp));

            double vFw = Velocity * uFw;
            double vUp = Velocity * -uUp;

            Vector fDragX = -uFw * vFw * Math.Abs(vFw) * Drag.X;
            Vector fDragY = uUp * vUp * Math.Abs(vUp) * Drag.Y;
            Vector fDrag = fDragX + fDragY;

            Vector fGravity = new Vector(0, world.Gravity);

            Vector fLift = uUp * vFw * vFw * Lift;

            Velocity += (fDrag + fGravity + fLift) * world.Speed;

            // Sanity clamping.
            Velocity = new Vector(
                double.IsNaN(Velocity.X) ? 0 : Velocity.X,
                double.IsNaN(Velocity.Y) ? 0 : Velocity.Y);
            Velocity = new Vector(
                Math.Min(Math.Max(Velocity.X, -MaxSpeed), MaxSpeed),
                Math.Min(Math.Max(Velocity.Y, -MaxSpeed), MaxSpeed));

            Position += Velocity * world.Speed;

            if (Angle < -Math.PI) Angle += Math.PI * 2;
            else if (Angle > Math.PI) Angle -= Math.PI * 2;
        }

        public static readonly DependencyProperty AgilityProperty
            = DependencyProperty.RegisterAttached(
            "Agility", typeof(double), typeof(Pegasus), new PropertyMetadata(0.1));

        public static readonly DependencyProperty DragProperty
            = DependencyProperty.RegisterAttached(
            "Drag", typeof(Vector), typeof(Pegasus), new PropertyMetadata(new Vector(0.00001, 0.4)));

        public static readonly DependencyProperty LiftProperty
            = DependencyProperty.RegisterAttached(
            "Lift", typeof(double), typeof(Pegasus), new PropertyMetadata(0.001));

        public static readonly DependencyProperty MaxSpeedProperty
            = DependencyProperty.RegisterAttached(
            "MaxSpeed", typeof(double), typeof(Pegasus), new PropertyMetadata(1500.0));

        protected override void Render(FrameworkElement view)
        {
            Canvas.SetLeft(view, Left);
            Canvas.SetTop(view, Top);
            (view.RenderTransform as RotateTransform).Angle = -Angle * 180 / Math.PI;
            IsFacingLeft = Angle < -Math.PI / 2 || Angle > Math.PI / 2;
        }
    }
}