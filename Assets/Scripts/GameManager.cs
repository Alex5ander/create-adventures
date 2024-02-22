using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public List<Item> items;
    static public GameManager Instance;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        SaveManger.LoadGame();
    }

    // Update is called once per frame
    void Update()
    {

    }
    static public void Exit()
    {
        SaveManger.Instance.Save();
        SceneManager.LoadScene(0);
    }
}
