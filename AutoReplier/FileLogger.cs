using System;
using System.IO;

namespace AutoReplier
{
    public class FileLogger : ILogger
    {
        private static object _lock = new object();
        private readonly string _path;

        public FileLogger(string path)
        {
            _path = path;
        }

        public void Log(LogLevel logLevel, string text)
        {
            lock (_lock)
            {
                var message = string.Format("[{0}]: {1}\n", DateTime.Now, text);
                File.AppendAllText(_path, message);
            }
        }
    }
}
