namespace PyCSS_parser.Common;

public static class Tokens
{
    public const string NewLineCharacter = "\n";
    public const string CommentBeginning = "//";

    public static readonly IReadOnlySet<string> Combinators = new HashSet<string>
    {
        "+", ">", "<", ",", ":"
    };
}