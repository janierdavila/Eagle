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
        private EagleObserver _observer;
        public MainWindow()
        {
            InitializeComponent();
            this.Title = "Eagle Observer";

            TrayMinimizer.EnableMinimizeToTray(this);
            Loaded += OnWindowLoaded;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            this.OnStateChanged(EventArgs.Empty);

            _observer = new EagleObserver();
            _observer.Start();
        }

        public void ResetObserver()
        {
            _observer.Restart();
        }
    }
}
