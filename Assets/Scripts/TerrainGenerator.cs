using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour, ISaveManager
{
    int terrainWidth = 320;
    int terrainHeight = 100;
    float frequency = 1.61803398875f * 2.5f;
    [SerializeField] GameState gameState;
    [SerializeField] OptimizedBlock OptimizedBlockPrefab;
    [SerializeField] Item Dirt;
    [SerializeField] Item Stone;
    [SerializeField] Item Wood;
    [SerializeField] Item Leaves;
    [SerializeField] Item Coal;
    [SerializeField] Item Iron;
    [SerializeField] Item Gold;
    [SerializeField] Item Diamond;
    public void GenerateTerrain(int seed)
    {
        gameState.blocks = new Block[terrainWidth, terrainHeight];
        int size = terrainWidth * terrainHeight;
        for (int i = 0; i < size; i++)
        {
            int x = i % terrainWidth;
            int y = Mathf.RoundToInt(i / terrainWidth) % terrainHeight;

            float _x = (float)x / terrainWidth;
            int height = Mathf.RoundToInt(Mathf.PerlinNoise1D(_x * frequency + seed) * (terrainHeight / 2) + (terrainHeight / 2));
            if (y < height)
            {
                float _y = (float)y / height;

                float noise = 0;
                int octaves = 3;
                float lacunarity = frequency;
                float persistance = 50f;
                float t = 0;
                for (int k = 0; k < octaves; k++)
                {
                    float f = Mathf.Pow(lacunarity, octaves);
                    float a = Mathf.Pow(persistance, octaves);
                    persistance *= 0.5f;
                    t += a;
                    noise += Mathf.PerlinNoise(_x * f + seed, _y * f + seed) * a;
                }
                noise /= t;
                if (y == height - 1)
                {
                    CreateBlock(x, y, Dirt, 1);

                    if (noise > 0.5f)
                    {
                        CreateTree(x, y + 1);
                    }
                }
                else if (y > height * 0.8f)
                {
                    CreateBlock(x, y, Dirt);
                }
                else
                {
                    if (noise > 0.35f)
                    {
                        CreateBlock(x, y, Stone);
                    }
                    else
                    {
                        if (noise > 0.3f)
                        {
                            CreateBlock(x, y, Coal);
                        }
                        else if (noise > 0.26f)
                        {
                            CreateBlock(x, y, Iron);
                        }
                        else if (noise > 0.15f && noise < 0.26f && y < 20)
                        {
                            CreateBlock(x, y, Gold);
                        }
                        else if (noise > 0.1f && noise < 0.15f && y < 10)
                        {
                            CreateBlock(x, y, Diamond);
                        }
                    }
                }
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
