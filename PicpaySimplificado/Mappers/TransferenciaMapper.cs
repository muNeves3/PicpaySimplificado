using PicpaySimplificado.Models;
using PicpaySimplificado.Models.DTOs;

namespace PicpaySimplificado.Mappers
{
    public static class TransferenciaMapper
    {
        public static TransferenciaDto ToTransferenciaDto(this Transferencia transaction)
        {
            return new TransferenciaDto(
                transaction.IdTransferencia,
                transaction.Sender,
                transaction.Reciver,
                transaction.Valor
            );
        }
    }
}
