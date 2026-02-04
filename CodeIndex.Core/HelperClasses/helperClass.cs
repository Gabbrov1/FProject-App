namespace CodeIndex.Core;

public enum validFileTypes
{
    py,
    cs,
    js,
    jsx,
    ts,
    tsx,
}

public interface IFileManagement
{
    void getFiles(string path, validFileTypes fileType= validFileTypes.py);

    void readFile();

    void tokenizeFile();
}
