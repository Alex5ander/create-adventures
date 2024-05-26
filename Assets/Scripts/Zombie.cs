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
    [SerializeField] float direction = 0;
    float randomDirectionTime = 0;
    bool isGrounded = false;
    [Header("Stats")]
    float jumpPower = 15f;
    float speed = 20;
    float life = 10;
    float invencibleTime = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float targetDistance = Vector2.Distance(transform.position, gameState.position);
        if (targetDistance < 15)
        {
            direction = gameState.position.normalized.x - transform.position.normalized.x;
        }
        else if (Time.time - randomDirectionTime > 4f)
        {
            direction = RandomDirection();
            randomDirectionTime = Time.time;
        }
        transform.rotation = Quaternion.AngleAxis(direction > 0 ? 180 : 0, Vector3.up);
        animator.SetBool("Jump", !isGrounded);
        animator.SetBool("Walk", isGrounded && Mathf.Abs(body.velocity.x) > 0.1f);
    }

    void FixedUpdate()
    {
        isGrounded = IsGrounded();
        if (isGrounded)
        {
            float target = speed * Mathf.Sign(direction);
            float acceleration = target - body.velocity.x;
            float friction = body.velocity.x * 4f;
            body.AddForce((acceleration - friction) * Vector2.right);
            if (Physics2D.OverlapCircle(transform.position + new Vector3(direction * 5f, 0), 0.5f, layerMask))
            {
                body.AddForceY(jumpPower, ForceMode2D.Impulse);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        other.TryGetComponent(out Damageable damageable);
        if (damageable && Time.time - invencibleTime > 1)
        {
            life -= damageable.damage;
            gameState.life -= damageable.damage;
            animator.SetTrigger("Hurt");

            Vector2 forceDirection = -(other.transform.position - transform.position).normalized;
            body.AddForce(forceDirection * damageable.knockback, ForceMode2D.Impulse);
            if (life <= 0)
            {
                Destroy(gameObject);
            }
            invencibleTime = Time.time;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + new Vector3(direction * 5, 0), 0.5f);
    }
    bool IsGrounded() => Physics2D.CapsuleCast(capsuleCollider2D.bounds.center, capsuleCollider2D.size, capsuleCollider2D.direction, 0, Vector2.down, 0.1f, layerMask);
    float RandomDirection() => Mathf.Sign(Random.Range(-1, 0));
}
