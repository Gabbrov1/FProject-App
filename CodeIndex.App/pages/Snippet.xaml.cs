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
        public object? ChosenSnippet { get; set; }

        private Dictionary<string, string> snippets = new();

        

    /// <summary>
        /// Constructor for SnippetControl that initializes the control with a file path
        /// and loads snippets asynchronously.
    /// </summary>
    /// <param name="eventArgs">Event arguments that may contain file selection data</param>
    public SnippetControl(EventArgs eventArgs)
    {
        if (eventArgs == null)
        {
            Console.WriteLine("No event arguments provided.");
            return;
        }    
        // Check if the passed event args is a FileSelectedEventArgs (custom event type)
        if (eventArgs is FileSelectedEventArgs fileEventArgs)
            // Extract the file path from the event args and store it
            FilePath = fileEventArgs.FilePath;

        // Initialize all XAML components defined in the designer
        InitializeComponent();

        // Start the async task to load snippets from the file
        // The _ discard operator means we don't need the Task return value
        _ = InitializeSnippetsAsync().ContinueWith(t =>
        {
            // Check if the async task threw an exception during execution
            if (t.Exception != null)
                // Display an error message box to the user with the exception details
                // InnerException gets the actual error (not the wrapper AggregateException)
                MessageBox.Show($"Load error: {t.Exception.InnerException?.Message}");
        }, 
        // Execute the continuation on the UI thread to ensure MessageBox displays correctly
        TaskScheduler.FromCurrentSynchronizationContext());
    }

        private async Task InitializeSnippetsAsync()
        {
            if (FilePath is null or "")
            {
                Console.WriteLine("No file path provided.");
                return;
            }

            try
            {
                pageLoader loader = new pageLoader();
                FileDetails fileDetails = await loader.loadPageAsync(FilePath);

                
                if (fileDetails?.CodeSnippets is not null && fileDetails.CodeSnippets.Count() > 0)
                {
                    snippets = fileDetails.CodeSnippets;
                }
                else
                {
                    snippets = new Dictionary<string, string>
                    {
                        { "N/A", "No snippets found in this file." }
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load snippets: {ex.Message}");
                snippets = new Dictionary<string, string>
                {
                    { "Error", ex.Message }
                };
            }

            SnippetCombo.ItemsSource = snippets.Keys;
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
        
        private void SnippetCombo_SelectionChanged(object sender,System.EventArgs e)
        {
            if (SnippetCombo.SelectedItem is not string key) return;
            if (snippets.TryGetValue(key, out string? code))
                CodeDisplay.Text = code;
        }
    }
}