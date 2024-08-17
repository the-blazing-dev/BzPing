using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace BzPing;

public class Pinger(Printer printer)
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
                printer.PrintSuccess(hostAndAddress, reply.Status.ToString(), reply.RoundtripTime + "ms");
            }
            else
            {
                printer.PrintError(hostAndAddress, reply.Status.ToString());
            }
        }
        catch (PingException e) when (e.InnerException is SocketException sex)
        {
            printer.PrintError(host, sex.Message);
        }
        catch (PingException e)
        {
            printer.PrintError(host, $"Ping failed: {e.Message} {e.InnerException?.Message}");
        }
    }
}