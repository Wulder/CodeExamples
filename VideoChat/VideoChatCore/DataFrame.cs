using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoChatCore
{
   
    public class DataFrame
    {
        public event Action OnComplete;


        public byte[] Buffer => _buffer;

        private byte[] _buffer;
        private int _bufferPointer;
        private FrameState _state;


        public DataFrame(int bufferSize)
        {
            _buffer = new byte[bufferSize];
            _bufferPointer = 0;
            _state = FrameState.Uninitialized;
        }

        public void WriteData(Packet pack)
        {
            for(int i = 0; i < pack.Data.Length; i++)
            {
                _buffer[_bufferPointer + i] = pack.Data[i];
            }
            _bufferPointer += pack.Data.Length;
            _state = pack.State;

            if(_state == FrameState.Completed)
            {
                _buffer = _buffer[0.._bufferPointer];
                OnComplete?.Invoke();
            }
        }


    }

    public enum FrameState
    {
        Uninitialized,
        Write,
        Completed
    }
}
