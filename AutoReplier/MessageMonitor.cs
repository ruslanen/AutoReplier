using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AutoReplier
{
    public class MessageMonitor
    {
        private readonly string _fromMailAddress;
        private readonly int _delayIntervalInSeconds;
        private readonly MessageReader _messageReader;
        private readonly MessageSender _messageSender;
        private ILogger _logger;

        public MessageMonitor(string fromMailAddress, int delayIntervalInSeconds, MessageReader messageReader, MessageSender messageSender, ILogger logger)
        {
            _fromMailAddress = fromMailAddress;
            _delayIntervalInSeconds = delayIntervalInSeconds;
            _messageReader = messageReader;
            _messageSender = messageSender;
            _logger = logger;
        }

        public Task Track()
        {
            // Находит содержимое между текстом subject= и Body=Согласие
            // используется позитивная ретроспектива и позитивная опережающая проверки
            var subjectRegex = new Regex(@"(?<=subject\=).+?(?=\s+Body\=Соглacие)");
            _messageSender.StartSending();
            while (true)
            {
                var allNewMessages = _messageReader.GetNewMessages();
                foreach (var message in allNewMessages)
                {
                    if (message.From.Address == _fromMailAddress && !string.IsNullOrEmpty(message.Body))
                    {
                        var subjectMatch = subjectRegex.Match(message.Body);
                        if (subjectMatch.Success)
                        {
                            _logger.Log(LogLevel.Info, "Новое сообщение добавлено в очередь отправки: " + subjectMatch.Value);
                            _messageSender.AddToQueue(new MessageInfo
                            {
                                MailAddress = _fromMailAddress,
                                Subject = subjectMatch.Value,
                            });
                        }
                        else
                        {
                            _logger.Log(LogLevel.Info, "Тело письма некорректное: " + message.Body);
                        }
                    }
                }

                Thread.Sleep(_delayIntervalInSeconds * 1000);
            }
        }
    }
}
