using Microsoft.EntityFrameworkCore;
using PicpaySimplificado.Models;
using PicPaySimplificado.Infra;

namespace PicpaySimplificado.Infra.Repository.Carteiras
{
    public class CarteiraRepository : ICarteiraRepository
    {
        private readonly ApplicationDbContext _context;    

        public CarteiraRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Carteira carteira)
        {
            await _context.Wallets.AddAsync(carteira);
        }

        public async Task<Carteira?> GetByCpfCnpj(string cpfcnpj, string email)
        {
           return await _context.Wallets
                .FirstOrDefaultAsync(c => c.CPFCNPJ.Equals(cpfcnpj) || c.Email.Equals(email));
        }

        public async Task<Carteira?> GetByIdAsync(int id)
        {
            return await _context.Wallets.FindAsync(id);
        }

        public async Task UpdateAsync(Carteira carteira)
        {
            _context.Update(carteira);
        }


        public async Task CommitAsync()
        {
           await _context.SaveChangesAsync();
        }
    }
}
