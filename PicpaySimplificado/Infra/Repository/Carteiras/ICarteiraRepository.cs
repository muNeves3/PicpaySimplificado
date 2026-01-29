using PicpaySimplificado.Models;

namespace PicpaySimplificado.Infra.Repository.Carteiras
{
    public interface ICarteiraRepository
    {
        Task AddAsync(Carteira carteira);
        Task UpdateAsync(Carteira carteira);
        Task<Carteira?> GetByIdAsync(int id);
        Task<Carteira?> GetByCpfCnpj(string cpfcnpj, string email);
        Task CommitAsync();
    }
}
