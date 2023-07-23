using System.ComponentModel;

namespace Quine.Models;

public class QuineLanguage
{
    public HashSet<Token> ValidTokens { get; set; }
    public QuineLanguage()
    {
        ValidTokens = Enum.GetValues(typeof(Token)).Cast<Token>().ToHashSet();
    }

    public bool IsSemanticallyValid(QuineToken prevToken, QuineToken? nextToken, bool isVerbose, bool isDebugMode, bool isNoColor)
    {
        var result = AnalyzeTokenPrecedence(prevToken, nextToken);
        if (!result)
        {
            throw new InvalidProgramException($"It is a syntax error for token: '{nextToken?.TokenType.ToString() ?? "(no token)"}' to follow token: '{prevToken.TokenType}'");
        }
        else
        {
            if (isDebugMode)
            {
                if (!isNoColor) Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(isVerbose?  "No semantic issue with " : "OK:");
                if (!isNoColor) Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(isVerbose ? $"token:{nextToken?.TokenType.ToString() ?? "(no token)"}" : $"{nextToken?.TokenType.ToString() ?? "(eot)"}");
                if (!isNoColor) Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(isVerbose ? " following " : "<-");
                if (!isNoColor) Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write(isVerbose ? $"token:{prevToken.TokenType}{Environment.NewLine}" : $"{prevToken.TokenType};");
                if (!isNoColor) Console.ResetColor();
            }
        }

        return result;
    }

    private bool AnalyzeTokenPrecedence(QuineToken prevToken, QuineToken? nextToken)
    {
        if (nextToken == null)
        {
            return prevToken.TokenType == Token.e;
        }

        switch (nextToken.TokenType)
        {
            case Token.e: return prevToken.TokenType == Token.n;
            case Token.i: return prevToken.TokenType == Token.u;
            case Token.n: return prevToken.TokenType == Token.i;
            case Token.q: return prevToken == null;
            case Token.u: return prevToken.TokenType == Token.q;
            default: return false;
        }
    }
}

public enum Token
{
    [Description("raise `e`rror: dump the contents of the `main queue` to `standard output`, followed by 'e' to indicate that an error has occurred.")]
    e,
    [Description("disable copying of the previous token to the `main queue` (and `i`ndicate success) by writing current token to the `main queue`.")]
    i,
    [Description("copy previous a`n`d current token to the `main queue`.")]
    n,
    [Description("clear the main `q`ueue and commence normal operation.")]
    q,
    [Description("`u`ndertake continuous copying of the previous token `buffer` to the `main queue`.")]
    u
}
