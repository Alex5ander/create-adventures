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

    Animator animator;

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

        if (MainScene.isMobile)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.fingerId == MouseFollower.fingerId)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        clickTime = Time.time;
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        clickTime = Time.time;
                        Block.Selected = null;
                        animator.SetBool("Attack", false);
                    }
                    else
                    {
                        HandleInput(new(touch.position.x, touch.position.y + Camera.main.scaledPixelHeight / 4f));
                    }
                    break;
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                clickTime = Time.time;
            }
            else if (Input.GetMouseButton(0))
            {
                HandleInput(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                clickTime = Time.time;
                Block.Selected = null;
                animator.SetBool("Attack", false);
            }
        }

        if (!Block.Selected)
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

    public void HandleInput(Vector2 vector)
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(vector);
        int x = Mathf.RoundToInt(pos.x);
        int y = Mathf.RoundToInt(pos.y);

        float distance = Vector2.Distance(transform.position, pos);
        if (distance > 1 && distance < 4 && Time.time - clickTime > 0.3f)
        {
            Item item = inventory.GetItem();
            if (item != null)
            {
                Block.Selected = terrainGenerator.GetBlock(x, y);
                if (item.subtype == ItemSubType.BLOCK && Block.Selected == null)
                {
                    PlaceBlock(x, y, item.type);
                }
                else if (item.subtype == ItemSubType.TOOL && Block.Selected != null)
                {
                    Mine(inventory.GetItem().GetDamage());
                }
                animator.SetBool("Attack", true);
            }
        }
        else
        {
            Block.Selected = null;
            animator.SetBool("Attack", false);
        }
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
        if (IsGrounded() && body.velocity.y == 0)
        {
            body.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
        }
    }

    public void OnDrag(float x, float y)
    {
        horizontal = x;
        if (y > 0.5f)
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
            if (inventory.Add(drop.type))
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
