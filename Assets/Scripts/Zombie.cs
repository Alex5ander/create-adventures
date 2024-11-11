using UnityEngine;

public class Zombie : MonoBehaviour
{
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
    float speed = 100;
    float life = 10;
    bool invencible = false;
    Transform target;
    // Start is called before the first frame update
    void Start()
    {
        target = FindAnyObjectByType<Player>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        float targetDistance = target.position.x - transform.position.x;
        if (Mathf.Abs(targetDistance) < 10)
        {
            direction = target.position.x - transform.position.x > 0 ? 1 : -1;
            randomDirectionTime = Time.time;
        }
        else if (Time.time - randomDirectionTime > 4f)
        {
            direction = RandomDirection();
            randomDirectionTime = Time.time;
        }
        transform.rotation = Quaternion.AngleAxis(direction > 0 ? 180 : 0, Vector3.up);
        animator.SetBool("Jump", !isGrounded);
        animator.SetBool("Walk", isGrounded && Mathf.Abs(body.linearVelocity.x) > 0.1f);
    }

    void FixedUpdate()
    {
        isGrounded = IsGrounded();
        Vector2 newVelocity = body.linearVelocity;
        newVelocity.x = Mathf.Sign(direction) * speed * Time.deltaTime;
        body.linearVelocity = newVelocity;
        if (isGrounded && !invencible)
        {
            RaycastHit2D raycastHit2D = Physics2D.CircleCast(transform.position, 0.5f, new Vector2(direction, 0), 0.1f, layerMask);
            if (raycastHit2D)
            {
                body.AddForceY(jumpPower, ForceMode2D.Impulse);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        other.TryGetComponent(out Damageable damageable);
        if (damageable && !invencible)
        {
            animator.SetTrigger("Hurt");
            invencible = true;
            life -= damageable.damage;

            Vector2 forceDirection = -(other.transform.position - transform.position).normalized;
            body.linearVelocity = Vector2.zero;
            body.AddForce(forceDirection * damageable.knockback, ForceMode2D.Impulse);
            if (life <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
    bool IsGrounded() => Physics2D.CapsuleCast(capsuleCollider2D.bounds.center, capsuleCollider2D.size, capsuleCollider2D.direction, 0, Vector2.down, 0.1f, layerMask);
    float RandomDirection() => Mathf.Sign(Random.Range(-1, 1));
    public void SetInvencible()
    {
        invencible = false;
    }
}
