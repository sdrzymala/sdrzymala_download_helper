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
            WebClient Client = new WebClient();

            string currentFileName = Path.GetFileName(FileUrl);
            string currentFileOutputPath = Path.Combine(DownloadDirectory, currentFileName);

            bool fileAlreadyExists = File.Exists(currentFileOutputPath);

            if (fileAlreadyExists == false || (fileAlreadyExists == true && OverwriteExistingFile == true))
            {
                Client.DownloadFile(FileUrl, currentFileOutputPath);
            }
        }
    }
}
