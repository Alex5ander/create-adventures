using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    InputAction moveAction;
    InputAction jumpAction;
    [SerializeField] TerrainGenerator terrainGenerator;
    [SerializeField] Particles particles;
    [SerializeField] Inventory inventory;
    [SerializeField] LayerMask layerMask;
    BoxCollider2D collider2d;
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
        collider2d = GetComponent<BoxCollider2D>();
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        World world = SaveManger.Instance.GetWorld();
        transform.SetPositionAndRotation(world.playerPosition, world.playerRotation);

        moveAction = InputSystem.actions.FindAction("Move");
        moveAction.performed += Move;
        moveAction.canceled += Move;

        jumpAction = InputSystem.actions.FindAction("Jump");
        jumpAction.performed += (e) => Jump();
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
        }
        animator.SetBool("Jump", !isGrounded);
        if (Time.time - lastSaveTime > 5)
        {
            SaveManger.Instance.Save();
            lastSaveTime = Time.time;
        }
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

    void Move(InputAction.CallbackContext e)
    {
        Vector2 value = e.ReadValue<Vector2>();
        horizontal = value.x;
        if (value.y > 0)
        {
            Jump();
        }

        transform.rotation = horizontal != 0 ? Quaternion.AngleAxis(horizontal > 0 ? 180 : 0, Vector3.up) : transform.rotation;

        World world = SaveManger.Instance.GetWorld();
        world.playerPosition = transform.position;
        world.playerRotation = transform.rotation;
        animator.SetBool("Walk", horizontal != 0);
    }
    void PointerDown(int x, int y)
    {
        if (!inventory.Open)
        {
            float distance = Vector2.Distance(new(x, y), transform.position);
            if (distance < 4)
            {
                animator.SetBool("Attack", !inventory.Open);
                Slot slot = inventory.Slots[inventory.index];
                Item item = slot.item;
                Solid block = terrainGenerator.GetBlock<Solid>(x, y);
                if (block)
                {
                    particles.Play(new(x, y), block.GetComponent<SpriteRenderer>().sprite);
                }
                else
                {
                    particles.Stop();
                }
                if (item && item is IUsable)
                {
                    (item as IUsable).Use(x, y, inventory, terrainGenerator, true);
                }
            }
            else
            {
                animator.SetBool("Attack", false);
            }
        }
    }

    void PointerUp(int x, int y)
    {
        particles.Stop();
        animator.SetBool("Attack", false);
        Slot slot = inventory.Slots[inventory.index];
        Item item = slot.item;
        if (item && item is IUsable)
        {
            (item as IUsable).Use(x, y, inventory, terrainGenerator);
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
                if (touch.phase == UnityEngine.TouchPhase.Began || touch.phase == UnityEngine.TouchPhase.Moved || touch.phase == UnityEngine.TouchPhase.Stationary)
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

    void OnTriggerStay2D(Collider2D other)
    {
        // other.TryGetComponent(out Damageable damageable);
        // if (damageable != null && !invencible)
        // {
        //     animator.SetTrigger("Hurt");
        //     invencible = true;

        //     Vector2 forceDirection = -(other.transform.position - transform.position).normalized;
        //     body.linearVelocity = Vector2.zero;
        //     body.AddForce(forceDirection * damageable.knockback, ForceMode2D.Impulse);
        // }
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

    bool IsGrounded() => Physics2D.BoxCast(collider2d.bounds.center, collider2d.bounds.size, 0, Vector2.down, 0.1f, layerMask);

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
