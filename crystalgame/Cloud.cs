using System.Windows;
using Utilities;

namespace crystalgame
{
    [DefaultView(typeof(CloudView))]
    public class Cloud : Entity
    {
        public Cloud(FrameworkElement view)
            : base(view)
        {
            Bounce = GetBounce(view);
        }

        public double Bounce { get; set; }

        public static double GetBounce(FrameworkElement view)
        {
            Guard.ArgumentNotNull(view, "view");
            return (double)view.GetValue(BounceProperty);
        }

        public override void Simulate(World world)
        {
            Pegasus pegasus = world.Pegasus;
            if (!Entity.AreNearby(this, pegasus)) return;
            double distance = Distance(this, pegasus);
            if (distance > 0) return;
            pegasus.Velocity += (pegasus.Position - Position) * -distance * Bounce * world.Speed;
        }

        public static void SetBounce(FrameworkElement view, double value)
        {
            Guard.ArgumentNotNull(view, "view");
            view.SetValue(BounceProperty, value);
        }

        public static readonly DependencyProperty BounceProperty
            = DependencyProperty.RegisterAttached(
            "Bounce", typeof(double), typeof(Cloud), new PropertyMetadata(1.0));
    }
}