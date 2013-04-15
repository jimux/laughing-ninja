using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace FileWatcher
{
    class Program
    {
        private static string configname = @"paths.txt";
        private static List<string> entries = new List<string>();

        static void Main(string[] args)
        {
            // Is the config file there?
            var configinfo = new FileInfo(configname);
            if (!configinfo.Exists)
            {
                Console.WriteLine("Can't find config file! Looking at " + configinfo.FullName);
                Thread.Sleep(10000);
                return;
            }

            // Load it in.
            foreach (string line in File.ReadAllText(configname).Split('\n'))
            {
                // Ignore comments
                if (line.StartsWith("#")) continue;

                string trimmedline = line.TrimEnd('\r');
                string[] parts = trimmedline.Split(';');
                
                if (parts.Length < 2)
                {
                    Console.WriteLine("Invalid config line. Only one element: " + line);
                }

                string directory = parts[0];

                if(directory.StartsWith(@"\\"))
                {
                    Console.WriteLine("WARNING: Watching UNC paths is not reliable. " + directory);
                }

                string script = parts[1];

                var finfo = new FileInfo(script);
                if (!finfo.Exists)
                {
                    Console.WriteLine("Can't find script! Looking at " + finfo.FullName);
                    continue;
                }

                new WatchWrapper(directory, script);
            }

            while (true)
            {

            }
        }
    }
}
