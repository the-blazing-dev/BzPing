// See https://aka.ms/new-console-template for more information

using System.Net.NetworkInformation;
using BzPing;

if (args.Length == 0)
{
    Console.WriteLine("Usage: BzPing <IP/hostname/URL> [<IP/hostname/URL>...] [--live]");
    return;
}

var parameters = Parameters.Parse(args);
var printer = new Printer(parameters);
printer.PredictMaxTextLength(0, parameters.Hosts.Max(x => x.Length));
printer.PredictMaxTextLength(1, "255.255.255.255".Length);
// there are longer ones, but they should not occur that often. And if they do, it's still rendedered
printer.PredictMaxTextLength(2, IPStatus.TimedOut.ToString().Length); 
var pinger = new Pinger(printer);

while (true)
{
    printer.NotifyStartOfLoop();
    
    foreach (var host in parameters.Hosts)
    {
        pinger.ExecutePingToHost(host);
    }

    if (parameters.Spacer.GetValueOrDefault())
    {
        printer.PrintSpacer();
    }
    
    Thread.Sleep(1000);
}

