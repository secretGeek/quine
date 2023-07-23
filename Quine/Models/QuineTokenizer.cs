using Quine.Helpers;

namespace Quine.Models;

public class QuineTokenizer
{
    private readonly QuineLanguage ql;

    readonly bool _isVerbose;
    bool _isDebugMode;
    bool _isNoColor;

    public QuineTokenizer(bool isVerbose, bool isDebugMode, bool isNoColor)
    {
        ql = new QuineLanguage();
        _isVerbose = isVerbose;
        _isDebugMode = isDebugMode;
        _isNoColor = isNoColor;
    }

    internal IEnumerable<QuineToken> Tokenize(string sourceFile, TextWriter standardOut)
    {
        int location = 0;
        var reader = new StreamReader(sourceFile);
        while (!reader.EndOfStream)
        {
            var ch = (char)reader.Read();

            yield return new QuineToken(Parse(ch, location, standardOut));
            location++;
        }
    }

    internal IEnumerable<QuineToken> TokenizeText(string rawText, TextWriter standardOut)
    {
        int location = 0;

        foreach (var ch in rawText)
        {
            yield return new QuineToken(Parse(ch, location, standardOut));
            location++;
        }
    }

    public Ast<QuineToken> Parse(IEnumerable<QuineToken> tokens)
    {
        Ast<QuineToken>? previousAst = null;
        Ast<QuineToken>? firstAst = null;
        foreach (var t in tokens)
        {
            var ast = new Ast<QuineToken>(t);
            if (previousAst == null)
            {
                firstAst = ast;
                previousAst = ast;
            }
            else
            {
                previousAst.NextAst = ast;
                previousAst = previousAst.NextAst;
            }
        }

        return firstAst!;
    }

    private Token Parse(char ch, int location, TextWriter standardOut)
    {
        if (Enum.TryParse(ch.ToString(), false, out Token token))
        {
            if (ql.ValidTokens.Contains(token))
            {
                if (_isDebugMode)
                {
                    if (!_isVerbose)
                    {
                        if (!_isNoColor) Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        standardOut.Write(ch);
                        Console.ResetColor();
                        standardOut.Write("=>");
                        if (!_isNoColor) Console.ForegroundColor = ConsoleColor.Magenta;
                        standardOut.Write($"Token.{token}");
                        Console.ResetColor();
                        standardOut.Write(";");
                    }
                    else
                    {
                        standardOut.WriteLine("\n[Token::]");
                        if (!_isNoColor) Console.ForegroundColor = ConsoleColor.DarkCyan;
                        standardOut.WriteLine("Input:");
                        if (!_isNoColor) Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        standardOut.Write($"\t{ch}");
                        Console.ResetColor();
                        standardOut.WriteLine(";");
                        if (!_isNoColor) Console.ForegroundColor = ConsoleColor.Cyan;
                        standardOut.WriteLine("Parsed as Token:");
                        if (!_isNoColor) Console.ForegroundColor = ConsoleColor.Magenta;
                        standardOut.WriteLine($"\t{token.GetType().FullName}.{token}");
                        if (!_isNoColor) Console.ForegroundColor = ConsoleColor.DarkGray;
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

    // Apply the language's semantic validator (ql.IsSemanticallyValid) 
    // to the AST's structure
    internal bool IsSemanticallyValid(Ast<QuineToken> ast)
        => ast.IsSemanticallyValid((prevToken, nextToken) => ql.IsSemanticallyValid(prevToken, nextToken, _isVerbose, _isDebugMode, _isNoColor));

    public class Ast<tokenType>
    {
        public tokenType? Token { get; set; }

        public Ast<tokenType>? NextAst { get; set; }
        public Ast(tokenType token)
        {
            Token = token;
            NextAst = new Ast<tokenType>();
        }

        public Ast()
        {
            Token = default;
            NextAst = default;
        }

        internal IEnumerable<Ast<tokenType>> TraverseAst()
        {
            if (Token != null)
            {
                yield return this!;

                if (NextAst != null)
                {
                    foreach (var nextAst in NextAst.TraverseAst())
                    {
                        yield return nextAst;
                    }
                }
            }
        }

        internal bool IsSemanticallyValid(Func<tokenType, tokenType?, bool> validate)
        {
            var result = true;

            if (Token != null && NextAst != null)
            {
                result = validate(Token, NextAst.Token);
            }

            if (result && NextAst != null)
            {
                result = NextAst.IsSemanticallyValid(validate);
            }

            return result;
        }
    }
}
