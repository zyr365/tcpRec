using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace recTCP
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //RecviveImage();
            textBox1.Text = "8008";
        }

        private void RecviveImage(int port)
        {
            new Thread(delegate () {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, port);
            socket.Bind(ipep);
            socket.Listen(2);
            while (true)
                {
                    try
                    {
                        byte[] data = new byte[8];
                        Socket clientSocket = socket.Accept();
                        if (clientSocket.Connected)
                        {
                            clientSocket.Receive(data, data.Length, SocketFlags.None);
                            long contentLength = BitConverter.ToInt64(data, 0);
                            int size = 0;
                            MemoryStream ms = new MemoryStream();
                            while (size < contentLength)
                            {
                                byte[] bits = new byte[128];
                                int r = clientSocket.Receive(bits, bits.Length, SocketFlags.None);
                                if (r <= 0) break;
                                ms.Write(bits, 0, r);
                                size += r;
                            }
                            Image img = Image.FromStream(ms);
                            this.Invoke((EventHandler)delegate 
                            {
                                pictureBox1.Image = img; //更新在窗体控件上                           
                            });
                            clientSocket.Close();
                            ms.Flush();
                            ms.Close();
                            ms.Dispose();
                        }
                    }
                    catch { }
                }
            })
            { IsBackground = true}.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RecviveImage(int.Parse(textBox1.Text));

        }
    }
}
