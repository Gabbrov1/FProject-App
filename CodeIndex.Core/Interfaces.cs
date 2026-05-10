/// Mental Model and structure.
/// 
/// Tasks Required:
///  - Process Start.
///  - User selects file to read.
///  - Start up the Embeding API and let it idle.
///  - File Extraction to JSON.
///  - Extract JSON snippets to memory from file. lets say 10 at a time.
///  - Optionally allow the user to view the data.
///  - Send data to Embeding API.
///  - Once API result is available, Upload to Backend API.
///  - API will return result and code.
///  - Display result to user.
interface IFileProcessor
{
    /// <summary>
    /// Asynchronously processes a file and extracts code snippets.
    /// </summary>
    /// <param name="filePath">The path of the file to process.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a dictionary of snippet names and their corresponding code.</returns>
    Task<Dictionary<string?, string>> ProcessFileAsync(string filePath);

    
}