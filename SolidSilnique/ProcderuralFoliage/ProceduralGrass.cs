using System;

namespace SolidSilnique.ProcderuralFoliage;
using SimplexNoise;
public class ProceduralGrass
{
    public int lenght;
    public int width;
    private string AssetName;
    public float[,] computedNoise;
    public float[,] DisNoise;
    public float[,] ChooseNoise;
    private int Exseed = 0;
    private int DisSeed = 0;
    private int ChooseSeed = 0;
    public ProceduralGrass(int lenght, int width)
    {
        this.lenght = lenght;
        this.width = width;
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
        ChooseNoise = Noise.Calc2D(width, lenght, 0.8f);
        
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
}