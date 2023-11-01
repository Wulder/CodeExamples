using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoChatClient
{
    internal class ScreenCapture : IVideoSource
    {
        public event Action<Bitmap> OnNewFrame;

        private int _frameRate;
        public ScreenCapture(int frameRate, Size res)
        {
            _frameRate = frameRate;
            CaptureScreen(res);
        }

        async void CaptureScreen(Size res)
        {
            Console.WriteLine(res.Width);
            Console.WriteLine(res.Height);
            await Task.Run(() =>
            {
                using (Bitmap bitmap = new Bitmap(res.Width, res.Height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        while (true)
                        {
                            g.CopyFromScreen(new Point(0, 0), Point.Empty, Screen.PrimaryScreen.Bounds.Size);
                            Thread.Sleep(1000 / _frameRate);
                            bitmap.SetResolution(640, 480);
                            OnNewFrame?.Invoke(bitmap);
                        }

                    }
                     

                }
            });


        }
        public void Dispose()
        {
            
        }
    }
}
