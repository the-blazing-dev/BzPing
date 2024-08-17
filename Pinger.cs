using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace BzPing;

public class Pinger(Printer printer)
{
    private readonly Ping _ping = new();
    private readonly HttpClient _httpClient = new();

    public void ExecutePingToHost(string hostOrIp)
    {
        if (Uri.IsWellFormedUriString(hostOrIp, UriKind.Absolute))
        {
            ExecuteHttpPing(hostOrIp);
        }
        else
        {
            ExecuteIcmpPing(hostOrIp);
        }
    }

    private void ExecuteHttpPing(string uri)
    {
        try
        {
            // it's quite difficult to get IP data out of the request/response message
            // and it's possible that the HTTP request connects to a different IP
            // but for now it's better than nothing to do a dedicated DNS lookup
            var parsedUri = new Uri(uri);
            var hostIp = Dns.GetHostEntry(parsedUri.Host).AddressList.FirstOrDefault();

            var sw = Stopwatch.StartNew();
            var request = new HttpRequestMessage(HttpMethod.Head, parsedUri);
            var response = _httpClient.Send(request);
            sw.Stop();

            var statusCodeWithText = $"{(int)response.StatusCode} {response.StatusCode}";
            var duration = sw.ElapsedMilliseconds + "ms";
            
            if (response.IsSuccessStatusCode)
            {
                printer.PrintSuccess(uri, hostIp?.ToString(), statusCodeWithText, duration);
            }
            else
            {
                printer.PrintWarning(uri, hostIp?.ToString(), statusCodeWithText, duration);
            }
        }
        catch (Exception e)
        {
            printer.PrintError(uri, $"HTTP HEAD failed: {e.InnerException?.Message ?? e.Message}");
        }
    }

    private void ExecuteIcmpPing(string hostOrIp)
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