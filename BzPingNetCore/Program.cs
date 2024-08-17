using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using BzPing;

if (args.Length == 0)
{
    Console.WriteLine("Usage: bzping <IP/hostname/URL> [<IP/hostname/URL>...] [--live]");
    Console.WriteLine("To install it for your local user: bzping --install");
    Console.WriteLine();
    return;
}

var parameters = Parameters.Parse(args);

if (parameters.IsInstall)
{
    Installer.Install();
    return;
}

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
        await pinger.ExecutePingToHostAsync(host);
    }

    if (parameters.Spacer.GetValueOrDefault())
    {
        printer.PrintSpacer();
    }

    await Task.Delay(1000);
}

