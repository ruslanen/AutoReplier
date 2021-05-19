using System;
using System.IO;

namespace AutoReplier
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var logger = new FileLogger(AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "log.txt");
			var configPath = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "appConfig.json";
			var configReader = new ConfigReader(configPath, logger);
			var appConfig = configReader.Config;

			string password = null;
			var i = 0;
			if (args.Length > 0)
			{
				var argument = args[i++];
				while (argument != null)
				{
					switch (argument)
					{
						case "--password":
							{
								if (args.Length < i)
								{
									throw new Exception("Пропущено значение аргумента --password");
								}

								password = args[i++];

								break;
							}
						default:
							throw new Exception("Нераспознанная команда: " + argument);
					}

					argument = i < args.Length ? args[i++] : null;
				}
			}
			else
            {
				password = appConfig.Password;
            }

			using (var messageReader = new MessageReader(appConfig.ImapHost, appConfig.ImapPort, appConfig.ImapSsl, appConfig.MailAddress, password, logger))
			{
				using (var messageSender = new MessageSender(appConfig.MailAddress, password, appConfig.SmtpHost, appConfig.SmtpPort, appConfig.SmtpSsl, appConfig.SendingIntervalDelayInSeconds, logger))
				{
					var messageMonitor = new MessageMonitor(appConfig.MailAddressFrom,appConfig.UpdatingIntervalDelayInSeconds, messageReader, messageSender, logger);
					logger.Log(LogLevel.Info, "Приложение запущено");
					logger.Log(LogLevel.Info, "Ожидание новых сообщений..");
					messageMonitor.Track();
				}
			}
		}
	}
}
