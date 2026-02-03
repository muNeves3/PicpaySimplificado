using Microsoft.AspNetCore.Mvc;
using Moq;
using PicpaySimplificado.Controllers;
using PicpaySimplificado.Models.Request;
using PicpaySimplificado.Models.Response;
using PicpaySimplificado.Models.DTOs;
using PicpaySimplificado.Services.Transferencias;
using Xunit;
using PicpaySimplificado.Models;
using PicpaySimplificado.Models.Enum;

namespace PicpaySimplificado.Tests.Controllers
{
    public class TransferenciaControllerTests
    {
        private readonly Mock<ITransferenciaService> _serviceMock;
        private readonly TransferenciaController _controller;

        public TransferenciaControllerTests()
        {
            _serviceMock = new Mock<ITransferenciaService>();
            var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TransferenciaController>>();
            _controller = new TransferenciaController(_serviceMock.Object, loggerMock.Object);
        }

        [Fact]
        public async Task PostTransfer_DeveRetornarOk_QuandoTransferenciaComSucesso()
        {
            var request = new TransferenciaRequest { EmetenteId = 1, DestinatarioId = 2, Valor = 100m };

            var emetente = new Carteira("Emetente", "12345678901", "emetente@email.com", "senha", UserType.USUARIO, 500m);
            var destinatario = new Carteira("Destinatario", "98765432100", "destinatario@email.com", "senha", UserType.USUARIO, 100m);
            var transferenciaDto = new TransferenciaDto(Guid.NewGuid(), emetente, destinatario, 100m);
            var resultSucesso = Result<TransferenciaDto>.Success(transferenciaDto);

            _serviceMock.Setup(s => s.ExecuteAsync(request)).ReturnsAsync(resultSucesso);

            var response = await _controller.PostTransfer(request);

            var okResult = Assert.IsType<OkObjectResult>(response);
            var returnValue = Assert.IsType<Result<TransferenciaDto>>(okResult.Value);
            Assert.True(returnValue.IsSuccess);
            Assert.Equal(100m, returnValue.value.ValorTransferido);
        }

        [Fact]
        public async Task PostTransfer_DeveRetornarBadRequest_QuandoAutorizacaoNegada()
        {
            var request = new TransferenciaRequest { EmetenteId = 1, DestinatarioId = 2, Valor = 100m };
            var mensagemErro = "Transferência não autorizada pelo serviço de autorização.";
            var resultFalha = Result<TransferenciaDto>.Failure(mensagemErro);

            _serviceMock.Setup(s => s.ExecuteAsync(request)).ReturnsAsync(resultFalha);

            var response = await _controller.PostTransfer(request);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(response);
            var returnValue = Assert.IsType<Result<TransferenciaDto>>(badRequestResult.Value);
            Assert.False(returnValue.IsSuccess);
            Assert.Equal(mensagemErro, returnValue.ErrorMessage);
        }

        [Fact]
        public async Task PostTransfer_DeveRetornarBadRequest_QuandoCarteiraNaoEncontrada()
        {
            var request = new TransferenciaRequest { EmetenteId = 1, DestinatarioId = 2, Valor = 100m };
            var mensagemErro = "Carteira do emitente ou destinatário não encontrada.";
            var resultFalha = Result<TransferenciaDto>.Failure(mensagemErro);

            _serviceMock.Setup(s => s.ExecuteAsync(request)).ReturnsAsync(resultFalha);

            var response = await _controller.PostTransfer(request);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(response);
            var returnValue = Assert.IsType<Result<TransferenciaDto>>(badRequestResult.Value);
            Assert.False(returnValue.IsSuccess);
            Assert.Equal(mensagemErro, returnValue.ErrorMessage);
        }

        [Fact]
        public async Task PostTransfer_DeveRetornarBadRequest_QuandoEmetenteLojista()
        {
            var request = new TransferenciaRequest { EmetenteId = 1, DestinatarioId = 2, Valor = 100m };
            var mensagemErro = "Carteiras do tipo lojista não podem realizar transferências.";
            var resultFalha = Result<TransferenciaDto>.Failure(mensagemErro);

            _serviceMock.Setup(s => s.ExecuteAsync(request)).ReturnsAsync(resultFalha);

            var response = await _controller.PostTransfer(request);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(response);
            var returnValue = Assert.IsType<Result<TransferenciaDto>>(badRequestResult.Value);
            Assert.False(returnValue.IsSuccess);
            Assert.Equal(mensagemErro, returnValue.ErrorMessage);
        }

        [Fact]
        public async Task PostTransfer_DeveRetornarBadRequest_QuandoErroNoProcessamento()
        {
            var request = new TransferenciaRequest { EmetenteId = 1, DestinatarioId = 2, Valor = 100m };
            var mensagemErro = "Erro ao processar a transferência: Erro no banco de dados";
            var resultFalha = Result<TransferenciaDto>.Failure(mensagemErro);

            _serviceMock.Setup(s => s.ExecuteAsync(request)).ReturnsAsync(resultFalha);

            var response = await _controller.PostTransfer(request);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(response);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task PostTransfer_DeveChamarServiceUmaVez()
        {
            var request = new TransferenciaRequest { EmetenteId = 1, DestinatarioId = 2, Valor = 100m };
            var emetente = new Carteira("Emetente", "12345678901", "emetente@email.com", "senha", UserType.USUARIO, 500m);
            var destinatario = new Carteira("Destinatario", "98765432100", "destinatario@email.com", "senha", UserType.USUARIO, 100m);
            var transferenciaDto = new TransferenciaDto(Guid.NewGuid(), emetente, destinatario, 100m);
            var resultSucesso = Result<TransferenciaDto>.Success(transferenciaDto);

            _serviceMock.Setup(s => s.ExecuteAsync(request)).ReturnsAsync(resultSucesso);

            await _controller.PostTransfer(request);

            _serviceMock.Verify(s => s.ExecuteAsync(request), Times.Once);
        }

        [Fact]
        public async Task PostTransfer_DeveRetornarDadosDaTransferencia_QuandoSucesso()
        {
            var request = new TransferenciaRequest { EmetenteId = 1, DestinatarioId = 2, Valor = 250.75m };

            var emetente = new Carteira("João Silva", "12345678901", "joao@email.com", "senha", UserType.USUARIO, 1000m);
            var destinatario = new Carteira("Maria Santos", "98765432100", "maria@email.com", "senha", UserType.USUARIO, 500m);
            var idTransacao = Guid.NewGuid();
            var transferenciaDto = new TransferenciaDto(idTransacao, emetente, destinatario, 250.75m);
            var resultSucesso = Result<TransferenciaDto>.Success(transferenciaDto);

            _serviceMock.Setup(s => s.ExecuteAsync(request)).ReturnsAsync(resultSucesso);

            var response = await _controller.PostTransfer(request);

            var okResult = Assert.IsType<OkObjectResult>(response);
            var returnValue = Assert.IsType<Result<TransferenciaDto>>(okResult.Value);
            Assert.Equal(idTransacao, returnValue.value.IdTransaction);
            Assert.Equal("João Silva", returnValue.value.Emetente.NomeCompleto);
            Assert.Equal("Maria Santos", returnValue.value.Destinatario.NomeCompleto);
            Assert.Equal(250.75m, returnValue.value.ValorTransferido);
        }
    }
}
