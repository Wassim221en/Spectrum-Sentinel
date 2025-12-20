public interface IWhatsAppService
{
    Task<bool> SendMessageAsync(string phone, string message);
}