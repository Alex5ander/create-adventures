using UnityEngine;

public class Zombie : MonoBehaviour
{
    [SerializeField]
    GameState gameState;
    [SerializeField]
    Rigidbody2D body;
    [SerializeField]
    Animator animator;
    [SerializeField] LayerMask layerMask;
    [SerializeField] CapsuleCollider2D capsuleCollider2D;
    public Vector2 direction;
    float jumpPower = 15.0f;
    float speed = 5;
    float attackTime = 0;
    bool isGrounded = false;
    // Start is called before the first frame update
    void Start()
    {
        direction = RandomDirection();
    }

    // Update is called once per frame
    void Update()
    {
        float targetDistance = Vector2.Distance(transform.position, gameState.position);
        if (targetDistance < 10)
        {
            direction = (gameState.position - transform.position).normalized;
        }
        else
        {
            direction = RandomDirection();
        }

        if (targetDistance < 1.5f && Time.time - attackTime > 1)
        {
            animator.SetTrigger("Attack");
            attackTime = Time.time;
            body.velocity = Vector2.zero;
        }

        transform.rotation = Quaternion.AngleAxis(direction.x > 0 ? 180 : 0, Vector3.up);
        animator.SetBool("Jump", !isGrounded);
        animator.SetBool("Walk", Mathf.Abs(body.velocity.x) > 0.2f);
    }

    void FixedUpdate()
    {
        isGrounded = IsGrounded();
        body.velocity = new(direction.x * speed, body.velocity.y);
        if (isGrounded)
        {
            if (Physics2D.OverlapCircle(transform.position, 0.5f, layerMask))
            {
                body.AddForceY(jumpPower, ForceMode2D.Impulse);
            }
        }
    }

    bool IsGrounded() => Physics2D.CapsuleCast(capsuleCollider2D.bounds.center, capsuleCollider2D.size, capsuleCollider2D.direction, 0, Vector2.down, 0.1f, layerMask);
    Vector2 RandomDirection() => new Vector2[] { Vector2.left, Vector2.right }[Random.Range(0, 2)];
}
