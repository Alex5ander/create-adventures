using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] Drop DropPrefab;
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

    public virtual void Mining(int x, int y, Item item, TerrainGenerator terrain)
    {
        life -= item.miningPower;
        if (life < 0)
        {
            Drop drop = Instantiate(DropPrefab, transform.position, Quaternion.identity);
            drop.Set(this.item);
            terrain.RemoveBlock(x, y, true);
        }
    }

    public virtual void Mining(int x, int y, Inventory inventory, TerrainGenerator terrain)
    {

    }
}