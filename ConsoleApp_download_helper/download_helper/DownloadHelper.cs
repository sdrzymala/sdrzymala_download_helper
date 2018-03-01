using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace download_helper
{
    public class DownloadHelper
    {
        public string ConfigFilePath;
        public string DownloadDirectory;
        public bool OverwriteExistingFile = true;
        private string ouputLogFileName = "download_helper_log_";

        public void DownloadAllFiles()
        {
            List<string> allFilesToDownload = GetAllFilesToDownloadFromConfigFile();
            StringBuilder outputLog = new StringBuilder();

            foreach (var file in allFilesToDownload)
            {
                var currentFileSize = CheckSingleFileSizeInMB(file);
                DownloadSingleFile(file);
                outputLog.AppendLine(DateTime.Now.ToString("yyyyMMddHHmmss") + " | " + file + " | " + currentFileSize);
            }

            SaveLog(outputLog, MethodBase.GetCurrentMethod().Name);
        }

        public void CheckSizeOfAllFiles()
        {
            List<string> allFilesToDownload = GetAllFilesToDownloadFromConfigFile();
            StringBuilder outputLog = new StringBuilder();

            foreach (var file in allFilesToDownload)
            {
                var currentFileSize = CheckSingleFileSizeInMB(file);
                outputLog.AppendLine(DateTime.Now.ToString("yyyyMMddHHmmss") + " | " + file + " | " + currentFileSize);
            }

            SaveLog(outputLog, MethodBase.GetCurrentMethod().Name);
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
                var fileSize = CheckSingleFileSizeInMB(FileUrl);
                client.DownloadFile(FileUrl, currentFileOutputPath);
            }
        }

        private double CheckSingleFileSizeInMB(string FileUrl)
        {
            WebClient client = new WebClient();
            client.OpenRead(FileUrl);
            Int64 bytes_total = Convert.ToInt64(client.ResponseHeaders["Content-Length"]);
            return Math.Round(((bytes_total / 1024f) / 1024f),2);
        }

        private void SaveLog(StringBuilder outputLog, string methodName)
        {
            string outputFileName = DownloadDirectory + ouputLogFileName + methodName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";

            File.WriteAllText(outputFileName, outputLog.ToString());
        }
    }
}
