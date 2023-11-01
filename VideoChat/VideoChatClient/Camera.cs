using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using AForge.Video;
using AForge.Video.DirectShow;


namespace VideoChatClient
{
    public class Camera : IVideoSource
    {
        public event Action<Bitmap> OnNewFrame; 
        private VideoCaptureDevice _videoSource;
        public Camera()
        {
            try
            {
                FilterInfoCollection devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                _videoSource = new VideoCaptureDevice(devices[0].MonikerString);
                _videoSource.NewFrame += NewFrame;
                _videoSource.Start();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Camera connect error: {ex.Message}");
            }          
        }

        public void Dispose()
        {
            _videoSource.SignalToStop();  
        }
        private void NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            OnNewFrame?.Invoke(eventArgs.Frame);
        }

        public static byte[] GetJpeg(Bitmap bmp, long quality)
        {
            System.Drawing.Imaging.Encoder myEncoder =
               System.Drawing.Imaging.Encoder.Quality;

            ImageCodecInfo jpgEncoder =  GetEncoder(ImageFormat.Jpeg);

            EncoderParameters myEncoderParameters = new EncoderParameters(1);

            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, quality);
            myEncoderParameters.Param[0] = myEncoderParameter;
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, jpgEncoder, myEncoderParameters);
            return ms.GetBuffer();
        }

        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        
    }
}
