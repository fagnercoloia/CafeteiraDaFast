using Xunit;
using System.Net.Mail;
using System.Net;
using System;
using CafeiteiraFast.Components;

namespace CafeteiraFast.Testes
{
    public class SendMailTestes
    {
        [Fact]
        public void TesteEnviarEmail()
        {
            SendMail.Send("fagner.coloia@gmail.com", "Teste", "Testando 123");
        }
    }
}
