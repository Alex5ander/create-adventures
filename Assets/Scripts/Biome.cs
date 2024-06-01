using System;
using UnityEngine;
[Serializable]
public class SurfaceBlock
{
    public Item item;
    public int index;
}

[Serializable]
public class Ore
{
    public Item item;
    public float frequency;
    public float threshold;
    public Texture2D texture;
}

[Serializable]
public class Biome
{
    public Color color;
    public SurfaceBlock SurfaceBlock;
    public Item Dirt;
    public Item Stone;
    public float frequency;
    public float threshold;
    public Texture2D texture;
    public Ore[] ores;
}
