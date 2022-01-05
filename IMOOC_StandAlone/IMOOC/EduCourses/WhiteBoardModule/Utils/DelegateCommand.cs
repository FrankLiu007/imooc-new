using System;
using System.Windows.Ink;
using System.Windows.Input;

namespace IMOOC.EduCourses.WhiteBoardModule
{
    public class DelegateCommand : ICommand
    {
        public Action ExecuteAction { set; get; }
        public Action<StrokeCollection> ExecuteActions { get; set; }
        public Func<object, bool> CanExecuteFunc { get; set; }

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action execute)
        {
            ExecuteAction = execute;
        }

        public DelegateCommand(Action<StrokeCollection> execute)
        {
            ExecuteActions = execute;
        }

        public bool CanExecute(object parameter)
        {
            if (CanExecuteFunc != null)
            {
                return CanExecuteFunc(parameter);
            }
            else
            {
                return true;
            }
        }

        public void Execute(object parameter)
        {
            if (ExecuteAction!=null)
            {
                ExecuteAction.Invoke();
            }
            else if (ExecuteActions!=null)
            {
                ExecuteActions.Invoke((StrokeCollection)parameter);
            }
            //ExecuteAction?.Invoke();
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged!=null)
            {
                CanExecuteChanged.Invoke(this, EventArgs.Empty);
            }
            //CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
