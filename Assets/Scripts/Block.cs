using UnityEngine;

public abstract class Block : MonoBehaviour
{
    public float resistence;
    public float life = 100;
    public Item item;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public abstract void Mining(int x, int y, Inventory inventory, TerrainGenerator terrain);
}