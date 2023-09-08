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

    Animator animator;
    bool mobile = false;
    bool pointerDown = false;

    static public Vector2 pointerPos = Vector2.zero;
    [SerializeField] RectTransform TouchArea;

    // Start is called before the first frame update
    void Start()
    {
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        #if UNITY_WEBGL && !UNITY_EDITOR
            mobile = MainScene.IsMobile();
        #endif
    }

    // Update is called once per frame
    void Update()
    {
        bool isGrounded = IsGrounded();
        if (!mobile)
        {
            horizontal = Input.GetAxis("Horizontal");
            if (Input.GetButtonDown("Jump"))
            {
                Jump();
            }
        }

        if(pointerDown)
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
            }else if(particles.isPlaying)
            {
                particles.Stop();
            }
            animator.SetBool("Attack", true);
        }else
        {
            if(Block.Selected)
            {
                Block.Selected = null;
            }
            if(animator.GetBool("Attack"))
            {
                animator.SetBool("Attack", false);
            }
            if(particles.isPlaying) 
            {
                particles.Stop();
            }
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
        if (particles.isPlaying == false)
        {
            particles.Play();
        }
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
    
    public void OnPointerDown()
    {
        if (mobile)
        {
            foreach (Touch touch in Input.touches)
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(TouchArea, touch.position, Camera.main, out Vector2 v))
                {
                    pointerDown = true;
                    pointerPos = Camera.main.ScreenToWorldPoint(touch.position);
                    pointerPos.y += 4;
                }
            }
        }
        else
        {
            pointerDown = true;
            pointerPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    public void OnPointerDrag()
    {
        if(mobile)
        {
            foreach(Touch touch in Input.touches)
            {
                if(RectTransformUtility.ScreenPointToLocalPointInRectangle(TouchArea, touch.position, Camera.main, out Vector2 v))
                {
                    pointerPos = Camera.main.ScreenToWorldPoint(touch.position);
                    pointerPos.y += 4;
                }
            }
        }else
        {
            pointerPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    public void OnPointerUp()
    {
        pointerDown = false;
    }

    public void OnPointerExit()
    {
        pointerDown = false;
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
