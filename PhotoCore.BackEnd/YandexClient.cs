using System;
using System.Collections.Generic;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MailKit;

namespace PhotoCore
{
    public class YandexClient
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        string host;
        int port;
        string login;
        string password;

        string warningMsg;
        string errorMsg;
        string traceMsg;
        string orderId;
        string userEmail;

        /// <summary> Событие успешной доставки почтового сообщения </summary>
        /// <param name="client"> клиент события </param>
        /// <param name="traceMessage"> сообщение трейсера </param>
        /// <param name="errorMessage"> сообщения с ошибками </param>
        /// <param name="warningMessage"> сообщения с предупреждениями </param>
        public delegate void MessageSentHandler(YandexClient client, string orderId, string userEmail, string traceMessage, string errorMessage, string warningMessage);
        public event MessageSentHandler OnMessageSent;

        /// <summary> Событие ошибки при попытке доставки почтового сообщения </summary>
        /// <param name="client"> клиент события </param>
        /// <param name="traceMessage"> сообщение трейсера </param>
        /// <param name="errorMessage"> сообщения с ошибками </param>
        /// <param name="warningMessage"> сообщения с предупреждениями </param>
        public delegate void ErrorHandler(YandexClient client, string orderId, string userEmail, string traceMessage, string errorMessage, string warningMessage);
        public event ErrorHandler OnError;


        public string OrderId { get { return orderId; } }
        public string UserEmail { get { return userEmail; } }
        public string TraceMessage { get { return traceMsg; } }
        public string ErrorMessage { get { return errorMsg; } }
        public string WarningMessage { get { return warningMsg; } }


        /// <summary> Инициализация SNMP-клиента </summary>
        /// <param name="host"> адрес почтового сервера </param>
        /// <param name="port"> порт почтового сервера </param>
        /// <param name="login"> имя учётной записи пользователя </param>
        /// <param name="password"> пароль доступа к учётной записи пользователя </param>
        public YandexClient(string host, int port, string login, string password)
        {
            this.host = host; this.port = port; this.login = login; this.password = password;
        }

        /// <summary>
        /// Метод опправки письма
        /// </summary>
        /// <param name="from"> Адрес отправителя письма </param>
        /// <param name="fromName"> Имя отправителя письма </param>
        /// <param name="to"> Адресат письма </param>
        /// <param name="bcc"> Список адресов скрытой копии </param>
        /// <param name="subject"> Тема письма </param>
        /// <param name="message"> Сообщение </param>
        /// <param name="timeout"> Таймаут ожидания в миллисекундах </param>
        /// <returns> Результат: true - письмо отправлено, false - письмо не отправлено. </returns>
        public bool SendEmail(string orderId, string from, string fromName, string to, List<string> bcc, string subject, string message, int timeout)
        {
            this.orderId = orderId; this.userEmail = to;
            Console.WriteLine(String.Format("{0} Initiated.", DateTime.Now));

            warningMsg = null;
            errorMsg = null;
            traceMsg = null;

            try { System.Net.Mail.MailAddress e = new System.Net.Mail.MailAddress(to); }
            catch (ArgumentNullException ex) { errorMsg = "Recipient address is null."; }
            catch (ArgumentException ex) { errorMsg = "Recipient address error. " + ex.Message; }
            catch (FormatException ex) { errorMsg = "Recipient address error. " + ex.Message; }
            catch (Exception ex) { errorMsg = "Recipient address error. " + ex.Message; }
            bool result = false;

            if (errorMsg == null)
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(fromName, from));
                emailMessage.To.Add(new MailboxAddress("", to));
                //emailMessage.Importance = MessageImportance.Normal;//High;
                //emailMessage.Priority = MessagePriority.Normal;//Urgent;
                //emailMessage.XPriority = XMessagePriority.High;//Highest;

                if (bcc != null && bcc.Count > 0)
                {
                    for (int i = 0; i < bcc.Count; i++)
                    {
                        try
                        {
                            System.Net.Mail.MailAddress e = new System.Net.Mail.MailAddress(bcc[i]);
                            emailMessage.Bcc.Add(new MailboxAddress(bcc[i]));
                        }
                        catch (ArgumentNullException ex) { warningMsg = "Copy address is null"; }
                        catch (ArgumentException ex) { warningMsg = "Copy email address error. " + ex.Message; }
                        catch (FormatException ex) { warningMsg = "Copy email address error. " + ex.Message; }
                        catch (Exception ex) { warningMsg = "Copy email address error. " + ex.Message; }
                    }
                }

