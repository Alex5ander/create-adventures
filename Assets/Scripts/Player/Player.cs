using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    InputAction moveAction;
    InputAction clickAction;
    InputAction pointAction;
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

        moveAction = InputSystem.actions.FindAction("Player/Move");
        moveAction.performed += Move;
        moveAction.canceled += Move;

        jumpAction = InputSystem.actions.FindAction("Player/Jump");
        jumpAction.performed += Jump;

        pointAction = InputSystem.actions.FindAction("UI/Point");

        clickAction = InputSystem.actions.FindAction("UI/Click");

        clickAction.performed += Click;
    }

    void OnDisable()
    {
        moveAction.performed -= Move;
        moveAction.canceled -= Move;
        jumpAction.performed -= Jump;
        clickAction.performed -= Click;
    }

    IEnumerator HandlePoint()
    {
        while (clickAction.ReadValue<float>() == 1)
        {
            (int x, int y) = ScreenToWorldPoint(pointAction.ReadValue<Vector2>());
            PointerDown(x, y);
            yield return new WaitForSeconds(0.1f);
        }
        (int _x, int _y) = ScreenToWorldPoint(pointAction.ReadValue<Vector2>());
        PointerUp(_x, _y);
    }

    // Update is called once per frame
    void Update()
    {
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
            Jump(e);
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
                Slot slot = inventory.Slots[inventory.index];
                Item item = slot.item;
                Solid block = terrainGenerator.GetBlock<Solid>(x, y);
                if (block)
                {
                    animator.SetBool("Attack", !inventory.Open);
                    particles.Play(new(x, y), block.GetComponent<SpriteRenderer>().sprite);
                }
                else
                {
                    animator.SetBool("Attack", false);
                    particles.Stop();
                }
                if (item && item is IUsable)
                {
                    animator.SetBool("Attack", !inventory.Open);
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

    void Jump(InputAction.CallbackContext e)
    {
        if (isGrounded && body.linearVelocity.y == 0)
        {
            body.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
        }
    }

    void Click(InputAction.CallbackContext e)
    {
        StartCoroutine(HandlePoint());
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
