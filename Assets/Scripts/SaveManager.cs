using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManger : MonoBehaviour
{
#if !UNITY_ANDROID
    [DllImport("__Internal")]
    private static extern string LoadData();

    [DllImport("__Internal")]
    private static extern string SaveData(string saveGame);
#endif
    string json = null;
    [SerializeField] Item[] InitialItems;
    public SaveGame saveGame;
    static public SaveManger Instance;
    void Awake()
    {
        Instance = this;
        Load();
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
        json = LoadData();
        SaveGame data = JsonUtility.FromJson<SaveGame>(json);
        saveGame = data;
#endif
        saveGame ??= new();
    }
    public void Save()
    {
        string json = JsonUtility.ToJson(saveGame);
#if UNITY_EDITOR
        Debug.Log(json);
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
        SaveData(json);
#endif
    }

    public void NewWorld()
    {
        saveGame.worlds.Add(new World(Random.Range(-1000, 1000), InitialItems));
        saveGame.worldIndex = saveGame.worlds.Count - 1;
        SceneManager.LoadScene(1);
    }

    public void SelectWorld(int windex)
    {
        saveGame.worldIndex = windex;
        SceneManager.LoadScene(1);
    }

    public void DeleteWorld(int index)
    {
        saveGame.worlds.RemoveAt(index);
        Save();
    }

    static public void SaveWorld(Block block, int x, int y)
    {
        World world = Instance.saveGame.GetWorld();
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
