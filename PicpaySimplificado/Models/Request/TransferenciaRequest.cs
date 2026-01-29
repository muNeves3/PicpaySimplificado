using System.ComponentModel.DataAnnotations;

namespace PicpaySimplificado.Models.Request
{
    public class TransferenciaRequest
    {
        [Required(ErrorMessage = "O campo valor é obrigatório.")]
        public decimal Valor { get; set; }
        [Required(ErrorMessage = "O campo EmetenteId é obrigatório.")]
        public int EmetenteId { get; set; }
        [Required(ErrorMessage = "O campo DestinatarioId é obrigatório.")]
        public int DestinatarioId { get; set; }
    }
}
