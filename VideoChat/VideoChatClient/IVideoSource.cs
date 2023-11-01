using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoChatClient
{
    public interface IVideoSource : IDisposable
    {
        public event Action<Bitmap> OnNewFrame;
    }
}
