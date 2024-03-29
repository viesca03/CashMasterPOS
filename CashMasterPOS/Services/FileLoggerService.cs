﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CashMasterPOS.Interfaces;
using System.Configuration;

namespace CashMasterPOS.Services
{
    public class FileLoggerService : ILogger
    {
        private readonly string _filePath;

        public FileLoggerService()
        {
            _filePath = ConfigurationManager.AppSettings["LogFilePath"];
            var directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        /// <summary>
        /// A method to insert a message in the Log File which is saved in the path provided in the App.config file.
        /// </summary>
        /// <param name="message">The message to save in the file as a string</param>
        public void Log(string message)
        {
            using (var streamWriter = new StreamWriter(_filePath, true))
            {
                streamWriter.WriteLine(DateTime.Now.ToString() + ": " + message);
            }
        }
    }
}