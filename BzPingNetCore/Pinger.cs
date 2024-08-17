using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace BzPing;

public class Pinger(Printer printer)
{
    private readonly Ping _ping = new();
    private readonly HttpClient _httpClient = new();

    public async Task ExecutePingToHostAsync(string hostOrIp)
    {
        if (IsHttpPing(hostOrIp))
        {
            await ExecuteHttpPingAsync(hostOrIp);
        }
        else
        {
            await ExecuteIcmpPingAsync(hostOrIp);
        }
    }

    private static bool IsHttpPing(string hostOrIp)
    {
        if (Uri.IsWellFormedUriString(hostOrIp, UriKind.Absolute))
        {
            return true;
        }

        var parts = hostOrIp.Split(' ');
        if (parts.Length == 2 &&
            Uri.IsWellFormedUriString(parts[1], UriKind.Absolute))
        {
            return true;
        }

        return false;
    }

    private async Task ExecuteHttpPingAsync(string uri)
    {
        var method = HttpMethod.Head;
        try
        {
            // could be "http://something.com" or "GET http://something.com"
            var uriParts = uri.Split(' ');
            Uri parsedUri = new Uri(uriParts.Last());
            
            if (uriParts.Length == 2)
            {
                method = new HttpMethod(uriParts[0]);
            }
            
            // it's quite difficult to get IP data out of the request/response message
            // and it's possible that the HTTP request connects to a different IP
            // but for now it's better than nothing to do a dedicated DNS lookup
            var hostIp = await GetIpAsync(parsedUri.Host);

            var sw = Stopwatch.StartNew();
            var request = new HttpRequestMessage(method, parsedUri);
            var response =  await _httpClient.SendAsync(request);
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
            printer.PrintError(uri, $"HTTP {method.ToString().ToUpper()} failed: {e.InnerException?.Message ?? e.Message}");
        }
    }

    private async Task ExecuteIcmpPingAsync(string hostOrIp)
    {
        try
        {
            // net framework "bug": reply.Address is null when there is a ping timeout
            // so we do the lookup upfront
            var ip = await GetIpAsync(hostOrIp);
            if (ip == null)
            {
                printer.PrintError(hostOrIp, null, IPStatus.Unknown.ToString());
                return;
            }
            
            PingReply? reply = await _ping.SendPingAsync(ip, 1000);
            if (reply == null)
            {
                printer.PrintWarning(hostOrIp, null, "Missing PingReply");
                return;
            }

            var ipStr = ip.ToString();
            if (ipStr == hostOrIp)
            {
                // printing the IP 2 times does not make sense
                ipStr = "";
            }

            if (reply.Status == IPStatus.Success)
            {
                printer.PrintSuccess(hostOrIp, ipStr, reply.Status.ToString(), reply.RoundtripTime + "ms");
            }
            else
            {
                printer.PrintError(hostOrIp, ipStr, reply.Status.ToString());
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

    private static async Task<IPAddress?> GetIpAsync(string host)
    {
        try
        {
            if (IPAddress.TryParse(host, out var ip))
            {
                return ip;
            }
            
            var addresses = await Dns.GetHostAddressesAsync(host);
            return addresses.FirstOrDefault();
        }
        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.HostNotFound)
        {
            return null;
        }
    }
}