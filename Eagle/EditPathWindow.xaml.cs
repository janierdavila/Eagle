using System.Windows;

namespace Eagle
{
    /// <summary>
    /// Interaction logic for EditPathWindow.xaml
    /// </summary>
    public partial class EditPathWindow : Window
    {
        public EditPathWindow()
        {
            InitializeComponent();
        }

        private void BtnOk_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
