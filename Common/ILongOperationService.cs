using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// Service for blocking screen when performing long operations
    /// </summary>
    public interface ILongOperationService
    {
        void ShowBlockingScreen();
        void HideBlockingScreen();
        void ShowMessage(string message);
        Task<T> StartLongOperation<T>(Func<Task<T>> operation);
        void StartLongBlockingOperation(string message);
        void FinishLongBlockingOperation();
        Task<TResult> StartLongOperation<T1, T2, TResult>(Func<T1, T2, CancellationToken, Task<TResult>> operation,
            T1 arg1, T2 arg2, string message, CancellationToken cancellationToken);
        string BlockingMessage { get; set; }
        bool IsBusy { get; set; }
        bool IsBlockingBusy { get; }
        void HandleException(Exception ex);
    }
}
