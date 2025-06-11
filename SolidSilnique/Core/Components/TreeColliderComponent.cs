using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace SolidSilnique.Core.Components;

public class TreeColliderComponent : Component
{
    public static List<GameObject>[,] instancesGrid = new List<GameObject>[100, 100];
	public static float gridCellSize = 10;

	public float Radius;
    public float HeightLimit;

	static TreeColliderComponent()
	{
		for (int x = 0; x < 100; x++)
		{
			for (int y = 0; y < 100; y++)
			{
				instancesGrid[x, y] = new List<GameObject>();
			}
		}
	}

    public static List<GameObject> getGridList(Vector3 pos) {



		
		int x = Math.Clamp((int)(pos.X / gridCellSize), 0, 100);
		int z = Math.Clamp((int)(pos.Z / gridCellSize), 0, 100);
		List<GameObject> res = instancesGrid[z, x];

		return res;

	}


	public TreeColliderComponent(float radius, float heightLimit)
    {
        this.Radius = radius;
        this.HeightLimit = heightLimit;
    }

    public override void Start()
    {
        Vector3 pos = gameObject.transform.position;
        int x = Math.Clamp((int) (pos.X / gridCellSize), 0, 100);
		int z = Math.Clamp((int) (pos.Z/ gridCellSize), 0, 100);
        instancesGrid[z,x].Add(gameObject);
    }

    public override void Update()
    {
        
    }
    
    
}