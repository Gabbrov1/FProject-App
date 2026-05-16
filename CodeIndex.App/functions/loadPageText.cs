using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using CodeIndex.Core;
using System.Diagnostics;
using System.Windows;

namespace CodeIndex.App
{
    public class pageLoader
    {
        public async Task<FileDetails?> loadPageAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new InvalidDataException($"FilePath not set. FilePath: {filePath}");

            string extension = Regex.Match(filePath, @"(\.[a-zA-Z0-9]+)$").Groups[1].Value;

            return extension switch
            {
                ".py" or ".cs" => await PythonReaderAsync(filePath, extension),
                _ => throw new NotSupportedException($"File type {extension} is not supported.")
            };
        }

        private async Task<FileDetails> PythonReaderAsync(string path, string extension)
        {
            string extractorPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "functions", "Extractors", "pythonExtractor.exe");

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = extractorPath,
                    Arguments = $"\"{path}\"",
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    #if DEBUG
                        CreateNoWindow = true,
                    #else
                        CreateNoWindow = false,
                    #endif
                }
            };

            process.Start();
            string stderr = await process.StandardError.ReadToEndAsync();

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            try { await process.WaitForExitAsync(cts.Token); }
            catch (OperationCanceledException) { process.Kill(); throw new Exception("Python extractor timed out."); }

            if (process.ExitCode != 0)
            {
                HandlePythonError(process.ExitCode, stderr);
                throw new Exception($"Python extractor failed with exit code {process.ExitCode}.");
            }

            if (!string.IsNullOrEmpty(stderr))
                Debug.WriteLine($"Python extractor stderr: {stderr}");

            string json = await File.ReadAllTextAsync("./temp/output.json");
            if (string.IsNullOrEmpty(json))
                throw new Exception("JSON output is empty.");

            var snippets = new List<CodeSnippetClass>();

            try
            {
                var root = JsonNode.Parse(json);
                if (root is JsonObject obj)
                    foreach (var fileEntry in obj)
                        foreach (JsonNode? item in fileEntry.Value?["Content"]?.AsArray() ?? new JsonArray())
                            if (item is not null)
                            {
                                var snippet = JsonSerializer.Deserialize<CodeSnippetClass>(item.ToJsonString(),
                                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                                if (snippet is not null) snippets.Add(snippet);
                            }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error parsing JSON: {ex.Message}");
                throw;
            }

            return new FileDetails
            {
                Extension = extension,
                Language = extension == ".py" ? "Python" : "C#",
                CommentSymbol = extension == ".py" ? "#" : "//",
                CodeSnippets = snippets
            };
        }

        public void HandlePythonError(int exitCode, string errorMessage)
        {
            Application.Current.Dispatcher.Invoke(() =>
                MessageBox.Show($"Python extractor failed with exit code {exitCode}.\nError: {errorMessage}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error));
        }
    }
}