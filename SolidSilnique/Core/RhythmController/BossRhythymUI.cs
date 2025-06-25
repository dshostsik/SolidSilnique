using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SolidSilnique.Core;
using SolidSilnique.Core.Components;
using SolidSilnique.Core.RhythmController;
using SolidSilnique.MonoAL;

public class BossRhythymUI
{
    float offset = 0.32f;
    public bool hasEnded = false;
    private Stack<int> buttons = new Stack<int>();
    private List<float> accuracy = new List<float>();
    public List<Note> loadedNotes = new List<Note>();
    private List<float> offsets = new List<float>();
    int[] buttonsPressed = new int[4];
    float[] accuracyPressed = new float[4];
    private float songTime;
    public int health = 100;
    public int combo = 0;
    bool turnedOff = false;
    KeyboardState kState = new KeyboardState();
    private GamePadState gpState;
    ContentManager content;
    public NAudioPlayer audio;
    SpriteBatch spriteBatch;
    private GUIRhythymController visuals;
    List<Texture2D> textures = new List<Texture2D>();
    private Texture2D goodHitTexture;
    private Texture2D badHitTexture;

    private Vector2[] _notePositions;

    private class Feedback
    {
        public Texture2D Texture;
        public Vector2   Position;
        public Color     Color;
        public float     StartTime;
        public float     Duration;
    }

    private List<Feedback> _feedbacks = new();

    private const float FeedbackDur = 0.2f;
    

    public event EventHandler<NoteHitEventArgs> hit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start(ContentManager content,SpriteBatch spriteBatch, int enemyIndex, string difficulty, string audioExtension)
    {
        this.spriteBatch = spriteBatch;
        this.content = content;
        textures.Add(content.Load<Texture2D>("Textures/YellowNotActive"));
        textures.Add(content.Load<Texture2D>("Textures/BlueNotActive"));
        textures.Add(content.Load<Texture2D>("Textures/GreenNotActive"));
        textures.Add(content.Load<Texture2D>("Textures/RedNotActive"));
        textures.Add(content.Load<Texture2D>("Textures/YellowActive"));
        textures.Add(content.Load<Texture2D>("Textures/BlueActive"));
        textures.Add(content.Load<Texture2D>("Textures/GreenActive"));
        textures.Add(content.Load<Texture2D>("Textures/RedActive"));
        goodHitTexture = content.Load<Texture2D>("Visuals/Perfect");
        badHitTexture = content.Load<Texture2D>("Visuals/X");
        EngineManager.InputManager.ActionPressed += OnActionPressed;

        audio = new NAudioPlayer();
        audio.LoadAudio("Content/Rlevels/enemy"+enemyIndex+"/song." + audioExtension);
        loadedNotes = NotesLoader.LoadNotesFromXml("Content/Rlevels/enemy"+enemyIndex+"/chart"+difficulty+".xml");
        visuals = new GUIRhythymController(loadedNotes,content);
        songTime = 0f;
		turnedOff = false;
		_feedbacks = new();
		audio.Play();

        var vp = spriteBatch.GraphicsDevice.Viewport;

        float cx = vp.Width * 0.5f;
        float cy = vp.Height * 0.5f;

        float netX = vp.Width * offset;
        float netY = vp.Height * offset;

        _notePositions = new Vector2[4]
        {
            // NW
            new Vector2(cx - netX, cy - netY),
            // NE
            new Vector2(cx + netX, cy - netY),
            // SE
            new Vector2(cx + netX, cy + netY),
            // SW
            new Vector2(cx - netX, cy + netY)
        };

    }

