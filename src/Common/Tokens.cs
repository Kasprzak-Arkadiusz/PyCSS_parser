namespace PyCSS_parser.Common;

public static class Tokens
{
    public const string NewLineCharacter = "\n";
    public const string CommentBeginning = "//";
    public const string Indent = "\t";
    public const string DeclarationEnding = ";";

    public static readonly IReadOnlySet<string> Combinators = new HashSet<string>
    {
        "+", ">", "<", ",", ":"
    };
}