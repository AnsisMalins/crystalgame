using System.Windows;
using Utilities;

namespace crystalgame
{
    [DefaultView(typeof(HoopView))]
    public class Hoop : Entity
    {
        public Hoop(FrameworkElement view)
            : base(view)
        {
            if (view.Name == "Finish")
                IsFinish = true;
        }

        public bool IsFinish { get; set; }

        public bool IsVisited { get; set; }

        public override void Simulate(World world)
        {
            if (IsVisited) return;
            Pegasus pegasus = world.Pegasus;
            if (!AreNearby(this, pegasus)) return;

            double angle = Direction(pegasus.Position - Position);
            double radius = EllipseRadius(Size, angle - Angle);
            if ((pegasus.Position - Position).Length < radius)
            {
                world.Player.Score++;
                IsVisited = true;
            }
        }

        protected override void Render(FrameworkElement view)
        {
        }
    }
}