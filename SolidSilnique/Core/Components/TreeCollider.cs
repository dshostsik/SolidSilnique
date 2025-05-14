using System.Collections.Generic;

namespace SolidSilnique.Core.Components;

public class TreeCollider : Component
{
    public static List<GameObject> instances = [];
    public float Radius;
    public float HeightLimit;
    public TreeCollider(float radius, float heightLimit)
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