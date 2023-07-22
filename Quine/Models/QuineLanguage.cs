using System.ComponentModel;

namespace Quine.Models;

public class QuineLanguage
{
    public HashSet<Token> ValidTokens { get; set; }
    public QuineLanguage()
    {
        ValidTokens = Enum.GetValues(typeof(Token)).Cast<Token>().ToHashSet();
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
