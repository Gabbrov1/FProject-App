using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;


namespace CodeIndex.App
{
    public class pageLoader
    {

        public pageLoader()
        {
            
        }

        public async Task<string> loadPageAsync(string filePath)
        {
            if(!String.IsNullOrEmpty(filePath.Trim()))
            {
                Match match = Regex.Match(filePath, @"(\.[a-zA-Z0-9]+)$");// Get everything after the dot in the file Extension;
                string extension = match.Groups[1].Value;
                string fileDetails = await getFileDetailsFromJsonAsync(extension);

            }
            
            return "";
        }

        public async Task<string> getFileDetailsFromJsonAsync(string fileType, string location = "filetypes.json")
        {
            string json = File.ReadAllText("data.json");

            Root root = JsonSerializer.Deserialize<Root>(json);

            return "";
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