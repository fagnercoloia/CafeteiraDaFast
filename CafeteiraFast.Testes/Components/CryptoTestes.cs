using System.Diagnostics;
using CafeiteiraFast.Components;
using Xunit;

namespace CafeteiraFast.Testes
{
    public class CryptoTestes
    {
        [Fact]
        public void TesteEncrypt()
        {
            var original = "fcoloia1983";
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
