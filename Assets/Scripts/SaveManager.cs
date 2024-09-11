using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class SaveManger : MonoBehaviour
{
#if !UNITY_ANDROID
    [DllImport("__Internal")]
    private static extern string LoadData();

    [DllImport("__Internal")]
    private static extern string SaveData(string saveGame);
#endif
    [SerializeField] Item[] InitialItems;
    public int worldIndex = 0;
    public List<World> worlds = new();
    public World GetWorld() => worlds[worldIndex];
    static public SaveManger Instance;
    [SerializeField] Inventory inventory;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Load();
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Load()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        JsonUtility.FromJsonOverwrite<SaveManger>(LoadData());
#endif
    }
    public void Save()
    {
        string json = JsonUtility.ToJson(Instance);
#if UNITY_EDITOR
        Debug.Log(json);
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
        SaveData(json);
#endif
    }

    public void NewWorld()
    {
        worlds.Add(new World(UnityEngine.Random.Range(-1000, 1000), InitialItems));
        worldIndex = worlds.Count - 1;
        inventory.Load();
        SceneManager.LoadScene(1);
    }

    public void SelectWorld(int windex)
    {
        worldIndex = windex;
        inventory.Load();
        SceneManager.LoadScene(1);
    }

    public void DeleteWorld(int index)
    {
        worlds.RemoveAt(index);
        Save();
    }

    static public void SaveWorld(Block block, int x, int y)
    {
        World world = Instance.GetWorld();
        ModifiedBlock modifiedBlock = world.modifiedBlocks.Find(e => e.x == x && e.y == y);
        if (modifiedBlock == null)
        {
            world.modifiedBlocks.Add(new(block, x, y));
        }
        else
        {
            modifiedBlock.block = block;
        }
    }
}

[Serializable]
public class ModifiedBlock
{
    public int x;
    public int y;
    public Block block;

    public ModifiedBlock(Block block, int x, int y)
    {
        this.block = block;
        this.x = x;
        this.y = y;
    }
}

[Serializable]
public class World
{
    public int seed = 0;
    public List<Slot> Slots = new();
    public int hotBarIndex = 0;
    public Vector2 playerPosition = new(20, 100);
    public Quaternion playerRotation = Quaternion.identity;
    public List<ModifiedBlock> modifiedBlocks = new();
    public World(int seed, Item[] items)
    {
        this.seed = seed;
        for (int i = 0; i < 36; i++)
        {
            if (i < items.Length)
            {
                Slots.Add(new Slot(items[i], 1));
            }
            else
            {
                Slots.Add(new Slot(null, 0));
            }
        }
    }
}