using System;
using UnityEngine;

[Serializable]
public class Ore
{
    public Block block;
    public float frequency;
    public float threshold;
    public Texture2D texture;
}

[Serializable]
public class Biome
{
    public Color color;
    public Block SurfaceBlock;
    public Block Dirt;
    public Block Stone;
    public float frequency;
    public float threshold;
    public Texture2D texture;
    public Ore[] ores;
}
