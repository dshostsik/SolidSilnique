using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SolidSilnique.Core;
using SolidSilnique.Core.Components;

namespace SolidSilnique.ProcderuralFoliage;
using SimplexNoise;
public class ProceduralGrass
{
    public int lenght;
    public int width;
    private string AssetName;
    public float[,] computedNoise;
    public float[,] DisNoise;
    public float[,] TransformNoise;
    private int Exseed = 0;
    private int DisSeed = 0;
    private int ChooseSeed = 0;
    private Texture2D levelMap;
    //By system działał poprawnie Textura i model muszą być na tym samym indexie w listach,w dictionary tekstur musi znajdować się tekstura level map
    public List<Model>			loadedModels;
    public List<Texture2D>	loadedTextures;
    public List<Model>		loadedTrees;
    public List<Texture2D>	loadedTexturesTrees;
    public EnvironmentObject enviro;


	public List<GameObject>	createdObjects = new List<GameObject>();
    public ProceduralGrass(List<Model>loadedModels,List<Texture2D>loadedTextures,List<Model>loadedTrees,List<Texture2D>loadedTexturesTrees,ContentManager Content, EnvironmentObject enviro)
    {
        levelMap = Content.Load<Texture2D>("levelMap");
        this.lenght = levelMap.Height;
        this.width = levelMap.Width;
        this.loadedModels = loadedModels;
        this.loadedTextures = loadedTextures;
        this.loadedTrees = loadedTrees;
        this.loadedTexturesTrees = loadedTexturesTrees;
        this.enviro = enviro;
    }

    public void precomputeNoise()
    {
        var random = new Random();
        if (Exseed != 0)
        {
            Noise.Seed = Exseed;
        }
        computedNoise = Noise.Calc2D(width, lenght, 0.8f);
        DisSeed = random.Next();
        if (DisSeed != 0)
        {
            Noise.Seed = DisSeed;
        }
        DisNoise = Noise.Calc2D(width, lenght, 0.8f);
        ChooseSeed = random.Next();
        if (ChooseSeed != 0)
        {
            Noise.Seed = ChooseSeed;
        }
        TransformNoise = Noise.Calc2D(width, lenght, 0.8f);
        
    }

    public void SetSeed(int seed)
    {
        this.Exseed = seed;
    }
    public void SetDisSeed(int seed)
    {
        this.DisSeed = seed;
    }
    public void SetChooseSeed(int seed)
    {
        this.ChooseSeed = seed;
    }

