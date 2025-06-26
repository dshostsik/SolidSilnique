using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace GUIRESOURCES;

public class Text : GuiElement
{
    public string text;
    public float fontSize;
    public string font;
    public SpriteFont fontCache;

    public Text(float PositionX, float PositionY,string name, string text, float fontSize, string font, float scale)
    {
        this.positionX = PositionX;
        this.positionY = PositionY;
        this.name = name;
        this.text = text;
        this.fontSize = fontSize;
        this.font = font;
        this.scale = scale;
    }
    
    public override void Draw(SpriteBatch spriteBatch)
    {
		if (!visible)
		{
			return;
		}
		var position = new Vector2(positionX, positionY);
        Vector2 FontOrigin = new Vector2(fontSize *text.Length  / 2,fontSize *text.Length / 2);
        // Draw the string
        spriteBatch.DrawString(fontCache,text,position,Color.White,0.0f,Vector2.Zero,fontSize * scale,SpriteEffects.None, 0);
    }

    public override void Load(SpriteFont fontCache)
    {
        this.fontCache = fontCache;
    }
}