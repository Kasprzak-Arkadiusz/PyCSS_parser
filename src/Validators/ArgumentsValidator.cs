using PyCSS_parser.Extensions;

namespace PyCSS_parser.Validators;

public static class ArgumentsValidator
{
    private const int ExpectedNumberOfArguments = 1;

    public static bool AreCorrect(string[] args)
    {
        return NumberOfArgumentsMatch(args.Length) && FileExists(args[0]);
    }

    private static bool NumberOfArgumentsMatch(int numberOfArguments)
    {
        if (numberOfArguments == ExpectedNumberOfArguments)
        {
            return true;
        }

        ConsoleWriter.WriteError(
            $"Invalid number of arguments. Expected {ExpectedNumberOfArguments}, received {numberOfArguments}");
        return false;
    }

    private static bool FileExists(string filePath)
    {
        if (File.Exists(filePath))
        {
            return true;
        }

        ConsoleWriter.WriteError("File does not exist");
        return false;
    }
}