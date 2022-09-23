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
        string ScanFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Scan";

        //####################################################################################

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(ScanFolder)) Directory.CreateDirectory(ScanFolder);
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            StartClient(new byte[] { 48 });
        }

        public void StartClient(byte[] cmd)
        {
            byte[] bytes = new byte[1000000];
            try
            {
                //IPAddress ipAddress = IPAddress.Parse("10.63.81.181");
                IPAddress ipAddress = IPAddress.Parse(txtIP.Text);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, int.Parse(txtPort.Text));
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    sender.Connect(remoteEP);
                    int bytesSent = sender.Send(cmd);
                    System.Threading.Thread.Sleep(1000);
                    int bytesRec = sender.Receive(bytes);
                    MemoryStream ms = new MemoryStream(bytes);
                    Bitmap bmp = (Bitmap)Image.FromStream(ms);
                    JpgToPdf(ref bmp);
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

        private void JpgToPdf(ref Bitmap b)
        {
            string dt = DateTime.Now.ToString("yyyy.MM.dd_HH.mm.ss");
            string pdf_fn = ScanFolder + "\\Scan(" + dt + ").pdf";
            string bmp_fn = ScanFolder + "\\Scan(" + dt + ").jpg";
            b.Save(bmp_fn);
            iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(b, iTextSharp.text.BaseColor.WHITE);
            var doc = new iTextSharp.text.Document(new iTextSharp.text.Rectangle(jpg.Width, jpg.Height), 0, 0, 0, 0);
            iTextSharp.text.pdf.PdfWriter.GetInstance(doc, new FileStream(pdf_fn, FileMode.Create));
            doc.Open();
            jpg.Alignment = iTextSharp.text.Element.ALIGN_BASELINE;
            doc.Add(jpg);
            doc.Close();
        }
    }
}
