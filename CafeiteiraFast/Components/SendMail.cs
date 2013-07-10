using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;
using System.Configuration;

namespace CafeiteiraFast.Components
{
    public static class SendMail
    {
        public static void Send(string to, string subject, string message)
        {
            var cliente = new SmtpClient();
            var credential = cliente.Credentials as NetworkCredential;
            credential.Password = Crypto.Decrypt(credential.Password);

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