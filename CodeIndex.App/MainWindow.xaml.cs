using System.Windows;


namespace CodeIndex.App;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var home = new homeControl();
        home.fileSelected += swap2Snippet;

        MainContent.Content = home;
    }

    private void swap2Snippet(object sender, EventArgs e)
    {
        MainContent.Content = new SnippetControl(e);

    }
}