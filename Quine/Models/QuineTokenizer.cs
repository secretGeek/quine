using Quine.Helpers;

namespace Quine.Models;

public class QuineTokenizer
{
    private readonly QuineLanguage ql;

    public QuineTokenizer()
    {
        ql = new QuineLanguage();
    }

    internal IEnumerable<QuineToken> Tokenize(string sourceFile, bool isVerbose, bool isDebugMode, bool isNoColor, TextWriter standardOut)
    {
        int location = 0;
        var reader = new StreamReader(sourceFile);
        while (!reader.EndOfStream)
        {
            var ch = (char)reader.Read();

            yield return new QuineToken(Parse(ch, location, isVerbose, isDebugMode, isNoColor, standardOut));
            location++;
        }
    }

    internal IEnumerable<QuineToken> TokenizeText(string rawText, bool isVerbose, bool isDebugMode, bool isNoColor, TextWriter standardOut)
    {
        int location = 0;

        foreach (var ch in rawText)
        {

            yield return new QuineToken(Parse(ch, location, isVerbose, isDebugMode, isNoColor, standardOut));
            location++;
        }
    }

    private Token Parse(char ch, int location, bool isVerbose, bool isDebugMode, bool isNoColor, TextWriter standardOut)
    {
        if (Enum.TryParse(ch.ToString(), false, out Token token))
        {
            if (ql.ValidTokens.Contains(token))
            {
                if (isDebugMode)
                {
                    if (!isVerbose)
                    {
                        if (!isNoColor) Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        standardOut.Write(ch);
                        Console.ResetColor();
                        standardOut.Write("=>");
                        if (!isNoColor) Console.ForegroundColor = ConsoleColor.Magenta;
                        standardOut.Write($"Token.{token}");
                        Console.ResetColor();
                        standardOut.Write(";");
                    }
                    else
                    {
                        standardOut.WriteLine("\n[Token::]");
                        if (!isNoColor) Console.ForegroundColor = ConsoleColor.DarkCyan;
                        standardOut.WriteLine("Input:");
                        if (!isNoColor) Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        standardOut.Write($"\t{ch}");
                        Console.ResetColor();
                        standardOut.WriteLine(";");
                        if (!isNoColor) Console.ForegroundColor = ConsoleColor.Cyan;
                        standardOut.WriteLine("Parsed as Token:");
                        if (!isNoColor) Console.ForegroundColor = ConsoleColor.Magenta;
                        standardOut.WriteLine($"\t{token.GetType().FullName}.{token}");
                        if (!isNoColor) Console.ForegroundColor = ConsoleColor.DarkGray;
                        standardOut.Write($"\t> {token.GetDescription()}");
                        Console.ResetColor();
                        standardOut.WriteLine(";\n[::End Of Token]");
                    }
                }

                return token;
            }
            else
            {
                throw new InvalidDataException($"Invalid Token {token} (from '{ch}') at location:{location}");
            }
        }

        throw new InvalidDataException($"Unrecognised Token '{ch}' at location:{location}");
    }
}
