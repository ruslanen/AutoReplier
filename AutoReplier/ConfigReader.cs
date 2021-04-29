using Newtonsoft.Json;
using System;
using System.IO;

namespace AutoReplier
{
    public class ConfigReader
    {
        private readonly string _path;
        private AppConfig _config;
        private ILogger _logger = new ConsoleLogger();

        public ConfigReader(string path)
        {
            _path = path;
        }

        public AppConfig Config
        {
            get
            {
                if (_config != null)
                {
                    return _config;
                }

                try
                {
                    _config = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(_path));
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, ex.ToString());
                }

                return _config;
            }
        }
    }
}
