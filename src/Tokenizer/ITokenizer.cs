namespace PyCSS_parser.Tokenizer;

public interface ITokenizer
{
    IReadOnlyList<string> TokenizeFile(string path);
}