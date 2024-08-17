using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace BzPing;

public class Pinger
{
    private readonly Ping _ping = new();

    public void ExecutePingToHost(string host)
    {
        try
        {
            PingReply reply = _ping.Send(host, 1000);

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
            Console.WriteLine($"{DateTime.Now}\t{host}\tPing failed: {e.Message} {e.InnerException}");
        }
    }
}