    public void GenerateObjects()
    {
        
        Color[] pixels = new Color[levelMap.Width * levelMap.Height];
        levelMap.GetData(pixels);
        
        

        for (int i = 0; i < lenght; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Vector3 values = pixels[i * levelMap.Width + j].ToVector3();
                int Red = (int)(values.X * 255);
                int Green = (int)(values.Y * 255);
                int Blue = (int)(values.Z * 255);
                switch (Green)
                {
                    case 0:
                        standardTerrain(j,i,Red,Blue);
                        break;
                    case 120:
                        generatePath(j, i, Red,Blue);
                        break;
                    case 255:
                        GenerateTreeWall(j, i,Red,Blue);
                        break;
                }
            }
        }
    }

    public void GenerateTreeWall(int i, int j,int Red,int Blue)
    {
        
        Random random = new Random();
        GameObject go = new GameObject("Tree") { 
            useInstancing = true,
            isStatic = true,
        };
        float randX = i * 3, randZ = j * 3;

        randX += DisNoise[i, j] / 60f;
        randZ -= DisNoise[i, j] / 60f;
        go.transform.position = new Vector3(randX, -0.3f, randZ);
        go.transform.position += Vector3.Up * enviro.GetHeight(go.transform.position);


		float MaximumHeight =1f + (TransformNoise[i,j] /255f * 1.5f);
        MaximumHeight = myCeiling(MaximumHeight,0.8f + Red/255f * 1.5f);
        float scaleY = MaximumHeight;
        float scaleXZ = 1.2f + ((TransformNoise[i,j] + computedNoise[i,j]) / 510f);
        
        go.transform.scale = new Vector3(scaleXZ/2f,scaleY, scaleXZ/2f);
        
        double rotationY = TransformNoise[i,j] * Math.PI * 2;
        go.transform.rotation = new Vector3((float)rotationY * 0.0f,(float)rotationY,-(float)rotationY * 0.0f);
        
        int randomTreeModel = (int)Math.Round(computedNoise[i, j] % loadedTrees.Count);
        int randomTreeTexture = (int)Math.Round(TransformNoise[i, j] % loadedTexturesTrees.Count);
        
        randomTreeModel = Math.Min(randomTreeModel, loadedTrees.Count-1);
        randomTreeTexture = Math.Min(randomTreeTexture, loadedTexturesTrees.Count-1);
        // mamy x modeli , wybranie modelu bazuje na noise a w wartościach między 0 a 255,tekstura na noise b
        go.model = loadedTrees[randomTreeModel];
        go.AddLOD(loadedTrees[randomTreeModel], 0f);
        go.AddLOD(loadedTrees[randomTreeModel], 100f);
        go.AddLOD(null, 200f);
        
        go.texture = loadedTexturesTrees[randomTreeModel];
        go.AddComponent(new TreeColliderComponent(0.6f*scaleXZ,10));
        createdObjects.Add(go);
    }

    public void generatePath(int i, int j, int Red, int Blue)
    {
    
        

        if (computedNoise[i,j] < Blue)
        {
            GameObject go = new GameObject("NotTree") { 
                isStatic = true,
                useInstancing = true

			};
            float randX = i * 3, randZ = j * 3;

            randX += DisNoise[i, j] / 60f;
            randZ -= DisNoise[i, j] / 60f;
            go.transform.position = new Vector3(randX, -0.3f, randZ);
			go.transform.position += Vector3.Up * enviro.GetHeight(go.transform.position);


			float MaximumHeight =1f + (TransformNoise[i,j] /255f * 1.5f);
            MaximumHeight = myCeiling(MaximumHeight,0.8f+ Red/255f * 1.5f);
            float scaleY = MaximumHeight;
            float scaleXZ = 0.9f + ((TransformNoise[i,j] + computedNoise[i,j]) / 510f);

            go.transform.scale = new Vector3(scaleXZ,scaleY,scaleXZ);

            double rotationY = TransformNoise[i,j] * Math.PI * 2;
            go.transform.rotation = new Vector3((float)rotationY * 0.01f,(float)rotationY,-(float)rotationY * 0.01f);
            
            int randomModel = (int)Math.Round(computedNoise[i, j] % loadedModels.Count);
            int randomTexture = (int)Math.Round(TransformNoise[i, j] / loadedTextures.Count);
            
            randomModel = Math.Min(randomModel, loadedModels.Count-1);
            randomTexture = Math.Min(randomTexture, loadedTextures.Count-1);

            go.model = loadedModels[randomModel];
            go.AddLOD(loadedModels[randomModel], 0f);
            go.AddLOD(loadedModels[randomModel], 100f);
            go.AddLOD(null, 200f);
            
                go.texture = loadedTextures[randomModel];
                
            
            

            createdObjects.Add(go);
        }
    }

    public void standardTerrain(int i, int j, int Red, int Blue)
    {
        float Choose = (computedNoise[i,j] + DisNoise[i, j] + TransformNoise[i,j]);
        if (Choose > 250)
        {
            generatePath(i, j, Red, Blue);
        }
        else
        {
            GenerateTreeWall(i,j,Red,Blue);
        }
    }

    private float myCeiling(float a, float b)
    {
        if (a > b)
            return b;
        else
        {
            return a;
        }
    }





}