namespace Quine.Commands;

public interface IConsoleCommand
{
    IEnumerable<IConsoleCommandError> Validate();
    bool IsVerbose { get; set; }

    bool IsNoColor { get; set; }
    abstract int Execute();
}

public interface IConsoleCommandError
{
    string Name { get; }
    string Details { get; }
}

