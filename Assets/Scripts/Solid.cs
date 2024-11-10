using System.Collections;
using UnityEngine;

public class Solid : Block
{
    [SerializeField] Drop DropPrefab;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            isMining = false;
            life = 100;
        }
    }
    bool isMining;
    public IEnumerator MiningCoroutine(int x, int y, Inventory inventory, TerrainGenerator terrain)
    {
        isMining = true;

        while (isMining)
        {
            Item item = inventory.Slots[inventory.index].item;
            life -= item.miningPower;
            if (life < 0)
            {
                isMining = false;
                Drop drop = Instantiate(DropPrefab, transform.position, Quaternion.identity);
                drop.Set(this.item);
                terrain.RemoveBlock(x, y, true);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public override void Mining(int x, int y, Inventory inventory, TerrainGenerator terrain)
    {
        if (!isMining && life == 100)
        {
            StartCoroutine(MiningCoroutine(x, y, inventory, terrain));
        }
    }
}