using System;
using System.IO;
using System.Windows;
using Eagle.Utility;

namespace Eagle
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private static EagleConfigurationModel _model;
        private System.Windows.Forms.FolderBrowserDialog _fbd;

        public Settings()
        {
            InitializeComponent();
            this.Loaded += OnLoaded;

            _model = EagleConfigurationModel.Current;
            this.DataContext = _model;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _fbd = new System.Windows.Forms.FolderBrowserDialog { RootFolder = Environment.SpecialFolder.Desktop };
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            _model.Save();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            _model.Save();
            this.Close();
        }

        private void AddDirectory_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDir.Text))
            {
                MessageBox.Show("Nothing to add", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_model.Directories.Contains(txtDir.Text))
            {
                MessageBox.Show("Directory had already been added.", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                if (!Directory.Exists(txtDir.Text))
                {
                    MessageBox.Show("Directory is invalid or inaccessible.", "Error", MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    return;
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _model.Directories.Add(txtDir.Text);
            txtDir.Text = string.Empty;
        }

        private void RemoveDir_Click(object sender, RoutedEventArgs e)
        {
            if (LsbDirectories.Items.Count <= 0)
            {
                MessageBox.Show("Nothing to delete", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (LsbDirectories.SelectedItem == null)
            {
                MessageBox.Show("Please, select something to delete", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var str = LsbDirectories.SelectedItem.ToString();
            _model.Directories.Remove(str);
        }

        private void AddEmail_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Nothing to add", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!Utilities.IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Invalid Email...", "Invalid Email", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _model.Emails.Add(txtEmail.Text);
            txtEmail.Text = string.Empty;
        }

        private void RemoveEmail_Click(object sender, RoutedEventArgs e)
        {
            if (LsbEmails.Items.Count <= 0)
            {
                MessageBox.Show("Nothing to delete", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (LsbEmails.SelectedItem == null)
            {
                MessageBox.Show("Please, select something to delete", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var str = LsbEmails.SelectedItem.ToString();
            _model.Emails.Remove(str);
        }

        private void AddFilter_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtExtension.Text))
            {
                MessageBox.Show("Nothing to add", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_model.Exts.Count == 1)
            {
                MessageBox.Show("Only one filter is supported at the moment. Use *.* for all files", "Only one filter...", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            _model.Exts.Add(txtExtension.Text);
            txtExtension.Text = string.Empty;
        }

        private void RemoveFilter_Click(object sender, RoutedEventArgs e)
        {
            if (LsbExtensions.Items.Count <= 0)
            {
                MessageBox.Show("Nothing to delete", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (LsbExtensions.SelectedItem == null)
            {
                MessageBox.Show("Please, select something to delete", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var str = LsbExtensions.SelectedItem.ToString();
            _model.Exts.Remove(str);
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            if (_fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtDir.Text = _fbd.SelectedPath;
            }
        }
    }
}
