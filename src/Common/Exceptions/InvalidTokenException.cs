namespace PyCSS_parser.Common.Exceptions;

public class InvalidTokenException : Exception
{
    public InvalidTokenException(int lineNumber, string message)
        : base($"Nieoczekiwany token w linii nr. {lineNumber}.\n" + message) { }
}