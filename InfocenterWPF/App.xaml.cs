using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace InfocenterWPF
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon notifyIcon;
        public MainWindow mainWindow;
        public FormMessage formMessage;
        protected override void OnStartup(StartupEventArgs e)
        {
            
            base.OnStartup(e);
            mainWindow = new MainWindow();
            formMessage = new FormMessage(mainWindow);

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");

            mainWindow.ShowInTaskbar = false;
            mainWindow.Show();
            Current.MainWindow = mainWindow;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
            mainWindow.myClient.StopClient();
            base.OnExit(e);
        }
    }
}
