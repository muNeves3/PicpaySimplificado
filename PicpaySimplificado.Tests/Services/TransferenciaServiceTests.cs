using Moq;
using Xunit;
using PicpaySimplificado.Services.Transferencias;
using PicpaySimplificado.Infra.Repository.Carteiras;
using PicpaySimplificado.Infra.Repository.Transferencias;
using PicpaySimplificado.Services.Autorizador;
using PicpaySimplificado.Services.Notificacoes;
using PicpaySimplificado.Models.Request;
using PicpaySimplificado.Models;
using PicpaySimplificado.Models.Enum;
using Microsoft.EntityFrameworkCore.Storage;

namespace PicpaySimplificado.Tests.Services
{
    public class TransferenciaServiceTests
    {
        private readonly Mock<ITransferenciaRepository> _transferenciaRepositoryMock;
        private readonly Mock<ICarteiraRepository> _carteiraRepositoryMock;
        private readonly Mock<IAutorizadorService> _autorizadorServiceMock;
        private readonly Mock<INotificacaoService> _notificacaoServiceMock;
        private readonly Mock<IDbContextTransaction> _transactionMock;
        private readonly TransferenciaService _service;

        public TransferenciaServiceTests()
        {
            _transferenciaRepositoryMock = new Mock<ITransferenciaRepository>();
            _carteiraRepositoryMock = new Mock<ICarteiraRepository>();
            _autorizadorServiceMock = new Mock<IAutorizadorService>();
            _notificacaoServiceMock = new Mock<INotificacaoService>();
            _transactionMock = new Mock<IDbContextTransaction>();

            _service = new TransferenciaService(
                _transferenciaRepositoryMock.Object,
                _carteiraRepositoryMock.Object,
                _autorizadorServiceMock.Object,
                _notificacaoServiceMock.Object
            );
        }

        [Fact]
        public async Task ExecuteAsync_DeveRetornarFalha_QuandoAutorizacaoNegada()
        {
            var request = new TransferenciaRequest { EmetenteId = 1, DestinatarioId = 2, Valor = 100m };
            _autorizadorServiceMock.Setup(a => a.AuthorizeAsync()).ReturnsAsync(false);

            var result = await _service.ExecuteAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Equal("Transferência não autorizada pelo serviço de autorização.", result.ErrorMessage);
            _carteiraRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_DeveRetornarFalha_QuandoEmetenteNaoEncontrado()
        {
            var request = new TransferenciaRequest { EmetenteId = 1, DestinatarioId = 2, Valor = 100m };
            _autorizadorServiceMock.Setup(a => a.AuthorizeAsync()).ReturnsAsync(true);
            _carteiraRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Carteira?)null);
            _carteiraRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Carteira("Dest", "123", "dest@email.com", "senha", UserType.USUARIO, 100m));

