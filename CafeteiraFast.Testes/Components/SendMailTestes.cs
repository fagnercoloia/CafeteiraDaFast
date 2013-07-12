using CafeiteiraFast.Components;
using Xunit;

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
