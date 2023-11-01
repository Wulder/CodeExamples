using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using VideoChatCore;

namespace VideoChatClient
{
    internal class Client
    {
        private VideoCapture _videoCapture;
        private VideoReceiver _videoReceiver;
        private AudioPlayer _audioPlayer;
        private Connection _connection;
        private IntPtr _handle;
        #region View
        private PictureBox _localView, _remoteView;
        #endregion
        public Client(IntPtr handle, PictureBox localPreview, PictureBox remotePreview)
        {
            _handle = handle;
            _audioPlayer = new AudioPlayer();
            _videoReceiver = new VideoReceiver();

            _localView = localPreview;
            _remoteView = remotePreview;


           // SetVideoCapture(VideoCaptureType.Camera);


            //remote video receive
            _videoReceiver.OnVideoFrameComplete += OnCompleteVideoFrame;
            _videoReceiver.OnAudioFrameComplete += OnCompleteAudioFrame;
        }

        public void SetVideoCapture(VideoCaptureType captType)
        {
            if(_videoCapture != null )
            {
                _videoCapture.Dispose();
                _videoCapture.OnNewCameraFrame -= ReceiveFrame;
                _videoCapture.OnNewAudioSample -= ReceiveSample;
            }
            _videoCapture = new VideoCapture(_handle, captType);
            _videoCapture.OnNewCameraFrame += ReceiveFrame;
            _videoCapture.OnNewAudioSample += ReceiveSample;
        }
        public void Connect(IPEndPoint address)
        {
            if (_connection != null) return;
             _connection = new Connection(64000, address);
            _connection.OnReceivePacket += ReceivePacket;
        }

        public void Disconnect()
        {
            if (_connection == null) return;
            _connection.Dispose();
            _connection = null;
        }

        private void ReceiveFrame(Bitmap bmp)
        {
            _localView.Image = new Bitmap(bmp);
            if (_connection == null) return;
            _connection.SendFrame(Camera.GetJpeg(bmp, 50), PacketType.Video);
        }
        private void ReceiveSample(NAudio.Wave.WaveInEventArgs samp)
        {
            if (_connection == null) return;
            _connection.SendFrame(samp.Buffer, PacketType.Audio);
        }

        private void ReceivePacket(Packet pack)
        {
            _videoReceiver.AddPacket(pack);
        }

        private void OnCompleteVideoFrame(DataFrame frame)
        {
            Console.WriteLine("Video Frame complete!");

            try
            {
                MemoryStream s = new MemoryStream(frame.Buffer);
                var bmp = new Bitmap(Image.FromStream(s));

                _remoteView.Image = bmp;
            }
            catch
            {

            }


            frame = null;
        }

        private void OnCompleteAudioFrame(DataFrame frame)
        {
            Console.WriteLine(frame.Buffer.Length);
            _audioPlayer.WaveProvider.AddSamples(frame.Buffer, 0, frame.Buffer.Length);
        }
    }
}
