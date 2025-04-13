using Microsoft.Xna.Framework;

namespace SolidSilnique;

public class Spotlight : PointLight
{
    private Vector3 direction;
    private float innerCut;
    private float outerCut;

    public Vector3 Direction
    {
        get { return direction; }
        set { direction = value; }
    }

    public float InnerCut
    {
        get { return innerCut; }
        set { innerCut = value; }
    }

    public float OuterCut
    {
        get { return outerCut; }
        set { outerCut = value; }
    }

    public Spotlight(float linear, float quadratic, Vector3 direction, float innerCut, float outerCut) : base(linear, quadratic)
    {
        this.direction = direction;
        this.innerCut = innerCut;
        this.outerCut = outerCut;
    }
}