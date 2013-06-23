using System.Windows;

namespace crystalgame
{
    public class Cloud : Entity
    {
        public double Bounciness { get; set; }

        public void Run(World world)
        {
            Pegasus pegasus = world.Pegasus;
            double angle = Vector.AngleBetween(Position, pegasus.Position);
            double r1 = EllipseRadiusAtAngle(Size, Angle + angle);
            double r2 = EllipseRadiusAtAngle(world.Pegasus.Size, pegasus.Angle + angle);
            double distance = (Position - pegasus.Position).Length - r1 - r2;
        }
    }
}
