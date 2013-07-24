using CafeteiraDaFast.Components;
using Xunit;

namespace CafeteiraDaFast.Testes
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
