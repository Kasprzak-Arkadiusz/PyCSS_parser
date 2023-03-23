namespace PyCSS_parser.Parser;

public interface IParser
{
    void Parse(IReadOnlyList<string> tokens);
}