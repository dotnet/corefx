using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnYourWayHome.ApplicationModel.Presentation
{
    public interface IView
    {
        void Bind(object context);
    }
}
