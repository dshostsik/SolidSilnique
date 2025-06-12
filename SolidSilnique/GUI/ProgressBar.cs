using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GUIRESOURCES;

public class ProgressBar : GuiElement
{
    public float progress = 30;
    private bool IsVertical = false;
    private bool Inverse = false;
    private float width = 0;
    private Texture2D texture;

    public ProgressBar(float posX, float posY, string name, bool isVertical, bool inverse, float scale)
    {
        this.positionX = posX;
        this.positionY = posY;
        this.name = name;
        this.IsVertical = isVertical;
        this.Inverse = inverse;
        this.scale = scale;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Vector2 newScale = new Vector2(scale,scale);
        var position = new Vector2(positionX, positionY);
        if (IsVertical)
            newScale = new Vector2(scale * progress / 100, scale);
        else
            newScale = new Vector2(scale, scale * progress / 100);
        spriteBatch.Draw(texture, position,null, Color.White,0,Vector2.Zero,newScale,SpriteEffects.None,0);
    }

    public override void Load(Texture2D texture)
    {
        this.texture = texture;
    }

    public void setProgress(float progress)
    {
        this.progress = progress;
    }
}