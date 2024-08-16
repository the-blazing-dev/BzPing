// See https://aka.ms/new-console-template for more information

using System.Net.NetworkInformation;
using System.Net.Sockets;

if (args.Length != 1)
{
    Console.WriteLine("Usage: BzPing <hostname or IP address>");
    return;
}

string host = args[0];
Ping pingSender = new Ping();

while (true)
{
    try
    {
        PingReply reply = pingSender.Send(host, 1000);

        var hostAndAddress = host;
        if (host != reply.Address.ToString())
        {
            hostAndAddress += $"\t{reply.Address}";
        }

        if (reply.Status == IPStatus.Success)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{DateTime.Now}\t");
            Console.WriteLine($"{hostAndAddress,-10}\t{reply.Status.ToString(),-10}\t{reply.RoundtripTime}ms");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"{DateTime.Now}\t");
            Console.WriteLine($"{hostAndAddress}\t{reply.Status}");
        }
    }
    catch (PingException e) when (e.InnerException is SocketException sex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write($"{DateTime.Now}\t");
        Console.WriteLine($"{host}\t{sex.Message}");
    }
    catch (PingException e)
    {
        Console.WriteLine($"Ping failed: {e.Message} {e.InnerException}");
    }
    
    Thread.Sleep(1000);
}