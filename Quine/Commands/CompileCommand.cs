using CommandLine;
using CommandLine.Text;
using Quine.Models;

namespace Quine.Commands;

[Verb("compile", aliases: new string[] { "c" }, HelpText = "Compile a program")]
public class CompileCommand : BaseCommand
{
    [Option('s', "source-file", HelpText = "Source code (.quine file) to be compiled", Required = false)]
    public string SourceFile { get; set; } = "";

    [Option('r', "raw-quine-text", HelpText = "Raw quine text to be compiled", Required = false)]
    public string RawText { get; set; } = "";


    [Option('x', "execute-immediately", HelpText = "Immediately Execute the compiled source (i.e. run the compiled program, if compilation was successfull)", Required = false)]
    public bool ExecImmediate { get; set; } = false;

    [Option('d', "debug-mode", HelpText = "During execution prints the machine state before each instruction is executed", Required = false)]
    public bool IsDebugMode { get; set; } = false;

    [Usage(ApplicationAlias = Program.APPNAME)]
    public static IEnumerable<Example> Examples
    {
        get
        {
            yield return new Example("Compile a .Quine file into the intermediate Quine VM Language", new CompileCommand
            { SourceFile = "Program.quine", });

            yield return new Example("Compile a .Quine file, and execute it, with verbose output", new CompileCommand
            { SourceFile = "Program.quine", IsVerbose = true, ExecImmediate = true });

            yield return new Example("Compile a .Quine file, and execute it, with debug mode set", new CompileCommand
            { SourceFile = "Program.quine", IsVerbose = true, ExecImmediate = true, IsDebugMode = true });

            yield return new Example("Compile some Quine code, and execute it", new CompileCommand
            { RawText = "quine", IsVerbose = true, ExecImmediate = true });
        }
    }

    public override int Execute()
    {
        var qt = new QuineTokenizer();
        var tokens =
            !string.IsNullOrWhiteSpace(RawText)
            ?
                qt.TokenizeText(RawText, IsVerbose, IsDebugMode, IsNoColor, Console.Out)
            :
                qt.Tokenize(SourceFile, IsVerbose, IsDebugMode, IsNoColor, Console.Out);


        var machineCodes = new List<QuineMachineCode>();
        foreach (var token in tokens)
        {
            machineCodes.Add(token.ToMachineCode());
        }

        if (ExecImmediate)
        {
            var Vm = new QuineVirtualMachine(machineCodes, Console.Out, IsVerbose, IsDebugMode, IsNoColor);
            return Vm.Exec();
        }
        else
        {
            if (IsVerbose)
            {
                Console.Out.WriteLine("Compiled, but did not execute. (Execution requires the `--execute-immediately` (or `-x`) flag)");
            }
        }

        return 0;
    }

    public override IEnumerable<IConsoleCommandError> Validate()
    {
        if (string.IsNullOrWhiteSpace(RawText) && string.IsNullOrWhiteSpace(SourceFile))
            yield return new ConsoleCommandError("--source-file or --raw-quine-text", $"You *MUST* either specify a source file, or some raw quine text. You specified neither.");

        if (!string.IsNullOrWhiteSpace(RawText) && !string.IsNullOrWhiteSpace(SourceFile))
            yield return new ConsoleCommandError("--source-file or --raw-quine-text", $"You must either specify a source file, *OR* some raw quine text. Both were specified.");

        if (!string.IsNullOrWhiteSpace(SourceFile) && !File.Exists(SourceFile))
            yield return new ConsoleCommandError("--source-file", $"Source File does not exist: '{SourceFile}'");
    }
}
