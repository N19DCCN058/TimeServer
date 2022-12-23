using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace TimeClient
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        public IPAddress IP = null;
        public MainWindow()
        {
            InitializeComponent();

        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 0, 0, 500),
                IsEnabled = true
            };
            timer.Tick += TimerOnTick;
            timer.Start();
            lbTimeZone.Content = TimeZoneInfo.Local.ToString();
            tbIpServer.Focus();
        }
        private void TimerOnTick(object sender, EventArgs e)
        {
            lbTxtTimeClient.Content = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }
        private void BtnSync_OnClick(object sender, RoutedEventArgs e)
        {
            if (!checkIP())
            {
                LvConsole.Items.Add(DateTime.Now.ToString("HH:mm:ss") + ": Ip Server Wrong");
                return;
            }
            LvConsole.Items.Add(DateTime.Now.ToString("HH:mm:ss") + ": Waiting for sync time from " + tbIpServer.Text);
            try
            {
                SyncTime(IP);
            }
            catch (Exception)
            {
                LvConsole.Items.Add(DateTime.Now.ToString("HH:mm:ss") + ": Something went wrong when sync time from server ");
            }

        }

        private Boolean checkIP()
        {
            string input = tbIpServer.Text;
            var ipParts = input.Split('.');

            if (ipParts.Length != 4)
            {
                return false;
            }

            foreach (var part in ipParts)
            {
                if (part.StartsWith("0") && part.Length > 1)
                {
                    return false;
                }

                if (part.Length != part.Trim().Length)
                {
                    return false;
                }

                int number;
                var result = Int32.TryParse(part, out number);

                if (!result || number > 255 || number < 0)
                {
                    return false;
                }
            }
            if (!IPAddress.TryParse(input, out IP))
            {
                return false;
            }
            return true;
        }

        private void SyncTime(IPAddress ip)
        {
            //Initialize client
            ASCIIEncoding encoding = new ASCIIEncoding();
            TcpClient client = new TcpClient();

            client.Connect(ip, int.Parse(txtPort.Text));
            if (client.Connected)
            {
                LvConsole.Items.Add(DateTime.Now.ToString("HH:mm:ss") + ": Connected to server ");
                DateTime[] value = new DateTime[4];

                //save time send resquest
                value[0] = DateTime.UtcNow;

                byte[] data = encoding.GetBytes(value[0].ToString("dd/MM/yyyy HH:mm:ss"));

                Stream stream = client.GetStream();

                //send request sync time
                stream.Write(data, 0, data.Length);

                byte[] receive1 = new byte[1024];
                byte[] receive2 = new byte[1024];

                //read message send from server (time server receive and time server send message)
                stream.Read(receive1, 0, 1024);
                stream.Read(receive2, 0, 1024);

                //save time receive message from server
                value[3] = DateTime.UtcNow;

                value[1] = ConverStringToDate(encoding.GetString(receive1));
                value[2] = ConverStringToDate(encoding.GetString(receive2));

                //calculator ofset time

                TimeSpan f1 = value[1].Subtract(value[0]);
                TimeSpan f2 = value[2].Subtract(value[3]);

                TimeSpan time = f1 + f2;

                double timeOfset = Math.Round(time.TotalMilliseconds / 2);

                //add ofset time
                DateTime trueTime = DateTime.Now.AddMilliseconds(timeOfset);


                //set time to system
                SetTimeForSystem(trueTime);
                LvConsole.Items.Add(DateTime.Now.ToString("HH:mm:ss") + ": Sync time success from server " + ip);
            }
            //cannot connect to server
            else
            {
                LvConsole.Items.Add(DateTime.Now.ToString("HH:mm:ss") + ": Cannot connect to server ");
            }

            client.Close();

        }

        [DllImport("kernel32.dll", EntryPoint = "SetSystemTime", SetLastError = true)]
        public static extern bool SetSystemTime(ref SYSTEMTIME st);

        private void SetTimeForSystem(DateTime time)
        {
            int timeset = (int)TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow).TotalHours;
            time = time.AddHours(-timeset);
            SYSTEMTIME st = new SYSTEMTIME();
            st.wYear = (short)time.Year; // must be short
            st.wMonth = (short)time.Month;
            st.wDay = (short)time.Day;
            st.wHour = (short)time.Hour;
            st.wMinute = (short)time.Minute;
            st.wSecond = (short)time.Second;
            bool check = SetSystemTime(ref st);
        }

        private DateTime ConverStringToDate(string input)
        {

            DateTime result = DateTime.MinValue;

            string[] sliptStrings = input.Split('/');
            int day = int.Parse(sliptStrings[0]);
            int month = int.Parse(sliptStrings[1]);
            int year = int.Parse(sliptStrings[2].Substring(0, 4));

            input = sliptStrings[2].Substring(5);

            sliptStrings = input.Split(':');

            int hour = int.Parse(sliptStrings[0]);
            int minutes = int.Parse(sliptStrings[1]);
            int second = int.Parse(sliptStrings[2].Split(' ')[0]);

            result = new DateTime(year, month, day, hour, minutes, second);

            return result;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
