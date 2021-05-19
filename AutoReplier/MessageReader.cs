using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using S22.Imap;

namespace AutoReplier
{
    public class MessageReader : IDisposable
    {
        private readonly ImapClient _imapClient;
        private ILogger _logger;

        public MessageReader(string host, int port, bool enableSsl, string email, string password, ILogger logger)
        {
            _logger = logger;

            try
            {
                _imapClient = new ImapClient(host, port, enableSsl);
                _imapClient.Login(email, password, AuthMethod.Login);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "Ошибка при авторизации по IMAP: " + ex.ToString());
                throw ex;
            }
        }

        public IEnumerable<MailMessage> GetNewMessages()
        {
            IEnumerable<MailMessage> messages = null;
            try
            {
                var newMessageUids = _imapClient.Search(SearchCondition.Unseen());
                messages = _imapClient.GetMessages(newMessageUids);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Info, "Повторное получение сообщений: " + ex.ToString());
                return new MailMessage[0];
            }

            return messages != null ? messages : new MailMessage[0];
        }

        public void Dispose()
        {
            _imapClient.Dispose();
        }
    }
}
