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
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // float targetDistance = Vector2.Distance(transform.position, gameState.position);
        // if (targetDistance < 15)
        // {
        //     direction = gameState.position.normalized.x - transform.position.normalized.x;
        // }
        // else if (Time.time - randomDirectionTime > 4f)
        // {
        //     direction = RandomDirection();
        //     randomDirectionTime = Time.time;
        // }
        transform.rotation = Quaternion.AngleAxis(direction > 0 ? 180 : 0, Vector3.up);
        animator.SetBool("Jump", !isGrounded);
        animator.SetBool("Walk", isGrounded && Mathf.Abs(body.velocity.x) > 0.1f);
    }

    void FixedUpdate()
    {
        isGrounded = IsGrounded();
        if (isGrounded && !invencible)
        {
            Vector2 newVelocity = body.velocity;
            newVelocity.x = Mathf.Sign(direction) * speed * Time.deltaTime;
            body.velocity = newVelocity;
            RaycastHit2D raycastHit2D = Physics2D.CircleCast(transform.position, 0.5f, new Vector3(direction, 0), layerMask);
            print(raycastHit2D.collider.gameObject);
            if (raycastHit2D)
            {
                print(isGrounded);
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
            body.velocity = Vector2.zero;
            body.AddForce(forceDirection * damageable.knockback, ForceMode2D.Impulse);
            if (life <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
    bool IsGrounded() => Physics2D.CapsuleCast(capsuleCollider2D.bounds.center, capsuleCollider2D.size, capsuleCollider2D.direction, 0, Vector2.down, 0.1f, layerMask);
    float RandomDirection() => Mathf.Sign(Random.Range(-1, 0));
    public void SetInvencible()
    {
        invencible = false;
    }
}
