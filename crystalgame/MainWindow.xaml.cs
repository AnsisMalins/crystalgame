using System.Windows;
using Utilities;

namespace crystalgame
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Menu.Content = View.Create(new Menu(this));
        }
    }
}