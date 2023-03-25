using System.Text.RegularExpressions;

namespace PyCSS_parser.Common;

public static class Regexes
{
    public static Regex Identifier { get; } = new(@"^[:]?[a-zA-Z\*\#_\.][0-9a-zA-Z\*\#_\.]*$");
}