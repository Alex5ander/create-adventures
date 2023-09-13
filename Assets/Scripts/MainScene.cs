using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class State
{
    public int index = -1;
    public ItemType type = ItemType.NONE;

    public State(ItemType type, int index)
    {
        this.type = type;
        this.index = index;
    }
}

[Serializable]
public class World
{
    public string name = "";
    public int seed = 0;
    public int selected = 0;
    public List<Item> items = new()
    {
        new(0, ItemType.IRONSWORD),
        new(1, ItemType.IRONPICK),
        new(2, ItemType.IRONAXE),
    };
    public List<State> state = new();
}

[Serializable]
public class Database
{
    public List<World> worlds = new();
}

public class MainScene : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern string LoadData();

    [DllImport("__Internal")]
    private static extern string SaveData(string saveGame);

    [DllImport("__Internal")]
    public static extern bool IsMobile();

    static public Database database;
    static public World world;
    static public bool isMobile;

    [SerializeField] GameObject worldButtonsContainer;
    [SerializeField] WorldButton worldButtonPrefab;
    [SerializeField] TextMeshProUGUI worldNameTextInput;    

    string json = null;
    // Start is called before the first frame update
    void Start()
    { 
#if UNITY_WEBGL && !UNITY_EDITOR
            isMobile = MainScene.IsMobile();
#endif
        Load();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Load()
    {
    #if UNITY_WEBGL && !UNITY_EDITOR
        json = LoadData();
        database = JsonUtility.FromJson<Database>(json);
    #endif
        if (database != null)
        {
            for (int i = 0; i < database.worlds.Count; i++)
            {
                World world = database.worlds[i];
                WorldButton gameObject = Instantiate(worldButtonPrefab);
                gameObject.text.text = world.name;
                gameObject.world = world;
                gameObject.transform.SetParent(worldButtonsContainer.transform, false);
            }

            Save();
        }
        else
        {
            database = new();
            Save();
        }
    }

    static public void NewWorldPressed(TMP_InputField input)
    {
        world = new()
        {
            seed = UnityEngine.Random.Range(-100, 100),
            name = string.IsNullOrWhiteSpace(input.text) ? "World #" + UnityEngine.Random.Range(-100, 100) : input.text
        };

        database.worlds.Add(world);
        Save();
        SceneManager.LoadScene(1);
    }

    static public void SelectWorld(World w) {
        world = w;
        SceneManager.LoadScene(1);
    }

    static public void DeleteWorld(World w)
    {
        database.worlds.Remove(w);
        Save();
    }

    static public void Save() {
        string json = JsonUtility.ToJson(database);
#if UNITY_EDITOR
        Debug.Log(json);
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
        SaveData(json);
#endif
    }
}
