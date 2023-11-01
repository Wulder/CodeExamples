using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Accord.Audio;
using Accord.DirectSound;
using NAudio;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace VideoChatClient
{
    internal class Microphone
    {
        public event Action<NAudio.Wave.WaveInEventArgs> OnNewSample;

        WaveInEvent waveIn = new NAudio.Wave.WaveInEvent
        {
            DeviceNumber = 0, // indicates which microphone to use
            WaveFormat = new NAudio.Wave.WaveFormat(rate: 44100, bits: 16, channels: 1),
            BufferMilliseconds = 20
        };
        public Microphone(IntPtr handle)
        {
            waveIn.DataAvailable += WaveIn_DataAvailable;
            waveIn.StartRecording();
        }



        private void WaveIn_DataAvailable(object? sender, NAudio.Wave.WaveInEventArgs e)
        {
            OnNewSample?.Invoke(e);

        }


    }
}
