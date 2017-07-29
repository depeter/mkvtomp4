using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ConvertMKVToMP4
{
    class Program
    {
        private static string _workingFolder;
        private static bool _deleteOriginal = true;

        static void Main(string[] args)
        {
            if (string.IsNullOrWhiteSpace(args[0]))
            {
                throw new ArgumentException("Provide the root path as the first parameter that should be watched for converting mkv files to mp4.");
            }

            _workingFolder = args[0];

            if (!Directory.Exists(args[0]))
            {
                throw new ArgumentException("The provided path does not exists.");
            }

            if (args.Length > 1)
                bool.TryParse(args[1], out _deleteOriginal);

            var fsw = new FileSystemWatcher(_workingFolder)
            {
                IncludeSubdirectories = true,
                EnableRaisingEvents = true,
                Filter = "*.mkv"
            };
            fsw.Changed += (sender, eventArgs) => ConvertFile(eventArgs.FullPath);
            fsw.Created += (sender, eventArgs) => ConvertFile(eventArgs.FullPath);
            fsw.Renamed += (sender, eventArgs) => ConvertFile(eventArgs.FullPath);

            Console.WriteLine("scanning for .mkv files...");
            var mkvFiles = Directory.GetFiles(_workingFolder, "*.mkv", SearchOption.AllDirectories);
            Console.WriteLine($"{mkvFiles.Length} files found. Starting conversion...");
            foreach (var mkvFile in mkvFiles)
            {
                ConvertFile(mkvFile);
            }

            while (true)
            {
                Console.WriteLine("Waiting for mkv files...");
                fsw.WaitForChanged(WatcherChangeTypes.All);
            }
        }

        private static void ConvertFile(string fullPath)
        {
            Console.WriteLine("Starting conversion for " + fullPath);
            var extension = Path.GetExtension(fullPath);
            if (extension != null && !extension.Equals(".mkv", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("Not a valid mkv file, aborting.");
                return;
            }

            var mp4Location = Path.ChangeExtension(fullPath, ".mp4");

            if (File.Exists(mp4Location))
            {
                Console.WriteLine("This mkv file has already been converted, aborting.");
                return;
            }

            ConvertToMp4(fullPath, mp4Location);

            if (_deleteOriginal)
            {
                if (File.Exists(mp4Location))
                {
                    if (fullPath != null)
                    {
                        File.Delete(fullPath);
                        Console.WriteLine("Original file deleted.");
                    }
                }
            }
            Console.WriteLine("Conversion complete.");
        }

        private static void ConvertToMp4(string infile, string outfile)
        {
            ProcessStartInfo psi;
            Process proc;

            psi = new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                FileName = Directory.GetCurrentDirectory() + @"\ffmpeg\ffmpeg.exe",
                Arguments = "-i \"" + infile + "\" -vcodec copy -acodec copy \"" + outfile + "\""
            };


            proc = new Process
            {
                StartInfo = psi,
                EnableRaisingEvents = true
            };

            proc.OutputDataReceived += delegate (object sender, DataReceivedEventArgs e)
            {
                Console.WriteLine(e.Data);
            };
            proc.ErrorDataReceived += delegate (object sender, DataReceivedEventArgs e)
            {
                Console.WriteLine(e.Data);
            };

            proc.Start();
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();

            proc.WaitForExit();

            if (proc.HasExited)
            {
                proc.CancelErrorRead();
                proc.CancelOutputRead();
                proc.Close(); 
                
            }
        }
    }
}
