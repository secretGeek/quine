namespace Quine.Models;

public class QuineMachineCode
{
    public readonly int Value;
    public readonly QuineToken Token;

    public QuineMachineCode(int value, QuineToken token)
    {
        Value = value;
        Token = token;
    }
}

