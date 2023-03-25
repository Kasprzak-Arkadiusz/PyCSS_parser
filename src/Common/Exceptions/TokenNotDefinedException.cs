namespace PyCSS_parser.Common.Exceptions;

public class TokenNotDefinedException : Exception
{
    public TokenNotDefinedException(string message) : base(message) { }
}