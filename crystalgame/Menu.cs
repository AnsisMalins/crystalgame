using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Utilities;

namespace crystalgame
{
    [DefaultView(typeof(MenuView))]
    public class Menu : ViewModel
    {
        private MainWindow window;
        private World world;
        private bool fullscreen;

        public Menu(MainWindow window)
        {
            Guard.ArgumentNotNull(window, "window");
            this.window = window;
            window.KeyDown += window_KeyDown;
            window.PreviewKeyDown += window_PreviewKeyDown;
            window.World.Content = new TitleView();
            Exec.OnMain(() => Focus());
        }

        public bool CanContinue
        {
            get { return world != null; }
        }

        private bool _IsVisible = true;
        public bool IsVisible
        {
            get { return _IsVisible; }
            set
            {
                if (!Set("IsVisible", ref _IsVisible, value)) return;
                window.World.IsEnabled = !_IsVisible;
                if (world != null) world.IsRunning = !_IsVisible;
                if (_IsVisible) Focus();
            }
        }

        public void Continue()
        {
            if (!CanContinue) return;
            IsVisible = false;
            world.Start();
        }

        public void Credits()
        {
            IsVisible = false;
            DestroyScene();
            window.World.Content = new CreditsView();
        }

        public void Exit()
        {
            window.Close();
        }

        public void NewGame()
        {
            IsVisible = false;
            DestroyScene();
            var levelView = new LevelView();
            world = new World(levelView.World);
            world.Player = new Player(window);
            window.World.Content = levelView;
            window.Hud.Content = View.Create(world.Player);
            world.Start();
            OnPropertyChanged("CanContinue");

            if (Debugger.IsAttached) BindingOperations.SetBinding(
                window, Window.TitleProperty, new Binding("Fps")
            {
                Source = world,
                Converter = ValueConverters.FormatString,
                ConverterParameter = "{0} FPS"
            });
        }

        public void Tutorial()
        {
            IsVisible = false;
            DestroyScene();
            window.World.Content = new TutorialView();
        }

        private void DestroyScene()
        {
            if (world != null)
            {
                world.Dispose();
                world = null;
            }
            window.World.Content = null;
            window.Hud.Content = null;
        }

        private void Focus()
        {
            var menuView = window.Menu.Content as FrameworkElement;
            if (menuView == null) return;
            var newGame = menuView.FindName("NewGame") as UIElement;
            if (newGame == null) return;
            Exec.OnMain(() => Keyboard.Focus(newGame));
        }

        private void window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    IsVisible = !IsVisible;
                    break;
            }
        }

        private void window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.F11) return;

            if (!fullscreen)
            {
                bool maximized = window.WindowState == WindowState.Maximized;
                window.WindowState = WindowState.Normal;
                window.WindowStyle = WindowStyle.None;
                if (maximized) window.WindowState = WindowState.Maximized;
                window.ResizeMode = ResizeMode.NoResize;
            }
            else
            {
                window.WindowStyle = WindowStyle.SingleBorderWindow;
                window.ResizeMode = ResizeMode.CanResize;
            }
            fullscreen = !fullscreen;

            e.Handled = true;
        }
    }
}