using System;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.NetworkInformation;
using System.Threading;
using System.Drawing;
using System.Runtime.InteropServices;

///////////////////////////////////////////////////////////////////////////////
//
//  www.interestprograms.ru /программы, игры и их исходные коды/
//  Протокол TCP. Часть 2. Отправки файлов и сообщений по сети. 
//
///////////////////////////////////////////////////////////////////////////////

namespace TcpSendFiles
{
    public partial class Form1 : Form
    {
        TcpModule _tcpmodule = new TcpModule();

        private uint fPreviousExecutionState;

        public Form1()
        {
            InitializeComponent();


            _tcpmodule.Receive += new TcpModule.ReceiveEventHandler(_tcpmodule_Receive);
            _tcpmodule.Disconnected += new TcpModule.DisconnectedEventHandler(_tcpmodule_Disconnected);
            _tcpmodule.Connected += new TcpModule.ConnectedEventHandler(_tcpmodule_Connected);
            _tcpmodule.Accept += new TcpModule.AcceptEventHandler(_tcpmodule_Accept);

            _tcpmodule.Parent = this;


            listBox1.HorizontalScrollbar = true;

            // Set new state to prevent system sleep
            fPreviousExecutionState = NativeMethods.SetThreadExecutionState(
                NativeMethods.ES_CONTINUOUS | NativeMethods.ES_SYSTEM_REQUIRED | NativeMethods.ES_DISPLAY_REQUIRED);
            if (fPreviousExecutionState == 0)
            {
                Console.WriteLine("SetThreadExecutionState failed. Do something here...");
                Close();
            }

        }


        void _tcpmodule_Accept(object sender)
        {
            ShowReceiveMessage("Клиент подключился!");
        }

        void _tcpmodule_Connected(object sender, string result)
        {
            ShowReceiveMessage(result);
        }

        void _tcpmodule_Disconnected(object sender, string result)
        {
            ShowReceiveMessage(result);
        }

        void _tcpmodule_Receive(object sender, ReceiveEventArgs e)
        {

            if (e.sendInfo.message != null)
            {
                ShowReceiveMessage("Письмо: " + e.sendInfo.message);
            }

            if (e.sendInfo.filesize > 0)
            {
                ShowReceiveMessage("Файл: " + e.sendInfo.filename);
            }
            
        }

        private void buttonStartServer_Click(object sender, EventArgs e)
        {
            _tcpmodule.StartServer();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            _tcpmodule.ConnectClient(textBoxIPserver.Text);
        }

        private void buttonSendData_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(_tcpmodule.SendData);
            t.Start();
        }

        private void buttonAddFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _tcpmodule.SendFileName = dlg.FileName;
            }
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _tcpmodule.CloseSocket();
        }

        // Код доступа к свойствам объектов главной формы  из других потоков

        delegate void UpdateReceiveDisplayDelegate(string message);
        public void ShowReceiveMessage(string message)
        {
            if (listBox1.InvokeRequired == true)
            {
                UpdateReceiveDisplayDelegate rdd = new UpdateReceiveDisplayDelegate(ShowReceiveMessage);

                // Данный метод вызывается в дочернем потоке,
                // ищет основной поток и выполняет делегат указанный в качестве параметра 
                // в главном потоке, безопасно обновляя интерфейс формы.
                Invoke(rdd, new object[] { message }); 
            }
            else
            {
                // Если не требуется вызывать метод Invoke, обратимся напрямую к элементу формы.
                listBox1.Items.Add( (listBox1.Items.Count + 1).ToString() +  ". " + message); 
            }
        }

        delegate void BackColorFormDelegate(Color color);
        public void ChangeBackColor(Color color)
        {
            if (this.InvokeRequired == true)
            {
                BackColorFormDelegate bcf = new BackColorFormDelegate(ChangeBackColor);

                // Данный метод вызывается в дочернем потоке,
                // ищет основной поток и выполняет делегат указанный в качестве параметра 
                // в главном потоке, безопасно обновляя интерфейс формы.
                Invoke(bcf, new object[] { color });
            }
            else
            {
                this.BackColor = color;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //while (_tcpmodule.IsConnectTrue == false)
            //{
            //    int i = 126;
            //    _tcpmodule.ConnectClient(Convert.ToString(i) + ".0.0.1");
            //    i++;
            //}
            //_tcpmodule.ConnectClient("127.0.0.1");

        }

        private void textBoxIPserver_TextChanged(object sender, EventArgs e)
        {

        }

        private void labelFileName_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            //while (true)
            //{
            //if (File.Exists("totigr.dat") == true)
            //{
            //    Thread t = new Thread(_tcpmodule.SendData);
            //    t.Start();
            //}
            //}
            //Thread t = new Thread(_tcpmodule.SendData);
            //t.Start();
            new Thread(_ =>
            {
                while (true)
                {
                    Thread.Sleep(50);
                    if (File.Exists("totigr.dat") == true)
                    {
                        Thread t = new Thread(_tcpmodule.SendData);
                        t.Start();
                    }
                }
            }).Start();

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(1);
            base.OnClosed(e);

            // Restore previous state
            if (NativeMethods.SetThreadExecutionState(fPreviousExecutionState) == 0)
            {
                // No way to recover; already exiting
            }
        }

        private void f(object sender, EventArgs e)
        {

        }

        private void textBoxSend_TextChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
    internal static class NativeMethods
    {
        // Import SetThreadExecutionState Win32 API and necessary flags
        [DllImport("kernel32.dll")]
        public static extern uint SetThreadExecutionState(uint esFlags);
        public const uint ES_CONTINUOUS = 0x80000000;
        public const uint ES_SYSTEM_REQUIRED = 0x00000001;
        public const uint ES_DISPLAY_REQUIRED = 0x00000002;
    }

}
