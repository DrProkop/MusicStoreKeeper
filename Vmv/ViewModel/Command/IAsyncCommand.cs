using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync();
        void RaiseCanExecuteChanged();
        bool CanExecute();
    }

    public interface IAsyncCommand<T> : ICommand
    {
        Task ExecuteAsync(T parameter);
        void RaiseCanExecuteChanged();
    }

   
   
}