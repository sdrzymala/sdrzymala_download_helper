using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using download_helper;

namespace ConsoleApp_test_download_helper
{
    class Program
    {
        static void Main(string[] args)
        {
            DownloadHelper dh = new DownloadHelper();
            dh.ConfigFilePath = @"C:\temp\download_helper_config_file.txt";
            dh.DownloadDirectory = @"C:\temp\";
            dh.OverwriteExistingFile = true;
            dh.DownloadFileLimit = 10;
            //dh.CheckFileLimit = 999999;
            //dh.CheckSizeOfAllFiles();
            dh.DownloadAllFiles();

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
