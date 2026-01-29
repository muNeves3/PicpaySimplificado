namespace PicpaySimplificado.Services.Notificacoes
{
    public class NotificacaoService: INotificacaoService
    {
        public async Task SendNotification()
        {
            await Task.Delay(1000);
            Console.WriteLine("Cliente Notificado");
        }
    }
}
