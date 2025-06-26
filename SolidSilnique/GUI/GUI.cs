using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GUIRESOURCES;

public class GUI
{
    public List<GuiElement> elements = new List<GuiElement>();
    private string name = "";
    public List<ProgressBar> progressBars = new List<ProgressBar>();
    public List<Text> texts = new List<Text>();
    public List<TextInput> textInputs= new List<TextInput>();
    public List<Image> images = new List<Image>();
    public List<Button> buttons = new List<Button>();

    public GUI(string path,ContentManager Content)
    {
        elements = XMLLoader.readUIFile(path);
        for (int i = 0; i < elements.Count; i++)
        {
            if((elements[i] is Text) == false  && (elements[i] is TextInput) == false)
            elements[i].Load(Content.Load<Texture2D>(elements[i].name));
            
            if(elements[i] is ProgressBar)
                progressBars.Add(elements[i] as ProgressBar);
            else if (elements[i] is Text)
            {
                texts.Add(elements[i] as Text);
                SpriteFont font = Content.Load<SpriteFont>(texts[texts.Count-1].font);
                
                texts[texts.Count-1].Load(font);
            }
            else if (elements[i] is TextInput)
            {
                textInputs.Add(elements[i] as TextInput);
                SpriteFont font = Content.Load<SpriteFont>(textInputs[textInputs.Count-1].font);
                textInputs[textInputs.Count-1].Load(font);
            }
                
            else if(elements[i] is Image)
                images.Add(elements[i] as Image);
            else if(elements[i] is Button)
                buttons.Add(elements[i] as Button);
            
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (GuiElement element in elements)
        {
            element.Draw(spriteBatch);
        }
    }
}