namespace BzPing;

public class Printer
{
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

    private static void PrintCore(string[] text)
    {
        Console.Write(DateTime.Now);
        foreach (var part in text)
        {
            Console.Write("    ");
            Console.Write(part);
        }

        Console.WriteLine();
    }
}