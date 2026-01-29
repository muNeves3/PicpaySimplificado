namespace PicpaySimplificado.Models.DTOs
{
    public record TransferenciaDto(Guid IdTransaction, Carteira Emetente, Carteira Destinatario, decimal ValorTransferido);
}
