using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Item> items;
    static public GameManager Instance;

    [DllImport("__Internal")]
    static extern bool IsMobile();
    static public bool isMobile;
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
            isMobile = IsMobile();
#endif
        Instance = this;
        SaveManger.LoadGame();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