    // Update is called once per frame
    public void Update()
    {
        
        if (turnedOff)
        {
            return;
        }

        songTime = (float)audio.GetCurrentPosition().TotalSeconds;
        
        
        visuals.updateGUIRhythym(loadedNotes,buttonsPressed,songTime);
        visuals.drawNotes(spriteBatch);
        
        
        
        Color[] colors = new Color[2];
        colors[0] = Color.Black;
        colors[1] = Color.White;
        
        
        //EngineManager.renderQueueUI.Enqueue(new Tuple<Texture2D, Vector2, Color>(textures[0], new Vector2(896, 476), colors[buttonsPressed[0]])); // I
        
        EngineManager.renderQueueUI.Enqueue(new Tuple<Texture2D, Vector2, Color>(textures[buttonsPressed[0] * 4+0], new Vector2(896, 476-80 + 32), Color.White)); // I
        EngineManager.renderQueueUI.Enqueue(new Tuple<Texture2D, Vector2, Color>(textures[buttonsPressed[1] * 4+1], new Vector2(896-80 + 32, 476), Color.White)); // <
        EngineManager.renderQueueUI.Enqueue(new Tuple<Texture2D, Vector2, Color>(textures[buttonsPressed[2] * 4+2], new Vector2(896, 476+80 - 32), Color.White)); // K
        EngineManager.renderQueueUI.Enqueue(new Tuple<Texture2D, Vector2, Color>(textures[buttonsPressed[3] * 4+3], new Vector2(896+80 - 32, 476), Color.White)); // >
        
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

        for (int i = _feedbacks.Count - 1; i >= 0; i--)
        {
            var fb = _feedbacks[i];
            if (songTime - fb.StartTime <= fb.Duration)
            {
                EngineManager.renderQueueUI.Enqueue(
                    Tuple.Create(fb.Texture,
                                 fb.Position,
                                 fb.Color));
            }
            else
            {
                _feedbacks.RemoveAt(i);
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
        Array.Clear(buttonsPressed, 0, 4);
        Array.Clear(accuracyPressed, 0, 4); ;
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
            
            limit = loadedNotes.Count;
        }
        for (int i = 0; i < limit; i++)
        {
            
            
            if (a == loadedNotes[i].Button && Math.Abs(pressTime-(float)loadedNotes[i].Time-0.014) < 0.14f)
            {
                hit?.Invoke(this,new NoteHitEventArgs(Math.Abs(pressTime-(float)loadedNotes[i].Time - 0.014f),a,combo+1));
                NoteHit?.Invoke(this, new NoteEventArgs
                {
                    ButtonIndex = a,
                    Accuracy = Math.Abs(pressTime - (float)loadedNotes[i].Time - 0.014f)
                });
                offsets.Add(Math.Abs(pressTime-(float)loadedNotes[i].Time));
                Console.WriteLine(songTime - loadedNotes[i].Time);
                accuracy.Add(Math.Abs(pressTime-(float)loadedNotes[i].Time)/50f);
                
                
                
                //Console.WriteLine(loadedNotes[i].Button);

                Vector2 hitPos = new Vector2(960, 100);
                _feedbacks.Add(new Feedback
                {
                    Texture = goodHitTexture,
                    Position = _notePositions[a],
                    Color = Color.White,
                    StartTime = songTime,
                    Duration = FeedbackDur
                });
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
            if (loadedNotes[i].Time < songTime - 0.14f)
            {
                int a = loadedNotes[i].Button;  // which arrow missed
                _feedbacks.Add(new Feedback
                {
                    Texture = badHitTexture,
                    Position = _notePositions[a],
                    Color = Color.White,
                    StartTime = songTime,
                    Duration = FeedbackDur
                });
                combo = 0;
                loadedNotes.RemoveAt(i);
                if (health >= 5)
                {
                    //health -= 5;
                }
                
            }
        }
        
       
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

        hasEnded = true;
    }

    
    private void OnActionPressed(string action)
    {
        int slot = action switch
        {
            "NoteI" => 0,
            "NoteJ" => 1,
            "NoteK" => 2,
            "NoteL" => 3,
            _ => -1
        };
        if (slot < 0) return;
        buttonsPressed[slot] = 1;
        accuracyPressed[slot] = songTime;

        CheckCorespondingNote(slot, accuracyPressed[slot]);
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
    public class NoteHitEventArgs : EventArgs
    {
        
        public float Accuracy { get; private set; }
        public int NoteType { get; private set; }
        public int Combo { get; private set; }

        public NoteHitEventArgs(float accuracy, int noteType, int combo)
        {
            Accuracy = accuracy;
            NoteType = noteType;
            Combo = combo;
        }
    }
    public class NoteEventArgs : EventArgs
    {

        public int ButtonIndex;
        public float Accuracy;
    }

    public event EventHandler<NoteEventArgs> NoteHit;



}
