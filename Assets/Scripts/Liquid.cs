using UnityEngine;
public class Liquid : Block
{
    [SerializeField] Item miningItem;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Mining(ref Item item)
    {
        if (item == miningItem)
        {
            life = 0;
            item = this.item;
        }
    }

    public override void CreateDrop()
    {
        // Destroy(gameObject);
    }
}
