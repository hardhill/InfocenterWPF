using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace InfocenterWPF
{
    class NotifyIconViewModel
    {
        public ICommand ShowWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                   // CanExecuteFunc = () => Application.Current.MainWindow == null,
                    CommandAction = () =>
                    {

                        
                        if (Application.Current.MainWindow != null)
                        {
                            Application.Current.MainWindow.Show();
                        }
                       
                    }
                };
            }
        }


        /// <summary>
        /// Hides the main window. This command is only enabled if a window is open.
        /// </summary>
        public ICommand ShowMessanger
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () => {
                        if (Application.Current.MainWindow != null)
                        {
                            MainWindow mform = Application.Current.MainWindow as MainWindow;
                            mform.formMessage.Show();
                        }
                        
                    }
                    //CanExecuteFunc = () => Application.Current.MainWindow != null
                };
            }
        }

        /// <summary>
        /// Shuts down the application.
        /// </summary>
        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand { CommandAction = () => Application.Current.Shutdown() };
            }
        }

        
    }

    public class DelegateCommand : ICommand
    {
        public Action CommandAction { get; set; }
        public Func<bool> CanExecuteFunc { get; set; }

        public void Execute(object parameter)
        {
            CommandAction();
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null || CanExecuteFunc();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
