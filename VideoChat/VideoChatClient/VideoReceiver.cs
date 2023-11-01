using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoChatCore;

namespace VideoChatClient
{
    internal class VideoReceiver
    {
        public event Action<DataFrame> OnVideoFrameComplete, OnAudioFrameComplete;

        private DataFrame _videoFrame, _audioFrame;

        public VideoReceiver()
        {
            _videoFrame = new DataFrame(64000 * 10);
            _audioFrame = new DataFrame(64000 * 10);

            _videoFrame.OnComplete += VideoFrameComplete;
            _audioFrame.OnComplete += AudioFrameComplete;
        }

        public void AddPacket(Packet pack)
        {
            switch(pack.Type)
            {
                case PacketType.Video:
                    {
                        _videoFrame.WriteData(pack);
                        break;
                    }
                case PacketType.Audio:
                    {
                        _audioFrame.WriteData(pack);
                        break;
                    }
            }
        }

        private void VideoFrameComplete()
        {
            OnVideoFrameComplete?.Invoke(_videoFrame);
            _videoFrame = new DataFrame(64000 * 10);
            _videoFrame.OnComplete += VideoFrameComplete;
        }

        private void AudioFrameComplete()
        {
            OnAudioFrameComplete?.Invoke(_audioFrame);
            _audioFrame = new DataFrame(64000 * 10);
            _audioFrame.OnComplete += AudioFrameComplete;
        }
    }
}
