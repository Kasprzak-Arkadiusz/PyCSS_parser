using PyCSS_parser;
using PyCSS_parser.Validators;

if (!ArgumentsValidator.AreCorrect(args))
{
    return;
}

ITokenizer tokenizer = new Tokenizer();
var tokens = tokenizer.TokenizeFile(args[0]);