using System;
using System.Reflection;
using System.Windows.Input;

namespace Utilities
{
    public class ReflectiveCommand : ICommand
    {
        public event EventHandler CanExecuteChanged = delegate { };
        private MethodInfo method;
        private ViewModel viewModel;
        private PropertyInfo canExecute;

        public ReflectiveCommand(ViewModel viewModel, MethodInfo method) :
            this(viewModel, method, null)
        {
        }

        public ReflectiveCommand(ViewModel viewModel, MethodInfo method, PropertyInfo canExecute)
        {
            Guard.ArgumentNotNull(viewModel, "viewModel");
            Guard.ArgumentNotNull(method, "method");
            this.viewModel = viewModel;
            this.method = method;
            this.canExecute = canExecute;
            if (canExecute != null) viewModel.PropertyChanged 
                += (sender, e) => CanExecuteChanged(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || (bool)canExecute.GetValue(viewModel, null);
        }

        public void Execute(object parameter)
        {
            int parameterCount = method.GetParameters().Length;
            if (parameterCount == 0) method.Invoke(viewModel, null);
            else if (parameterCount == 1) method.Invoke(viewModel, new[] { parameter });
        }
    }
}