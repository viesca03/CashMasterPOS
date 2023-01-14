using System;
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

        public FileLoggerService(string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            _filePath = filePath;
        }

        public void Log(string message)
        {
            using (var streamWriter = new StreamWriter(_filePath, true))
            {
                streamWriter.WriteLine(DateTime.Now.ToString() + ": " + message);
            }
        }
    }
}