using System.Threading;
using System.Windows.Input;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    public interface ICancelCommand : ICommand
    {
        CancellationToken GetCancellationToken();
        void RaiseCanExecuteChanged();
    }
}