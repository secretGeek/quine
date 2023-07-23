namespace Quine.Models;

public class QuineToken
{
    public readonly Token TokenType;
    public readonly int MachineCode;

    public QuineToken(Token tokenType)
    {
        TokenType = tokenType;
        MachineCode = tokenType.GetHashCode();
    }

    public QuineMachineCode ToMachineCode()
        => new QuineMachineCode(MachineCode, this);
}
