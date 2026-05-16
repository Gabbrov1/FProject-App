using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CodeIndex.App
{
    public partial class UploadControl : UserControl
    {

        private CancellationTokenSource _cts;
        private FileHandler fileHandler;
        private ObservableCollection<JsonFile> jsonFiles = new();

        public UploadControl()
        {
            _cts = new CancellationTokenSource();
            InitializeComponent();
            fileHandler = new FileHandler();
            JsonFilesList.ItemsSource = jsonFiles;

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private async Task LoadJsonFiles()
        {
            try
            {
                var freshFiles = fileHandler.GetJsonFilesFromTemp();

                // Remove files that no longer exist
                var toRemove = jsonFiles.Where(f => !freshFiles.Any(n => n.FilePath == f.FilePath)).ToList();
                foreach (var file in toRemove)
                    jsonFiles.Remove(file);

                // Add new files that aren't already listed
                foreach (var fresh in freshFiles)
                {
                    if (!jsonFiles.Any(f => f.FilePath == fresh.FilePath))
                        jsonFiles.Add(fresh);
                }

                StatusText.Text = $"Found {jsonFiles.Count} JSON files";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error: {ex.Message}";
            }
        }


        private async Task RefreshTimer(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await LoadJsonFiles();
                await Task.Delay(TimeSpan.FromSeconds(2), ct);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _cts = new CancellationTokenSource();
            _ = RefreshTimer(_cts.Token);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _cts.Cancel();
        }

        private void SelectAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var file in jsonFiles)
            {
                file.IsSelected = true;
            }
        }

        private void SelectAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var file in jsonFiles)
            {
                file.IsSelected = false;
            }
        }

        private void FileCheckBox_Click(object sender, RoutedEventArgs e)
        {
            bool allSelected = jsonFiles.All(f => f.IsSelected);
            SelectAllCheckBox.IsChecked = allSelected;
        }

        private async void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedFiles = jsonFiles.Where(f => f.IsSelected).ToList();
            if (selectedFiles.Count == 0) { StatusText.Text = "No files selected"; return; }

            var items = new List<object>();

            foreach (var file in selectedFiles)
            {
                try
                {
                    var root = JsonNode.Parse(File.ReadAllText(file.FilePath!));
                    if (root is null) continue;

                    if (root is JsonObject obj)
                        foreach (var entry in obj)
                            foreach (var item in entry.Value?["Content"]?.AsArray() ?? new JsonArray())
                                if (item is not null) items.Add(item);

                    else if (root is JsonArray arr)
                        foreach (JsonNode? arrItem in arr)
                            if (arrItem is not null) items.Add(arrItem);
                }
                catch (Exception ex) { StatusText.Text = $"Error: {ex.Message}"; return; }
            }

            if (items.Count == 0) { StatusText.Text = "No content found"; return; }

            try
            {
                var json = JsonSerializer.Serialize(new { files = items });
                await new ApiClient().PostAsync("http://127.0.0.1:5000/upload/", json);
                StatusText.Text = $"Uploaded {items.Count} functions from {selectedFiles.Count} files";
            }
            catch (Exception ex) { StatusText.Text = $"Upload error: {ex.Message}"; }
        }
        

    }

    
}