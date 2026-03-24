namespace CodeIndex.Core{

// CodeIndex.Core
    public class FileSelectedEventArgs : EventArgs
    {
        public string FilePath { get; }

        public FileSelectedEventArgs(string filePath)
        {
            FilePath = filePath;
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