using InfocenterWPF.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using WebSocket4Net;

namespace InfocenterWPF
{
    public class MyClient
    {
        public delegate void CloseSocket(object sender, EventArgs e);
        public delegate void DataRecieved(object sender, WebSocket4Net.DataReceivedEventArgs e);
        public delegate void ErrorClient(object sender, SuperSocket.ClientEngine.ErrorEventArgs e);
        public delegate void OpenedClient(object sender, EventArgs e);
        public delegate void MessageRecievedClient(object sender, MessageReceivedEventArgs e);
        public delegate void ChangeContactList(List<ContactUser> contacts);
        public delegate void ChangeDialogList(List<Dialoge> dialoges);
        public event CloseSocket OnCloseSocket;
        public event DataRecieved OnDataRecieved;
        public event ErrorClient OnErrorClient;
        public event OpenedClient OnOpenedClient;
        public event MessageRecievedClient OnMessageRecievedClient;
        public event ChangeContactList OnChangeContactList;
        public event ChangeDialogList OnChangeDialogList;

        private string url;
        private string protocol;
        private WebSocketVersion version;
        private string _id;

        private WebSocket webSocket;
        private string _username;
        private ContactUser _contactuser;
        //private Timer timer1;
        internal bool Active;
        List<ContactUser> contacts = new List<ContactUser>();
        List<Dialoge> dialoge = new List<Dialoge>();

        public string Winlogin { get { return _username; } set { _username = value; } }
        public ContactUser Contactuser { get { return _contactuser; } }

        public MyClient(string uri, string protocol)
        {
            this.url = uri;
            this.protocol = protocol;
            this.version = WebSocketVersion.Rfc6455;

            this.Active = false;
            //this.timer1 = new Timer(new TimerCallback(TickTimer1),null,1000,3000);
            webSocket = new WebSocket(this.url, this.protocol, this.version);
            webSocket.AutoSendPingInterval = 2000;

            webSocket.Closed += WebSocket_Closed;
            webSocket.DataReceived += WebSocket_DataReceived;
            webSocket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(WebSocket_Error);
            webSocket.Opened += WebSocket_Opened;
            webSocket.MessageReceived += WebSocket_MessageReceived;

        }

        

        private void WebSocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            string value = e.Message;
            value = ControllerCommandIn(value);
            if (value != "{}")
            {
                webSocket.Send(value);
            }
            OnMessageRecievedClient?.Invoke(sender, e);
        }
        //соединение установлено
        private void WebSocket_Opened(object sender, EventArgs e)
        {
            if (webSocket.State == WebSocketState.Open)
            {
                Active = true;

            }

            OnOpenedClient?.Invoke(sender, e);
        }

        // делаем команду отправкуи сообщения
        internal void SendMessage(string Address, string text)
        {
            Comm comm_req = new Comm();
            comm_req.CommName = "MSG";
            MessageSend msg = new MessageSend();
            msg.Adress = Address;
            msg.Sender = Winlogin;
            msg.Message = text;
            comm_req.Body = msg;
            string json = JsonConvert.SerializeObject(comm_req);
            webSocket.Send(json);
        }

        internal void GetClients()
        {
            Comm comm_req = new Comm();
            comm_req.CommName = "GETCL";
            comm_req.Body = new Client() { UserName = this.Winlogin };
            string json = JsonConvert.SerializeObject(comm_req);
            webSocket.Send(json);
        }

        internal void GetDialog()
        {
            Comm comm_req = new Comm();
            comm_req.CommName = "GETDLG";
            //TODO сформировать команду
            string json = JsonConvert.SerializeObject(comm_req);
            webSocket.Send(json);
        }

        private void WebSocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            Active = false;
            OnErrorClient?.Invoke(sender, e);
        }

        internal void SetId(string idSession)
        {
            _id = idSession;
        }

        internal string GetId()
        {
            return _id;
        }

        private void WebSocket_DataReceived(object sender, WebSocket4Net.DataReceivedEventArgs e)
        {
            OnDataRecieved?.Invoke(sender, e);
        }

        private void WebSocket_Closed(object sender, EventArgs e)
        {

            if (webSocket.State == WebSocketState.Closed)
            {
                Active = false;
            }
            OnCloseSocket?.Invoke(sender, e);
        }

        public void StartClient()
        {
            webSocket.Close();
            if ((string)_username != "")
            {
                this.RestartTry();
            }
        }

        public void StopClient()
        {
            webSocket.Close();
        }

        public void RestartTry()
        {
            if (webSocket.State != WebSocketState.Closed)
            {
                webSocket.Close();
            }
            while (true)
            {
                if (!this.Active)
                {
                    try
                    {
                        webSocket.Open();
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Try connecting fail...");
                        webSocket.Close();
                    }
                    break;
                }
                Thread.Sleep(500);
            }

        }

        //процессор приема сообщений
        private string ControllerCommandIn(string value)
        {
            string returnjson = "{}";
            Comm comm = JsonConvert.DeserializeObject<Comm>(value);
            string commandName = comm.CommName;


            switch (commandName)
            {
                case "SENDID":
                    Client cli = new Client();
                    cli = JsonConvert.DeserializeObject<Client>(comm.Body.ToString());
                    SetId(cli.IdSession);//установить идентификатор сессии для клиента
                    //отправить инфу для сервера
                    Comm comm_resp = new Comm() { CommName = "NEWUSER", Body = new Client() { UserName = this.Winlogin, IdSession = GetId() } };
                    string json = JsonConvert.SerializeObject(comm_resp);
                    returnjson = json;
                    break;
                case "NEWUSER":
                    contacts = JsonConvert.DeserializeObject<List<ContactUser>>(comm.Body.ToString());
                    //исключить из списка контактов себя самого
                    contacts = VerifyContacts(contacts, Winlogin);
                    //инициировать событие
                    OnChangeContactList?.Invoke(contacts);
                    break;
                case "MSG":
                    MessageSend msg = JsonConvert.DeserializeObject<MessageSend>(comm.Body.ToString());
                    if (msg.Adress == Winlogin)
                    {
                        dialoge.Add(new Dialoge() { Author = msg.Sender, ComeIn = true, MessageText = msg.Message, DtMsg = DateTime.Now });
                        //событие на изменение списка диалога
                    }
                    else if (msg.Sender == Winlogin)
                    {
                        dialoge.Add(new Dialoge() { Author = msg.Adress, ComeIn = false, MessageText = msg.Message, DtMsg = DateTime.Now });
                    }

                    OnChangeDialogList(dialoge);
                    break;
                case "GETCL":
                    contacts = JsonConvert.DeserializeObject<List<ContactUser>>(comm.Body.ToString());
                    //исключить из списка контактов себя самого
                    contacts = VerifyContacts(contacts, Winlogin);
                    OnChangeContactList?.Invoke(contacts);
                    break;
                default:
                    StopClient();
                    break;
            }
            return returnjson;
        }

        private List<ContactUser> VerifyContacts(List<ContactUser> contacts, string winlogin)
        {
            List<ContactUser> lstCont = new List<ContactUser>();
            ContactUser user = contacts.Find(x => x.Winlogin == winlogin);
            this._contactuser = user;
            lstCont = contacts.FindAll(x => x.Winlogin != winlogin);
            return lstCont;
        }





        internal void Free()
        {
            webSocket.Dispose();
        }
    }
}