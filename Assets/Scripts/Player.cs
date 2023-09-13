using UnityEngine;

public class Player : MonoBehaviour
{
    
    CapsuleCollider2D capsuleCollider2D;
    Rigidbody2D body;
    [SerializeField] LayerMask layerMask;
    [SerializeField] Inventory inventory;
    [SerializeField] TerrainGenerator terrainGenerator;
    [SerializeField] ParticleSystem particles;

    float jumpPower = 15.0f;
    float speed = 5.0f;
    float horizontal = 0.0f;
    float clickTime = 0.0f;
    bool pointerDown = false;

    Animator animator;    

    static public Vector2 pointerPos = Vector2.zero;
    [SerializeField] RectTransform TouchArea;

    // Start is called before the first frame update
    void Start()
    {
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        bool isGrounded = IsGrounded();
        if (!MainScene.isMobile)
        {
            horizontal = Input.GetAxis("Horizontal");
            if (Input.GetButtonDown("Jump"))
            {
                Jump();
            }
        }
        
        if (pointerDown)
        {
            GetPointer();
            if (Time.time - clickTime > 0.5f)
            {
                OnPointerDown();
            }
        }

        if(!Block.Selected)
        {
            particles.gameObject.SetActive(false);
        }

        if (horizontal != 0)
        {
            Quaternion rotation = transform.rotation;
            rotation.y = horizontal > 0 ? 180 : 0;
            transform.rotation = rotation;
        }
        animator.SetBool("Walk", horizontal != 0);
        animator.SetBool("Jump", !isGrounded);
    }
    
    public void PointerDown()
    {
        clickTime = Time.time;
        pointerDown = true;
        GetPointer();
        OnPointerDown();
    }

    public void PointerUp()
    {
        clickTime = Time.time;
        animator.SetBool("Attack", false);
        pointerDown = false;
    }

    bool GetPointer()
    {
        if (MainScene.isMobile)
        {
            foreach (Touch touch in Input.touches)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(TouchArea, touch.position))
                {
                    pointerPos = Camera.main.ScreenToWorldPoint(touch.position);
                    pointerPos.y += 4;
                    return true;
                }
            }
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(TouchArea, Input.mousePosition))
        {
            pointerPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return true;
        }
        return false;
    }

    void OnPointerDown()
    {
        Item item = inventory.GetItem();
        int x = Mathf.RoundToInt(pointerPos.x);
        int y = Mathf.RoundToInt(pointerPos.y);
        Block.Selected = terrainGenerator.GetBlock(x, y);
        if (Block.Selected && item?.subtype == ItemSubType.TOOL)
        {
            Mine(item.GetDamage());
        }
        else if (!Block.Selected && item?.subtype == ItemSubType.BLOCK)
        {
            PlaceBlock(x, y, item.type);
        }
        animator.SetBool("Attack", true);
    }

    public void PlaceBlock(int x, int y, ItemType type)
    {
        if (inventory.Remove(type))
        {
            terrainGenerator.PlaceBlock(x, y, type);
        }
    }

    public void Mine(float damage)
    {
        Block.Selected.Hit(damage);
        particles.transform.position = Block.Selected.transform.position;
        particles.GetComponent<Renderer>().material.mainTexture = Block.Selected.GetComponent<SpriteRenderer>().sprite.texture;
        particles.gameObject.SetActive(true);
        if (Block.Selected.GetLife() <= 0)
        {
            int x = Mathf.RoundToInt(Block.Selected.transform.position.x);
            int y = Mathf.RoundToInt(Block.Selected.transform.position.y);
            terrainGenerator.DestroyBlock(x, y);
        }
    }

    private void FixedUpdate()
    {
        body.velocity = new(horizontal * speed, body.velocity.y);
    }

    void Jump()
    {
        if(IsGrounded() && body.velocity.y == 0)
        {
            body.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
        }
    }

    public void OnDrag(float x, float y)
    {
        horizontal = x;
        if(y > 0.5f)
        {
            Jump();
        }
    }

    bool IsGrounded() => Physics2D.CapsuleCast(capsuleCollider2D.bounds.center, capsuleCollider2D.bounds.size, CapsuleDirection2D.Vertical, 0, Vector2.down, 0.1f, layerMask);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Drop drop = collision.GetComponent<Drop>();
        if (drop != null)
        {
            if(inventory.Add(drop.type))
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
