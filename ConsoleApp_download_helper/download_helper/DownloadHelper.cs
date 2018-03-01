using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace download_helper
{
    public class DownloadHelper
    {
        public string ConfigFilePath;
        public string DownloadDirectory;
        public bool OverwriteExistingFile = true;

        public void GetAllFiles()
        {
            List<string> allFilesToDownload = GetAllFilesToDownloadFromConfigFile();

            foreach(var file in allFilesToDownload)
            {
                DownloadSingleFile(file);
            }
        }

        private List<string> GetAllFilesToDownloadFromConfigFile()
        {
            return new List<string>(File.ReadAllLines(ConfigFilePath));; 
        }

        private void DownloadSingleFile(string FileUrl)
        {
            string currentFileName = Path.GetFileName(FileUrl);
            string currentFileOutputPath = Path.Combine(DownloadDirectory, currentFileName);
            WebClient client = new WebClient();

            bool fileAlreadyExists = File.Exists(currentFileOutputPath);

            if (fileAlreadyExists == false || (fileAlreadyExists == true && OverwriteExistingFile == true))
            {
                var fileSize = CheckFileSizeInMB(FileUrl);
                client.DownloadFile(FileUrl, currentFileOutputPath);
            }
        }

        private double CheckFileSizeInMB(string FileUrl)
        {
            WebClient client = new WebClient();
            client.OpenRead(FileUrl);
            Int64 bytes_total = Convert.ToInt64(client.ResponseHeaders["Content-Length"]);
            return Math.Round(((bytes_total / 1024f) / 1024f),2);
        }
    }
}
