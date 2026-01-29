using PicpaySimplificado.Models.Enum;

namespace PicpaySimplificado.Models
{
    public class Carteira
    {
        public int Id { get; set; }
        public string NomeCompleto { get; set; }
        public string CPFCNPJ { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public decimal SaldoConta { get; set; }
        public UserType UserType { get; set; }

        private Carteira() { }

        public Carteira(string nomeCompleto, string cpfcnpj, string email, string senha,
            UserType userType, decimal saldoConta = 0)
        {
            NomeCompleto = nomeCompleto;
            CPFCNPJ = cpfcnpj;
            Email = email;
            Senha = senha;
            UserType = userType;
            SaldoConta = saldoConta;
        }

        public void DebitarSaldo(decimal valor)
        {
            if (SaldoConta < valor)
            {
                throw new InvalidOperationException("Não é possível debitar esse valor");
            }

            SaldoConta -= valor;
        }

        public void CreditarSaldo(decimal valor)
        {
            SaldoConta += valor;
        }
    }
}