                if (subject != null) emailMessage.Subject = subject;

                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message };

                using (System.IO.MemoryStream logStream = new System.IO.MemoryStream())
                {
                    using (MailKit.Net.Smtp.SmtpClient smtpClient = new MailKit.Net.Smtp.SmtpClient(new ProtocolLogger(logStream, false)))
                    {
                        smtpClient.Timeout = timeout;
                        try
                        {
                            smtpClient.Connect(host, port, SecureSocketOptions.Auto);
                            Console.WriteLine(String.Format("{0} Connected.", DateTime.Now));
                        }
                        catch (SmtpCommandException ex)
                        {
                            errorMsg = String.Format("Error trying to connect: {0}, StatusCode: {1}", ex.Message, ex.StatusCode);
                            traceMsg = GetTraceMessage(logStream);
                        }
                        catch (SmtpProtocolException ex)
                        {
                            errorMsg = String.Format("Protocol error while trying to connect: {0}", ex.Message);
                            traceMsg = GetTraceMessage(logStream);
                        }
                        catch (Exception ex)
                        {
                            if (ex.InnerException != null) errorMsg = ex.InnerException.Message;
                            else errorMsg = ex.Message;
                            traceMsg = GetTraceMessage(logStream);
                        }

                        if (smtpClient.IsConnected)
                        {
                            if (smtpClient.Capabilities.HasFlag(SmtpCapabilities.Authentication))
                            {
                                try
                                {
                                    smtpClient.Authenticate(login, password);
                                    Console.WriteLine(String.Format("{0} Authenticated.", DateTime.Now));
                                }
                                catch (MailKit.Security.AuthenticationException ex)
                                {
                                    errorMsg = "Invalid user name or password.";
                                    traceMsg = GetTraceMessage(logStream);
                                }
                                catch (SmtpCommandException ex)
                                {
                                    errorMsg = String.Format("Error trying to authenticate: {0}, StatusCode: {1}", ex.Message, ex.StatusCode);
                                    traceMsg = GetTraceMessage(logStream);
                                }
                                catch (SmtpProtocolException ex)
                                {
                                    errorMsg = String.Format("Protocol error while trying to authenticate: {0}", ex.Message);
                                    traceMsg = GetTraceMessage(logStream);
                                }
                                catch (Exception ex)
                                {
                                    if (ex.InnerException != null) errorMsg = ex.InnerException.Message;
                                    else errorMsg = ex.Message;
                                    traceMsg = GetTraceMessage(logStream);
                                }
                            }
                        }

                        if (smtpClient.IsAuthenticated)
                        {
                            try
                            {
                                smtpClient.Send(emailMessage);

                                Console.WriteLine(String.Format("{0} Sent.", DateTime.Now));
                                result = true;
                                traceMsg = GetTraceMessage(logStream);
                                if (OnMessageSent != null) OnMessageSent(this, orderId, to, traceMsg, errorMsg, warningMsg);
                            }
                            catch (SmtpCommandException ex)
                            {
                                switch (ex.ErrorCode)
                                {
                                    case SmtpErrorCode.RecipientNotAccepted:
                                        errorMsg = String.Format("Error sending message: {0}, StatusCode: {1}. Recipient not accepted: {2}", ex.Message, ex.StatusCode, ex.Mailbox);
                                        traceMsg = GetTraceMessage(logStream);
                                        break;
                                    case SmtpErrorCode.SenderNotAccepted:
                                        errorMsg = String.Format("Error sending message: {0}, StatusCode: {1}. Sender not accepted: {2}", ex.Message, ex.StatusCode, ex.Mailbox);
                                        traceMsg = GetTraceMessage(logStream);
                                        break;
                                    case SmtpErrorCode.MessageNotAccepted:
                                        errorMsg = String.Format("Error sending message: {0}, StatusCode: {1}. Message not accepted.", ex.Message, ex.StatusCode);
                                        traceMsg = GetTraceMessage(logStream);
                                        break;
                                    default:
                                        errorMsg = String.Format("Error sending message: {0}, StatusCode: {1}", ex.Message, ex.StatusCode);
                                        traceMsg = GetTraceMessage(logStream);
                                        break;
                                }
                            }
                            catch (SmtpProtocolException ex)
                            {
                                errorMsg = String.Format("Protocol error while sending message: {0}", ex.Message);
                                traceMsg = GetTraceMessage(logStream);
                            }
                            catch (Exception ex)
                            {
                                if (ex.InnerException != null) errorMsg = ex.InnerException.Message;
                                else errorMsg = ex.Message;
                                traceMsg = GetTraceMessage(logStream);
                            }
                        }
                        try { if (smtpClient.IsConnected) smtpClient.Disconnect(false); }
                        catch { }
                    }
                }
            }
            if (errorMsg != null && OnError != null) OnError(this, orderId, to, traceMsg, errorMsg, warningMsg);
            return result;
        }

        /// <summary> Метод получения сообщения трейса обмена </summary>
        /// <param name="logStream"></param>
        /// <returns> трейс обмена почтового клиента с сервером </returns>
        public string GetTraceMessage(System.IO.MemoryStream logStream)
        {
            string message = null;
            try
            {
                logStream.Position = 0;
                using (System.IO.StreamReader reader = new System.IO.StreamReader(logStream))
                {
                    message = reader.ReadToEnd();
                }
            }
            catch (Exception exc) { }
            try
            {
                logStream.Dispose();
            }
            catch { }
            return message;
        }
    }
}
