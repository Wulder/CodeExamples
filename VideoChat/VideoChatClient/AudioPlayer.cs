using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoChatClient
{
    internal class AudioPlayer
    {

        public BufferedWaveProvider WaveProvider => _prov;
        
        private WaveOut _wo = new WaveOut();
        private BufferedWaveProvider _prov;

        public AudioPlayer()
        {
            _prov = new BufferedWaveProvider(new WaveFormat(44100, 16, 1));
            _wo.Init(_prov);
            _wo.Play();
 
        }

        
    }
}
