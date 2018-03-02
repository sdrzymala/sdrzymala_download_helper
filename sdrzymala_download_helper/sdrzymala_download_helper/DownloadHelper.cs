using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.ComponentModel;
using NLog;

namespace download_helper
{
    public class DownloadHelper
    {
        public string ConfigFilePath;
        public string DownloadDirectory;
        public bool OverwriteExistingFile = true;
        public bool ClearOutputLogFile = false;
        private string ouputLogFileName = "download_helper_log";

        Logger logger = LogManager.GetCurrentClassLogger();

        public DownloadHelper()
        {
        }

        public void DownloadAllFiles()
        {
            List<string> allFilesToDownload = GetAllFilesToDownloadFromConfigFile();

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
                    logger.Info(" | " + file + " | " + currentFileSize + " | " + InfoType.FileDownloaded.ToString());

                }
                else if (fileAlreadyExists == true && OverwriteExistingFile == true)
                {
                    client.DownloadFile(file, currentFileOutputPath);
                    logger.Info(" | " + file + " | " + currentFileSize + " | " + InfoType.FileOverwriten.ToString());
                }
                else
                {
                    logger.Info("|" + MethodBase.GetCurrentMethod().Name + " | " + file + " | " + currentFileSize + " | " + InfoType.FileAlreadyExists.ToString());
                }
            }


        }

        public void CheckSizeOfAllFiles()
        {

            List<string> allFilesToDownload = GetAllFilesToDownloadFromConfigFile();

            foreach (var file in allFilesToDownload)
            {
                var currentFileSize = CheckSingleFileSizeInMB(file);
                if (currentFileSize != null)
                {
                    logger.Info("|" + MethodBase.GetCurrentMethod().Name + " | " + file + " | " + currentFileSize + " | " + InfoType.FileSizeChecked.ToString());
                }
            }
        }

        private List<string> GetAllFilesToDownloadFromConfigFile()
        {
            return new List<string>(File.ReadAllLines(ConfigFilePath)); ;
        }


        private string CheckSingleFileSizeInMB(string FileUrl)
        {
            WebClient client = new WebClient();
            try
            {
                client.OpenRead(FileUrl);
                Int64 bytes_total = Convert.ToInt64(client.ResponseHeaders["Content-Length"]);
                return Convert.ToString(Math.Round(((bytes_total / 1024f) / 1024f), 2)) + " MB";
            } catch(Exception exc)
            {
                logger.Error("Problem with checking the file size for " + FileUrl + " | Exception: " + exc.Message);
                return null; 
            }
            
        }

    }
}
