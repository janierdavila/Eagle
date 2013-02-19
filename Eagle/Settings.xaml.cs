using System;
using System.IO;
using System.Linq;
using System.Windows;
using Eagle.Model;
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
            //Only update if user provided one
            if (!string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                //Need to do this with the password. NO DP
                _model.SmtpInfo.Password = txtPassword.Password;
            }

            _model.Save();
            this.Close();
        }

        private void AddDirectory_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDir.Text))
            {
                MessageBox.Show("Invalid Directory", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFilter.Text))
            {
                MessageBox.Show("Invalid Filter", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_model.Directories.Any(d => d.FileName == txtDir.Text))
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

            _model.Directories.Add(new PathInfo { FileName = txtDir.Text, Filter = txtFilter.Text });
            txtDir.Text = string.Empty;
            txtFilter.Text = string.Empty;
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

            var pathInfo = (PathInfo)LsbDirectories.SelectedItem;
            _model.Directories.Remove(pathInfo);
        }

        private void BtnEdit_OnClick(object sender, RoutedEventArgs e)
        {
            if (LsbDirectories.SelectedItem == null)
            {
                return;
            }
            var pathInfo = (PathInfo)LsbDirectories.SelectedItem;
            var editWindow = new EditPathWindow();
            editWindow.DataContext = pathInfo;
            editWindow.ShowDialog();
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

            if (_model.Emails.Contains(txtEmail.Text))
            {
                MessageBox.Show("Email had already been added.", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            if (_fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtDir.Text = _fbd.SelectedPath;
            }
        }

        private void BtnRemoveAllFolders_OnClick(object sender, RoutedEventArgs e)
        {
            _model.Directories.Clear();
        }

        private void BtnRemoveAllEmails_OnClick(object sender, RoutedEventArgs e)
        {
            _model.Emails.Clear();
        }


    }
}
