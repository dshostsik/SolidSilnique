using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace SolidSilnique.MonoAL;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
public class AtomicSoundTrack
{
    private string path;
    private Song song;
    private float volume;
    public AtomicSoundTrack(string path,ContentManager Content,float volume)
    {
        this.path = path;
        song = Content.Load<Song>(path);
        this.volume = volume;
    }

    public void Play()
    {
        // check the current state of the MediaPlayer.
        if(MediaPlayer.State != MediaState.Stopped)
        {
            MediaPlayer.Stop(); // stop current audio playback if playing or paused.
        }

// Play the selected song reference.
        MediaPlayer.Volume = volume;
        MediaPlayer.Play(song);
    }

    public void Stop()
    {
        MediaPlayer.Stop();
    }

    public float songTime()
    {
        return (float)MediaPlayer.PlayPosition.TotalSeconds;
    }
    
}