using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class MainScene : MonoBehaviour, ISaveManager
{
    [DllImport("__Internal")]
    static extern bool IsMobile();
    static public bool isMobile;
    [SerializeField] VerticalLayoutGroup verticalLayoutGroup;
    [SerializeField] WorldButton ButtonPrefab;
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR && !UNITY_ANDROID
            isMobile = IsMobile();
#endif
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void NewWorld()
    {
        SaveManger.Instance.NewWorld();
    }
    static public void SelectedWorld(int index)
    {
        SaveManger.Instance.SelectWorld(index);
    }

    static public void DeleteWorld(int index)
    {
        SaveManger.Instance.DeleteWorld(index);
    }
    public void Load(SaveGame saveGame)
    {
        for (int i = 0; i < saveGame.worlds.Count; i++)
        {
            WorldButton worldButton = Instantiate(ButtonPrefab);
            worldButton.Init(i);
            worldButton.transform.SetParent(verticalLayoutGroup.transform, false);
        }
    }
}
