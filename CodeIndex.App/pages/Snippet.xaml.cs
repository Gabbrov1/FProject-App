using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using CodeIndex.Core;

namespace CodeIndex.App
{
    public partial class SnippetControl : UserControl
    {
        public string FilePath { get; set; }

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

            pageLoader loader = new pageLoader();
            FileDetails fileDetails = loader.loadPageAsync(FilePath).Result;
            snippets = fileDetails?.CodeSnippets ?? new Dictionary<int, string>();


            SnippetCombo.ItemsSource = snippets.Keys;
        }

        private void LoadSnippet_Click(object sender, RoutedEventArgs e)
        {
            if (SnippetCombo.SelectedItem is int selectedKey && snippets.TryGetValue(selectedKey, out string code))
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