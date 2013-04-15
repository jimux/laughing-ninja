using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace RemovableStorageWatcher
{
    class Program
    {
        private static List<char> activeDrives = new List<char>(); // List of currently already known removable drives.
		private static string script = @"newdrive.bat"; // Script to run when a new drive is detected.

        static void Main(string[] args)
        {
			// "newdrive.bat" no good? Pass in a filename or whole path.
			if(args.Length == 1)
			{
				script = args[0];
			}

			// If the given script doesn't exist, bail out. Nothing to 
			if(!new FileInfo(script).Exists)
			{
				Console.WriteLine("Script " + script + " could not be found.");
				Thread.Sleep(5000); // Keep the window open for five seconds, so there's time to see the error.
				return;
			}

            while (true)
            {
				Thread.Sleep(1000); // Poll every one second. No need to hammer the system.
                DriveInfo[] allDrives = DriveInfo.GetDrives();

				// Gather a list of currently active drives.
                var newActiveDrives = new List<char>();
                foreach (DriveInfo drinfo in allDrives)
                {
                    if (drinfo.DriveType == DriveType.Removable && drinfo.IsReady)
                    {
                        newActiveDrives.Add(drinfo.Name[0]);
                    }
                }

				// Compare with the previous list.
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

					// This is a new drive! Kick off the script.
                    if (isNewAvailable)
                    {
                        Console.WriteLine("New drive available: " + letter + @":\");
                        RunScript(letter);
                    }
                }

                activeDrives = newActiveDrives;
            }
        }

        private static void RunScript(char drive)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = script;
            proc.StartInfo.Arguments = drive.ToString(); // Only the drive letter itself is passed.
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
