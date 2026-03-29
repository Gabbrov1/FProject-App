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

    public class CodeSnippetClass
    {
        public string Name { get; set; }
        public string Kind { get; set; }
        public int Lineno { get; set; }
        public int EndLineno { get; set; }
        public string Source { get; set; }
    }
    public class FileDetails
    {
        public required string Extension { get; set; }
        public string? Language { get; set; }
        public string? CommentSymbol { get; set; }

        public Dictionary<string, string>? CodeSnippets { get; set; }


    }
}