using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.ComponentModel;

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
                string currentFileName = Path.GetFileName(file);
                string currentFileOutputPath = Path.Combine(DownloadDirectory, currentFileName);
                WebClient client = new WebClient();
                var currentFileSize = CheckSingleFileSizeInMB(file);
                bool fileAlreadyExists = File.Exists(currentFileOutputPath);

                if (fileAlreadyExists == false)
                {
                    client.DownloadFile(file, currentFileOutputPath);
                    outputLog.AppendLine(DateTime.Now.ToString("yyyyMMddHHmmss") + " | " + file + " | " + currentFileSize + " | " + InfoType.FileDownloaded.ToString());
                }
                else if (fileAlreadyExists == true && OverwriteExistingFile == true)
                {
                    client.DownloadFile(file, currentFileOutputPath);
                    outputLog.AppendLine(DateTime.Now.ToString("yyyyMMddHHmmss") + " | " + file + " | " + currentFileSize + " | " + InfoType.FileOverwriten.ToString());
                }
                else
                {
                    outputLog.AppendLine(DateTime.Now.ToString("yyyyMMddHHmmss") + " | " + file + " | " + currentFileSize + " | " + InfoType.FileAlreadyExists.ToString());
                }
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
                outputLog.AppendLine(DateTime.Now.ToString("yyyyMMddHHmmss") + " | " + file + " | " + currentFileSize + " | " + InfoType.FileSizeChecked.ToString());
            }

            SaveLog(outputLog, MethodBase.GetCurrentMethod().Name);
        }

        private List<string> GetAllFilesToDownloadFromConfigFile()
        {
            return new List<string>(File.ReadAllLines(ConfigFilePath));; 
        }


        private string CheckSingleFileSizeInMB(string FileUrl)
        {
            WebClient client = new WebClient();
            client.OpenRead(FileUrl);
            Int64 bytes_total = Convert.ToInt64(client.ResponseHeaders["Content-Length"]);
            return Convert.ToString(Math.Round(((bytes_total / 1024f) / 1024f),2)) + " MB";
        }

        private void SaveLog(StringBuilder outputLog, string methodName)
        {
            string outputFileName = DownloadDirectory + ouputLogFileName + methodName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";

            File.WriteAllText(outputFileName, outputLog.ToString());
        }
    }
}
