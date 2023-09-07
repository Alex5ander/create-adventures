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
        if(!mobile)
        {
            horizontal = Input.GetAxis("Horizontal");
        }
        bool isGrounded = IsGrounded();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float distance = Vector2.Distance(transform.position, worldPos);
        
        if(!inventory.IsOpen() && distance < 4)
        {
            if (Input.GetMouseButton(0))
            {
                Item item = inventory.GetItem();
                int x = Mathf.RoundToInt(worldPos.x);
                int y = Mathf.RoundToInt(worldPos.y);

                Block block = terrainGenerator.GetBlock(x, y);

                if (block)
                {
                    if (item?.subtype == ItemSubType.TOOL)
                    {
                        Block.Selected = block;
                        particles.transform.position = Block.Selected.transform.position;
                        particles.GetComponent<Renderer>().material.mainTexture = Block.Selected.GetComponent<SpriteRenderer>().sprite.texture;
                        if(particles.isPlaying == false)
                        {
                            particles.Play();
                        }
                        Block.Selected.Hit(item.GetDamage());
                        if (Block.Selected.GetLife() <= 0)
                        {
                            terrainGenerator.DestroyBlock(x, y);
                        }
                    }
                }
                else if(item?.subtype == ItemSubType.BLOCK)
                {
                    if (inventory.Remove(item.type))
                    {
                        terrainGenerator.PlaceBlock(x, y, item.type);
                    }
                }
                animator.SetBool("Attack", true);
            }
        }

        if(Input.GetMouseButtonUp(0))
        {
            animator.SetBool("Attack", false);
        }

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
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

    private void FixedUpdate()
    {
        body.velocity = new(horizontal * speed, body.velocity.y);
    }

    void Jump()
    {
        if(IsGrounded())
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

    public void OnPointerMove(float x, float y)
    {

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
