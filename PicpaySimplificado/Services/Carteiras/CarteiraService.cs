using PicpaySimplificado.Infra.Repository.Carteiras;
using PicpaySimplificado.Models;
using PicpaySimplificado.Models.Request;
using PicpaySimplificado.Models.Response;

namespace PicpaySimplificado.Services.Carteiras
{
    public class CarteiraService : ICarteiraService
    {

        private readonly ICarteiraRepository carteiraRepository;

        public CarteiraService(ICarteiraRepository carteiraRepository)
        {
            this.carteiraRepository = carteiraRepository;
        }

        public async Task<Result<bool>> CriarCarteiraAsync(CarteiraRequest request)
        {
            var carteiraExistente = await carteiraRepository.GetByCpfCnpj(request.CPFCNPJ, request.Email);  

            if(carteiraExistente is not null)
            {
                return Result<bool>.Failure("Já existe uma carteira cadastrada com esse CPF/CNPJ ou Email.");   
            }

            var carteira = new Carteira(
                nomeCompleto: request.NomeCompleto,
                cpfcnpj: request.CPFCNPJ,
                email: request.Email,
                senha: request.Senha,
                userType: request.UserType,
                saldoConta: request.Saldo
            );

            await carteiraRepository.AddAsync(carteira);
            await carteiraRepository.CommitAsync();

            return Result<bool>.Success(true);
        }
    }
}
