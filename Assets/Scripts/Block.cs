using UnityEngine;

public class Block : MonoBehaviour
{
    public ItemType type;
    float life = 5;
    float maxLife = 5;
    SpriteRenderer spriteRenderer;
    public static Block Selected = null;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(type == ItemType.WOOD || type == ItemType.LEAVES || type == ItemType.CACTUS)
        {
            GetComponent<BoxCollider2D>().enabled = false;
        }else if(type == ItemType.WATER)
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Selected != this && life != maxLife)
        {
            life = maxLife;
            spriteRenderer.color = Color.white;
        }
        if(Input.GetMouseButtonUp(0))
        {
            Selected = null;
        }
    }

    public float GetLife() => life;

    public void Hit(float damage)
    {
        life -= damage * Time.deltaTime;
        float color = life / maxLife;
        spriteRenderer.color = new(color, color, color);
    }
}
