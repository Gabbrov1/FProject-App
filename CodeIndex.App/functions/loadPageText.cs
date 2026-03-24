using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using CodeIndex.Core;
using System.Diagnostics;


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
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = $"./Extractors/pythonExtractor.py \"{path}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            Debug.WriteLine("Starting process...");
            process.Start();
            Debug.WriteLine("Process started.");

            // Read both simultaneously using Task.WhenAll to prevent buffer deadlock
            var stdoutTask = process.StandardOutput.ReadToEndAsync();
            var stderrTask = process.StandardError.ReadToEndAsync();

            Debug.WriteLine("Waiting for output...");
            await Task.WhenAll(stdoutTask, stderrTask);
            Debug.WriteLine("Output received.");

            await process.WaitForExitAsync();
            Debug.WriteLine($"Process exited with code: {process.ExitCode}");

            string json = stdoutTask.Result;
            string error = stderrTask.Result;

            Debug.WriteLine($"JSON: {json}");
            Debug.WriteLine($"Error: {error}");

            if (!string.IsNullOrEmpty(error))
                throw new Exception($"Python error: {error}");

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