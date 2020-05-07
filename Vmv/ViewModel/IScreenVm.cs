using Common;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    public interface IScreenVm
    {
        IUserNotificationService UserNotificationService { get; }
        ILongOperationService LongOperationService { get; }
    }
}