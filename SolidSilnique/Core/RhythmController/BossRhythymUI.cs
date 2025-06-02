using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SolidSilnique.Core;
using SolidSilnique.Core.RhythmController;
using SolidSilnique.MonoAL;
using static NotesLoader;
public class BossRhythymUI
{
    float offset = 0.32f;
    private Stack<int> buttons = new Stack<int>();
    private List<float> accuracy = new List<float>();
    private List<Note> loadedNotes = new List<Note>();
    private List<float> offsets = new List<float>();
    int[] buttonsPressed = new int[4];
    float[] accuracyPressed = new float[4];
    private float songTime;
    public int health = 100;
    private int combo = 0;
    bool turnedOff = false;
    KeyboardState kState = new KeyboardState();
    ContentManager content;
    AtomicSoundTrack audio;
    SpriteBatch spriteBatch;
    private GUIRhythymController visuals;
    List<Texture2D> textures = new List<Texture2D>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start(ContentManager content,SpriteBatch spriteBatch)
    {
        this.spriteBatch = spriteBatch;
        this.content = content;
        textures.Add(content.Load<Texture2D>("Textures/redNote"));
        textures.Add(content.Load<Texture2D>("Textures/blueNote"));
        textures.Add(content.Load<Texture2D>("Textures/yellowNote"));
        textures.Add(content.Load<Texture2D>("Textures/violetNote"));
        
        audio = new AtomicSoundTrack("master house",
            content, 0.1f);
        loadedNotes = NotesLoader.LoadNotesFromXml("Content/level.xml");
        visuals = new GUIRhythymController(loadedNotes,content);
        songTime = 0f;
        audio.Play();
        
    }

    // Update is called once per frame
    public void Update()
    {
        
        kState = Keyboard.GetState();
        if (turnedOff)
        {
            return;
        }
        songTime += Time.deltaTime;
        if (audio.songTime() < 0)
        {
            Console.WriteLine("o kurwa");
        }
        
        visuals.updateGUIRhythym(loadedNotes,buttonsPressed,songTime);
        visuals.drawNotes(spriteBatch);
        readInput();
        
        
        Color[] colors = new Color[2];
        colors[0] = Color.Black;
        colors[1] = Color.White;
        EngineManager.renderQueueUI.Enqueue(new Tuple<Texture2D, Vector2, Color>(textures[0], new Vector2(1080-64, 80), colors[buttonsPressed[0]])); // I
        EngineManager.renderQueueUI.Enqueue(new Tuple<Texture2D, Vector2, Color>(textures[1], new Vector2(400, 720-64), colors[buttonsPressed[1]])); // <
        EngineManager.renderQueueUI.Enqueue(new Tuple<Texture2D, Vector2, Color>(textures[2], new Vector2(1080-64, 1296), colors[buttonsPressed[2]])); // K
        EngineManager.renderQueueUI.Enqueue(new Tuple<Texture2D, Vector2, Color>(textures[3], new Vector2(1676, 720-64), colors[buttonsPressed[3]])); // >
        if (CheckNotZero(buttonsPressed) > 0)
        {
            for (int i = 0; i < 4; i++)
            {
                if (buttonsPressed[i] != 0)
                {
                    if (i == 0 || i == 2)
                    {
                        
                        CheckCorespondingNote(i, accuracyPressed[i]); 
                    }
                    CheckCorespondingNote(i, accuracyPressed[i]);   
                }
                    
                
            }
            
        }
        checkTooOldNotes();
        if (loadedNotes.Count == 0)
        {
            EndingScreen();
            audio.Stop();
            turnedOff = true;
        }

        if (health == 0)
        {
            EndingScreen();
            audio.Stop();
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
        
        int limit = 4;
        if (loadedNotes.Count < 4)
        {
            Console.WriteLine(loadedNotes.Count);
            limit = loadedNotes.Count;
        }
        for (int i = 0; i < limit; i++)
        {
            
            
            if (a == loadedNotes[i].Button && Math.Abs(pressTime-(float)loadedNotes[i].Time -3.5) < 1.2f)
            {
                offsets.Add(Math.Abs(pressTime-(float)loadedNotes[i].Time));
                
                accuracy.Add(Math.Abs(pressTime-(float)loadedNotes[i].Time)/50f);
                EngineManager.renderQueueUI.Enqueue(new Tuple<Texture2D, Vector2, Color>(textures[3], new Vector2(1300, 720-64), Color.White));
                
                Console.WriteLine(songTime - loadedNotes[i].Time);
                Console.WriteLine(loadedNotes[i].Button);
                Console.WriteLine("przerwa");
                
                combo++;
                if (health < 96)
                {
                    health += 5;
                }
                visuals.setNoteInvisible(loadedNotes[i].Button, loadedNotes[i].Time);
                loadedNotes.RemoveAt(i);
                limit = loadedNotes.Count;
            }

            if (i >= limit)
            {
                break;
            }
        }
        
    }

    void checkTooOldNotes()
    {
        for (int i = 0; i < loadedNotes.Count; i++)
        {
            if (loadedNotes[i].Time < songTime - 1.5f)
            {
                
                
                loadedNotes.RemoveAt(i);
                if (health >= 5)
                {
                    health -= 5;
                }
                
            }
        }
        
        combo = 0;
    }

    void EndingScreen()
    {
        float totalScore = 0;
        for (int i = 0; i < accuracy.Count; i++)
        {
            totalScore += accuracy[i];
        }
       
        float avgOffset = 0;
        for (int i = 0; i < offsets.Count; i++)
        {
            avgOffset += offsets[i];
        }
    }

    void readInput()
    {
        if (kState.IsKeyDown(Keys.J))
        {
            buttonsPressed[1] = 1;
            accuracyPressed[1] = audio.songTime();
        }  
             
              if  (kState.IsKeyDown(Keys.I))
             { 
                        buttonsPressed[0] = 1         ;
                accuracyPressed[0] = audio.songTime();
            }
            
            if (kState.IsKeyDown(Keys.K))
            {
                buttonsPressed[2] = 1; 
                accuracyPressed[2] = audio.songTime();
            }
            
            if (kState.IsKeyDown(Keys.L))
            {
                buttonsPressed[3] = 1; 
                accuracyPressed[3] = audio.songTime();
            }
            
        
    }

    public float ReturnScoresAndAccuracy()
    {
        float score = 0;
        for (int i = 0; i < accuracy.Count; i++)
        {
          score += accuracy[i];  
        }
        return score;
    }
}
