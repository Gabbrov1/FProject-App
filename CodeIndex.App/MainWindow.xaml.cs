using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace CodeIndex.App;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
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
            MessageBox.Show(filePath);
            FilePathTextBox.Text = filePath;
        }
    }
}