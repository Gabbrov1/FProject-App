using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using CodeIndex.Core;


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

                FileDetails fileDetails;


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
                        return null;
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
            FileDetails fileContent =  new FileDetails
            {
                Extension = ".py",
                Language = "Python",
                CommentSymbol = "#",
                CodeSnippets = new Dictionary<int, string>
                {
                    { 1, $"def hello_world():\n    print(\"Hello, World! {path}\")" }
                }
            };

            return fileContent;
        }
    }

}