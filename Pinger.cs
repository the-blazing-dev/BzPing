using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace BzPing;

public class Pinger(Printer printer)
{
    private readonly Ping _ping = new();

    public void ExecutePingToHost(string hostOrIp)
    {
        try
        {
            PingReply reply = _ping.Send(hostOrIp, 1000);

            string ip = "";
            if (hostOrIp != reply.Address.ToString())
            {
                ip = reply.Address.ToString();
            }

            if (reply.Status == IPStatus.Success)
            {
                printer.PrintSuccess(hostOrIp, ip, reply.Status.ToString(), reply.RoundtripTime + "ms");
            }
            else
            {
                printer.PrintError(hostOrIp, ip, reply.Status.ToString());
            }
        }
        catch (PingException e) when (e.InnerException is SocketException sex)
        {
            printer.PrintError(hostOrIp, sex.Message);
        }
        catch (PingException e)
        {
            printer.PrintError(hostOrIp, $"Ping failed: {e.Message} {e.InnerException?.Message}");
        }
    }
}