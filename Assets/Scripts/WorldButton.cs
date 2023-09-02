using TMPro;
using UnityEngine;

public class WorldButton : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI text;    
    public World world;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnWorldPressed() => MainScene.SelectWorld(world);

    public void OnDeletePressed()
    {
        MainScene.DeleteWorld(world);
        Destroy(gameObject);
    }
}
