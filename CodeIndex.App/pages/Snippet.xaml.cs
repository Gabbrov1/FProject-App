using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Windows;
using System.Windows.Controls;
using CodeIndex.Core;

namespace CodeIndex.App
{
    public partial class SnippetControl : UserControl
    {
        public string? FilePath { get; set; }

        private Dictionary<string?, string> snippets;

        public SnippetControl(EventArgs eventArgs)
        {
            Console.WriteLine("SnippetControl initialized with event args: " + eventArgs);
            if (eventArgs is FileSelectedEventArgs fileEventArgs)
            {
                FilePath = fileEventArgs.FilePath;
            }

            InitializeComponent();
            InitializeSnippetsAsync();
        }

        private async Task InitializeSnippetsAsync()
        {
            if (string.IsNullOrEmpty(FilePath))
            {
                MessageBox.Show("No file path provided.");
                return;
            }
            bool firstLoad = true;
            if(!firstLoad)
            {
                pageLoader loader = new pageLoader();
                FileDetails fileDetails = await loader.loadPageAsync(FilePath);
                if (fileDetails == null)
                {
                    
                    MessageBox.Show("Failed to load file details.");
                    return;
                }
            
                if (fileDetails?.CodeSnippets != null)
                {
                    snippets = fileDetails.CodeSnippets;
                }
                else
                {
                    // If Python failed or returned nothing, manually create the fallback
                    snippets = new Dictionary<string?, string>
                    {
                        { "N/A", "No snippets found in this file." }
                    };
                }
            }
            SnippetCombo.ItemsSource = snippets.Keys;
            if (firstLoad)
            {
                firstLoad = false;
                MessageBox.Show("Snippets loaded. Please select a snippet from the dropdown.");
                await InitializeSnippetsAsync(); // Load snippets after initial load
            }
        }

        private void LoadSnippet_Click(object sender, RoutedEventArgs e)
        {
            string key = SnippetCombo.SelectedItem as string;
            // Tries to get value for snippet key, if it exists display it in the TextBlock
            if (snippets.TryGetValue(key, out string code))
            {
                CodeDisplay.Text = code;
            }
        }

        private async void RefreshSnippets_ClickAsync(object sender, RoutedEventArgs e)
        {
            try 
            {
                await InitializeSnippetsAsync();
            }
            catch (Exception ex)
            {
                // This will tell you if it's a Python error, a File error, or a Logic error.
                MessageBox.Show($"Refresh failed: {ex.Message}");
            }
        }

    }
}