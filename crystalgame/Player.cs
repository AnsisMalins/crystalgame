using System.Windows.Input;
using Utilities;

namespace crystalgame
{
    [DefaultView(typeof(HudView))]
    public class Player
    {
        private bool leftKeyDown;
        private bool rightKeyDown;
        private bool spaceKeyDown;

        public Player(MainWindow window)
        {
            Guard.ArgumentNotNull(window, "window");
            window.KeyDown += window_KeyDown;
            window.KeyUp += window_KeyUp;
        }

        public void Render()
        {
        }

        public void Simulate(World world)
        {
            Pegasus pegasus = world.Pegasus;
            if (pegasus == null) return;
            if (leftKeyDown) pegasus.Angle += pegasus.Agility;
            if (rightKeyDown) pegasus.Angle -= pegasus.Agility;
            pegasus.WingsSpread = spaceKeyDown;
        }

        void window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left: leftKeyDown = true; break;
                case Key.Right: rightKeyDown = true; break;
                case Key.Space: spaceKeyDown = true; break;
            }
        }

        void window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left: leftKeyDown = false; break;
                case Key.Right: rightKeyDown = false; break;
                case Key.Space: spaceKeyDown = false; break;
            }
        }
    }
}