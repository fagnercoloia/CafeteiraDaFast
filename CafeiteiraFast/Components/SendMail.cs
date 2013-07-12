using System.Net;
using System.Net.Mail;

namespace CafeiteiraFast.Components
{
    public static class SendMail
    {
        public static void Send(string to, string subject, string message)
        {
            var cliente = new SmtpClient();
            var credential = cliente.Credentials as NetworkCredential;

            var remetente = new MailAddress(credential.UserName);
            var destinatario = new MailAddress(to);

            var mensagem = new MailMessage(remetente, destinatario)
            {
                Subject = subject,
                Body = message, 
                IsBodyHtml = true
            };

            cliente.Send(mensagem);
        }
    }
}