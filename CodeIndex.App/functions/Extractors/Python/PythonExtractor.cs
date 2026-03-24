

using System.Diagnostics;
using System.Text.Json;

namespace CodeIndex.App
{
    public class PythonExtractor
    {

        public static async Task<string> RunPythonAsync(string scriptPath, object data)
        {
            
            var inputJson = JsonSerializer.Serialize(data);

            var psi = new ProcessStartInfo
            {
                FileName = "python",           // or "python" on Windows
                Arguments = scriptPath,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = psi })
            {
                process.Start();

                // Write the JSON input to the Python script
                await process.StandardInput.WriteLineAsync(inputJson);
                process.StandardInput.Close();

                // Read the output from the Python script
                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();

                if (!string.IsNullOrEmpty(error))
                {
                    throw new Exception($"Python error: {error}");
                }

                return output;
            }
        }
    }
}

public class PythonNode
{
    public string Type { get; set; }
    public string Name { get; set; }
    public int Start { get; set; }
    public int End { get; set; }
    public string Docstring { get; set; }
    public List<PythonNode> Children { get; set; }
}