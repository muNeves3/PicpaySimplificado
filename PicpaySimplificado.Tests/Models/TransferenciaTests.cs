using PicpaySimplificado.Models;
using PicpaySimplificado.Models.Enum;
using Xunit;

namespace PicpaySimplificado.Tests.Models
{
    public class TransferenciaTests
    {
        [Fact]
        public void Transferencia_DeveCriarComValoresCorretos()
        {
            int senderId = 1;
            int reciverId = 2;
            decimal valor = 100.50m;

            var transferencia = new Transferencia(senderId, reciverId, valor);

            Assert.NotEqual(Guid.Empty, transferencia.IdTransferencia);
            Assert.Equal(senderId, transferencia.SenderId);
            Assert.Equal(reciverId, transferencia.ReciverId);
            Assert.Equal(valor, transferencia.Valor);
        }

        [Fact]
        public void Transferencia_DeveGerarGuidUnico_ParaCadaInstancia()
        {
            var transferencia1 = new Transferencia(1, 2, 50m);
            var transferencia2 = new Transferencia(1, 2, 50m);

            Assert.NotEqual(transferencia1.IdTransferencia, transferencia2.IdTransferencia);
        }

        [Fact]
        public void Transferencia_DeveAceitarValorZero()
        {
            var transferencia = new Transferencia(1, 2, 0m);

            Assert.Equal(0m, transferencia.Valor);
        }

        [Fact]
        public void Transferencia_DeveAceitarValoresDecimais()
        {
            decimal valorDecimal = 123.456789m;
            var transferencia = new Transferencia(1, 2, valorDecimal);

            Assert.Equal(valorDecimal, transferencia.Valor);
        }

        [Fact]
        public void Transferencia_DevePermitirMesmoSenderEReciver()
        {
            var transferencia = new Transferencia(1, 1, 100m);

            Assert.Equal(transferencia.SenderId, transferencia.ReciverId);
        }

        [Fact]
        public void Transferencia_DevePermitirNavigationProperties()
        {
            var sender = new Carteira("Sender", "12345678901", "sender@email.com", "senha123", UserType.USUARIO, 500m);
            var reciver = new Carteira("Reciver", "98765432100", "reciver@email.com", "senha456", UserType.USUARIO, 100m);

            var transferencia = new Transferencia(1, 2, 100m)
            {
                Sender = sender,
                Reciver = reciver
            };

            Assert.Equal(sender, transferencia.Sender);
            Assert.Equal(reciver, transferencia.Reciver);
            Assert.Equal("Sender", transferencia.Sender.NomeCompleto);
            Assert.Equal("Reciver", transferencia.Reciver.NomeCompleto);
        }

        [Fact]
        public void Transferencia_DeveAceitarValoresAltos()
        {
            decimal valorAlto = 999999999.99m;
            var transferencia = new Transferencia(1, 2, valorAlto);

            Assert.Equal(valorAlto, transferencia.Valor);
        }
    }
}
