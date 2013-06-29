using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Utilities
{
    public abstract class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void OnPropertyChanged()
        {
            string callerName = new StackFrame(1).GetMethod().Name;
            if (!callerName.StartsWith("set_")) throw new InvalidOperationException();
            OnPropertyChanged(callerName.Substring(4));
        }

        protected void OnPropertyChanged(string propertyName)
        {
            var e = new PropertyChangedEventArgs(propertyName);
            Exec.OnMain(() => PropertyChanged(this, e));
        }

        protected bool Set<T>(ref T field, T value)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            string callerName = new StackFrame(1).GetMethod().Name;
            if (!callerName.StartsWith("set_")) throw new InvalidOperationException();
            OnPropertyChanged(callerName.Substring(4));
            return true;
        }
    }
}