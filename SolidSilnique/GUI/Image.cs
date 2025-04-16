using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
namespace GUIRESOURCES;

public class Image : GuiElement
{
    private Texture2D texture;
    public Image(float positonX, float positonY, string name,float scale)
    {
        this.positionX = positonX;
        this.positionY = positonY;
        this.name = name;
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