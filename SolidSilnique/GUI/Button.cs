using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace GUIRESOURCES;

public class Button : GuiElement
{
    public float width;
    public float height;
    Texture2D texture;

    public Button(float posX, float posY, string name, float width, float height, float scale)
    {
        this.positionX = posX;
        this.positionY = posY;
        this.name = name;
        this.width = width;
        this.height = height;
        this.scale = scale;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Vector2 position = new Vector2(positionX, positionY);
        spriteBatch.Draw(texture, position,null, Color.White,0,Vector2.Zero,scale,SpriteEffects.None,0);
    }
    
    public override void Load(Texture2D texture)
    {
        this.texture = texture;
    }
}
