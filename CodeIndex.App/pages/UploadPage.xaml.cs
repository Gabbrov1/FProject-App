using System;
using System.IO;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;
using System.Linq;
using System.Net.Http;

namespace CodeIndex.App
{
    public partial class UploadControl : UserControl
    {
        private FileHandler fileHandler;
        private ObservableCollection<JsonFile> jsonFiles;

        public UploadControl()
        {
            InitializeComponent();
            fileHandler = new FileHandler();
            LoadJsonFiles();
        }

        private void LoadJsonFiles()
        {
            try
            {
                jsonFiles = fileHandler.GetJsonFilesFromTemp();
                JsonFilesList.ItemsSource = jsonFiles;
                StatusText.Text = $"Found {jsonFiles.Count} JSON files";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error: {ex.Message}";
            }
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
            
            if (selectedFiles.Count == 0)
            {
                StatusText.Text = "No files selected";
                return;
            }

            var apiClient = new ApiClient();
            
            // Read file contents
            var filesToUpload = new List<object>();
            
            foreach (var file in selectedFiles)
            {
                try
                {
                    string fileContent = File.ReadAllText(file.FilePath);
                    filesToUpload.Add(new 
                    { 
                        fileName = file.FileName, 
                        content = fileContent 
                    });
                }
                catch (Exception ex)
                {
                    StatusText.Text = $"Error reading {file.FileName}: {ex.Message}";
                    return;
                }
            }
            
            // Serialize to JSON
            string jsonData = JsonSerializer.Serialize(new { files = filesToUpload });
            
            try
            {
                string response = await apiClient.PostAsync("https://api.example.com/upload", jsonData);
                StatusText.Text = $"Uploaded {selectedFiles.Count} files";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error: {ex.Message}";
            }
        }
    }

    
}