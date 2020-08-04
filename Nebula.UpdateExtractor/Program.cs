using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Nebula.UpdateExtractor
{
    public static class Program
    {
        private const int    ExtractDelay = 2000;
        private const string FileName     = "NebulaUpdateContent.zip";

        private static async Task Main(string[] args)
        {
            Args = args;
            WriteLine("Updating Nebula...", ConsoleColor.Yellow);
            await ExtractUpdate();
        }

        private static string[] Args { get; set; }

        private static void WriteLine(string message, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
        }

        private static void ShutdownNebula()
        {
            foreach (Process process in Process.GetProcessesByName("Nebula"))
            {
                WriteLine($"Found running process '{process.ProcessName}', Killing it.", ConsoleColor.Yellow);
                process.Kill();
            }
        }

        private static async Task ExtractUpdate()
        {
            ShutdownNebula();
            try
            {
                await Task.Delay(ExtractDelay);
                await using FileStream stream = new FileStream(FileName, FileMode.Open);
                using ZipArchive zip = new ZipArchive(stream);
                foreach (ZipArchiveEntry zipEntry in zip.Entries)
                {
                    string path = Path.Combine(Environment.CurrentDirectory, zipEntry.FullName);
                    string directoryName = Path.GetDirectoryName(path);
                    if (!Directory.Exists(directoryName))
                        Directory.CreateDirectory(directoryName);
                    if (!string.IsNullOrWhiteSpace(zipEntry.Name))
                    {
                        zipEntry.ExtractToFile(path, true);
                        WriteLine($"Extracted '{path}'.", ConsoleColor.Yellow);
                    }
                }

                stream.Close();
                WriteLine("Nebula update completed ! Nebula is restarting.");
            }
            catch (Exception e)
            {
                WriteLine("Failed to update Nebula.", ConsoleColor.Red);
                WriteLine($"{e.Message}{Environment.NewLine}{e.StackTrace}", ConsoleColor.Red);
                Console.Read();
            }

            Process.Start("Nebula.exe", string.Join(" ", Args, "/justUpdated"));
        }
    }
}