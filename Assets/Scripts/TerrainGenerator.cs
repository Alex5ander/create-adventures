using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour, ISaveManager
{
    int terrainWidth = 320;
    int terrainHeight = 100;
    float frequency = 1.61803398875f * 2.5f;
    [SerializeField] GameState gameState;
    [SerializeField] OptimizedBlock OptimizedBlockPrefab;
    public void GenerateTerrain(int seed)
    {
        gameState.blocks = new Block[terrainWidth, terrainHeight];
        for (int i = 0; i < terrainWidth; i++)
        {
            float x = (float)i / terrainWidth;
            int height = Mathf.RoundToInt(Mathf.PerlinNoise1D(x * frequency + seed) * (terrainHeight / 2) + (terrainHeight / 2));
            for (int j = 0; j < height; j++)
            {
                float y = (float)j / height;

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
                    noise += Mathf.PerlinNoise(x * f + seed, y * f + seed) * a;
                }
                noise /= t;
                if (j == height - 1)
                {
                    CreateBlock(i, j, 0, 1);

                    if (noise > 0.5f)
                    {
                        CreateTree(i, j + 1);
                    }
                }
                else if (j > height * 0.8f)
                {
                    CreateBlock(i, j, 0);
                }
                else
                {
                    if (noise > 0.4f)
                    {
                        CreateBlock(i, j, 1);
                    }
                    else if (j < 30)
                    {
                        if (noise > 0.38f)
                        {
                            CreateBlock(i, j, 5);
                        }
                        else if (noise > 0.3f)
                        {
                            CreateBlock(i, j, 6);
                        }
                        else if (noise > 0.25f)
                        {
                            CreateBlock(i, j, 7);
                        }
                        else if (noise < 0.2f)
                        {
                            CreateBlock(i, j, 8);
                        }
                    }
                }
            }
        }
    }
    public void CreateBlock(int x, int y, int type, int meta = 0, bool save = false)
    {
        if (x < 0 || x > gameState.blocks.GetLength(0) - 1 || y < 0 || y > gameState.blocks.GetLength(1) - 1) { return; }
        if (gameState.blocks[x, y] == null)
        {
            OptimizedBlock optimizer = Instantiate(OptimizedBlockPrefab, new(x, y, 0), Quaternion.identity);
            SpriteRenderer spriteRenderer = optimizer.block.GetComponent<SpriteRenderer>();
            Item item = GameManager.Instance.items.Find(e => e.type == type);
            optimizer.block.item = item;
            spriteRenderer.sprite = item.sprites[meta];
            gameState.blocks[x, y] = optimizer.block;
            if (save)
            {
                SaveManger.SaveWorld(type, x, y);
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
                SaveManger.SaveWorld(-1, x, y);
            }
            Destroy(block.transform.parent.gameObject);
            gameState.blocks[x, y] = null;
        }
    }

    void CreateTree(int x, int y)
    {
        int height = 4 + Mathf.RoundToInt(Mathf.PerlinNoise(x * frequency + x, y * frequency + x)) * 4;
        CreateBlock(x, y + height, 4, 0);
        CreateBlock(x + 1, y + height, 4, 0);
        CreateBlock(x - 1, y + height, 4, 0);

        CreateBlock(x, y + height + 1, 4, 0);
        CreateBlock(x + 1, y + height + 1, 4, 0);
        CreateBlock(x - 1, y + height + 1, 4, 0);

        CreateBlock(x, y + height + 2, 4, 0);

        for (int i = 0; i < height; i++)
        {
            if (i == 0)
            {
                CreateBlock(x, y + i, 3, 1);
            }
            else
            {
                CreateBlock(x, y + i, 3, 0);
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
            if (modifiedBlock.type == -1)
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
