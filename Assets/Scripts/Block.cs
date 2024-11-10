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

    public virtual void Mining(ref Item item)
    {
        if (item)
        {
            life -= item.miningPower;
            if (life < 0)
            {
                CreateDrop();
            }
        }
    }

    public virtual void CreateDrop()
    {
        Drop drop = Instantiate(DropPrefab, transform.position, Quaternion.identity);
        drop.Set(item);
    }
}