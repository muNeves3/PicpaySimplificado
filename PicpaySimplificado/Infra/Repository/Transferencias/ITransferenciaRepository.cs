using Microsoft.EntityFrameworkCore.Storage;
using PicpaySimplificado.Models;

namespace PicpaySimplificado.Infra.Repository.Transferencias
{
    public interface ITransferenciaRepository
    {
        Task AddTranferencia(Transferencia transferencia);

        Task CommitAsync();

        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
