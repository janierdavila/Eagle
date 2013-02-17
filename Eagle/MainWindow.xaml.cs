using System;
using System.Windows;

using Eagle.Utility;



namespace Eagle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TrayMinimizer.EnableMinimizeToTray(this);
            Loaded += OnWindowLoaded;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            this.OnStateChanged(EventArgs.Empty);
        }
    }
}
