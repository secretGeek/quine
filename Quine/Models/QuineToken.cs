namespace Quine.Models;

public class QuineToken
{
    public readonly Token TokenType;
    public readonly int MachineCode;

    public QuineToken(Token tokenType)
    {
        this.TokenType = tokenType;
        this.MachineCode = tokenType.GetHashCode();
    }

    public QuineMachineCode ToMachineCode()
        => new QuineMachineCode(this.MachineCode, this);
}
