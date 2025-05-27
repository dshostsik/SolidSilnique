using System;
using Microsoft.Xna.Framework;

namespace SolidSilnique.Core.RhythmController;

public class HitNoteVisual
{
    private int positionX;
    private int positionY;
    private float noteTime;
    private int noteButton;
    private bool noteVisible = false;
    public HitNoteVisual(float noteTime, int noteButton)
    {
        this.noteTime = noteTime;
        this.noteButton = noteButton;
        switch (noteButton)
        {
            case 0:
                positionX +=  64;
                break;
            case 1:
                positionY += 64;
                break;
            case 2:
                positionX += 64;
                break;
            case 3:
                positionY += 64;
                break;
                
        }
        
    }
    

    public void updatePos(GameTime gameTime,float songTime)
    {
        if (songTime + 2f >= songTime && !noteVisible)
        {
            noteVisible = true;
        }
        switch (noteButton)
        {
            case 0:
                positionY +=  gameTime.ElapsedGameTime.Milliseconds * 10;
                break;
            case 1:
                positionX -=  gameTime.ElapsedGameTime.Milliseconds * 10;
                break;
            case 2:
                positionY -=  gameTime.ElapsedGameTime.Milliseconds * 10;
                break;
            case 3:
                positionX +=  gameTime.ElapsedGameTime.Milliseconds * 10;
                break;
                
        }

        if (noteTime < songTime + 1f)
        {
            Console.WriteLine("usun jakos te notke");
            noteVisible = false;
        }
    }
}