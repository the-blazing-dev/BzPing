﻿// See https://aka.ms/new-console-template for more information

using System.Net.NetworkInformation;
using System.Net.Sockets;

if (args.Length == 0)
{
    Console.WriteLine("Usage: BzPing <hostname or IP address>");
    return;
}

var hosts = args;
Ping pingSender = new Ping();

while (true)
{
    foreach (var host in hosts)
    {
        ExecutePingToHost(pingSender, host);
    }

    Thread.Sleep(1000);
}

static void ExecutePingToHost(Ping ping, string host)
{
    try
    {
        PingReply reply = ping.Send(host, 1000);

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