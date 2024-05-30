using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour, ISaveManager
{
    int terrainWidth = 320;
    int terrainHeight = 100;
    float frequency = 0.04f;
    [SerializeField] GameState gameState;
    [SerializeField] OptimizedBlock OptimizedBlockPrefab;
    [Header("Blocks")]
    [SerializeField] Item Dirt;
    [SerializeField] Item Stone;
    [SerializeField] Item Sand;
    [SerializeField] Item Snow;
    [SerializeField] Item Ice;
    [SerializeField] Item Wood;
    [SerializeField] Item Cactus;
    [SerializeField] Item Leaves;
    [SerializeField] Item Coal;
    [SerializeField] Item Iron;
    [SerializeField] Item Gold;
    [SerializeField] Item Diamond;
    [SerializeField] Item Zombie;
    [SerializeField] Item Water;
    [SerializeField] Item Lava;
    [SerializeField] Item MushroomBrown;
    [SerializeField] Item MushroomRed;
    [SerializeField] Item MushroomTan;
    [SerializeField] Item Grass;
    [Header("Coal")]
    [SerializeField] float coalFrequency;
    [SerializeField] float coalSize;
    public Texture2D coal;
    [Header("Iron")]
    [SerializeField] float ironFrequency;
    [SerializeField] float ironSize;
    public Texture2D iron;
    [Header("Gold")]
    [SerializeField] float goldFrequency;
    [SerializeField] float goldSize;
    public Texture2D gold;
    [Header("Diamond")]
    [SerializeField] float diamondFrequency;
    [SerializeField] float diamondSize;
    public Texture2D diamond;
    [Header("Biomes")]
    [SerializeField] Texture2D biome;
    [SerializeField] Gradient biomeColors;
    [SerializeField] float biomeFrequency;
    [Header("Cave")]
    [SerializeField] Texture2D cave;
    [SerializeField] float caveFrequency;
    [SerializeField] float caveSize;
    [Header("Tree")]
    [SerializeField] Texture2D tree;
    [SerializeField] float treeFrequency;
    [SerializeField] float treeSize;
    [Header("Grass")]
    [SerializeField] Texture2D grass;
    [SerializeField] float grassFrequency;
    [SerializeField] float grassSize;
    [Header("Mushroom")]
    [SerializeField] Texture2D mushroom;
    [SerializeField] float mushroomFrequency;
    [SerializeField] float mushroomSize;
    [Header("Lava")]
    [SerializeField] Texture2D lava;
    [SerializeField] float lavaFrequency;
    [SerializeField] float lavaSize;

    // Start is called before the first frame update
    void Start()
    {

    }

    void GenerateNoiseTextures(int seed)
    {
        coal = GenerateNoiseTexture(coalFrequency, coalSize, seed);
        iron = GenerateNoiseTexture(ironFrequency, ironSize, seed);
        gold = GenerateNoiseTexture(goldFrequency, goldSize, seed);
        diamond = GenerateNoiseTexture(diamondFrequency, diamondSize, seed);
        cave = GenerateNoiseTexture(caveFrequency, caveSize, seed);
        tree = GenerateNoiseTexture(treeFrequency, treeSize, seed);
        grass = GenerateNoiseTexture(grassFrequency, grassSize, seed);
        mushroom = GenerateNoiseTexture(mushroomFrequency, mushroomSize, seed);
        lava = GenerateNoiseTexture(lavaFrequency, lavaSize, seed);
        biome = GenerateBiomeTexture(seed);
    }

    void OnValidate()
    {
        GenerateNoiseTextures(Random.Range(-10000, 10000));
    }

    public float Noise(float x, float y, int seed, float fr)
    {
        float noise = 0;
        int octaves = 3;
        float lacunarity = fr;
        float persistance = fr;
        float maxAmplitude = 0;
        for (int k = 0; k < octaves; k++)
        {
            float frequency = Mathf.Pow(lacunarity, octaves);
            float amplitude = Mathf.Pow(persistance, octaves);
            persistance *= 0.5f;
            maxAmplitude += amplitude;
            noise += Mathf.PerlinNoise((x + seed) * frequency, (y + seed) * frequency) * amplitude;
        }
        noise /= maxAmplitude;
        return noise;
    }

    public Texture2D GenerateNoiseTexture(float freq, float threshold, int seed)
    {
        Texture2D texture2D = new(terrainWidth, terrainHeight);
        int size = terrainWidth * terrainHeight;
        for (int i = 0; i < size; i++)
        {
            int x = i % terrainWidth;
            int y = Mathf.RoundToInt(i / terrainWidth) % terrainHeight;

            float noise = Noise(x, y, seed, freq);
            if (noise > threshold)
            {
                texture2D.SetPixel(x, y, Color.white);
            }
            else
            {
                texture2D.SetPixel(x, y, Color.black);
            }
        }
        texture2D.Apply();
        return texture2D;
    }

    public Texture2D GenerateBiomeTexture(int seed)
    {
        Texture2D texture2D = new(terrainWidth, terrainHeight);
        int size = terrainWidth * terrainHeight;
        for (int i = 0; i < size; i++)
        {
            int x = i % terrainWidth;
            int y = Mathf.RoundToInt(i / terrainWidth) % terrainHeight;

            float noise = Noise(x, y, seed, biomeFrequency);
            Color color = biomeColors.Evaluate(noise);
            texture2D.SetPixel(x, y, color);
        }
        texture2D.Apply();
        return texture2D;
    }

    public void GenerateTerrain(int seed)
    {
        GenerateNoiseTextures(seed);

        gameState.blocks = new Block[terrainWidth, terrainHeight];
        int size = terrainWidth * terrainHeight;
        for (int i = 0; i < size; i++)
        {
            int x = i % terrainWidth;
            int y = Mathf.RoundToInt(i / terrainWidth) % terrainHeight;

            int superfaceY = Mathf.RoundToInt(Mathf.PerlinNoise1D((x + seed) * frequency) * (terrainHeight / 2) + (terrainHeight / 2));

            float seaLevel = 70;

            Color color = biome.GetPixel(x, y);

            if (y < superfaceY)
            {
                if (y == superfaceY - 1)
                {
                    Color featureColor = tree.GetPixel(x, y);
                    Color grassColor = grass.GetPixel(x, y);
                    Color mushroomColor = mushroom.GetPixel(x, y);

                    if (color == Color.green)
                    {
                        CreateBlock(x, y, Dirt, 1);
                        if (featureColor.r > 0.5f)
                        {
                            CreateTree(x, y + 1);
                        }
                        else if (grassColor.r > 0.5f)
                        {
                            CreateBlock(x, y + 1, Grass);
                        }
                        else if (mushroomColor.r > 0.5f)
                        {
                            CreateBlock(x, y + 1, MushroomRed);
                        }
                    }
                    else if (color == Color.white)
                    {
                        CreateBlock(x, y, Dirt, 2);
                        if (mushroomColor.r > 0.5f)
                        {
                            CreateBlock(x, y + 1, MushroomBrown);
                        }
                    }
                    else
                    {
                        CreateBlock(x, y, Dirt, 3);
                        if (featureColor.r > 0.5f)
                        {
                            CreateCactus(x, y + 1);
                        }
                        else if (grassColor.r > 0.5f)
                        {
                            CreateBlock(x, y + 1, Grass, 1);
                        }
                        else if (mushroom.GetPixel(x, y).r > 0.5f)
                        {
                            CreateBlock(x, y + 1, MushroomTan);
                        }
                    }
                }
                else if (y < superfaceY * .75f)
                {
                    if (cave.GetPixel(x, y).r < 0.5f)
                    {
                        if (coal.GetPixel(x, y).r > 0.5)
                        {
                            CreateBlock(x, y, Coal);
                        }
                        else if (iron.GetPixel(x, y).r > 0.5)
                        {
                            CreateBlock(x, y, Iron);
                        }
                        else if (gold.GetPixel(x, y).r > 0.5)
                        {
                            CreateBlock(x, y, Gold);
                        }
                        else if (diamond.GetPixel(x, y).r > 0.5)
                        {
                            CreateBlock(x, y, Diamond);
                        }
                        else
                        {
                            if (color == Color.green)
                            {
                                CreateBlock(x, y, Stone);
                            }
                            else if (color == Color.white)
                            {
                                CreateBlock(x, y, Ice);
                            }
                            else
                            {
                                CreateBlock(x, y, Sand);
                            }
                        }
                    }
                    else
                    {
                        Color lavaColor = lava.GetPixel(x, y);
                        if (lavaColor.r > 0.5f)
                        {
                            CreateBlock(x, y, Lava);
                        }
                    }
                }
                else
                {
                    if (color == Color.green)
                    {
                        CreateBlock(x, y, Dirt);
                    }
                    else if (color == Color.white)
                    {
                        CreateBlock(x, y, Snow);
                    }
                    else
                    {
                        CreateBlock(x, y, Sand);
                    }
                }
            }
            else if (y < seaLevel)
            {
                CreateBlock(x, y, Water);
            }
        }
    }
    public void CreateBlock(int x, int y, Item item, int meta = 0, bool save = false)
    {
        if (x < 0 || x > gameState.blocks.GetLength(0) - 1 || y < 0 || y > gameState.blocks.GetLength(1) - 1) { return; }
        if (gameState.blocks[x, y] == null)
        {
            OptimizedBlock optimizer = Instantiate(OptimizedBlockPrefab, new(x, y, 0), Quaternion.identity);
            SpriteRenderer spriteRenderer = optimizer.block.GetComponent<SpriteRenderer>();
            optimizer.block.item = item;
            spriteRenderer.sprite = item.sprites[meta];
            gameState.blocks[x, y] = optimizer.block;
            if (save)
            {
                SaveManger.SaveWorld(item, x, y);
            }
        }
    }
    public Block GetBlock(int x, int y)
    {
        if (x >= 0 && x < gameState.blocks.GetLength(0) && y >= 0 && y < gameState.blocks.GetLength(1))
        {
            return gameState.blocks[x, y];
        }
        return null;
    }
    public void Remove(int x, int y, bool save = false)
    {
        if (gameState.blocks[x, y])
        {
            Block block = gameState.blocks[x, y];
            if (save)
            {
                block.CreateDrop();
                SaveManger.SaveWorld(null, x, y);
            }
            Destroy(block.transform.parent.gameObject);
            gameState.blocks[x, y] = null;
        }
    }

    void CreateTree(int x, int y)
    {
        int height = 4 + Mathf.RoundToInt(Mathf.PerlinNoise(x * frequency + x, y * frequency + x)) * 4;
        CreateBlock(x, y + height, Leaves, 0);
        CreateBlock(x + 1, y + height, Leaves, 0);
        CreateBlock(x - 1, y + height, Leaves, 0);

        CreateBlock(x, y + height + 1, Leaves, 0);
        CreateBlock(x + 1, y + height + 1, Leaves, 0);
        CreateBlock(x - 1, y + height + 1, Leaves, 0);

        CreateBlock(x, y + height + 2, Leaves, 0);

        for (int i = 0; i < height; i++)
        {
            if (i == 0)
            {
                CreateBlock(x, y + i, Wood, 1);
            }
            else
            {
                CreateBlock(x, y + i, Wood, 0);
            }
        }
    }

    void CreateCactus(int x, int y)
    {
        int height = 4 + Mathf.RoundToInt(Mathf.PerlinNoise(x * frequency + x, y * frequency + x)) * 4;
        for (int i = 0; i < height; i++)
        {
            CreateBlock(x, y + i, Cactus, 0);
        }
    }

    public void Load(SaveGame saveGame)
    {
        World world = saveGame.GetWorld();
        List<ModifiedBlock> modifiedBlocks = world.modifiedBlocks;
        GenerateTerrain(world.seed);
        foreach (ModifiedBlock modifiedBlock in modifiedBlocks)
        {
            Block block = gameState.blocks[modifiedBlock.x, modifiedBlock.y];
            if (modifiedBlock.type == null)
            {
                if (block != null)
                {
                    Remove(modifiedBlock.x, modifiedBlock.y);
                }
            }
            else if (block == null)
            {
                CreateBlock(modifiedBlock.x, modifiedBlock.y, modifiedBlock.type, 0);
            }
        }
    }
}
