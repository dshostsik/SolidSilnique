using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SolidSilnique.Core;

using static NotesLoader;
public class BossRhythymUI
{
    
    private Stack<int> buttons = new Stack<int>();
    private Stack<float> accuracy = new Stack<float>();
    private List<Note> loadedNotes = new List<Note>();
    int[] buttonsPressed = new int[4];
    float[] accuracyPressed = new float[4];
    private float songTime;
    public int health = 100;
    private int combo = 0;
    bool turnedOff = false;
    KeyboardState kState = new KeyboardState();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        loadedNotes = NotesLoader.LoadNotesFromXml("level.xml");
        songTime = 0f;
    }

    // Update is called once per frame
    void Update(GameTime time)
    {
        kState = Keyboard.GetState();
        if (turnedOff)
        {
            return;
        }
        songTime += Time.deltaTime;
        readInput();
        if (CheckNotZero(buttonsPressed) > 0)
        {
            for (int i = 0; i < 4; i++)
            {
                if(buttonsPressed[i] != 0)
                    CheckCorespondingNote(buttonsPressed[i], accuracyPressed[i]);
            }
            checkTooOldNotes();
        }

        if (loadedNotes.Count == 0)
        {
            EndingScreen();
            turnedOff = true;
        }
        buttonsPressed = new int[4];
        accuracyPressed = new float[4];
    }

    int CheckNotZero(int[] buttons)
    {
        int a = 0;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != 0)
                a++;
        }

        return a;
    }

    void CheckCorespondingNote(int a,float pressTime)
    {
        for (int i = 0; i < 4; i++)
        {
            if (a == loadedNotes[i].Button && Math.Abs(pressTime-(float)loadedNotes[i].Time) < 0.5f)
            {
                loadedNotes.RemoveAt(i);
                accuracy.Push(Math.Abs(pressTime-(float)loadedNotes[i].Time)/50f);
                Console.WriteLine("Hit in Time");
                Console.WriteLine("Accuracy");
                Console.WriteLine(Math.Abs(pressTime-(float)loadedNotes[i].Time)/50f);
                combo++;
            }
        }
        
    }

    void checkTooOldNotes()
    {
        for (int i = 0; i < 5; i++)
        {
            if (loadedNotes[i].Time < songTime - 0.5)
            {
                loadedNotes.RemoveAt(i);
            }
        }
        
        combo = 0;
    }

    void EndingScreen()
    {
        float totalScore = 0;
        for (int i = 0; i < accuracy.Count; i++)
        {
            totalScore += accuracy.Pop();
        }
        Console.WriteLine(totalScore);
    }

    void readInput()
    {
        if (kState.IsKeyDown(Keys.J))
        {
            buttonsPressed[0] = 1;
            accuracyPressed[0] = songTime;
        }
            
            if (kState.IsKeyDown(Keys.I))
            {
                buttonsPressed[1] = 1;
                accuracyPressed[1] = songTime;
            }
            
            if (kState.IsKeyDown(Keys.K))
            {
                buttonsPressed[2] = 1; 
                accuracyPressed[1] = songTime;
            }
            
            if (kState.IsKeyDown(Keys.L))
            {
                buttonsPressed[3] = 1; 
                accuracyPressed[1] = songTime;
            }
            
        
    }
}
