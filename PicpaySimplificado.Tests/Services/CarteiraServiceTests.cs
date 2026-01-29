using Moq;
using Xunit;
using PicpaySimplificado.Services.Carteiras;
using PicpaySimplificado.Infra.Repository.Carteiras;
using PicpaySimplificado.Models.Request;
using PicpaySimplificado.Models;
using PicpaySimplificado.Models.Enum;

namespace PicpaySimplificado.Tests.Services
{
    public class CarteiraServiceTests
    {
        private readonly Mock<ICarteiraRepository> _repositoryMock;
        private readonly CarteiraService _service;

        public CarteiraServiceTests()
        {
            _repositoryMock = new Mock<ICarteiraRepository>();
            _service = new CarteiraService(_repositoryMock.Object);
        }

        [Fact]
        public async Task CriarCarteiraAsync_DeveRetornarFalha_QuandoCarteiraJaExiste()
        {
            var request = new CarteiraRequest { CPFCNPJ = "123", Email = "teste@teste.com" };
            _repositoryMock.Setup(r => r.GetByCpfCnpj(request.CPFCNPJ, request.Email))
                           .ReturnsAsync(new Carteira("Existente", "123", "teste@teste.com", "123", UserType.USUARIO));

            var result = await _service.CriarCarteiraAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Equal("Já existe uma carteira cadastrada com esse CPF/CNPJ ou Email.", result.ErrorMessage);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Carteira>()), Times.Never);
        }

        [Fact]
        public async Task CriarCarteiraAsync_DeveRetornarSucesso_QuandoDadosSaoValidos()
        {
            var request = new CarteiraRequest
            {
                NomeCompleto = "Novo Usuario",
                CPFCNPJ = "456",
                Email = "novo@email.com",
                Senha = "password",
                UserType = UserType.USUARIO,
                Saldo = 100
            };

            _repositoryMock.Setup(r => r.GetByCpfCnpj(request.CPFCNPJ, request.Email))
                           .ReturnsAsync((Carteira?)null);
            var result = await _service.CriarCarteiraAsync(request);

            Assert.True(result.IsSuccess);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Carteira>()), Times.Once);
            _repositoryMock.Verify(r => r.CommitAsync(), Times.Once);
        }
    }
}