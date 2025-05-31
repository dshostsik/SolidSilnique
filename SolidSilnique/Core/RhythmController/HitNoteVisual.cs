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
        positionX = 1080-64;
        positionY = 720-64;
        noteVisible = false;
        /*
        switch (noteButton)
        {
            case 0:
                
                break;
            case 1:
                positionX = 1080;
                positionY = 720 + 64;
                break;
            case 2:
                positionX = 1080 - 64;
                positionY = 720 + 64;
                break;
            case 3:
                positionX = 1080 + 64;
                positionY = 720 + 64;
                break;
                
        }
*/
        noteTexture = TextureNotes[noteButton];
    }
    

    public void updatePos(float songTime)
    {
        if (songTime + 0.5f >= noteTime && !noteVisible)
        {
            noteVisible = true;
        }
        float gameTime = Time.deltaTime;
        if (noteVisible)
        {
            switch (noteButton)
            {
                
                case 0:
                    positionY -=  (int)(gameTime * 1152);
                    break;
                case 1:
                    positionX -=  (int)(gameTime * 1152);
                    break;
                case 2:
                    positionY +=  (int)(gameTime * 1152);
                    break;
                case 3:
                    positionX +=  (int)(gameTime * 1152);
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