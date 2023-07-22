using CommandLine;
using Quine.Commands;
using Quine.Helpers;
using System.Reflection;

namespace Quine;

internal class Program
{
    public const string APPNAME = nameof(Quine);

    static public int Main(string[] args)
    {
        int result = 1;
        Parser.Default.ParseArguments(args,
          typeof(CompileCommand))
             .WithParsed<IConsoleCommand>(command =>
             {
                 var errors = command.Validate();
                 if (!errors.Any())
                 {
                     try
                     {
                         result = command.Execute();
                     }
                     catch (Exception e)
                     {
                         result = 4;
                         if (!command.IsNoColor) Console.ForegroundColor = ConsoleColor.Red;
                         Console.Error.Write($"{e.GetType()}");
                         Console.ResetColor();
                         Console.Error.WriteLine($": {e.Message}");

                         if (command.IsVerbose)
                         {
                             if (!command.IsNoColor) Console.ForegroundColor = ConsoleColor.DarkGray;
                             Console.Error.WriteLine(e.StackTrace);
                             Console.ResetColor();
                         }
                     }
                 }
                 else
                 {
                     result = 3;
                     if (!command.IsNoColor) Console.ForegroundColor = ConsoleColor.Red;
                     Console.Error.WriteLine($"Problem with {command.GetType().GetCustomAttributes<VerbAttribute>().First().Name} command --");
                     Console.ResetColor();

                     foreach (var e in errors)
                     {
                         Console.Out.Write("* ");
                         if (!command.IsNoColor) Console.ForegroundColor = ConsoleColor.Yellow;
                         Console.Out.Write(e.Name);
                         Console.ResetColor();
                         Console.Out.Write(": ");
                         Console.Out.WriteLine(e.Details);
                     }
                 }
             });

        return result;
    }
}