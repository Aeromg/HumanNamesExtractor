using System;
using System.Windows.Input;

namespace IndexerWpf.ViewModels
{
    public class LambdaCommand : ICommand
    {
        public Func<bool> CanExecuteFunc { get; set; }
        public Action ExecuteAction { get; set; }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (ExecuteAction != null)
                ExecuteAction();
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null || CanExecuteFunc();
        }

        public void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;

            if(handler == null)
                return;

            handler(this, EventArgs.Empty);
        }
    }
}
