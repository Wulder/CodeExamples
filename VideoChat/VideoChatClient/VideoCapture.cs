using Accord.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VideoChatClient
{
    internal class VideoCapture : IDisposable
    {

        public event Action<Bitmap> OnNewCameraFrame;
        public event Action<NAudio.Wave.WaveInEventArgs> OnNewAudioSample;

        private IVideoSource _videoSource;
        private Microphone _microphone;
        private IntPtr _handle;
        public VideoCapture(IntPtr handle, VideoCaptureType videoCapture)
        {
            _handle = handle;

            if(videoCapture == VideoCaptureType.Camera)
                _videoSource = new Camera();

            if (videoCapture == VideoCaptureType.ScreenCapture)
                _videoSource = new ScreenCapture(30, new Size(Screen.AllScreens[0].Bounds.Size.Width, Screen.AllScreens[0].Bounds.Size.Height));


            _microphone = new Microphone(_handle);

            _videoSource.OnNewFrame += NewVideoFrame;
            _microphone.OnNewSample += NewAudioSample;
        }

        

        private void NewVideoFrame(Bitmap map)
        {
            OnNewCameraFrame?.Invoke(map);
        }

        private void NewAudioSample(NAudio.Wave.WaveInEventArgs e)
        {
            OnNewAudioSample?.Invoke(e);
        }

        public void Dispose()
        {
            
            _videoSource.Dispose();
        }
    }

    public enum VideoCaptureType
    {
        Camera,
        ScreenCapture
    }
}
