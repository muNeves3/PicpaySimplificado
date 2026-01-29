using Microsoft.EntityFrameworkCore.Storage;
using PicpaySimplificado.Models;
using PicPaySimplificado.Infra;

namespace PicpaySimplificado.Infra.Repository.Transferencias
{
    public class TransferenciaRepository : ITransferenciaRepository
    {
        private readonly ApplicationDbContext _context;
        public TransferenciaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddTranferencia(Transferencia transferencia)
        {
            await _context.Transfers.AddAsync(transferencia);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
