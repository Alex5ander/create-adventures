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
    void PointerDown(int x, int y)
    {
        if (!inventory.Open)
        {
            float distance = Vector2.Distance(new(x, y), transform.position);
            animator.SetBool("Attack", true);
            if (distance < 4)
            {
                Slot slot = inventory.Slots[inventory.index];
                Item item = slot.item;
                if (item)
                {
                    item.Use(x, y, inventory, terrainGenerator);
                }
            }
        }
        else
        {
            animator.SetBool("Attack", false);
        }
    }

    void PointerUp(int x, int y)
    {
        animator.SetBool("Attack", false);
        Slot slot = inventory.Slots[inventory.index];
        Item item = slot.item;

        if (item)
        {
            item.Use(x, y, inventory, terrainGenerator);
        }
    }
    (int x, int y) ScreenToWorldPoint(Vector2 position)
    {
        Vector3 vector3 = Camera.main.ScreenToWorldPoint(position);
        int x = Mathf.RoundToInt(vector3.x);
        int y = Mathf.RoundToInt(vector3.y);
        return (x, y);
    }
    void HandleMouse()
    {
        (int x, int y) = ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButton(0))
        {
            PointerDown(x, y);
        }
        if (Input.GetMouseButtonUp(0))
        {
            PointerUp(x, y);
        }
    }

    void HandleTouch()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.fingerId == MouseFollower.fingerId)
            {
                (int x, int y) = ScreenToWorldPoint(touch.position);
                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    PointerDown(x, y);
                }
                else
                {
                    PointerUp(x, y);
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
