using PicpaySimplificado.Models.DTOs;
using System.ComponentModel.DataAnnotations.Schema;

namespace PicpaySimplificado.Models
{
    public class Transferencia
    {
        public Guid IdTransferencia { get; set; }

        public int SenderId { get; set; }
        public Carteira Sender { get; set; }

        public int ReciverId { get; set; }
        public Carteira Reciver { get; set; }
        public decimal Valor { get; set; }

        public Transferencia(int senderId, int reciverId, decimal valor)
        {
            IdTransferencia = Guid.NewGuid();
            SenderId = senderId;
            ReciverId = reciverId;
            Valor = valor;
        }

        private Transferencia() { }
    }
}
