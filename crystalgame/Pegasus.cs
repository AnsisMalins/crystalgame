using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Utilities;

namespace crystalgame
{
    public class Pegasus : Entity
    {
        public Pegasus(FrameworkElement view)
            : base(view)
        {
            Agility = 0.1;
        }

        public double Agility { get; set; }

        public double HorizontalDrag { get; set; }

        public double VerticalDrag { get; set; }

        public double Lift { get; set; }

        public bool WingsSpread { get; set; }

        public override void Simulate(World world)
        {
            Vector vForward = new Vector(Math.Cos(Angle), Math.Sin(Angle));
            Velocity += vForward * world.Speed;
            Position += Velocity * world.Speed;
        }

        protected override void Render(FrameworkElement view)
        {
            Canvas.SetLeft(view, Left);
            Canvas.SetTop(view, Top);
            (view.RenderTransform as RotateTransform).Angle = Angle * 180 / Math.PI;
        }
    }
}