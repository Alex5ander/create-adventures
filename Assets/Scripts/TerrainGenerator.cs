using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TerrainGenerator : MonoBehaviour
{

    int terrainWidth = 320;
    int terrainHeight = 100;
    int chunkSize = 16;
    int seed = 0;
    float frequency = 0.01618f;

    [SerializeField] Transform playerTransform;
    
    [SerializeField] Block BlockPrefab;
    Dictionary<int, Block> blocks = new();
    Dictionary<int, GameObject> chunks = new();
  
    // Start is called before the first frame update
    void Start()
    {
        seed = MainScene.world.seed;
        GenerateTerrain();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateChunks();
    }

    public void OnChange(int index, ItemType type)
    {
        State state = MainScene.world.state.Find(e => e.index == index);
        if(state == null)
        {
            MainScene.world.state.Add(new(type, index));
        }else
        {
            state.type = type;
            state.index = index;
        }
        MainScene.Save();
    }

    void GenerateTerrain()
    {
        for(int i = 0; i < terrainWidth; i++)
        {
            int height = Mathf.RoundToInt(Mathf.PerlinNoise1D((i + seed) * frequency) * terrainHeight / 2) + (terrainHeight / 2);
            for(int j = 0; j < height; j++) {
                if(i == terrainWidth / 2 && j == height - 1)
                {
                    playerTransform.position = new(i, j + 1, -1);
                }
                
                int meta = 0;
                ItemType type = ItemType.STONE;

                if(j > height * 0.75f)
                {
                    float temperature = Mathf.PerlinNoise((i + seed) * frequency * 1.3f, (j + seed) * frequency * 1.3f);
                    type = temperature > 0.5f ? ItemType.SAND : temperature > 0.25f ? ItemType.DIRT : ItemType.SNOW;
                    if (j == height - 1)
                    {
                        float feactureNoise = Mathf.PerlinNoise((i + seed) * frequency * 25f, (j + seed) * frequency * 25f);
    
                        if (feactureNoise > 0.6f)
                        {
                            if (type == ItemType.SAND)
                            {
                                GenerateCactus(i, j);
                            }
                            else if(type == ItemType.DIRT)
                            {
                                CreateTree(i, j);
                                meta = temperature < 0.35f ? 2 : 1;
                            }
                        }
                    }
                }else
                {
                    float coalFrequency = 10f * frequency;
                    float coal = Mathf.PerlinNoise((i + seed) * coalFrequency, (j + seed) * coalFrequency);

                    float ironFrequency = 15f * frequency;
                    float iron = Mathf.PerlinNoise((i + seed) * ironFrequency, (j + seed) * ironFrequency);

                    float goldFrequency = 20f * frequency;
                    float gold = Mathf.PerlinNoise((i + seed) * goldFrequency, (j + seed) * goldFrequency);

                    float diamondFrequency = 25f * frequency;
                    float diamond = Mathf.PerlinNoise((i + seed) * diamondFrequency, (j + seed) * diamondFrequency);

                    if(coal > 0.7f)
                    {
                        type = ItemType.COAL;
                        meta = coal > 0.75 ? 1 : 0;
                    }else if(iron > 0.7f && j < height * 0.5f)
                    {
                        type = ItemType.IRON;
                        meta = iron > 0.75 ? 1 : 0;
                    }
                    else if (gold > 0.7f && j < height * 0.3f)
                    {
                        type = ItemType.GOLD;
                        meta = gold > 0.75 ? 1 : 0;
                    }
                    else if (diamond > 0.8f && j < height * 0.25f)
                    {
                        type = ItemType.DIAMOND;
                        meta = diamond > 0.85 ? 1 : 0;
                    }
                }
                CreateBlock(i, j, type, meta);
            }
        }

        for(int i = 0; i < MainScene.world.state.Count; i++)
        {
            State state = MainScene.world.state[i];
            int x = state.index % terrainWidth;
            int y = state.index / terrainWidth;

            if (blocks.ContainsKey(state.index) && blocks[state.index].type != state.type)
            {
                DestroyBlock(x, y);   
            }

            if (state.type != ItemType.NONE)
            {
                CreateBlock(x, y, state.type);
            }
        }
    }

    public void UpdateChunks()
    {
        foreach (GameObject chunk in chunks.Values)
        {
            int index = Mathf.FloorToInt(playerTransform.position.x / chunkSize);
            index = index < 1 ? 1 : index > chunks.Count - 2 ? chunks.Count - 2 : index;
            int start = index - 1;
            int end = index + 1;
            chunk.SetActive(chunk == chunks[index] || chunk == chunks[start] || chunk == chunks[end]);
        }
    }

    public void CreateBlock(int x, int y, ItemType type, int meta = 0)
    {
        int blockIndex = y * terrainWidth + x;
        if (!blocks.ContainsKey(blockIndex))
        {
            Block obj = Instantiate(BlockPrefab, new(x, y, 0), Quaternion.identity);
            obj.SetType(type, meta);
            blocks[blockIndex] = obj;

            int chunkIndex = Mathf.FloorToInt(x / chunkSize);

            if (chunks.ContainsKey(chunkIndex) == false)
            {
                GameObject chunk = new GameObject();
                chunk.SetActive(false);
                chunk.name = chunkIndex.ToString();
                chunks[chunkIndex] = chunk;
            }
            obj.transform.SetParent(chunks[chunkIndex].transform, false);
        }
    }

    public void PlaceBlock(int x, int y, ItemType type)
    {
        CreateBlock(x, y, type);
        OnChange(y * terrainWidth + x, type);
    } 

    public void DestroyBlock(int x, int y)
    {
        int index = y * terrainWidth + x;
        if(blocks.ContainsKey(index))
        {
            blocks.Remove(index);
            OnChange(index, ItemType.NONE);
        }
    }

    public Block GetBlock(int x, int y)
    {
        int index = y * terrainWidth + x;
        return blocks.ContainsKey(index) ? blocks[index] : null;
    }

    public void CreateTree(int x, int y)
    {
        int height = Mathf.RoundToInt(Mathf.PerlinNoise(x * frequency, y * frequency) * 5);
        for (int i = 0; i < height; i++)
        {
            int meta = i == 0 ? 1 : 0;
            CreateBlock(x, y + i + 1, ItemType.WOOD, meta);

            if(i == height - 1)
            {
                CreateBlock(x, y + i + 2, ItemType.LEAVES);
                CreateBlock(x, y + i + 3, ItemType.LEAVES);
                CreateBlock(x, y + i + 4, ItemType.LEAVES);

                CreateBlock(x - 1, y + i + 2, ItemType.LEAVES);
                CreateBlock(x + 1, y + i + 2, ItemType.LEAVES);

                CreateBlock(x -1, y + i + 3, ItemType.LEAVES);
                CreateBlock(x + 1, y + i + 3, ItemType.LEAVES);

                CreateBlock(x - 1, y + i + 4, ItemType.LEAVES);
                CreateBlock(x + 1, y + i + 4, ItemType.LEAVES);
            }
        }
    }

    public void GenerateCactus(int x, int y)
    {
        int height = Mathf.RoundToInt(Mathf.PerlinNoise(x * frequency, y * frequency) * 5);
        for (int i = 0; i < height; i++)
        {             
            CreateBlock(x, y + i + 1, ItemType.CACTUS);
        }
    }

    static public void Exit() => SceneManager.LoadScene(0);
}
