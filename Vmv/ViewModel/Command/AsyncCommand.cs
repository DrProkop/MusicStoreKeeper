using Common;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicStoreKeeper.Vmv.ViewModel
{
    public class AsyncCommand : IAsyncCommand
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public AsyncCommand(Func<Task> execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        #region [  fields  ]

        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;
        private bool _isExecuting;

        #endregion [  fields  ]

        #region [  execute  ]

        public async Task ExecuteAsync()
        {
            if (CanExecute())
            {
                try
                {
                    _isExecuting = true;
                    await _execute();
                }
                finally
                {
                    _isExecuting = false;
                }
            }
        }

        /// <summary>Defines the method to be called when the command is invoked.</summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public async void Execute(object parameter)
        {
            await ExecuteAsync();
        }

        #endregion [  execute  ]

        #region [  canexecute  ]

        public bool CanExecute(object parameter) => CanExecute();

        public bool CanExecute()
        {
            return !_isExecuting && (_canExecute?.Invoke() ?? true);
        }

        /// <summary>Occurs when changes occur that affect whether or not the command should execute.</summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        #endregion [  canexecute  ]
    }

    public interface ICancelCommand : ICommand
    {
        CancellationToken GetCancellationToken();
    }

    public class CancelAsyncCommand : ICancelCommand
    {
        #region [  fields  ]

        private bool _isExecuting;
        private CancellationTokenSource _cts = new CancellationTokenSource();

        #endregion [  fields  ]

        #region [  public methods  ]

        public CancellationToken GetCancellationToken()
        {
            return _cts.Token;
        }

        #endregion [  public methods  ]

        /// <summary>Defines the method that determines whether the command can execute in its current state.</summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter)
        {
            return !_isExecuting;
        }

        /// <summary>Defines the method to be called when the command is invoked.</summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter)
        {
            _isExecuting = true;

            _cts.Cancel();
            _cts.Dispose();
            _cts = new CancellationTokenSource();

            _isExecuting = false;
            RaiseCanExecuteChanged();
        }

        /// <summary>Occurs when changes occur that affect whether or not the command should execute.</summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public class AsyncCommand<T> : IAsyncCommand<T>
    {
        public AsyncCommand(Func<T, CancellationToken, Task> execute, ILongOperationService longOperationService, ICancelCommand cancelCommand, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _longOperationService = longOperationService;
            _cancelCommand = cancelCommand;
            _canExecute = canExecute;
        }

        #region [  fields  ]

        private readonly Func<T, CancellationToken, Task> _execute;
        private readonly Func<T, bool> _canExecute;
        private readonly ILongOperationService _longOperationService;
        private readonly ICancelCommand _cancelCommand;
        private bool _isExecuting;

        #endregion [  fields  ]

        #region [  execute  ]

        public async Task ExecuteAsync(T parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    _cancelCommand.GetCancellationToken().ThrowIfCancellationRequested();
                    _isExecuting = true;
                    await _execute(parameter, _cancelCommand.GetCancellationToken());
                }
                catch (OperationCanceledException opEx)
                {
                    _longOperationService.HandleException(opEx);
                }
                catch (Exception ex)
                {
                    _longOperationService.HandleException(ex);
                }
                finally
                {
                    _isExecuting = false;
                    _longOperationService.FinishLongBlockingOperation();
                }
            }
        }

        /// <summary>Defines the method to be called when the command is invoked.</summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public async void Execute(object parameter)
        {
            await ExecuteAsync((T)parameter);
        }

        #endregion [  execute  ]

        #region [  canexecute  ]

        /// <summary>Defines the method that determines whether the command can execute in its current state.</summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(T parameter)
        {
            return !_isExecuting && (_canExecute?.Invoke(parameter) ?? true);
        }

        /// <summary>Defines the method that determines whether the command can execute in its current state.</summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter) => CanExecute((T)parameter);

        /// <summary>Occurs when changes occur that affect whether or not the command should execute.</summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        #endregion [  canexecute  ]
    }
}