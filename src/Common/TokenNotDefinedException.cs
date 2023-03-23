namespace PyCSS_parser.Common;

public class TokenNotDefinedException : Exception
{
    public TokenNotDefinedException(string message) : base(message) { }
}