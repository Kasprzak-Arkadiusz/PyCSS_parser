using PyCSS_parser.Common;
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
}
catch (TokenNotDefinedException e)
{
    ConsoleWriter.WriteError(e.Message);
}

ConsoleWriter.WriteInformation("Pomyślnie zwalidowano plik wejściowy.");