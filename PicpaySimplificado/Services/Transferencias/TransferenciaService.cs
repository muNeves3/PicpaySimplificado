using PicpaySimplificado.Infra.Repository.Carteiras;
using PicpaySimplificado.Infra.Repository.Transferencias;
using PicpaySimplificado.Mappers;
using PicpaySimplificado.Models;
using PicpaySimplificado.Models.DTOs;
using PicpaySimplificado.Models.Enum;
using PicpaySimplificado.Models.Request;
using PicpaySimplificado.Models.Response;
using PicpaySimplificado.Services.Autorizador;
using PicpaySimplificado.Services.Notificacoes;

namespace PicpaySimplificado.Services.Transferencias
{
    public class TransferenciaService : ITransferenciaService
    {
        private readonly ITransferenciaRepository transferenciaRepository;
        private readonly ICarteiraRepository carteiraRepository;
        private readonly IAutorizadorService autorizadorService;
        private readonly INotificacaoService notificacaoService;

        public TransferenciaService(
            ITransferenciaRepository transferenciaRepository,
            ICarteiraRepository carteiraRepository,
            IAutorizadorService autorizadorService,
            INotificacaoService notificacaoService)
        {
            this.transferenciaRepository = transferenciaRepository;
            this.carteiraRepository = carteiraRepository;
            this.autorizadorService = autorizadorService;
            this.notificacaoService = notificacaoService;
        }

        public async Task<Result<TransferenciaDto>> ExecuteAsync(TransferenciaRequest request)
        {
            if(!await autorizadorService.AuthorizeAsync())
                return Result<TransferenciaDto>.Failure("Transferência não autorizada pelo serviço de autorização.");

            var emetente = await carteiraRepository.GetByIdAsync(request.EmetenteId);
            var destinatario = await carteiraRepository.GetByIdAsync(request.DestinatarioId);


            if(emetente is null || destinatario is null)
                return Result<TransferenciaDto>.Failure("Carteira do emitente ou destinatário não encontrada.");
            
            if(emetente.UserType == UserType.LOJISTA)
                return Result<TransferenciaDto>.Failure("Carteiras do tipo lojista não podem realizar transferências.");

            emetente.DebitarSaldo(request.Valor);
            destinatario.CreditarSaldo(request.Valor);

            var transferencia = new Transferencia(
                valor: request.Valor,
                senderId: emetente.Id,
                reciverId: destinatario.Id
            );

            using (var transferenciaScope = await transferenciaRepository.BeginTransactionAsync())
            {
                try 
                {
                    var updateTasks = new List<Task>
                    {
                        carteiraRepository.UpdateAsync(emetente),
                        carteiraRepository.UpdateAsync(destinatario),
                        transferenciaRepository.AddTranferencia(transferencia)
                    };

                    await Task.WhenAll(updateTasks);

                    await carteiraRepository.CommitAsync();
                    await transferenciaRepository.CommitAsync();
                    await transferenciaScope.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transferenciaScope.RollbackAsync();
                    return Result<TransferenciaDto>.Failure("Erro ao processar a transferência: " + ex.Message);
                }
            }

            await notificacaoService.SendNotification();
            return Result<TransferenciaDto>.Success(transferencia.ToTransferenciaDto());
        }
    }
}


