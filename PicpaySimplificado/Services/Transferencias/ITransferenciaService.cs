using PicpaySimplificado.Models.DTOs;
using PicpaySimplificado.Models.Request;
using PicpaySimplificado.Models.Response;

namespace PicpaySimplificado.Services.Transferencias
{
    public interface ITransferenciaService
    {
        Task<Result<TransferenciaDto>> ExecuteAsync(TransferenciaRequest request);
    }
}
