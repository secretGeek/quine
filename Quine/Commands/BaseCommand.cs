using CommandLine;

namespace Quine.Commands;

public abstract class BaseCommand : IConsoleCommand
{
    [Option('v', "verbose",
         Default = false,
         HelpText = "Prints all messages to standard output",
           Required = false)]
    public bool IsVerbose { get; set; }

    [Option('v', "no-color",
         Default = false,
         HelpText = "Reduce color output to the bare minimum",
           Required = false)]
    public bool IsNoColor { get; set; } 

    public abstract int Execute();

    public abstract IEnumerable<IConsoleCommandError> Validate();
}

public class ConsoleCommandError : IConsoleCommandError
{
    public string Name { get; }

    public string Details { get; }

    public ConsoleCommandError(string name, string details)
    {
        this.Name = name;
        this.Details = details;
    }
}