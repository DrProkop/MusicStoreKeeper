namespace Common
{
    public interface IUserNotificationService
    {
        string StatusBarMessage { get; set; }
        void ShowUserMessage(string message);
    }
    
}