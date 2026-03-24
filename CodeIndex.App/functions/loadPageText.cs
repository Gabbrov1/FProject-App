using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;


namespace CodeIndex.App
{
    public class pageLoader
    {

        public async Task<string> loadPageAsync(string filePath)
        {
            string fileDetails = "Unknown";
            if(!String.IsNullOrEmpty(filePath.Trim()))
            {
                Match match = Regex.Match(filePath, @"(\.[a-zA-Z0-9]+)$");// Get everything after the dot in the file Extension;
                string extension = match.Groups[1].Value;
                fileDetails = await getFileDetailsFromJsonAsync(extension);


                switch (fileDetails)
                {
                    case "C#":
                        fileDetails = "This is a C# file.";
                        break;
                    case "Python":
                        fileDetails = "This is a Python file.";
                        break;
                    case "JavaScript":
                        fileDetails = "This is a JavaScript file.";
                        break;
                    default:
                        fileDetails = $"This is an unknown file type with extension {extension}.";
                        break;
                }
            }
            
            return fileDetails;
        }

        public async Task<string> getFileDetailsFromJsonAsync(string fileType, string location = "filetypes.json")
        {
            string json = File.ReadAllText("data.json");

            Root root = JsonSerializer.Deserialize<Root>(json);

            return root.Extensions.Where(e => e.Extension == fileType).FirstOrDefault()?.Language ?? "Unknown";
        }

    }

    public class fileCodeExtractor
    {
        public async Task<string> extractCodeAsync(string filePath, FileExtension fileExtension)
        {
            
            switch (fileExtension.Extension)
            {
                case ".cs":
                    // Call C# code extraction logic
                    break;
                case ".py":
                    // Call Python code extraction logic

                    string result = await PythonExtractor.RunPythonAsync("./Extractors/Python/extract.py", new { FilePath = filePath });
                    break;
                case ".js": 
                    // Call JavaScript code extraction logic
                    break;
                default:
                    // Handle unknown file types
                    break;
            }



            // Placeholder for code extraction logic
            return $"Extracted code from {filePath} with extension {fileExtension.Extension}.";
        }
    }

    public class FileExtension
    {
        public required string Extension { get; set; }
        public string? Language { get; set; }
        public string? CommentSymbol { get; set; }
    }

    public class Root
    {
        public List<FileExtension> Extensions { get; set; }
    }
}