using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SolidSilnique.Core.RhythmController;

public class GUIRhythymController
{
    private float songTime;
    List<HitNoteVisual> hitNoteVisuals = new List<HitNoteVisual>();
    public GUIRhythymController(List<Note> notesList)
    {
        
        for (int i = 0; i > notesList.Count; i++)
        {
            hitNoteVisuals.Add(new HitNoteVisual((float)notesList[i].Time, notesList[i].Button));
        }
    }

    public void updateGUIRhythym(List<Note> notesList,int[] buttonsPressed,GameTime gameTime)
    {
        for (int i = 0; i < hitNoteVisuals.Count; i++)
        {
            hitNoteVisuals[i].updatePos(gameTime,songTime);
        }
    }
    
}