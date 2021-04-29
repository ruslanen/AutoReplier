using System;

namespace AutoReplier
{
    public class ConsoleLogger : ILogger
    {
        public void Log(LogLevel logLevel, string text)
        {
            if (logLevel == LogLevel.Error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            var message = string.Format("[{0}]: {1}", DateTime.Now, text);
            
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }

    public enum LogLevel
    {
        Info = 1,
        Error = 2,
    }
}
