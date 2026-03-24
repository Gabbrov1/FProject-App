using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using CodeIndex.Core;

namespace CodeIndex.App
{
    public partial class SnippetControl : UserControl
    {
        public string? FilePath { get; set; }

        private Dictionary<int, string> snippets;

        public SnippetControl(EventArgs eventArgs)
        {
            Console.WriteLine("SnippetControl initialized with event args: " + eventArgs);
            if (eventArgs is FileSelectedEventArgs fileEventArgs)
            {
                FilePath = fileEventArgs.FilePath;
            }

            InitializeComponent();
            InitializeSnippets();
        }

        private void InitializeSnippets()
        {
            if (string.IsNullOrEmpty(FilePath))
            {
                MessageBox.Show("No file path provided.");
                return;
            }

            pageLoader loader = new pageLoader();
            FileDetails fileDetails = loader.loadPageAsync(FilePath)?.Result;
            snippets = fileDetails?.CodeSnippets ?? new Dictionary<int, string>();


            SnippetCombo.ItemsSource = snippets.Keys;
        }

        private void LoadSnippet_Click(object sender, RoutedEventArgs e)
        {
            int key = (int)SnippetCombo.SelectedItem;
            // Tries to get value for snippet key, if it exists display it in the TextBlock
            if (snippets.TryGetValue(key, out string code))
            {
                CodeDisplay.Text = code;
            }
        }

        private void RefreshSnippets_Click(object sender, RoutedEventArgs e)
        {
            InitializeSnippets();
        }

    }
}