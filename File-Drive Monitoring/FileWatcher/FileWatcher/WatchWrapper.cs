using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace FileWatcher
{
    class WatchWrapper
    {
        private string directory;
        private string script;
        private char DriveLetter;
        private FileSystemWatcher watcher;
        private readonly object syncLock = new object();

        // Synchronized or not? Should uploads be serial or parallel?
        private static readonly bool isSynchronized = false;

        public WatchWrapper(string directory, string script)
        {
            this.directory = directory;
            this.script = script;
            this.DriveLetter = new DirectoryInfo(directory).FullName[0];

            watcher = new FileSystemWatcher();
            watcher.Path = directory;
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Created += new FileSystemEventHandler(NewFile);
            watcher.EnableRaisingEvents = true;
            Console.WriteLine("Watching directory " + directory + " with script " + script);
        }

        private void NewFile(object source, FileSystemEventArgs e)
        {
            if (isSynchronized)
            {
                lock (syncLock)
                {
                    ProcessFile(e.FullPath);
                }
            }
            else
            {
                ProcessFile(e.FullPath);
            }
        }

        public void Close()
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }

        private void ProcessFile(string path)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = this.script;
            proc.StartInfo.Arguments = path;
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
