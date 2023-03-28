using PyCSS_parser.Common.Exceptions;
using PyCSS_parser.Extensions;
using PyCSS_parser.Parser;
using PyCSS_parser.Tokenizer;
using PyCSS_parser.Validators;

if (!ArgumentsValidator.AreCorrect(args))
{
    return;
}

var fileContent = File.ReadAllText(args[0]);
ITokenizer tokenizer = new Tokenizer();
var tokens = tokenizer.TokenizeFile(fileContent);

IParser parser = new Parser(tokens);
try
{
    parser.Parse();
    ConsoleWriter.WriteInformation("Pomyślnie zwalidowano plik wejściowy.");
}
catch (TokenNotDefinedException tokenNotDefined)
{
    ConsoleWriter.WriteError(tokenNotDefined.Message);
}
catch (InvalidTokenException invalidToken)
{
    ConsoleWriter.WriteError(invalidToken.Message);
}