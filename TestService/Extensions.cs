using System;
using System.IO;

namespace TestService
{
    public class Extensions
    {
        public const string filePath = "C:\\Services\\tmp\\";
        public const string databaseName = "log.db3";
        public const string fileName = "lastServiceBroadcast.txt";
        public const string sqlLiteDataSource = "Data Source=" + Extensions.filePath + Extensions.databaseName + ";";
        public const string timeLogFile = Extensions.filePath + fileName;

        public static void WriteBroadcastTimeLogFile(DateTime date)
        {
            using (StreamWriter file = new StreamWriter(Extensions.timeLogFile, false))
            {
                file.WriteLine(date);
            }
        }
    }
}
