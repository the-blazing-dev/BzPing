using System;
using System.Collections.Generic;

namespace BzPing;

public class Printer
{
    private Dictionary<int, int> _maxTextLength = new();
    private readonly Parameters _parameters;

    public Printer(Parameters parameters)
    {
        _parameters = parameters;
        if (_parameters.LiveMode)
        {
            Console.Clear();
        }
    }

    public void NotifyStartOfLoop()
    {
        if (_parameters.LiveMode)
        {
            Console.SetCursorPosition(0, 0);
        }
    }
    
    public void PrintSuccess(params string?[] text)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        PrintCore(text);
    }
    
    public void PrintWarning(params string?[] text)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        PrintCore(text);
    }

    public void PrintError(params string?[] text)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        PrintCore(text);
    }

    private void PrintCore(string?[] text)
    {
        ClearLine();
        
        Console.Write(DateTime.Now);
        for (var i = 0; i < text.Length; i++)
        {
            var part = text[i];
            var maxTextLength = _maxTextLength.GetValueOrDefault(i, 0);
            Console.Write("    "); // the output looks better with predictable 4 spaces instead of tab
            Console.Write((part ?? "").PadRight(maxTextLength));
        }

        Console.WriteLine();
        Console.ResetColor();
    }

    private void ClearLine()
    {
        if (_parameters.LiveMode)
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth)); // Clear the line
            Console.SetCursorPosition(0, Console.CursorTop);
        }
    }

    public void PredictMaxTextLength(int paramsIndex, int maxLength)
    {
        _maxTextLength[paramsIndex] = maxLength;
    }

    public void PrintSpacer()
    {
        Console.WriteLine();
    }
}