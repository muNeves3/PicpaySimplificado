using Microsoft.AspNetCore.Mvc;
using Moq;
using PicpaySimplificado.Controllers;
using PicpaySimplificado.Models;
using PicpaySimplificado.Models.Request;
using PicpaySimplificado.Models.Response;
using PicpaySimplificado.Services.Carteiras;
using Xunit;

namespace PicpaySimplificado.Tests.Controllers
{
    public class CarteiraControllerTests
    {
        private readonly Mock<ICarteiraService> _serviceMock;
        private readonly CarteiraController _controller;

        public CarteiraControllerTests()
        {
            _serviceMock = new Mock<ICarteiraService>();
            var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<CarteiraController>>();
            _controller = new CarteiraController(_serviceMock.Object, loggerMock.Object);
        }

        [Fact]
        public async Task PostCarteira_DeveRetornarCreated_QuandoSucesso()
        {
            var request = new CarteiraRequest { NomeCompleto = "Teste" };
            var resultSucesso = Result<bool>.Success(true);

            _serviceMock.Setup(s => s.CriarCarteiraAsync(request))
                        .ReturnsAsync(resultSucesso);

            var response = await _controller.PostCarteira(request);

            Assert.IsType<CreatedResult>(response);
        }

        [Fact]
        public async Task PostCarteira_DeveRetornarBadRequest_QuandoFalhaNoServico()
        {
            var request = new CarteiraRequest { NomeCompleto = "Teste" };
            var mensagemErro = "Erro de validação";
            var resultFalha = Result<bool>.Failure(mensagemErro);

            _serviceMock.Setup(s => s.CriarCarteiraAsync(request))
                        .ReturnsAsync(resultFalha);

            var response = await _controller.PostCarteira(request);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(response);

            Assert.NotNull(badRequestResult.Value);
        }
    }
}