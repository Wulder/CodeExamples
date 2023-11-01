using Accord.DirectSound;
using System.Drawing.Imaging;
using System.Net;
using VideoChatCore;

namespace VideoChatClient
{
    public partial class Form1 : Form
    {
        private Client _client;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Start");

            _client = new Client(this.Handle, LocalPicture, RemotePicture);

        }

        #region Buttons
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                IPEndPoint address = new IPEndPoint(IPAddress.Parse(Ip_textBox.Text), int.Parse(port_textBox.Text));
                _client.Connect(address);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        }
        private void button3_Click(object sender, EventArgs e)
        {
            _client.Disconnect();
        }

        #endregion

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _client.Disconnect();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Ip_textBox_TextChanged(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void port_textBox_TextChanged(object sender, EventArgs e)
        {
        }

        bool IsWraped = false;
        private void ResizeBtn_Click(object sender, EventArgs e)
        {
            IsWraped = !IsWraped;
            if (IsWraped)
            {
                RemotePicture.BringToFront();
                RemotePicture.Dock = DockStyle.Fill;
                ResizeBtn.BringToFront();
                RotateBtn.BringToFront();

            }
            else
            {

                RemotePicture.Dock = DockStyle.None;

            }


        }



        private void RotateBtn_Click(object sender, EventArgs e)
        {
            RemotePicture.Image.RotateFlip(RotateFlipType.Rotate180FlipX);
        }





        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _client.SetVideoCapture((VideoCaptureType)comboBox1.SelectedIndex);
            Console.WriteLine($"Selected new videoCapture: {comboBox1.SelectedText} ({comboBox1.SelectedIndex}) - {(VideoCaptureType)comboBox1.SelectedIndex}");
        }
    }
}