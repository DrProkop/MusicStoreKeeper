using Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    public class LongOperationService : NotifyPropertyChangedBase, ILongOperationService
    {
        public LongOperationService()
        {
            _scheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }

        private TaskScheduler _scheduler;

        #region [  properties  ]

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        private bool _isBlockingBusy;

        public bool IsBlockingBusy
        {
            get => _isBlockingBusy;
            private set { _isBlockingBusy = value;OnPropertyChanged(); }
        }

        

        private string _blockingMessage;

        public string BlockingMessage
        {
            get => _blockingMessage;
            set
            {
                _blockingMessage = value;
                OnPropertyChanged();
            }
        }

        #endregion [  properties  ]

        #region [  public methods  ]

        public void StartLongBlockingOperation(string message)
        {
            IsBlockingBusy = true;
            BlockingMessage = message;
        }

        public void FinishLongBlockingOperation()
        {
            IsBlockingBusy = false;
        }


        public void HandleException(Exception ex)
        {
            Console.WriteLine($"иксэпшон {ex.Message}.");
        }
        public async Task<TResult> StartLongOperation<T1, T2, TResult>(Func<T1, T2, CancellationToken, Task<TResult>> operation, T1 arg1, T2 arg2, string message, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<TResult>();

            try
            {
                ShowWaitControl(message);
                await operation.Invoke(arg1, arg2, cancellationToken).ContinueWith(t =>
                {
                    if (TaskHasExceptions(tcs, t))
                    {
                       
                    }
                    else
                    {
                        tcs.SetResult(t.Result);
                    }
                    HideWaitControl();

                });
                return tcs.Task.Result;
            }
            catch (Exception e)
            {
                BlockingMessage = e.Message;
                tcs.SetException(e);
                HideWaitControl();
                return tcs.Task.Result;
            }
        }

        public async Task<T> StartLongOperation<T>(Func<Task<T>> operation)
        {
            try
            {
                IsBusy = true;
                var yolo = await operation.Invoke();
                IsBusy = false;
                return yolo;
            }
            catch (Exception e)
            {
                BlockingMessage = e.Message;
                var ololo = new TaskCompletionSource<T>();
                ololo.SetException(e);
                return ololo.Task.Result;
            }
        }

        public void ShowBlockingScreen()
        {
            throw new System.NotImplementedException();
        }

        public void HideBlockingScreen()
        {
            throw new System.NotImplementedException();
        }

        public void ShowMessage(string message)
        {
            throw new System.NotImplementedException();
        }

        #endregion [  public methods  ]

        #region [  private methods  ]

        private bool TaskHasExceptions<T>(TaskCompletionSource<T> tcs, Task<T> task)
        {
            if (!task.IsFaulted || task.Exception==null) return false;
            tcs.SetException(task.Exception.InnerExceptions);
            return true;
        }

        private void ShowWaitControl(string message)
        {
            BlockingMessage = message;
            IsBusy = true;
           // var ololo= RunInUiContext(() =>
           // {
           //     BlockingMessage = message;
           //     IsBusy = true;
           // });
           //var azaza = ololo.IsCompleted;
        }

        private void HideWaitControl()
        {
            RunInUiContext(() =>
            {
                BlockingMessage = "";
                IsBusy = false;
            });
        }

        private Task RunInUiContext(Action action)
        {
            return Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, _scheduler);
        }

        #endregion
    }
}