﻿using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace SolidSilnique.MonoAL;
public class PositionalHelper {
    private List<AudioEmitter> emitters;
    private List<AudioListener> listeners;

    private PositionalHelper()
    {
        emitters = new List<AudioEmitter>();
        listeners = new List<AudioListener>();
    }
    
    private static PositionalHelper _instance;
    
    public static PositionalHelper GetInstance()
    {
        if (_instance == null)
        {
            _instance = new PositionalHelper();
        }
        return _instance;
    }

    public void AddEmitter(AudioEmitter emitter)
    {
        emitters.Add(emitter);
    }

    public void AddListener(AudioListener listener)
    {
        listeners.Add(listener);
    }

    public AudioEmitter[] getEmitters()
    {
        return emitters.ToArray();
    }

    public AudioListener[] getListeners()
    {
        return listeners.ToArray();
    }
}