using System;

namespace SolidSilnique.MonoAL;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
public class AudioPlayer
{
    private SoundEffect[] SoundEffects;
    private bool positional = false;
    private float volume = 1f;
    private float pitch = 1f;
    private AudioEmitter emitter;
    private PositionalHelper helper;

    public AudioPlayer(string[] sounds,PositionalHelper helper,float volume,float pitch)
    {
       SoundEffects = new SoundEffect[sounds.Length];
       for(int i=0;i<SoundEffects.Length;i++)
       {
           using Stream soundfile = TitleContainer.OpenStream(sounds[i]);
           SoundEffects[i] = SoundEffect.FromStream(soundfile);
       }
       this.helper = helper;
       this.volume = volume;
       this.pitch = pitch;
    }

    public void setAudioVolume(float volume)
    {
        this.volume = volume;
    }

    public void setPitch(float pitch)
    {
        this.pitch = pitch;
    }

    public void playAudio(int audio,float volChange=0, float pitchChange=0)
    {
        SoundEffectInstance sound = SoundEffects[audio].CreateInstance();
        sound.Pitch = pitchChange + pitch;
        sound.Volume = volChange + volume;
        if (positional)
        {
            playPositional(sound);
            
        }
        else
        {
            playNotPositional(sound);
        }
    }

    private void playPositional(SoundEffectInstance sound)
    {
        AudioListener listener = helper.getListeners()[0];
        sound.Apply3D(listener,emitter);
        sound.Play();
    }

    private void playNotPositional(SoundEffectInstance sound)
    {
        sound.Play();
    }

    private void makePositional()
    {
        emitter = new AudioEmitter();
        positional = true;
        emitter.Position = new Vector3(0,1,0);
        
    }
}