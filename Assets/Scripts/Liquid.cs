using UnityEngine;
public class Liquid : Block
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Mining(int x, int y, Inventory inventory, TerrainGenerator terrain)
    {
        inventory.Set(inventory.index, item, 1);
        terrain.RemoveBlock(x, y, true);
    }
}
