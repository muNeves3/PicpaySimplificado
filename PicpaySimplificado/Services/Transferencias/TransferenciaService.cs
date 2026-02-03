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
using PicpaySimplificado.Metrics;
using System.Diagnostics;

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
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if(!await autorizadorService.AuthorizeAsync())
                {
                    ApplicationMetrics.AutorizacoesNegadas.Inc();
                    ApplicationMetrics.TransferenciasRealizadas.WithLabels("falha_autorizacao").Inc();
                    return Result<TransferenciaDto>.Failure("Transferência não autorizada pelo serviço de autorização.");
                }

                var emetente = await carteiraRepository.GetByIdAsync(request.EmetenteId);
                var destinatario = await carteiraRepository.GetByIdAsync(request.DestinatarioId);

                if(emetente is null || destinatario is null)
                {
                    ApplicationMetrics.ErrosValidacao.WithLabels("carteira_nao_encontrada").Inc();
                    ApplicationMetrics.TransferenciasRealizadas.WithLabels("falha_validacao").Inc();
                    return Result<TransferenciaDto>.Failure("Carteira do emitente ou destinatário não encontrada.");
                }
                
                if(emetente.UserType == UserType.LOJISTA)
                {
                    ApplicationMetrics.ErrosValidacao.WithLabels("lojista_como_emetente").Inc();
                    ApplicationMetrics.TransferenciasRealizadas.WithLabels("falha_validacao").Inc();
                    return Result<TransferenciaDto>.Failure("Carteiras do tipo lojista não podem realizar transferências.");
                }

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

                        ApplicationMetrics.TransferenciasRealizadas.WithLabels("sucesso").Inc();
                        ApplicationMetrics.TransferenciasPorTipoUsuario.WithLabels(emetente.UserType.ToString()).Inc();
                        ApplicationMetrics.ValoresTransferencia.Observe((double)request.Valor);
                    }
                    catch (Exception ex)
                    {
                        await transferenciaScope.RollbackAsync();
                        ApplicationMetrics.TransferenciasRealizadas.WithLabels("falha_transacao").Inc();
                        return Result<TransferenciaDto>.Failure("Erro ao processar a transferência: " + ex.Message);
                    }
                }

                await notificacaoService.SendNotification();
                return Result<TransferenciaDto>.Success(transferencia.ToTransferenciaDto());
            }
            finally
            {
                stopwatch.Stop();
                ApplicationMetrics.DuracaoTransferencias.Observe(stopwatch.Elapsed.TotalSeconds);
            }
        }
    }
}


