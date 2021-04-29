using System.IO;
using System.Linq;
using System.Text;
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
        private readonly ILogger _logger = new ConsoleLogger();

        public MessageMonitor(string fromMailAddress, int delayIntervalInSeconds, MessageReader messageReader, MessageSender messageSender)
        {
            _fromMailAddress = fromMailAddress;
            _delayIntervalInSeconds = delayIntervalInSeconds;
            _messageReader = messageReader;
            _messageSender = messageSender;
        }

        public Task Track()
        {
            // Находит содержимое между текстом <a href=" и ">Гарантируем
            // используется позитивная ретроспектива и позитивная опережающая проверки
            var regexAnchor = new Regex(@"(?<=\<a href\=\"").+?(?=\""\>Гарантируем)");
            // Находим содержимое для темы письма
            var regexSubject = new Regex(@"act_.+?(?=\"")");
            _messageSender.StartSending();
            while (true)
            {
                var allNewMessages = _messageReader.GetNewMessages();
                foreach (var message in allNewMessages)
                {
                    if (message.From.Address == _fromMailAddress)
                    {
                        var stream = message.AlternateViews.Select(x => x.ContentStream).FirstOrDefault();
                        using (TextReader reader = new StreamReader(stream, Encoding.UTF8, true))
                        {
                            var text = reader.ReadToEnd();
                            var anchorContentMatch = regexAnchor.Match(text);
                            if (anchorContentMatch.Success)
                            {
                                var subjectMatch = regexSubject.Match(anchorContentMatch.Value);
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
                                    _logger.Log(LogLevel.Info, "Ссылка \"Гарантируем\" содержит некорректную тему письма: " + anchorContentMatch.Value);
                                }
                            }
                            else
                            {
                                _logger.Log(LogLevel.Info, "Текст сообщения не содержит ссылки \"Гарантируем\": " + text);
                            }
                        }
                    }
                }

                Thread.Sleep(_delayIntervalInSeconds * 1000);
            }
        }
    }
}
