using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using CodeIndex.Core;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Threading;


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
                        return await PythonReaderAsync(filePath);
                    case ".cs":
                        //ASYNC read c# file using custom reader
                        return await PythonReaderAsync(filePath);//TODO: Implement C# reader
                        
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
        private async Task<FileDetails> PythonReaderAsync(string path)
        {
            
            string extractorPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "functions", "Extractors", "pythonExtractor.exe");

            bool createWindow = false; // Set to true to show the console window for debugging
            #if DEBUG
                createWindow = true;
            #endif

            using var process = new Process
            {

                StartInfo = new ProcessStartInfo
                {
                    FileName = extractorPath,
                    Arguments = $"\"{path}\"",
                    RedirectStandardOutput = false,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = createWindow,
                }
            };
            Console.WriteLine("Starting process...");
            process.Start();
            Console.WriteLine("Process started.");

            string stderrTask = await process.StandardError.ReadToEndAsync();

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60)); // 60 second timeout
            try
            {
                await process.WaitForExitAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                process.Kill();
                throw new Exception("Python extractor timed out after 60 seconds.");
            }

            int exitCode = process.ExitCode;

            // Check for errors
            if (exitCode != 0)
            {
                HandlePythonError(exitCode, stderrTask);
                throw new Exception($"Python extractor failed with exit code {exitCode}.");
            }

            // Log python error output for debugging
            string error = stderrTask;

            Debug.WriteLine($"Error: {error}");

            if (!string.IsNullOrEmpty(error))
            {
                Debug.WriteLine($"Python extractor error: {error}");
            }

            string? json = null;
            try
            {
                json = await File.ReadAllTextAsync("./temp/output.json");
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("JSON output file not found.");
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading JSON file: {ex.Message}");
                throw;
            }
            if (string.IsNullOrEmpty(json))
            {
                Debug.WriteLine("JSON output is empty.");
                throw new Exception("JSON output is empty.");
            }

            List<CodeSnippetClass?>? snippets = null;

            
            try
            {
                snippets = JsonSerializer.Deserialize<List<CodeSnippetClass>>
                (json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                Debug.WriteLine("JSON output read successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error logging JSON content: {ex.Message}");
            }
            

            return new FileDetails
            {
                Extension = ".py",
                Language = "Python",
                CommentSymbol = "#",
                CodeSnippets = snippets?
                .Where(s => s is not null)          // filter out null items
                .ToDictionary(
                    s => String.IsNullOrEmpty(s!.Name) ?    // sets the key to Name if it exists
                        s.Lineno.ToString() : s.Name,       // otherwise its set to line number
                s => s!.Source)                             // sets the source as value
            };
        }

        public void HandlePythonError(int exitCode, string errorMessage)
        {
            // The simple way to show an error
            System.Windows.Application.Current.Dispatcher.Invoke(() => 
            {
                MessageBox.Show($"Python extractor failed with exit code {exitCode}.\nError message: {errorMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }
    }

}