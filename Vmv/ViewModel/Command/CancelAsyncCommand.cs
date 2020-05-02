using System;
using System.Threading;
using System.Windows.Input;

namespace MusicStoreKeeper.Vmv.ViewModel
{
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
            if (CanExecute(parameter))
            {
                try
                {
                    _isExecuting = true;
                    _cts.Cancel();
                    _cts.Dispose();
                    _cts = new CancellationTokenSource();
                }
                finally
                {
                    _isExecuting = false;
                    RaiseCanExecuteChanged();
                }
            }
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
}