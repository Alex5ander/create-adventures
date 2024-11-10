using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] TerrainGenerator terrainGenerator;
    [SerializeField] Particles particles;
    [SerializeField] InventoryUI inventoryUI;
    [SerializeField] Inventory inventory;
    [SerializeField] LayerMask layerMask;
    CapsuleCollider2D capsuleCollider2D;
    Rigidbody2D body;
    float jumpPower = 15;
    float speed = 200f;
    float horizontal = 0.0f;
    bool isGrounded = false;
    Animator animator;
    float lastSaveTime = 0;
    bool invencible = false;
    // Start is called before the first frame update
    void Start()
    {
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        World world = SaveManger.Instance.GetWorld();
        transform.SetPositionAndRotation(world.playerPosition, world.playerRotation);
    }

    // Update is called once per frame
    void Update()
    {
        if (MainScene.isMobile)
        {
            HandleTouch();
        }
        else
        {
            HandleMouse();
            HandleKeyBoard();
        }
        transform.rotation = horizontal != 0 ? Quaternion.AngleAxis(horizontal > 0 ? 180 : 0, Vector3.up) : transform.rotation;

        World world = SaveManger.Instance.GetWorld();
        world.playerPosition = transform.position;
        world.playerRotation = transform.rotation;
        if (Time.time - lastSaveTime > 5)
        {
            SaveManger.Instance.Save();
            lastSaveTime = Time.time;
        }
        animator.SetBool("Walk", horizontal != 0);
        animator.SetBool("Jump", !isGrounded);
    }
    private void FixedUpdate()
    {
        isGrounded = IsGrounded();
        Vector2 newVelocity = body.linearVelocity;
        newVelocity.x = horizontal * speed * Time.fixedDeltaTime;
        if (!invencible)
        {
            body.linearVelocity = newVelocity;
        }
    }
    void HandleKeyBoard()
    {
        horizontal = Input.GetAxis("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }
    Block selectedBlock;
    void Mining(Block block, int x, int y)
    {
        if (block != selectedBlock)
        {
            if (block)
            {
                particles.Play(new Vector2(x, y), block.GetComponent<SpriteRenderer>().sprite);
            }
            else
            {
                particles.Stop();
            }

            if (selectedBlock)
            {
                selectedBlock.life = 100;
            }
        }

        selectedBlock = block;

        if (selectedBlock)
        {
            particles.Play(new Vector2(x, y), selectedBlock.GetComponent<SpriteRenderer>().sprite);
            selectedBlock.Mining(ref inventory.Slots[inventoryUI.index].item);
            if (selectedBlock.life <= 0)
            {
                terrainGenerator.RemoveBlock(x, y, true);
                inventoryUI.OnChange(inventoryUI.index);
            }
        }
    }
    void PointerDown(Vector3 position)
    {
        if (!inventory.Open)
        {
            Vector3 vector3 = Camera.main.ScreenToWorldPoint(position);
            int x = Mathf.RoundToInt(vector3.x);
            int y = Mathf.RoundToInt(vector3.y);
            float distance = Vector2.Distance(vector3, transform.position);
            animator.SetBool("Attack", true);
            if (distance < 4)
            {
                Block block = terrainGenerator.GetBlock(x, y);
                Slot slot = inventory.Slots[inventoryUI.index];
                Item item = slot.item;


                if (slot.amount > 0 && item.block && !block)
                {
                    terrainGenerator.CreateBlock(x, y, item.block, true);
                    inventory.Remove(inventoryUI.index, 1);
                }
                else if (block)
                {
                    Mining(block, x, y);
                }
                else { particles.Stop(); }
            }
            else
            {
                particles.Stop();
            }
        }
        else
        {
            if (selectedBlock)
            {
                selectedBlock.life = 100;
            }
            selectedBlock = null;
            animator.SetBool("Attack", false);
            particles.Stop();
        }
    }

    void PointerUp()
    {
        if (selectedBlock)
        {
            selectedBlock.life = 100;
        }
        selectedBlock = null;
        animator.SetBool("Attack", false);
        particles.Stop();
    }
    void HandleMouse()
    {
        if (Input.GetMouseButton(0))
        {
            PointerDown(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0))
        {
            PointerUp();
        }
    }

    void HandleTouch()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.fingerId == MouseFollower.fingerId)
            {
                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    PointerDown(touch.position);
                }
                else
                {
                    PointerUp();
                }
            }
        }
    }

    void Jump()
    {
        if (isGrounded && body.linearVelocity.y == 0)
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

    void OnTriggerStay2D(Collider2D other)
    {
        other.TryGetComponent(out Damageable damageable);
        if (damageable != null && !invencible)
        {
            animator.SetTrigger("Hurt");
            invencible = true;

            Vector2 forceDirection = -(other.transform.position - transform.position).normalized;
            body.linearVelocity = Vector2.zero;
            body.AddForce(forceDirection * damageable.knockback, ForceMode2D.Impulse);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.TryGetComponent(out Drop drop);
        if (drop)
        {
            inventory.Add(drop.item, 1);
            Destroy(drop.gameObject);
        }
    }

    bool IsGrounded() => Physics2D.CapsuleCast(capsuleCollider2D.bounds.center, capsuleCollider2D.bounds.size, capsuleCollider2D.direction, 0, Vector2.down, 0.1f, layerMask);

    public void Load()
    {
        World world = SaveManger.Instance.GetWorld();
        transform.SetPositionAndRotation(world.playerPosition, world.playerRotation);
    }

    public void SetInvencible()
    {
        invencible = false;
    }
}
