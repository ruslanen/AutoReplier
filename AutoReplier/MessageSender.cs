using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading;

namespace AutoReplier
{
    public class MessageSender : IDisposable
    {
        private readonly string _address;
        private readonly int _sendingDelay;

        private Queue<MessageInfo> _sendingQueue = new Queue<MessageInfo>();
        private bool _isStopped = true;
        private readonly SmtpClient _smtpClient;
        private readonly ILogger _logger = new ConsoleLogger();

        public MessageSender(string address, string password, string host, int port, bool enableSsl, int sendingDelay)
        {
            _address = address;
            _sendingDelay = sendingDelay;
            _smtpClient = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                Credentials = new NetworkCredential(address, password),
            };
        }

        public void AddToQueue(MessageInfo messageInfo)
        {
            _sendingQueue.Enqueue(messageInfo);
        }

        public void StartSending()
        {
            _isStopped = false;
            TrackQueue();
        }

        public void StopSending()
        {
            _isStopped = true;
        }

        private void TrackQueue()
        {
            var worker = new Thread(() =>
            {
                while (!_isStopped)
                {
                    if (_sendingQueue.Count > 0)
                    {
                        var message = _sendingQueue.Dequeue();

                        if (!TrySendMessage(message))
                        {
                            _sendingQueue.Enqueue(message);
                        }

                        Thread.Sleep(_sendingDelay * 1000);
                    }
                }
            });
            worker.Start();
        }

        private bool TrySendMessage(MessageInfo messageInfo)
        {
            var message = new MailMessage(_address, messageInfo.MailAddress, messageInfo.Subject, messageInfo.Body);
            try
            {
                _smtpClient.Send(message);
                _logger.Log(LogLevel.Info, "Сообщение отправлено: " + messageInfo.Subject);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "Ошибка при отправке сообщения: " + ex.ToString());
                return false;
            }

            return true;
        }

        public void Dispose()
        {
            _smtpClient.Dispose();
        }
    }
}
