namespace PyCSS_parser.Extensions;

public static class ConsoleWriter
{
    private const ConsoleColor ErrorColor = ConsoleColor.Red;
    private const ConsoleColor InformationColor = ConsoleColor.Green;

    private static void Write(string message, ConsoleColor color)
    {
        var previousForegroundColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = previousForegroundColor;
    }

    public static void WriteError(string message) => Write(message, ErrorColor);
    public static void WriteInformation(string message) => Write(message, InformationColor);
}