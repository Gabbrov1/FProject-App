using System.Windows;
using CodeIndex.Core;

namespace CodeIndex.App;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        LoadHomeControl();
    }

    private void LoadHomeControl()
    {
        MainContent.Content = new HomeControl();
        if (MainContent.Content is HomeControl home)
        {
            home.fileSelected += OnFileSelected;
        }
    }

    private void OnFileSelected(object? sender, EventArgs e)
    {
        if (e is FileSelectedEventArgs fileArgs)
        {
            MainContent.Content = new SnippetControl(fileArgs);
        }
    }

    private void UploadMenu_Click(object sender, RoutedEventArgs e)
    {
        
        MainContent.Content = new UploadControl();
    }

    private void HomeMenu_Click(object sender, RoutedEventArgs e)
    {
        LoadHomeControl();
    }
}