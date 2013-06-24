using System;
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

        public Menu(MainWindow window)
        {
            Guard.ArgumentNotNull(window, "window");
            this.window = window;
            window.KeyDown += window_KeyDown;
            window.World.Content = new TitleView();
        }

        public bool CanContinue
        {
            get { return world != null; }
        }

        private bool _IsVisible = true;
        public bool IsVisible
        {
            get { return _IsVisible; }
            set { Set(ref _IsVisible, value); }
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
        }

        private void window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    if (world != null) world.Stop();
                    IsVisible = true;
                    break;
            }
        }
    }
}