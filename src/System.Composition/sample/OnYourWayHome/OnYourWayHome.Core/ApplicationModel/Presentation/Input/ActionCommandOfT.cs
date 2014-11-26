using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnYourWayHome.ApplicationModel.Presentation.Input
{
    internal class ActionCommand<T> : Command<T>
    {
        private readonly Action<T> _action;

        public ActionCommand(Action<T> action)
        {
            Requires.NotNull(action, "action");

            _action = action;
        }

        public override void Execute(T parameter)
        {
            _action(parameter);
        }
    }
}
