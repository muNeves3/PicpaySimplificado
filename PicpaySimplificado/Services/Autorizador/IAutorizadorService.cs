namespace PicpaySimplificado.Services.Autorizador
{
    public interface IAutorizadorService
    {
        Task<bool> AuthorizeAsync();
    }
}
