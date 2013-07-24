using System.Diagnostics;
using CafeteiraDaFast.Components;
using Xunit;

namespace CafeteiraDaFast.Testes
{
    public class CryptoTestes
    {
        [Fact]
        public void TesteEncrypt()
        {
            var original = "teste";
            Debug.WriteLine(Crypto.Encrypt(original));
        }

        [Fact]
        public void TesteDecrypt()
        {
            var original = "teste";

            var encriptado = Crypto.Encrypt("teste");
            Assert.Equal("/fNryZz2gaM=", encriptado);

            var desencriptado = Crypto.Decrypt(encriptado);
            Assert.Equal(original, desencriptado);
        }
    }
}
