using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace OnYourWayHome.ApplicationModel.Presentation.Input
{
    internal abstract class Command : ICommand
    {
        protected Command()
        {
        }

        public event EventHandler CanExecuteChanged;

        public virtual bool CanExecute()
        {
            return true;
        }

        public abstract void Execute();

        protected void FireCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            if (parameter != null)
                throw new ArgumentException();  // Don't support parameters

            return CanExecute();
        }

        void ICommand.Execute(object parameter)
        {
            if (parameter != null)
                throw new ArgumentException();  // Don't support parameters

            Execute();
        }
    }
}
