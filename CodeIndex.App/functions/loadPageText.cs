using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using CodeIndex.Core;
using System.Diagnostics;
using System.Linq.Expressions;


namespace CodeIndex.App
{
    public class pageLoader
    {

        public async Task<FileDetails>? loadPageAsync(string filePath)
        {
            if(!String.IsNullOrEmpty(filePath))
            {
                Match match = Regex.Match(filePath, @"(\.[a-zA-Z0-9]+)$");// Get everything after the dot in the file Extension;
                string extension = match.Groups[1].Value;

                switch (extension)
                {
                    case ".py":
                        // ASYNC read python file using custom reader
                        return await PythonReader(filePath);
                    case ".cs":
                        //ASYNC read c# file using custom reader
                        return await PythonReader(filePath);//TODO: Implement C# reader
                        
                    default:
                        throw new NotSupportedException($"File type {extension} is not supported.");
                }
            }
            return null;
        }

        /// <summary>
        /// Reads a Python file and extracts details about it.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>FileDetails </returns>
        private async Task<FileDetails> PythonReader(string path)
        {
            
            string extractorPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "functions", "Extractors", "pythonExtractor.exe");
            var process = new Process
            {

                StartInfo = new ProcessStartInfo
                {
                    FileName = extractorPath,
                    Arguments = $"\"{path}\"",
                    RedirectStandardOutput = false,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = false,
                }
            };

            Debug.WriteLine("Starting process...");
            process.Start();
            Debug.WriteLine("Process started.");

            var stderrTask = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();
            Debug.WriteLine($"Process exited with code: {process.ExitCode}");

            string error = stderrTask;

            Debug.WriteLine($"Error: {error}");

            if (!string.IsNullOrEmpty(error))
                throw new Exception($"Python error: {error}");


            string json = await File.ReadAllTextAsync("output.json");
            try
            {
                json = await File.ReadAllTextAsync("output.json");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading JSON file: {ex.Message}");
                throw;
            }
            var snippets = JsonSerializer.Deserialize<List<CodeSnippetClass>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return new FileDetails
            {
                Extension = ".py",
                Language = "Python",
                CommentSymbol = "#",
                CodeSnippets = snippets?.ToDictionary(s => s.Lineno, s => s.Source)
            };
        }
    }

}