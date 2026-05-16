using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace CodeIndex.App
{
    public class FileHandler
    {
        public ObservableCollection<JsonFile> GetJsonFilesFromTemp()
        {
            string tempPath = Path.GetFullPath("temp/");
            
            if (!Directory.Exists(tempPath))
            {
                return new ObservableCollection<JsonFile>();
            }
            
            var files = Directory.GetFiles(tempPath, "*.json", SearchOption.TopDirectoryOnly)
                .Select(f => new JsonFile 
                { 
                    FileName = Path.GetFileName(f), 
                    FilePath = f, 
                    IsSelected = false 
                })
                .ToList();

            return new ObservableCollection<JsonFile>(files);
        }
    }

    public class JsonFile : INotifyPropertyChanged
    {
        public string? FileName { get; set; }
        public string? FilePath { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(nameof(IsSelected)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}