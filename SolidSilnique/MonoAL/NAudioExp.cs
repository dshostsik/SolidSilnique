using System;
using NAudio.Wave;

namespace SolidSilnique.MonoAL
{
    public class NAudioPlayer : IDisposable
    {
        private AudioFileReader audioFile;
        private WasapiOut outputDevice;
        private bool isDisposed;

        public NAudioPlayer()
        {
            outputDevice = new WasapiOut();
        }

        public void LoadAudio(string filePath)
        {
            // Dispose previous audio file if exists
            if (audioFile != null)
            {
                audioFile.Dispose();
            }
            
            audioFile = new AudioFileReader(filePath);
            audioFile.Volume = 0.4f;
            outputDevice.Init(audioFile);
        }

        public void Play()
        {
            if (audioFile == null)
                throw new InvalidOperationException("No audio file loaded");

            outputDevice.Play();
        }

        public void Stop()
        {
            if (audioFile == null)
                return;

            outputDevice.Stop();
            // Reset position to beginning
            audioFile.Position = 0;
        }

        public void Pause()
        {
            if (audioFile == null)
                return;

            outputDevice.Pause();
        }

        public TimeSpan GetCurrentPosition()
        {
            if (audioFile == null)
                return TimeSpan.Zero;

            return audioFile.CurrentTime;
        }

        public TimeSpan GetTotalDuration()
        {
            if (audioFile == null)
                return TimeSpan.Zero;

            return audioFile.TotalTime;
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                if (audioFile != null)
                {
                    audioFile.Dispose();
                    audioFile = null;
                }

                if (outputDevice != null)
                {
                    outputDevice.Dispose();
                    outputDevice = null;
                }

                isDisposed = true;
            }
        }
    }
}