using System.Collections.Generic;

namespace SolidSilnique.Core.Components;

public class TreeColliderComponent : Component
{
    public static List<GameObject> instances = [];
    public float Radius;
    public float HeightLimit;
    public TreeColliderComponent(float radius, float heightLimit)
    {
        this.Radius = radius;
        this.HeightLimit = heightLimit;
    }

    public override void Start()
    {
        instances.Add(gameObject);
    }

    public override void Update()
    {
        
    }
    
    
}