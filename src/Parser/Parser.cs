namespace PyCSS_parser.Parser;

public class Parser : IParser
{

    public void Parse(IReadOnlyList<string> tokens)
    {
        for (var i = 0; i < tokens.Count; i++)
        {
            if (tokens[i] == Tokens.NewLineCharacter)
            {
                continue;
            }
        }
    }
}