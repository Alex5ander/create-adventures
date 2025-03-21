using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    int terrainWidth = 640;
    int terrainHeight = 100;
    float frequency = 0.04f;
    Block[,] blocks;
    [SerializeField] GameObject ChunkPrefab;
    [Header("Blocks")]
    [SerializeField] Block WoodMid;
    [SerializeField] Block WoodBottom;
    [SerializeField] Block Cactus;
    [SerializeField] Block Leaves;
    [SerializeField] Block Water;
    [SerializeField] Block Lava;
    [Header("Biomes")]
    [SerializeField] Texture2D biome;
    [SerializeField] Gradient biomeColors;
    [SerializeField] float biomeFrequency;
    [SerializeField] Biome[] biomes;
    [SerializeField] Texture2D tree;
    [SerializeField] float treeFrequency;
    [SerializeField] float treeThreshold;
    readonly Dictionary<int, GameObject> chunks = new();
    [SerializeField] Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        World world = SaveManger.Instance.GetWorld();
        Block[,] terrain = GenerateTerrain(world.seed);
        foreach (ModifiedBlock modifiedBlock in world.modifiedBlocks)
        {
            terrain[modifiedBlock.x, modifiedBlock.y] = modifiedBlock.block;
        }
        for (int x = 0; x < terrain.GetLength(0); x++)
        {
            for (int y = 0; y < terrain.GetLength(1); y++)
            {
                if (terrain[x, y])
                {
                    CreateBlock(x, y, terrain[x, y]);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        int x = Mathf.FloorToInt(playerTransform.position.x / terrainWidth * 10);
        foreach (int i in chunks.Keys)
        {
            GameObject chunk = chunks[i];
            chunk.SetActive(x == i || x == i + 1 || x == i - 1);
            if (chunk.activeSelf)
            {

            }
        }
    }

    void GenerateNoiseTextures(int seed)
    {
        biome = GenerateBiomeTexture(seed);
        tree = GenerateNoiseTexture(treeFrequency, treeThreshold, seed);
        foreach (Biome biome in biomes)
        {
            biome.texture = GenerateNoiseTexture(biome.frequency, biome.threshold, seed);
            foreach (Ore ore in biome.ores)
            {
                ore.texture = GenerateNoiseTexture(ore.frequency, ore.threshold, seed);
            }
        }
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

    public Block[,] GenerateTerrain(int seed)
    {
        GenerateNoiseTextures(seed);
        Block[,] terrain = new Block[terrainWidth, terrainHeight];
        blocks = new Block[terrainWidth, terrainHeight];
        int size = terrainWidth * terrainHeight;
        for (int i = 0; i < size; i++)
        {
            int x = i % terrainWidth;
            int y = Mathf.RoundToInt(i / terrainWidth) % terrainHeight;

            int superfaceY = Mathf.RoundToInt(Mathf.PerlinNoise((x + seed) * frequency, seed * frequency) * (terrainHeight / 2) + (terrainHeight / 2));

            float seaLevel = 70;

            Color color = biome.GetPixel(x, y);
            Biome cBiome = biomes.ToList().Find(e => e.color == color);

            if (cBiome != null)
            {
                if (y < superfaceY)
                {
                    if (y == superfaceY - 1)
                    {
                        terrain[x, y] = cBiome.SurfaceBlock;
                        if (tree.GetPixel(x, y).r > 0.5f)
                        {
                            if (cBiome.color == Color.green)
                            {
                                // CreateTree(x, y + 1, terrain);
                            }
                            else if (cBiome.color.Equals(new Color(1, 127f / 255f, 0, 1)))
                            {
                                // CreateCactus(x, y + 1);
                            }
                        }
                    }
                    else if (y < superfaceY * .75f)
                    {
                        bool stone = true;
                        for (int j = 0; j < cBiome.ores.Length; j++)
                        {
                            Ore ore = cBiome.ores[j];
                            if (ore.texture.GetPixel(x, y).r > 0.5f)
                            {
                                terrain[x, y] = ore.block;
                                stone = false;
                                break;
                            }
                        }
                        if (stone)
                        {
                            terrain[x, y] = cBiome.Stone;
                        }
                    }
                    else
                    {
                        terrain[x, y] = cBiome.Dirt;
                    }
                }
                else if (y < seaLevel)
                {
                    // terrain[x, y] = Water;
                }
            }
        }
        return terrain;
    }
    public void CreateBlock(int x, int y, Block blockPrefab, bool save = false)
    {
        if (x < 0 || x > blocks.GetLength(0) - 1 || y < 0 || y > blocks.GetLength(1) - 1) { return; }
        if (blocks[x, y] == null)
        {
            Block block = Instantiate(blockPrefab, new(x, y), Quaternion.identity);
            blocks[x, y] = block;
            int chunkIndex = Mathf.FloorToInt(x / (float)terrainWidth * 10);
            if (!chunks.ContainsKey(chunkIndex))
            {
                GameObject chunk;
                chunk = CreateChunk(x);
                chunks.Add(chunkIndex, chunk);
            }
            block.transform.SetParent(chunks[chunkIndex].transform);
            if (save)
            {
                SaveManger.SaveWorld(blockPrefab, x, y);
            }
        }
    }

    public GameObject CreateChunk(int x)
    {
        GameObject chunk = Instantiate(ChunkPrefab, new Vector3(x, 0, 0), Quaternion.identity);
        return chunk;
    }

    public T GetBlock<T>(int x, int y) where T : Block
    {
        if (x >= 0 && x < blocks.GetLength(0) && y >= 0 && y < blocks.GetLength(1))
        {
            Block block = blocks[x, y];
            if (block is T typedBlock)
            {
                return typedBlock;
            }
        }
        return null;
    }
    public void RemoveBlock(int x, int y, bool save = false)
    {
        Block block = blocks[x, y];
        if (block)
        {
            Destroy(block.gameObject);
            if (save)
            {
                SaveManger.SaveWorld(null, x, y);
            }
            blocks[x, y] = null;
        }
    }

    void CreateTree(int x, int y, Block[,] terrain)
    {
        int height = 4 + Mathf.RoundToInt(Mathf.PerlinNoise(x * frequency + x, y * frequency + x)) * 4;
        terrain[x, y + height] = Leaves;
        terrain[x + 1, y + height] = Leaves;
        terrain[x - 1, y + height] = Leaves;

        terrain[x, y + height + 1] = Leaves;
        terrain[x + 1, y + height + 1] = Leaves;
        terrain[x - 1, y + height + 1] = Leaves;

        terrain[x, y + height + 2] = Leaves;

        for (int i = 0; i < height; i++)
        {
            if (i == 0)
            {
                terrain[x, y + i] = WoodBottom;
            }
            else
            {
                terrain[x, y + i] = WoodMid;
            }
        }
    }

    void CreateCactus(int x, int y)
    {
        int height = 4 + Mathf.RoundToInt(Mathf.PerlinNoise(x * frequency + x, y * frequency + x)) * 4;
        for (int i = 0; i < height; i++)
        {
            CreateBlock(x, y + i, Cactus);
        }
    }
}
