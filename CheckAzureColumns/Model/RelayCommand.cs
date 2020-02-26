using System;
using System.Diagnostics;
using System.Windows.Input;

namespace CheckAzureColumns
{
    public class RelayCmd : ICommand
    {
        #region Members
        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;
        #endregion Members
        #region ctor
        public RelayCmd(Action<object> Execute)
            : this(Execute, null)
        {

        }
        public RelayCmd(Action<object> Execute, Predicate<object> CanExecute)
        {
            if (Execute == null)
                throw new ArgumentNullException("Execute");
            _execute = Execute;
            _canExecute = CanExecute;
        }
        #endregion ctor
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }

}
