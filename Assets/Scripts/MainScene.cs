using System.Runtime.InteropServices;
using UnityEngine;

public class MainScene : MonoBehaviour
{
#if !UNITY_ANDROID
    [DllImport("__Internal")]
    static extern bool IsMobile();
#endif
    static public bool isMobile;
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR && !UNITY_ANDROID
            isMobile = IsMobile();
#endif
#if UNITY_ANDROID
        isMobile = true;
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
}
