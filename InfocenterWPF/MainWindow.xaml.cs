using InfocenterWPF.Models;
using System.Windows.Threading;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using SuperSocket.ClientEngine;
using WebSocket4Net;
using Newtonsoft.Json;

namespace InfocenterWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public FormMessage formMessage;
        public MyClient myClient;
        private ContactUser CurrentContactUser;
        private DispatcherTimer timerMain;
        public MainWindow()
        {
            InitializeComponent();
            formMessage = new FormMessage(this);

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            //
            string server = ConfigurationManager.AppSettings["Server"];
            string http = ConfigurationManager.AppSettings["Http"];
            wb.Source = new Uri(http);
            myClient = new MyClient(server, "");
            myClient.OnOpenedClient += MyClient_OnOpenedClient;
            myClient.OnCloseSocket += MyClient_OnCloseSocket;
            myClient.OnErrorClient += MyClient_OnErrorClient;
            myClient.OnMessageRecievedClient += MyClient_OnMessageRecievedClient;
            myClient.OnChangeContactList += MyClient_OnChangeContactList;
            myClient.OnChangeDialogList += MyClient_OnChangeDialogList;
            CurrentContactUser = new ContactUser();
            timerMain = new DispatcherTimer();
            timerMain.Tick += TimerMain_Tick;
            timerMain.Interval = new TimeSpan(0,0,2);//через каждые 2 секунды
            try
            {
                var name = WindowsIdentity.GetCurrent().Name;
                myClient.Winlogin = name.Substring(name.IndexOf('\\') + 1);
                //запуск клиента по таймеру
                timerMain.Start();

            }
            catch (Exception)
            {

            }
        }

        private void MyClient_OnChangeDialogList(List<Dialoge> dialoges)
        {
            //изменение списка диалога(?)

        }

        private void MyClient_OnChangeContactList(List<ContactUser> contacts)
        {
            //изменения списка контактов
        }

        private void MyClient_OnMessageRecievedClient(object sender, MessageReceivedEventArgs e)
        {
            //сообщения приняты
            //получение новых сообщений
                Comm comm_resp = new Comm()
                {
                    CommName = JsonConvert.DeserializeObject<Comm>(e.Message).CommName,
                    Body = JsonConvert.DeserializeObject<Comm>(e.Message).Body
                };
                if (comm_resp.CommName == "MSG")
                {
                    MessageSend msg = JsonConvert.DeserializeObject<MessageSend>(comm_resp.Body.ToString());
                    if (msg.Sender != myClient.Winlogin)
                    {
                        //this.SetBalloonTip("Вам пришло сообщение\r\n" + "От: " + msg.Sender);
                    }
                }
        }

        private void MyClient_OnErrorClient(object sender, ErrorEventArgs e)
        {
            //ошибки клиента
        }

        private void MyClient_OnCloseSocket(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                formMessage.Title = "Сообщения (нет соединения)";
            }));
        }

        private void MyClient_OnOpenedClient(object sender, EventArgs e)
        {
           
            //HwndSource hwnd = (HwndSource)PresentationSource.FromVisual(formMessage);
            Dispatcher.BeginInvoke((Action)(() => {
                formMessage.Title = myClient.Winlogin;
            }));

        }

        private void TimerMain_Tick(object sender, EventArgs e)
        {
            if (!myClient.Active)
            {
                myClient.RestartTry();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
                e.Cancel = true;
                this.Hide();
        }
    }


    //------------------------------------------------------------------------------------------------
    

    
}
