using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolidSilnique.Core.RhythmController;


public class HitNoteVisual
{
    private int positionX=0;
    private int positionY=0;
    public float noteTime;
    public int noteButton;
    public bool noteVisible = false;
    Texture2D noteTexture;
    public HitNoteVisual(float noteTime, int noteButton,List<Texture2D> TextureNotes)
    {
        this.noteTime = noteTime;
        this.noteButton = noteButton;
        positionX = 896;
        positionY = 476;
        noteVisible = false;
       
        switch (noteButton)
        {
            case 0:
                positionX = 896;
                positionY =  -40+32;
                break;
            case 1:
                positionX = 460-80+32;
                positionY = 476;
                break;
            case 2:
                positionX = 896;
                positionY = 912+80-32;
                break;
            case 3:
                positionX = 1332+80-32;
                positionY = 476;
                break;
                
        }

        noteTexture = TextureNotes[noteButton];
    }
    

    public void updatePos(float songTime)
    {
        if (songTime + 1f >= noteTime && !noteVisible)
        {
            noteVisible = true;
        }
        float gameTime = Time.deltaTime;
        if (noteVisible)
        {
            switch (noteButton)
            {
                
                case 0:
                    positionY +=  (int)(gameTime * 436);
                    break;
                case 1:
                    positionX +=  (int)(gameTime * 436);
                    break;
                case 2:
                    positionY -=  (int)(gameTime * 436);
                    break;
                case 3:
                    positionX -=  (int)(gameTime * 436);
                    break;
                
            }  
        }
        

        if (noteTime < songTime -0.05f)
        {
            noteVisible = false;
        }
    }

    public void draw(SpriteBatch spriteBatch)
    {
        if(noteVisible)
            EngineManager.renderQueueUI.Enqueue(new Tuple<Texture2D, Vector2, Color>(noteTexture, new Vector2(positionX, positionY), Color.White));
    }
}