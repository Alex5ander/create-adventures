using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetSprite([SerializeField] Sprite sprite) => spriteRenderer.sprite = sprite;
}
