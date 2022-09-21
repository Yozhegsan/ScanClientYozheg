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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScanClientYozheg
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            StartClient(new byte[] { 48 });
        }

        public void StartClient(byte[] cmd)
        {
            byte[] bytes= new byte[1000000];
            try
            {
                IPAddress ipAddress = IPAddress.Parse("10.63.81.181");
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 80);
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    sender.Connect(remoteEP);
                    int bytesSent = sender.Send(cmd);
                    System.Threading.Thread.Sleep(1000);
                    int bytesRec = sender.Receive(bytes);
                    MemoryStream ms = new MemoryStream(bytes);
                    Bitmap bmp = (Bitmap)Image.FromStream(ms);
                    pictureBox1.Image = bmp;
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }
                catch (ArgumentNullException ane) { MessageBox.Show("ArgumentNullException : " + ane.ToString()); }
                catch (SocketException se) { MessageBox.Show("SocketException : " + se.ToString()); }
                catch (Exception e) { MessageBox.Show("Unexpected exception : " + e.ToString()); }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            GC.Collect();
        }
    }
}
