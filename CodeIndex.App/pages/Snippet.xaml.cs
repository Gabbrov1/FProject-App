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
        private Dictionary<string?, string> snippets = new();

        public SnippetControl(EventArgs eventArgs)
        {
            if (eventArgs is FileSelectedEventArgs fileEventArgs)
                FilePath = fileEventArgs.FilePath;

            InitializeComponent();

            _ = InitializeSnippetsAsync().ContinueWith(t =>
            {
                if (t.Exception != null)
                    MessageBox.Show($"Load error: {t.Exception.InnerException?.Message}");
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async Task InitializeSnippetsAsync()
        {
            if (string.IsNullOrEmpty(FilePath))
            {
                MessageBox.Show("No file path provided.");
                return;
            }

            try
            {
                pageLoader loader = new pageLoader();
                FileDetails? fileDetails = await loader.loadPageAsync(FilePath);

                if (fileDetails?.CodeSnippets != null && fileDetails.CodeSnippets.Count > 0)
                {
                    snippets = fileDetails.CodeSnippets;
                }
                else
                {
                    snippets = new Dictionary<string?, string>
                    {
                        { "N/A", "No snippets found in this file." }
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load snippets: {ex.Message}");
                snippets = new Dictionary<string?, string>
                {
                    { "Error", ex.Message }
                };
            }

            SnippetCombo.ItemsSource = snippets.Keys;
        }

        private void LoadSnippet_Click(object sender, RoutedEventArgs e)
        {
            if (SnippetCombo.SelectedItem is not string key) return;
            if (snippets.TryGetValue(key, out string? code))
                CodeDisplay.Text = code;
        }

        private async void RefreshSnippets_ClickAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                await InitializeSnippetsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Refresh failed: {ex.Message}");
            }
        }
    }
}