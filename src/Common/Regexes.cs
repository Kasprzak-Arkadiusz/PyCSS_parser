using System.Text.RegularExpressions;

namespace PyCSS_parser.Common;

public static class Regexes
{
    public static Regex Identifier { get; } = new(@"^[:]?[a-zA-Z\*\#_\.][0-9a-zA-Z\*\#_\.]*$");
    public static Regex ExpressionLabel { get; } = new(@"^[a-z]+(?:[-]?[a-z])+$");
    public static Regex UnitValue { get; } = new(@"[0-9][.]?[0-9]+(?:px|rem|em|cm|mm|in|pt|pc|ch|vw|vh|vmin|vmax|%)");
    public static Regex ColorHexValue { get; } = new(@"^#[0-9a-fA-F]{3}$|^#[0-9a-fA-F]{6}$");
    public static Regex UrlValue { get; } = new(@"url\([\w|\?|\=]*\)");
    public static Regex StringValue { get; } = new(@"""[\w| *]*""");
    public static Regex TextValue { get; } = new(@"^[a-zA-Z]+$");
    public static Regex NumberValue { get; } = new(@"[1-9][0-9]*");
}