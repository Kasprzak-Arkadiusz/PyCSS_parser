using System.Text;
using PyCSS_parser.Common;
using PyCSS_parser.Common.Exceptions;
using static PyCSS_parser.Common.Tokens;

namespace PyCSS_parser.Parser;

public class Parser : IParser
{
    private readonly IReadOnlyList<string> _tokens;
    private int _lineNumber;

    public Parser(IReadOnlyList<string> tokens)
    {
        _tokens = tokens;
        _lineNumber = 1;
    }

    public void Parse()
    {
        for (var i = 0; i < _tokens.Count; i++)
        {
            var token = _tokens[i];
            if (token == NewLineCharacter)
            {
                _lineNumber++;
            }
            else if (token == CommentBeginning)
            {
                i++;
                i = ParseComment(i);
            }
            else if (Regexes.Identifier.Match(token).Success)
            {
                i++;
                i = ParseClause(i);
            }
            else
            {
                throw new TokenNotDefinedException(FormatErrorMessage(i,
                    $"Analizowany token nie został uwzględniony w gramatyce: \"{token}\"\n"));
            }
        }
    }

    private int ParseComment(int currentTokenIndex)
    {
        while (currentTokenIndex < _tokens.Count - 1)
        {
            if (_tokens[currentTokenIndex] == NewLineCharacter)
            {
                _lineNumber++;
                return currentTokenIndex;
            }

            currentTokenIndex++;
        }

        return currentTokenIndex;
    }

    private int ParseClause(int currentTokenIndex)
    {
        currentTokenIndex = ParseClauseHeader(currentTokenIndex);
        currentTokenIndex = ParseClauseBody(currentTokenIndex);
        return currentTokenIndex;
    }

    private int ParseClauseHeader(int currentTokenIndex)
    {
        while (_tokens[currentTokenIndex] != NewLineCharacter &&
               _tokens[currentTokenIndex] != CommentBeginning)
        {
            var isCombinator = Combinators.Contains(_tokens[currentTokenIndex]);
            if (isCombinator)
            {
                currentTokenIndex++;
                switch (_tokens[currentTokenIndex])
                {
                    case NewLineCharacter:
                        currentTokenIndex++;
                        _lineNumber++;
                        break;
                    case CommentBeginning:
                        currentTokenIndex++;
                        currentTokenIndex = ParseComment(currentTokenIndex);
                        break;
                }
            }

            if (!Regexes.Identifier.Match(_tokens[currentTokenIndex]).Success)
            {
                currentTokenIndex++;
                throw new InvalidTokenException(FormatErrorMessage(currentTokenIndex,
                    $"Identyfikator powinien być zgodny z następującym wyrażeniem regularnym:\n {Regexes.Identifier} \n"));
            }

            currentTokenIndex++;
        }

        if (_tokens[currentTokenIndex] == CommentBeginning)
        {
            currentTokenIndex++;
            currentTokenIndex = ParseComment(currentTokenIndex);
            currentTokenIndex++;
            return currentTokenIndex;
        }

        _lineNumber++;
        currentTokenIndex++;
        return currentTokenIndex;
    }

    private int ParseClauseBody(int currentTokenIndex)
    {
        while (currentTokenIndex < _tokens.Count - 1)
        {
            if (_tokens[currentTokenIndex] != Indent)
            {
                throw new InvalidTokenException(FormatErrorMessage(currentTokenIndex,
                    "Brakujące wcięcie (oczekiwana tabulacja) w ciele wyrażenia.\n"));
            }

            currentTokenIndex++;
            currentTokenIndex = ParseExpression(currentTokenIndex);

            if (_tokens[currentTokenIndex] == DeclarationEnding)
            { 
                currentTokenIndex++;
            }
            else if (_tokens[currentTokenIndex] == NewLineCharacter &&
                     _tokens[currentTokenIndex + 1] != NewLineCharacter)
            {
                throw new InvalidTokenException(FormatErrorMessage(currentTokenIndex - 1,
                    "Wyrażenia muszą kończyć się średnikiem.\n"));
            }

            if (_tokens[currentTokenIndex] == NewLineCharacter)
            {
                currentTokenIndex++;
                _lineNumber++;
            }

            if (_tokens[currentTokenIndex] != NewLineCharacter)
            {
                continue;
            }

            _lineNumber++;
            return currentTokenIndex;
        }

        return currentTokenIndex;
    }

