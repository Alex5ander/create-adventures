using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
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
