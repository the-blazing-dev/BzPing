using System;
using System.Diagnostics;
using System.IO;

namespace BzPing;

public static class Installer
{
    public static void Install()
    {
        try 
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appDirectory = Path.Combine(localAppData, "BzPing");

            if (!Directory.Exists(appDirectory))
            {
                Directory.CreateDirectory(appDirectory);
            }

            string processName = Process.GetCurrentProcess().ProcessName; // extension is missing here
            string currentFileName = Path.Combine(AppContext.BaseDirectory, processName + ".exe");
            string exePath = Path.Combine(appDirectory, "bzping.exe");
            
            var callingInstallOnInstalledApp = currentFileName == exePath;
            if (callingInstallOnInstalledApp)
            {
                Console.WriteLine("You called --install on the already installed version.");
                Console.WriteLine("If you try to update, you maybe need to call: ./bzping.exe --install");
                return;
            }

            // always copy so we can also update the app
            File.Copy(currentFileName, exePath, true);

            string pathEnv = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User)!;

            if (!pathEnv.Contains(appDirectory))
            {
                string newPathEnv = pathEnv + ";" + appDirectory;
                Environment.SetEnvironmentVariable("PATH", newPathEnv, EnvironmentVariableTarget.User);
                Console.WriteLine($"Added {appDirectory} to the PATH environment variable.");
                Console.WriteLine("Restart any open command prompts for the change to take effect.");
            }
            else
            {
                Console.WriteLine($"{appDirectory} is already in the PATH environment variable.");
            }

            Console.WriteLine("Installation complete.");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}