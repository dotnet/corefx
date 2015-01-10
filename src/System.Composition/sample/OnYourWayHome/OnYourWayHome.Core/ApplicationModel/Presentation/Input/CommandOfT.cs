using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace OnYourWayHome.ApplicationModel.Presentation.Input
{
    internal abstract class Command<T> : ICommand
    {
        protected Command()
        {
        }

        public event EventHandler CanExecuteChanged;

        public virtual bool CanExecute(T parameter)
        {
            return true;
        }

        public abstract void Execute(T parameter);

        bool ICommand.CanExecute(object parameter)
        {
            if (parameter != null && !(parameter is T))
                throw new ArgumentException();

            return CanExecute((T)parameter);
        }

        void ICommand.Execute(object parameter)
        {
            if (parameter != null && !(parameter is T))
                throw new ArgumentException();

            Execute((T)parameter);
        }

        protected void FireCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
