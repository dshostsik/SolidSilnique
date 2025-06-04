using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SolidSilnique.Core.RhythmController;

public class GUIRhythymController
{
    private float songTime;
    List<HitNoteVisual> hitNoteVisuals = new List<HitNoteVisual>();
    List<Texture2D> textures = new List<Texture2D>();
    public GUIRhythymController(List<Note> notesList,ContentManager contentManager)
    {
        
        textures.Add(contentManager.Load<Texture2D>("Textures/redNote"));
        textures.Add(contentManager.Load<Texture2D>("Textures/blueNote"));
        textures.Add(contentManager.Load<Texture2D>("Textures/yellowNote"));
        textures.Add(contentManager.Load<Texture2D>("Textures/violetNote"));
        for (int i = 0; i < notesList.Count; i++)
        {
            hitNoteVisuals.Add(new HitNoteVisual((float)notesList[i].Time, notesList[i].Button,textures));
        }
    }

    public void updateGUIRhythym(List<Note> notesList,int[] buttonsPressed,double songTime)
    {
        this.songTime = (float)songTime;
        for (int i = 0; i < hitNoteVisuals.Count; i++)
        {
            hitNoteVisuals[i].updatePos(this.songTime);
        }
    }

    public void drawNotes(SpriteBatch spriteBatch)
    {
        
        for (int i = 0; i < hitNoteVisuals.Count; i++)
        {
            hitNoteVisuals[i].draw(spriteBatch);
        } 
        
    }

    public void setNoteInvisible(int Button, double Time)
    {
        for (int i = 0; i < hitNoteVisuals.Count; i++)
        {
            if (hitNoteVisuals[i].noteButton == Button && hitNoteVisuals[i].noteTime == Time)
            {
                hitNoteVisuals[i].noteVisible = false;
            }
        }
    }
}