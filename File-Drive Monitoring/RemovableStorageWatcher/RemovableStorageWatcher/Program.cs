using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemovableStorageWatcher
{
    class Program
    {
        private static List<char> activeDrives = new List<char>();

        static void Main(string[] args)
        {
            Console.WriteLine("----------------------------");
            while (true)
            {
                DriveInfo[] allDrives = DriveInfo.GetDrives();

                var newActiveDrives = new List<char>();
                foreach (DriveInfo drinfo in allDrives)
                {
                    if (drinfo.DriveType == DriveType.Removable && drinfo.IsReady)
                    {
                        newActiveDrives.Add(drinfo.Name[0]);
                    }
                }

                foreach (char letter in newActiveDrives)
                {
                    bool isNewAvailable = true;

                    foreach (char letter2 in activeDrives)
                    {
                        if (letter == letter2)
                        {
                            isNewAvailable = false;
                            break;
                        }
                    }

                    if (isNewAvailable)
                    {
                        Console.WriteLine("New drive available: " + letter + @":\");
                        RunScript(letter);
                        Console.WriteLine("Finished processing drive");
                        Console.WriteLine("----------------------------");
                    }
                }

                activeDrives = newActiveDrives;
            }
        }

        private static void RunScript(char drive)
        {
            Thread.Sleep(5000);
            Process proc = new Process();
            proc.StartInfo.FileName = @"newdrive.bat";
            proc.StartInfo.Arguments = drive.ToString();
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;

            proc.Start();

            string errorMessage = proc.StandardError.ReadToEnd();
            proc.WaitForExit();

            string outputMessage = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            proc.Close();

            Console.WriteLine(errorMessage);
            Console.WriteLine(outputMessage);
        }
    }
}
