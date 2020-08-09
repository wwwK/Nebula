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
            WriteLine("Updating Nebula...", ConsoleColor.Green);
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
                WriteLine($"Found running process '{process.ProcessName}', Killing it.", ConsoleColor.Green);
                process.Kill();
            }
        }

        private static void ClearFiles()
        {
            WriteLine("Cleaning old files...", ConsoleColor.Yellow);
            foreach (string file in Directory.GetFiles(Environment.CurrentDirectory, "*", SearchOption.AllDirectories)
            )
            {
                if (Path.GetFileName(file).ToLower().Contains("update"))
                    continue;
                File.Delete(file);
                WriteLine($"File {file} Deleted !", ConsoleColor.Green);
            }

            foreach (string directory in Directory.GetDirectories(Environment.CurrentDirectory))
            {
                Directory.Delete(directory, true);
                WriteLine($"Directory {directory} Deleted !", ConsoleColor.Green);
            }
        }

        private static async Task ExtractUpdate()
        {
            await Task.Delay(ExtractDelay);
            ShutdownNebula();
            try
            {
                await Task.Delay(ExtractDelay);
                ClearFiles();
                WriteLine("Extracting Update...", ConsoleColor.Yellow);
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
                        WriteLine($"Extracted '{path}'.", ConsoleColor.Green);
                    }
                }

                stream.Close();
                WriteLine("Nebula update completed ! Nebula is restarting.", ConsoleColor.Yellow);
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