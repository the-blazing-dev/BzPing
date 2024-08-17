// See https://aka.ms/new-console-template for more information

using System.Net.NetworkInformation;
using System.Net.Sockets;
using BzPing;

if (args.Length == 0)
{
    Console.WriteLine("Usage: BzPing <IP/hostname/URL> [<IP/hostname/URL>...]");
    return;
}

var hosts = args;
var printer = new Printer();
printer.PredictMaxTextLength(0, hosts.Max(x => x.Length));
printer.PredictMaxTextLength(1, "255.255.255.255".Length);
// there are longer ones, but they should not occur that often. And if they do, it's still rendedered
printer.PredictMaxTextLength(2, IPStatus.TimedOut.ToString().Length); 
var pinger = new Pinger(printer);

while (true)
{
    foreach (var host in hosts)
    {
        pinger.ExecutePingToHost(host);
    }

    if (hosts.Length > 3)
    {
        // spacer line
        Console.WriteLine();
    }
    
    Thread.Sleep(1000);
}

