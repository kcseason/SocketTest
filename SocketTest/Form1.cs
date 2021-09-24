using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocketTestServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IPAddress ip = IPAddress.Parse(textBox1.Text);
            IPEndPoint point = new IPEndPoint(ip, Int32.Parse(textBox2.Text));

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socket.Bind(point);
                socket.Listen(10);
                ShowMsg("服务器开始监听");

                Thread thread = new Thread(AcceptInfo);
                thread.IsBackground = true;
                thread.Start(socket);
            }
            catch (Exception ex) { ShowMsg(ex.Message); }
        }

        Dictionary<string, Socket> dic = new Dictionary<string, Socket>();

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                ShowMsg(richTextBox2.Text);
                string ip = comboBox1.Text;
                byte[] buffer = Encoding.UTF8.GetBytes(richTextBox2.Text);
                dic[ip].Send(buffer);
            }
            catch (Exception ex)
            {
                ShowMsg(ex.Message);
            }
        }

        void AcceptInfo(object obj)
        {
            Socket socket = obj as Socket;

            while (true)
            {
                try
                {
                    Socket tSocket = socket.Accept();
                    string point = tSocket.RemoteEndPoint.ToString();
                    ShowMsg(point + "连接成功！");
                    comboBox1.Items.Add(point);
                    dic.Add(point, tSocket);

                    //接收消息
                    Thread th = new Thread(ReceiveMsg);
                    th.IsBackground = true;
                    th.Start(tSocket);

                }
                catch (Exception ex) { }
            }
        }

        void ReceiveMsg(object o)
        {
            Socket client = o as Socket;

            while (true)
            {
                //接收客户端发送过来的数据
                try
                {
                    //定义byte数组存放从客户端接收过来的数据
                    byte[] buffer = new byte[1024 * 1024];
                    //将接收过来的数据放到buffer中，并返回实际接受数据的长度
                    int n = client.Receive(buffer);
                    //将字节转换成字符串
                    string words = Encoding.UTF8.GetString(buffer, 0, n);

                    ShowMsg(client.RemoteEndPoint.ToString() + ":" + words);
                }
                catch (Exception ex)
                {

                    ShowMsg(ex.Message);
                    break;
                }
            }
        }

        void ShowMsg(string msg)
        {

            richTextBox1.AppendText(msg + "\r\n");

        }
    }
}
