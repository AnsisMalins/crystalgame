using System.Windows;
using System.Windows.Media;
using Utilities;

namespace crystalgame
{
    public class Camera : Entity
    {
        private TranslateTransform translate;

        public Camera(FrameworkElement view)
            : base(view)
        {
            translate = new TranslateTransform(Left, Top);
            var worldView = view.Parent as FrameworkElement;
            if (worldView != null) worldView.RenderTransform = translate;
            Speed = GetSpeed(view);
        }

        public double Speed { get; set; }

        public static double GetSpeed(FrameworkElement view)
        {
            Guard.ArgumentNotNull(view, "view");
            return (double)view.GetValue(SpeedProperty);
        }

        public static void SetSpeed(FrameworkElement view, double value)
        {
            Guard.ArgumentNotNull(view, "view");
            view.SetValue(SpeedProperty, value);
        }

        public override void Simulate(World world)
        {
            Pegasus pegasus = world.Pegasus;
            if (pegasus == null) return;
            Position += (pegasus.Position - Position) * Speed;
        }

        public static readonly DependencyProperty SpeedProperty
            = DependencyProperty.RegisterAttached(
            "Speed", typeof(double), typeof(Camera), new PropertyMetadata(0.1));

        protected override void Render(FrameworkElement view)
        {
            translate.X = -Left;
            translate.Y = -Top;
        }
    }
}