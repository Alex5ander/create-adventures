using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] Drop DropPrefab;
    public Item item;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void CreateDrop()
    {
        Destroy(gameObject);
        Drop drop = Instantiate(DropPrefab, transform.position, Quaternion.identity);
        drop.Set(item);
    }
}