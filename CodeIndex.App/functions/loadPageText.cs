using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;


namespace CodeIndex.App
{
    public class pageLoader
    {

        public async Task<string> loadPageAsync(string filePath)
        {
            if(!String.IsNullOrEmpty(filePath))
            {
                Match match = Regex.Match(filePath, @"(\.[a-zA-Z0-9]+)$");// Get everything after the dot in the file Extension;
                string extension = match.Groups[1].Value;

                KeyValuePair<int, string> fileDetails;


                switch (extension)
                {
                    case ".py":
                        await File.ReadAllTextAsync(filePath);
                        break;
                    case ".cs":
                        await File.ReadAllTextAsync(filePath);
                        break;
                    default:
                        return "Unsupported file type.";
                }
                string fileDetails = await getFileDetailsFromJsonAsync(extension);

            }
            
            return "";
        }


    }
    public class FileDetails
    {
        public required string Extension { get; set; }
        public string? Language { get; set; }
        public string? CommentSymbol { get; set; }

        public Dictionary<int, string>? CodeSnippets { get; set; }


    }
}