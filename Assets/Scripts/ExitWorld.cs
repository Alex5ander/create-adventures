using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitWorld : MonoBehaviour
{
    public void Exit()
    {
        SaveManger.Instance.Save();
        SceneManager.LoadScene(0);
    }
}
