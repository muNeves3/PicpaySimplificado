using PicpaySimplificado.Models.Request;
using PicpaySimplificado.Models.Response;

namespace PicpaySimplificado.Services.Carteiras
{
    public interface ICarteiraService
    {
        Task<Result<bool>> CriarCarteiraAsync(CarteiraRequest request);   
    }
}
