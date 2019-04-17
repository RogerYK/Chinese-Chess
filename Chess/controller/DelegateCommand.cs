using System;
using System.Windows.Input;

namespace Chess.controller
{
    internal class DelegateCommand<T> : ICommand
    {
        private Action<T> action;

        public DelegateCommand(Action<T> action)
        {
            this.action = action;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            action((T)parameter);
        }
    }
}