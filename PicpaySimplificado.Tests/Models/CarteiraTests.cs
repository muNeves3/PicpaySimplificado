using PicpaySimplificado.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PicpaySimplificado.Models.Enum;

namespace PicpaySimplificadoTests.Models
{
    public class CarteiraTests
    {
        [Fact]
        public void DebitarSaldo_DeveDebitar50() 
        {
            var carteira = new Carteira(
                "teste da silva",
                "12345678909",
               "teste@email.com",
                "123",
                UserType.USUARIO,
                new Decimal(100.00)
            );

            carteira.DebitarSaldo(50);

            Assert.Equal(50, carteira.SaldoConta);
        }

        [Fact]
        public void DebitarSaldo_NaoDeveDebitar_SeSaldoMenorQueValor()
        {
            var carteira = new Carteira(
                "teste da silva",
                "12345678909",
               "teste@email.com",
                "123",
                UserType.USUARIO,
                new Decimal(20)
            );

            var exception = Assert.Throws<InvalidOperationException>(() => carteira.DebitarSaldo(50));

            Assert.Equal("Não é possível debitar esse valor", exception.Message);
        }

        [Fact]
        public void CreditarSaldo_DeveCreditar50()
        {
            var carteira = new Carteira(
               "teste da silva",
               "12345678909",
              "teste@email.com",
               "123",
               UserType.USUARIO,
               new Decimal(100.00)
           );

            carteira.CreditarSaldo(50);

            Assert.Equal(150, carteira.SaldoConta);
        }
    }
}
