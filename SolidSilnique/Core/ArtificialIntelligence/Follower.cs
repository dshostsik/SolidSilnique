#nullable enable

namespace SolidSilnique.Core.ArtificialIntelligence;

public class Follower
{
    private GameObject self;
    private GameObject? target;
    private float socialDistance;

    public GameObject Self
    {
        get => self;
    }
    
    public GameObject Target
    {
        get => target;
        set => target = value;
    }

    public float SocialDistance
    {
        get => socialDistance;
        set => socialDistance = value;
    }

    public Follower(GameObject self, float socialDistance = 200.0f)
    {
        this.self = self;
    }

    public void Follow()
    {
        if (target == null) return;
        // if (self.transform.position < target.transform.position + socialDistance)
        // {
        //     self.transform.position += ;
        // }
    }

}