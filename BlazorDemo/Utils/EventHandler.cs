using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorDemo.Utils
{
    public delegate void EventHandler<TObject, TValue>(TObject obj, TValue value);
}
