namespace BzPing;

public class Printer
{
    private Dictionary<int, int> _maxTextLength = new();
    
    public void PrintSuccess(params string[] text)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        PrintCore(text);
    }

    public void PrintError(params string[] text)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        PrintCore(text);
    }

    private void PrintCore(string[] text)
    {
        Console.Write(DateTime.Now);
        for (var i = 0; i < text.Length; i++)
        {
            var part = text[i];
            var maxTextLength = _maxTextLength.GetValueOrDefault(i, 0);
            Console.Write("    "); // the output looks better with predictable 4 spaces instead of tab
            Console.Write(part.PadRight(maxTextLength));
        }

        Console.WriteLine();
    }

    public void PredictMaxTextLength(int paramsIndex, int maxLength)
    {
        _maxTextLength[paramsIndex] = maxLength;
    }
}