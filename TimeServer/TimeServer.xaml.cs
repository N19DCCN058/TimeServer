using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace TimeServer
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private DispatcherTimer timer;
        private List<Socket> _clientSockets = new List<Socket>();
        private Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        byte[] bufferData = new byte[1024];
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            lbServerIp.Content = GetPrivateIPAddress();
            timer = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 0, 0, 500),
                IsEnabled = true
            };
            timer.Tick += TimerOnTick;
            timer.Start();

            ServerStart();
        }
        private void TimerOnTick(object sender, EventArgs e)
        {
            lbCurrentTime.Content = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        private void ServerStart()
        {
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 90));
            _serverSocket.Listen(10);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Socket socket = _serverSocket.EndAccept(ar);
            _clientSockets.Add(socket);
            lvIpConnect.Dispatcher.Invoke(DispatcherPriority.Render, new Action(delegate ()
                {
                    lvIpConnect.Items.Add(socket.LocalEndPoint.ToString() + " " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                }));
            socket.BeginReceive(bufferData, 0, bufferData.Length, SocketFlags.None, new AsyncCallback(ReceiceCallBack), socket);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private void ReceiceCallBack(IAsyncResult ar)
        {
            byte[] dataSend;
            Socket socket = (Socket)ar.AsyncState;

            //int receiver = socket.EndReceive(ar);

            //byte[] dataBuffer = new byte[receiver];

            //Array.Copy(bufferData, dataBuffer, receiver);

            //string text = Encoding.ASCII.GetString(dataBuffer);


            dataSend = Encoding.ASCII.GetBytes(DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss"));
            //Thread.Sleep(1000);
            socket.BeginSend(dataSend, 0, dataSend.Length, SocketFlags.None, new AsyncCallback(SendCallBack), socket);

            dataSend = Encoding.ASCII.GetBytes(DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss"));
            socket.BeginSend(dataSend, 0, dataSend.Length, SocketFlags.None, new AsyncCallback(SendCallBack), socket);
        }

        private void SendCallBack(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            socket.EndSend(ar);
        }

        

        private string GetPrivateIPAddress()
        {
            string localIP = string.Empty;

            try
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    localIP = endPoint.Address.ToString();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return localIP;
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            lbServerIp.Content = GetPrivateIPAddress();
        }
    }
}