    private int ParseExpression(int currentTokenIndex)
    {
        if (!Regexes.ExpressionLabel.Match(_tokens[currentTokenIndex]).Success)
        {
            throw new InvalidTokenException(FormatErrorMessage(currentTokenIndex, "Niepoprawna etykieta wyrażenia.\n"));
        }

        currentTokenIndex++;

        if (_tokens[currentTokenIndex] != DeclarationLabelSeparator)
        {
            throw new InvalidTokenException(FormatErrorMessage(currentTokenIndex,
                "Niepoprawny token oddzielający etykietę wyrażenia od wartości\n." +
                $" Oczekiwany token to {DeclarationLabelSeparator}\n"));
        }

        currentTokenIndex++;

        while (_tokens[currentTokenIndex] != DeclarationEnding &&
               _tokens[currentTokenIndex] != NewLineCharacter)
        {
            if (_tokens[currentTokenIndex] == Keyword)
            {
                currentTokenIndex++;
            }
            else if (Regexes.UnitValue.Match(_tokens[currentTokenIndex]).Success)
            {
                currentTokenIndex++;
            }
            else if (Regexes.ColorHexValue.Match(_tokens[currentTokenIndex]).Success)
            {
                currentTokenIndex++;
            }
            else if (Regexes.UrlValue.Match(_tokens[currentTokenIndex]).Success)
            {
                currentTokenIndex++;
            }
            else if (Regexes.StringValue.Match(_tokens[currentTokenIndex]).Success)
            {
                currentTokenIndex++;
            }
            else if (Regexes.TextValue.Match(_tokens[currentTokenIndex]).Success)
            {
                currentTokenIndex++;
            }
            else if (Regexes.NumberValue.Match(_tokens[currentTokenIndex]).Success)
            {
                currentTokenIndex++;
            }
            else if (_tokens[currentTokenIndex] == CommentBeginning)
            {
                currentTokenIndex = ParseComment(currentTokenIndex);
            }
            else
            {
                throw new InvalidTokenException(FormatErrorMessage(currentTokenIndex,
                    "Niepoprawna wartość wyrażenia. Wartość może być: \n" +
                    "- słowem kluczowym !important\n" +
                    "- liczbą z jednostką (np. 10rem lub 7.5in)\n" +
                    "- liczbą całkowitą\n" +
                    "- kolorem zapisanym w formacie heksadecymalnym (np. #fff lub #a01212)\n" +
                    "- adresem url (np. watch?v=Ct6BUPvE2sM)\n" +
                    "- napisem (np. \"see below\")\n" +
                    "- tekstem (np. avoid)\n"
                ));
            }
        }

        return currentTokenIndex;
    }

    private string FormatErrorMessage(int currentTokenIndex, string errorMessage)
    {
        var searchForwardIndex = currentTokenIndex;
        while (searchForwardIndex < _tokens.Count && _tokens[searchForwardIndex] != NewLineCharacter)
        {
            searchForwardIndex++;
        }

        var searchBackwardIndex = currentTokenIndex == 0 ? 0 : currentTokenIndex;
        while (searchBackwardIndex > 0 && _tokens[searchBackwardIndex] != NewLineCharacter)
        {
            searchBackwardIndex--;
        }

        var currentLineTokens = _tokens.Take(searchBackwardIndex..searchForwardIndex).ToList();
        var numberOfSpaces = _tokens.Take((searchBackwardIndex + 1)..currentTokenIndex)
            .Select(s => s == "\t" ? 8 : s.Length)
            .Sum();

        var numberOfSeparators = _tokens.Take(searchBackwardIndex..currentTokenIndex)
            .Count(s => s != "\t" && s != "\n");

        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"Linia nr.{_lineNumber}: \n");
        stringBuilder.AppendJoin(" ", currentLineTokens);

        stringBuilder.Append('\n');
        stringBuilder.Append(' ', numberOfSpaces + numberOfSeparators);
        var currentTokenLength = _tokens[currentTokenIndex] == "\t" ? 8 : _tokens[currentTokenIndex].Length;
        stringBuilder.Append('^', currentTokenLength);

        stringBuilder.Append('\n');
        stringBuilder.Append(errorMessage);

        return stringBuilder.ToString();
    }
}