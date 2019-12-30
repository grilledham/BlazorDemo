using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorDemo.Utils
{
    public interface ICommand
    {
        event Action CanExecuteChanged;
        bool CanExecute();
        void Execute();
    }

    public interface ICommand<T>
    {
        event Action CanExecuteChanged;
        bool CanExecute(T value);
        void Execute(T value);
    }

    public class DelegateCommand : ICommand
    {
        private Action canExecuteChanged;
        public event Action CanExecuteChanged
        {
            add
            {
                if (canExecuteDelegate != null)
                {
                    canExecuteChanged += value;
                }
            }
            remove
            {
                if (canExecuteDelegate != null)
                {
                    canExecuteChanged -= value;
                }
            }
        }

        private Func<bool> canExecuteDelegate;
        private Action executeDelegate;

        public DelegateCommand(Action executeDelegate, Func<bool>? canExecuteDelegate = null)
        {
            this.executeDelegate = executeDelegate ?? throw new ArgumentNullException(nameof(executeDelegate));
            this.canExecuteDelegate = canExecuteDelegate;
        }

        public bool CanExecute()
        {
            return canExecuteDelegate?.Invoke() ?? true;
        }

        public void Execute()
        {
            if (CanExecute())
            {
                executeDelegate();
            }
        }

        public void RaiseCanExecuteChanged()
        {
            canExecuteChanged?.Invoke();
        }
    }
}
