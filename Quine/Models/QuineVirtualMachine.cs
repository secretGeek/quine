using System.Diagnostics;
using System.Numerics;

namespace Quine.Models;

public class QuineVirtualMachine
{
    public const int UNRECOGNISED_TOKEN = 12;
    private bool CONTINUOUS_PREV_TOKEN_COPY_MODE = false;
    private readonly IEnumerable<QuineMachineCode> _machinesCodes;
    private readonly TextWriter _standardOut;
    private QuineToken? _previousToken;
    private readonly Queue<QuineToken> _tokenQueue;
    private readonly bool _isVerbose;
    private readonly bool _debugMode;
    private readonly bool _isNoColor;

    public QuineVirtualMachine(IEnumerable<QuineMachineCode> machineCodes, TextWriter standardOut, bool isVerbose, bool debugMode, bool isNoColor)
    {
        _machinesCodes = machineCodes;
        _standardOut = standardOut;
        _tokenQueue = new Queue<QuineToken>();
        _previousToken = null;
        _isVerbose = isVerbose;
        _debugMode = debugMode;
        _isNoColor = isNoColor;
    }

    internal int Exec()
    {
        int result = 0;
        foreach (QuineMachineCode code in _machinesCodes)
        {
            result = Exec(code.Value, code.Token, _isNoColor);
            if (result > 0) return result;
        }
        return result;
    }

    internal int Exec(int code, QuineToken currentToken, bool isNoColor)
    {
        Trace.Assert(currentToken.TokenType.GetHashCode() == code, "The Machine Code to be executed should match the current token");

        if (_debugMode) ShowQueue(currentToken, _previousToken, isNoColor);

        try
        {
            switch (code)
            {
                case 0: return Exec_Error(currentToken);
                case 1: return Exec_Indicate(currentToken);
                case 2: return Exec_PreviousAndCurrent(currentToken);
                case 3: return Exec_Queue_Initialize(currentToken);
                case 4: return Exec_Undertake_Continous_Copying(currentToken);
                default: return UNRECOGNISED_TOKEN;
            }
        }
        finally
        {
            if (CONTINUOUS_PREV_TOKEN_COPY_MODE)
            {
                EnqueueToken(_previousToken);
            }

            _previousToken = currentToken;
        }
    }

    /// <summary>
    /// `u`ndertake continuous copying of the previous token `buffer` to the `main queue`.
    /// </summary>
    /// <param name="currentToken"></param>
    /// <returns></returns>
    private int Exec_Undertake_Continous_Copying(QuineToken currentToken)
    {
        CONTINUOUS_PREV_TOKEN_COPY_MODE = true;

        return 0;
    }


    /// <summary>
    /// disable copying of the previous token to the main register (and `i`ndicate success) by writing current token to the main register.
    /// </summary>
    /// <returns></returns>
    private int Exec_Indicate(QuineToken currentToken)
    {

        if (CONTINUOUS_PREV_TOKEN_COPY_MODE)
            EnqueueToken(_previousToken);
        CONTINUOUS_PREV_TOKEN_COPY_MODE = false;
        return 0;
    }

    /// <summary>
    /// raise `e`rror: dump the contents of the main queue to standard output, followed by 'e' to indicate that an error has occurred.
    /// </summary>
    /// <returns></returns>        
    private int Exec_Error(QuineToken currentToken)
    {

        DumpQueue();
        _standardOut.Write(currentToken.TokenType.ToString());
        return 2;
    }

    private void DumpQueue()
    {
        while (_tokenQueue.TryDequeue(out QuineToken? token))
        {
            _standardOut.Write(token.TokenType.ToString());
        }
    }

    private void ShowQueue(QuineToken currentToken, QuineToken? previousToken, bool isNoColor)
    {
        if (_isVerbose)
        {
            if (!isNoColor) Console.ForegroundColor = ConsoleColor.DarkYellow;
            _standardOut.WriteLine("Current Main Queue State:");
        }

        if (!isNoColor) Console.ForegroundColor = ConsoleColor.DarkMagenta;
        foreach (var q in _tokenQueue)
        {
            if (!_isVerbose)
            {
                _standardOut.Write(q.TokenType.ToString());
            }
            else
            {
                if (!isNoColor) Console.ForegroundColor = ConsoleColor.DarkMagenta;
                _standardOut.Write(q.TokenType.ToString());
                _standardOut.Write($"\t{q.TokenType.GetType().FullName}.{q.TokenType}");
                Console.ResetColor();
                _standardOut.WriteLine(";");
            }
        }

        if (_isVerbose && !_tokenQueue.Any() )
            _standardOut.WriteLine("-- Main Queue Is Empty --");

        Console.ResetColor();

        if (_isVerbose)
            _standardOut.Write("Previous Token:\t");

        if (!isNoColor) Console.ForegroundColor = ConsoleColor.DarkGreen;
        _standardOut.Write(previousToken?.TokenType.ToString() ?? "0");

        Console.ResetColor();
        
        if (_isVerbose)
            _standardOut.Write($"{Environment.NewLine}Current Token:\t");

        if (!isNoColor) Console.ForegroundColor = ConsoleColor.Blue;
        _standardOut.Write(currentToken.TokenType.ToString());
        if (!isNoColor) Console.ForegroundColor = ConsoleColor.DarkGray;

        if (_isVerbose)
            _standardOut.WriteLine();
        else 
            _standardOut.Write("::");

        Console.ResetColor();
    }



    /// <summary>
    /// Add previous a`n`d current token to the main queue.
    /// </summary>
    /// <returns></returns>
    private int Exec_PreviousAndCurrent(QuineToken currentToken)
    {
        EnqueueToken(_previousToken);
        EnqueueToken(currentToken);

        return 0;
    }

    /// <summary>
    /// clear the main `q`ueue and commence normal operation.
    /// </summary>
    /// <param name="currentToken"></param>
    /// <returns></returns>
    private int Exec_Queue_Initialize(QuineToken currentToken)
    {
        _tokenQueue.Clear();
        return 0;
    }


    private void EnqueueToken(QuineToken? token)
    {
        if (token == null)
        {
            throw new InvalidOperationException("Attempt to enqueue null token");
        }
        _tokenQueue.Enqueue(token);
    }
}

