using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CodeIndex.App
{
    public partial class SnippetControl : UserControl
    {
        private Dictionary<string, string> snippets;

        public SnippetControl()
        {
            InitializeComponent();
            InitializeSnippets();
        }

        private void InitializeSnippets()
        {
            snippets = new Dictionary<string, string>
            {
                { "Hello World", "Console.WriteLine(\"Hello, World!\");" }
            };

            SnippetCombo.ItemsSource = snippets.Keys;
        }

        private void LoadSnippet_Click(object sender, RoutedEventArgs e)
        {
            if (SnippetCombo.SelectedItem is string selectedSnippet &&
                snippets.TryGetValue(selectedSnippet, out var code))
            {
                CodeDisplay.Text = code;
            }
        }
    }
}