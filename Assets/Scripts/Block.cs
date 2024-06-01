using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] Drop DropPrefab;
    public Item item;
    public List<Item> requiredTools;
    public bool destroy;
    public SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool CanBeMining(Item item) => requiredTools.Contains(item);

    public void Mining(float miningPower)
    {
        spriteRenderer.color -= new Color(miningPower * Time.deltaTime, miningPower * Time.deltaTime, miningPower * Time.deltaTime, 0);
        if (spriteRenderer.color.maxColorComponent <= 0)
        {
            destroy = true;
        }
    }

    public void OnEndMining()
    {
        spriteRenderer.color = Color.white;
    }

    public void CreateDrop()
    {
        Drop drop = Instantiate(DropPrefab, transform.position, Quaternion.identity);
        drop.Set(item);
    }
}