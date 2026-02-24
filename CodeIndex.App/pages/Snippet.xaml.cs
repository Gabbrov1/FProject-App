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
                { "Hello World", "Console.WriteLine(\"Hello, World!\");" },
                { "For Loop", "for (int i = 0; i < 10; i++)\n{\n    Console.WriteLine(i);\n}" },
                { "If Statement", "if (condition)\n{\n    // Your code here\n}" },
                { "Try Catch", "try\n{\n    // Your code here\n}\ncatch (Exception ex)\n{\n    Console.WriteLine(ex.Message);\n}" }
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