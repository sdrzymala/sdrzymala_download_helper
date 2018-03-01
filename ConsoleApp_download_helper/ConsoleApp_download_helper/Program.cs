using System;
using download_helper;

namespace ConsoleApp_download_helper
{
    class Program
    {
        static void Main(string[] args)
        {
            DownloadHelper dh = new DownloadHelper();
            dh.ConfigFilePath = @"C:\temp\download_helper_config_file.txt";
            dh.DownloadDirectory = @"C:\temp\";
            dh.OverwriteExistingFile = true;
            dh.CheckSizeOfAllFiles();
            dh.DownloadAllFiles();

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
