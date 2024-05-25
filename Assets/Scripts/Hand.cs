using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] GameState gameState;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Damageable damageable;
    Item selected;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Item item = gameState.inventory.GetByIndex(gameState.hotBarSelectedIndex);
        if (item != null && selected != item)
        {
            transform.localScale = item.placeable ? Vector2.one / 2 : Vector2.one;
            spriteRenderer.sprite = item.dropSprite;
            selected = item;

            damageable.damage = selected.damage;
            damageable.knockback = selected.knockback;
        }
        else if (item == null && selected != null)
        {
            selected = null;
            damageable.damage = 1;
            damageable.knockback = 1;
            spriteRenderer.sprite = null;
        }
    }
}
