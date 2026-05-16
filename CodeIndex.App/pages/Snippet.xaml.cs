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

        private List<CodeSnippetClass> snippets = new();

        

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
            if (string.IsNullOrEmpty(FilePath)) return;

            try
            {
                var loader = new pageLoader();
                FileDetails? fileDetails = await loader.loadPageAsync(FilePath);

                snippets = fileDetails?.CodeSnippets ?? new List<CodeSnippetClass>
                {
                    new() { Name = "N/A", Source = "No snippets found.", Kind = "", Lineno = 0, EndLineno = 0 }
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load snippets: {ex.Message}");
                snippets = new List<CodeSnippetClass>
                {
                    new() { Name = "Error", Source = ex.Message, Kind = "", Lineno = 0, EndLineno = 0 }
                };
            }

            SnippetCombo.ItemsSource = snippets.Select(s => $"{s.Name} ({s.Kind}, line {s.Lineno})");
        }

        private void SnippetCombo_SelectionChanged(object sender, System.EventArgs e)
        {
            if (SnippetCombo.SelectedIndex < 0 || SnippetCombo.SelectedIndex >= snippets.Count) return;
            CodeDisplay.Text = snippets[SnippetCombo.SelectedIndex].Source;
        }
        

        private void UploadSnippet_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Upload functionality is not implemented yet.");
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