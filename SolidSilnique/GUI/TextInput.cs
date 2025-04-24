using Microsoft.Xna.Framework.Graphics;

namespace GUIRESOURCES;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
public class TextInput : GuiElement
{
    public float fontSize;
    public string font;
    public string placeholder;
    public SpriteFont fontCache;

    public TextInput(float positionX,float positionY,string name,float fontSize, string font, string placeholder, float scale)
    {
        this.positionX = positionX;
        this.positionY = positionY;
        this.name = name;
        this.fontSize = fontSize;
        this.font = font;
        this.placeholder = placeholder;
        this.scale = scale;
    }
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        
    }
    
    public override void Load(SpriteFont fontCache)
    {
        this.fontCache = fontCache;
    }
}