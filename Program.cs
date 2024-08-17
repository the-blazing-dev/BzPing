// See https://aka.ms/new-console-template for more information

using System.Net.NetworkInformation;
using System.Net.Sockets;
using BzPing;

if (args.Length == 0)
{
    Console.WriteLine("Usage: BzPing <hostname or IP address>");
    return;
}

var hosts = args;
var pinger = new Pinger();

while (true)
{
    foreach (var host in hosts)
    {
        pinger.ExecutePingToHost(host);
    }

    Thread.Sleep(1000);
}

