using System;
using System.IO;

namespace AutoReplier
{
	public class Program
	{
		static void Main(string[] args)
		{
			var configPath = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "appConfig.json";
			var configReader = new ConfigReader(configPath);
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
				Console.WriteLine("Введите пароль от {0}: ", appConfig.MailAddress);
				password = Convert.ToString(Console.ReadLine()).Trim();
			}

			using (var messageReader = new MessageReader(appConfig.ImapHost, appConfig.ImapPort, appConfig.ImapSsl,
				appConfig.MailAddress, password))
			{
				using (var messageSender = new MessageSender(appConfig.MailAddress, password, appConfig.SmtpHost,
					appConfig.SmtpPort, appConfig.SmtpSsl, appConfig.SendingIntervalDelayInSeconds))
				{
					var messageMonitor = new MessageMonitor(appConfig.MailAddressFrom,
						appConfig.UpdatingIntervalDelayInSeconds, messageReader, messageSender);
					Console.WriteLine("Приложение запущено");
					Console.WriteLine("Ожидание новых сообщений..");
					messageMonitor.Track();
				}
			}
		}
	}
}