            var result = await _service.ExecuteAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Equal("Carteira do emitente ou destinatário não encontrada.", result.ErrorMessage);
        }

        [Fact]
        public async Task ExecuteAsync_DeveRetornarFalha_QuandoDestinatarioNaoEncontrado()
        {
            var request = new TransferenciaRequest { EmetenteId = 1, DestinatarioId = 2, Valor = 100m };
            _autorizadorServiceMock.Setup(a => a.AuthorizeAsync()).ReturnsAsync(true);
            _carteiraRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Carteira("Emetente", "123", "emetente@email.com", "senha", UserType.USUARIO, 500m));
            _carteiraRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync((Carteira?)null);

            var result = await _service.ExecuteAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Equal("Carteira do emitente ou destinatário não encontrada.", result.ErrorMessage);
        }

        [Fact]
        public async Task ExecuteAsync_DeveRetornarFalha_QuandoEmetenteEhLojista()
        {
            var request = new TransferenciaRequest { EmetenteId = 1, DestinatarioId = 2, Valor = 100m };
            _autorizadorServiceMock.Setup(a => a.AuthorizeAsync()).ReturnsAsync(true);

            var emetente = new Carteira("Lojista", "12345678901234", "lojista@email.com", "senha", UserType.LOJISTA, 500m);
            var destinatario = new Carteira("Usuario", "98765432100", "usuario@email.com", "senha", UserType.USUARIO, 100m);

            _carteiraRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(emetente);
            _carteiraRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(destinatario);

            var result = await _service.ExecuteAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Equal("Carteiras do tipo lojista não podem realizar transferências.", result.ErrorMessage);
        }

        [Fact]
        public async Task ExecuteAsync_DeveRetornarSucesso_QuandoTransferenciaValida()
        {
            var request = new TransferenciaRequest { EmetenteId = 1, DestinatarioId = 2, Valor = 100m };
            _autorizadorServiceMock.Setup(a => a.AuthorizeAsync()).ReturnsAsync(true);

            var emetente = new Carteira("Emetente", "12345678901", "emetente@email.com", "senha", UserType.USUARIO, 500m);
            emetente.GetType().GetProperty("Id")!.SetValue(emetente, 1);
            var destinatario = new Carteira("Destinatario", "98765432100", "destinatario@email.com", "senha", UserType.USUARIO, 100m);
            destinatario.GetType().GetProperty("Id")!.SetValue(destinatario, 2);

            _carteiraRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(emetente);
            _carteiraRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(destinatario);
            _transferenciaRepositoryMock.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(_transactionMock.Object);

            var result = await _service.ExecuteAsync(request);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.value);
            Assert.Equal(100m, result.value.ValorTransferido);
            Assert.Equal(400m, emetente.SaldoConta);
            Assert.Equal(200m, destinatario.SaldoConta);

            _carteiraRepositoryMock.Verify(r => r.UpdateAsync(emetente), Times.Once);
            _carteiraRepositoryMock.Verify(r => r.UpdateAsync(destinatario), Times.Once);
            _transferenciaRepositoryMock.Verify(r => r.AddTranferencia(It.IsAny<Transferencia>()), Times.Once);
            _notificacaoServiceMock.Verify(n => n.SendNotification(), Times.Once);
            _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_DeveRollback_QuandoOcorreExcecaoNaTransacao()
        {
            var request = new TransferenciaRequest { EmetenteId = 1, DestinatarioId = 2, Valor = 100m };
            _autorizadorServiceMock.Setup(a => a.AuthorizeAsync()).ReturnsAsync(true);

            var emetente = new Carteira("Emetente", "12345678901", "emetente@email.com", "senha", UserType.USUARIO, 500m);
            emetente.GetType().GetProperty("Id")!.SetValue(emetente, 1);
            var destinatario = new Carteira("Destinatario", "98765432100", "destinatario@email.com", "senha", UserType.USUARIO, 100m);
            destinatario.GetType().GetProperty("Id")!.SetValue(destinatario, 2);

            _carteiraRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(emetente);
            _carteiraRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(destinatario);
            _transferenciaRepositoryMock.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(_transactionMock.Object);
            _carteiraRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Carteira>())).ThrowsAsync(new Exception("Erro no banco de dados"));

            var result = await _service.ExecuteAsync(request);

            Assert.False(result.IsSuccess);
            Assert.Contains("Erro ao processar a transferência", result.ErrorMessage);
            _transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
            _notificacaoServiceMock.Verify(n => n.SendNotification(), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_DeveDebitarDoEmetente_ECreditarAoDestinatario()
        {
            var request = new TransferenciaRequest { EmetenteId = 1, DestinatarioId = 2, Valor = 150m };
            _autorizadorServiceMock.Setup(a => a.AuthorizeAsync()).ReturnsAsync(true);

            var emetente = new Carteira("Emetente", "12345678901", "emetente@email.com", "senha", UserType.USUARIO, 500m);
            emetente.GetType().GetProperty("Id")!.SetValue(emetente, 1);
            var destinatario = new Carteira("Destinatario", "98765432100", "destinatario@email.com", "senha", UserType.USUARIO, 100m);
            destinatario.GetType().GetProperty("Id")!.SetValue(destinatario, 2);

            _carteiraRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(emetente);
            _carteiraRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(destinatario);
            _transferenciaRepositoryMock.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(_transactionMock.Object);

            var result = await _service.ExecuteAsync(request);

            Assert.True(result.IsSuccess);
            Assert.Equal(350m, emetente.SaldoConta);
            Assert.Equal(250m, destinatario.SaldoConta);
        }

        [Fact]
        public async Task ExecuteAsync_DeveEnviarNotificacao_AposTransferenciaComSucesso()
        {
            var request = new TransferenciaRequest { EmetenteId = 1, DestinatarioId = 2, Valor = 50m };
            _autorizadorServiceMock.Setup(a => a.AuthorizeAsync()).ReturnsAsync(true);

            var emetente = new Carteira("Emetente", "12345678901", "emetente@email.com", "senha", UserType.USUARIO, 500m);
            emetente.GetType().GetProperty("Id")!.SetValue(emetente, 1);
            var destinatario = new Carteira("Destinatario", "98765432100", "destinatario@email.com", "senha", UserType.USUARIO, 100m);
            destinatario.GetType().GetProperty("Id")!.SetValue(destinatario, 2);

            _carteiraRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(emetente);
            _carteiraRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(destinatario);
            _transferenciaRepositoryMock.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(_transactionMock.Object);

            await _service.ExecuteAsync(request);

            _notificacaoServiceMock.Verify(n => n.SendNotification(), Times.Once);
        }
    }
}
