﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.ComponentModel;
using NLog;
using System.Linq;
using sdrzymala_download_helper;

namespace download_helper
{
    public class DownloadHelper
    {
        public string ConfigFilePath;
        public string DownloadDirectory;
        public bool OverwriteExistingFile = true;
        public bool ClearOutputLogFile = false;
        public int CheckFileLimit = 100;
        public int DownloadFileLimit = 100;
        public bool ExcludeExistingFiles = true;

        Logger logger = LogManager.GetCurrentClassLogger();

        public DownloadHelper()
        {
        }

        public void DownloadAllFiles()
        {
            List<string> allFilesToDownload = GetAllFilesToDownloadFromConfigFile(OperationType.DownloadFile);

            foreach (var file in allFilesToDownload)
            {
                string currentFileName = Path.GetFileName(file);
                string currentFileOutputPath = Path.Combine(DownloadDirectory, currentFileName);

                var currentFileSize = 0; // CheckSingleFileSizeInMB(file);
                bool fileAlreadyExists = File.Exists(currentFileOutputPath);

                if (fileAlreadyExists == false)
                {
                    using (MyWebClient client = new MyWebClient())
                    {
                        logger.Info(" | " + file + " | " + currentFileSize + " | " + InfoType.FileDownloaded.ToString());
                        client.DownloadFile(new Uri(file), currentFileOutputPath);
                    }

                }
                else if (fileAlreadyExists == true && OverwriteExistingFile == true)
                {
                    using (MyWebClient client = new MyWebClient())
                    {
                        logger.Info(" | " + file + " | " + currentFileSize + " | " + InfoType.FileOverwriten.ToString());
                        client.DownloadFile(new Uri(file), currentFileOutputPath);
                    }
                }
                else
                {
                    logger.Info("|" + MethodBase.GetCurrentMethod().Name + " | " + file + " | " + currentFileSize + " | " + InfoType.FileAlreadyExists.ToString());
                }
            }


        }

        public void CheckSizeOfAllFiles()
        {

            List<string> allFilesToDownload = GetAllFilesToDownloadFromConfigFile(OperationType.CheckFileSize);

            foreach (var file in allFilesToDownload)
            {
                var currentFileSize = CheckSingleFileSizeInMB(file);
                if (currentFileSize != null)
                {
                    logger.Info("|" + MethodBase.GetCurrentMethod().Name + " | " + file + " | " + currentFileSize + " | " + InfoType.FileSizeChecked.ToString());
                }
            }
        }

        private List<string> GetAllFilesToDownloadFromConfigFile(OperationType operationType)
        {

            List<string> allFiles = new List<string>(File.ReadAllLines(ConfigFilePath));
            List<string> outputFiles = new List<string>();

            int currentLimit = 0;

            if (operationType == OperationType.CheckFileSize)
            {
                currentLimit = CheckFileLimit;
            }
            else if (operationType == OperationType.DownloadFile)
            {
                currentLimit = DownloadFileLimit;
            }

            if (ExcludeExistingFiles == true)
            {
                int currentFileNumber = 1;
                

                    foreach (var file in allFiles)
                    {
                        string currentFileName = Path.GetFileName(file);
                        string currentFileOutputPath = Path.Combine(DownloadDirectory, currentFileName);
                        bool fileAlreadyExists = File.Exists(currentFileOutputPath);

                        if (fileAlreadyExists == false)
                        {
                            outputFiles.Add(file);

                            if (currentLimit == currentFileNumber)
                            {
                                return outputFiles;
                            }
                            
                            currentFileNumber++;
                        }
                    }

                
            }
            else
            {
                outputFiles = allFiles.Take(currentLimit).ToList();
            }

            return outputFiles;
        }

        private List<string> GetAllFilesToCheckFromConfigFile()
        {
            return new List<string>(File.ReadAllLines(ConfigFilePath)).Take(CheckFileLimit).ToList();
        }

        private string CheckSingleFileSizeInMB(string FileUrl)
        {
            string output;

            using (MyWebClient client = new MyWebClient())
            {
                try
                {
                    client.OpenRead(FileUrl);
                    Int64 bytes_total = Convert.ToInt64(client.ResponseHeaders["Content-Length"]);
                    output = Convert.ToString(Math.Round(((bytes_total / 1024f) / 1024f), 2)) + " MB";
                }
                catch (Exception exc)
                {
                    logger.Error("Problem with checking the file size for " + FileUrl + " | Exception: " + exc.Message);
                    output = "Error";
                }
            }

            return output;

        }

    }

    public class MyWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            var req = base.GetWebRequest(address);
            req.Timeout = 10800000; // 3 hours
            return req;
        }
    }
}
