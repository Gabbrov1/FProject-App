
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace CodeIndex.App
{
    public partial class homeControl: UserControl{

        public event EventHandler fileSelected;
        public homeControl(){
            InitializeComponent();
        }

        
        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Code files (*.cs;*.py;*.js)|*.cs;*.py;*.js|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                string filePath = dialog.FileName;
                //MessageBox.Show(filePath); // Display the selected file path in the TextBox
                FilePathTextBox.Text = filePath;
                SelectFileButton.IsEnabled = false;

                CancelButton.Visibility = Visibility.Visible;

                if (!String.IsNullOrEmpty(filePath))
                {
                    fileSelected?.Invoke(this, EventArgs.Empty);
                    
                }
            }

            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            FilePathTextBox.Text = string.Empty;
            SelectFileButton.IsEnabled = true;
            CancelButton.Visibility = Visibility.Collapsed;
        }
    }
}