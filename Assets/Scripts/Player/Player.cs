using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, ISaveManager
{
    [SerializeField] GameState gameState;
    [SerializeField] Inventory inventory;
    [SerializeField] LayerMask layerMask;
    [SerializeField] TerrainGenerator terrainGenerator;
    CapsuleCollider2D capsuleCollider2D;
    Rigidbody2D body;
    float jumpPower = 15;
    float speed = 20f;
    float horizontal = 0.0f;
    bool isGrounded = false;
    Animator animator;
    Dictionary<KeyCode, int> keyCodes = new()
    {
        [KeyCode.Keypad1] = 0,
        [KeyCode.Keypad2] = 1,
        [KeyCode.Keypad3] = 2,
        [KeyCode.Keypad4] = 3,
        [KeyCode.Keypad5] = 4,
        [KeyCode.Keypad6] = 5,
        [KeyCode.Keypad7] = 6,
        [KeyCode.Keypad8] = 7,
        [KeyCode.Keypad9] = 8,
        [KeyCode.Alpha1] = 0,
        [KeyCode.Alpha2] = 1,
        [KeyCode.Alpha3] = 2,
        [KeyCode.Alpha4] = 3,
        [KeyCode.Alpha5] = 4,
        [KeyCode.Alpha6] = 5,
        [KeyCode.Alpha7] = 6,
        [KeyCode.Alpha8] = 7,
        [KeyCode.Alpha9] = 8,
    };
    float lastSaveTime = 0;
    float invencibleTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        gameState.inventory = inventory;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = IsGrounded();
        if (MainScene.isMobile)
        {
            HandleTouch();
        }
        else
        {
            HandleMouse();
        }
        if (!MainScene.isMobile)
        {
            HandleKeyBoard();
        }
        if (horizontal != 0)
        {
            transform.rotation = Quaternion.AngleAxis(horizontal > 0 ? 180 : 0, Vector3.up);
        }
        animator.SetBool("Walk", horizontal != 0);
        animator.SetBool("Jump", !isGrounded);
        gameState.position = transform.position;

        World world = SaveManger.saveGame.GetWorld();
        world.playerPosition = gameState.position;
        world.playerRotation = transform.rotation;
        world.hotBarSelectedIndex = gameState.hotBarSelectedIndex;
        if (Time.time - lastSaveTime > 5)
        {
            SaveManger.Instance.Save();
            lastSaveTime = Time.time;
        }
    }
    private void FixedUpdate()
    {
        if (isGrounded)
        {
            float targetSpeed = speed * horizontal;
            float acceleration = targetSpeed - body.velocity.x;
            float friction = body.velocity.x * 4f;
            body.AddForce((acceleration - friction) * Vector2.right);
        }
    }
    void HandleKeyBoard()
    {
        horizontal = Input.GetAxis("Horizontal");
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        foreach (KeyCode keyCode in keyCodes.Keys)
        {
            if (Input.GetKeyDown(keyCode))
            {
                gameState.hotBarSelectedIndex = keyCodes[keyCode];
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
            Block block = terrainGenerator.GetBlock(x, y);
            gameState.selectedBlock = block;
            animator.SetBool("Attack", true);
            if (distance < 4)
            {
                Item item = inventory.GetByIndex(gameState.hotBarSelectedIndex);
                if (gameState.selectedBlock == null)
                {
                    if (item != null && item.placeable)
                    {
                        terrainGenerator.CreateBlock(x, y, item, 0, true);
                        inventory.Remove(gameState.hotBarSelectedIndex, 1);
                    }
                }
                if (gameState.selectedBlock != null && gameState.selectedBlock.destroy)
                {
                    terrainGenerator.Remove(x, y, true);
                    gameState.selectedBlock = null;
                }
            }
            else
            {
                gameState.selectedBlock = null;
            }
        }
        else
        {
            gameState.selectedBlock = null;
            animator.SetBool("Attack", false);
        }
    }

    void PointerUp()
    {
        gameState.selectedBlock = null;
        animator.SetBool("Attack", false);
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
        if (isGrounded && body.velocity.y == 0)
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
        if (damageable != null && Time.time - invencibleTime > 1)
        {
            gameState.life -= damageable.damage;
            animator.SetTrigger("Hurt");

            Vector2 forceDirection = -(other.transform.position - transform.position).normalized;
            body.AddForce(forceDirection * damageable.knockback, ForceMode2D.Impulse);
            invencibleTime = Time.time;
        }
    }

    bool IsGrounded() => Physics2D.CapsuleCast(capsuleCollider2D.bounds.center, capsuleCollider2D.bounds.size, CapsuleDirection2D.Vertical, 0, Vector2.down, 0.1f, layerMask);

    public void Load(SaveGame saveGame)
    {
        World world = saveGame.GetWorld();
        transform.SetPositionAndRotation(world.playerPosition, world.playerRotation);
        gameState.hotBarSelectedIndex = world.hotBarSelectedIndex;
    }
}
