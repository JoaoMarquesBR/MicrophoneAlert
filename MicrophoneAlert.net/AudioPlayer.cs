using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace MicrophoneAlert.net
{

    public class AudioPlayer
    {
        private IWavePlayer waveOutDevice;
        private AudioFileReader audioFile;
        private bool isDisposed { get; set; }

        public AudioPlayer()
        {
            waveOutDevice = new WaveOutEvent();
            audioFile = new AudioFileReader("C:\\Users\\MrPro\\Downloads\\Alarm Sound Effect.wav");
            waveOutDevice.Init(audioFile);
        }

        public void Play()
        {
            if (isDisposed)
            {
                waveOutDevice = new WaveOutEvent();
                audioFile = new AudioFileReader("C:\\Users\\MrPro\\Downloads\\Alarm Sound Effect.wav");
                waveOutDevice.Init(audioFile);
                waveOutDevice.Play();
                isDisposed = false;
            }
     
        }

        public void Stop()
        {
            waveOutDevice.Stop();
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                waveOutDevice.Stop();
                waveOutDevice.Dispose();
                audioFile.Dispose();
                isDisposed = true;
            }
           
        }
    }
}
