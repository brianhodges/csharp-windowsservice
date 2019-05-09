using System.Data.SQLite;
using System.Data;
using System;
using System.IO;
using System.Diagnostics;

namespace TestService
{
    public class LogManager
    {
        enum Level
        {
            Debug,
            Info,
            Warn,
            Error
        }

        public LogManager() 
        {
            if (!File.Exists(Extensions.timeLogFile))
            {
                Directory.CreateDirectory(Extensions.filePath);
                Extensions.WriteBroadcastTimeLogFile(DateTime.Now);
            }

            if (!File.Exists(Extensions.filePath + Extensions.databaseName))
            {
                SQLiteConnection.CreateFile(Extensions.filePath + Extensions.databaseName);
                SQLiteConnection m_dbConnection = new SQLiteConnection(Extensions.sqlLiteDataSource);
                m_dbConnection.Open();

                string sql = @"CREATE TABLE Log (
                                  Timestamp DATETIME,
                                  Level     TEXT,
                                  User      TEXT,
                                  Callsite  TEXT,
                                  Message   TEXT
                              )";

                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();
                m_dbConnection.Close();
            }
        }

        public void Debug(string message)
        {
            StackTrace trace = new StackTrace();
            LogRecord(Level.Debug.ToString(), message, trace.GetFrame(1).GetMethod().ReflectedType.FullName);
        }

        public void Info(string message)
        {
            StackTrace trace = new StackTrace();
            LogRecord(Level.Info.ToString(), message, trace.GetFrame(1).GetMethod().ReflectedType.FullName);
        }

        public void Warn(string message)
        {
            StackTrace trace = new StackTrace();
            LogRecord(Level.Warn.ToString(), message, trace.GetFrame(1).GetMethod().ReflectedType.FullName);
        }

        public void Error(string message)
        {
            StackTrace trace = new StackTrace();
            LogRecord(Level.Error.ToString(), message, trace.GetFrame(1).GetMethod().ReflectedType.FullName);
        }

        protected void LogRecord(string _level, string _message, string _stacktrace)
        {
            SQLiteConnection m_dbConnection = new SQLiteConnection(Extensions.sqlLiteDataSource);
            m_dbConnection.Open();

            string sql = @"INSERT INTO Log 
                                (Timestamp, Level, User, Callsite, Message) 
                           VALUES 
                                (@timestamp, @level, @user, @callsite, @message);";

            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);

            StackTrace trace = new StackTrace();

            command.Parameters.Add("@timestamp", DbType.DateTime).Value = DateTime.Now;
            command.Parameters.Add("@level", DbType.String).Value = _level;
            command.Parameters.Add("@user", DbType.String).Value = Environment.UserName;
            command.Parameters.Add("@callsite", DbType.String).Value = _stacktrace;
            command.Parameters.Add("@message", DbType.String).Value = _message;

            command.ExecuteNonQuery();
            m_dbConnection.Close();
        }
    }
}
