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

    public Spotlight(float linear, float quadratic, float constant, Vector3 direction, float innerCut, float outerCut) :
        base(linear, quadratic, constant)
    {
        this.direction = direction;
        this.innerCut = innerCut;
        this.outerCut = outerCut;
    }

    public override void SendToShader(Shader shader)
    {
        try
        {
            shader.SetUniform("spotlight1Enabled", Enabled);
            shader.SetUniform("spotlight1_direction", Direction);
            shader.SetUniform("spotlight1_innerCut", MathHelper.ToRadians(innerCut));
            shader.SetUniform("spotlight1_outerCut", MathHelper.ToRadians(outerCut));
            shader.SetUniform("spotlight1_linearAttenuation", Linear);
            shader.SetUniform("spotlight1_quadraticAttenuation", Quadratic);
            shader.SetUniform("spotlight1_constant", Constant);
            shader.SetUniform("spotlight1_ambientColor", AmbientColor);
            shader.SetUniform("spotlight1_diffuseColor", DiffuseColor);
            shader.SetUniform("spotlight1_specularColor", SpecularColor);
        }
        catch (UniformNotFoundException e)
        {
            throw new UniformNotFoundException(e.Message, " error source: Spotlight.cs");
        }
    }
}