namespace PyCSS_parser;

public interface ITokenizer
{
    IReadOnlyList<string> TokenizeFile(string path);
}