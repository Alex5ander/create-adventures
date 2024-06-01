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
    float speed = 200f;
    float horizontal = 0.0f;
    bool isGrounded = false;
    Animator animator;
    Block selectedBlock;
    [SerializeField] Particles particles;
    float lastSaveTime = 0;
    bool invencible = false;
    [SerializeField] Hand hand;
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
        if (!MainScene.isMobile)
        {
            HandleKeyBoard();
        }
        transform.rotation = horizontal != 0 ? Quaternion.AngleAxis(horizontal > 0 ? 180 : 0, Vector3.up) : transform.rotation;
        gameState.position = transform.position;

        World world = SaveManger.saveGame.GetWorld();
        world.playerPosition = gameState.position;
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
        Vector2 newVelocity = body.velocity;
        newVelocity.x = horizontal * speed * Time.fixedDeltaTime;
        if (!invencible)
        {
            body.velocity = newVelocity;
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
        if (damageable != null && !invencible)
        {
            animator.SetTrigger("Hurt");
            invencible = true;

            gameState.life -= damageable.damage;
            Vector2 forceDirection = -(other.transform.position - transform.position).normalized;
            body.velocity = Vector2.zero;
            body.AddForce(forceDirection * damageable.knockback, ForceMode2D.Impulse);
        }
    }

    bool IsGrounded() => Physics2D.CapsuleCast(capsuleCollider2D.bounds.center, capsuleCollider2D.bounds.size, capsuleCollider2D.direction, 0, Vector2.down, 0.1f, layerMask);

    public void Load(SaveGame saveGame)
    {
        World world = saveGame.GetWorld();
        transform.SetPositionAndRotation(world.playerPosition, world.playerRotation);
    }

    public void SetInvencible()
    {
        invencible = false;
    }
}
