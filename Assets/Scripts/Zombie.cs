using UnityEngine;

public class Zombie : MonoBehaviour, IDamageable
{
    [SerializeField]
    Rigidbody2D body;
    [SerializeField]
    Animator animator;
    [SerializeField] LayerMask layerMask;
    BoxCollider2D collider2d;
    [SerializeField] float direction = 0;
    float randomDirectionTime = 0;
    bool isGrounded = false;
    [Header("Stats")]
    float jumpPower = 15f;
    float speed = 100;
    bool invencible = false;
    Transform target;
    public float Health { get; set; }
    public int Defense { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        collider2d = GetComponent<BoxCollider2D>();
        target = FindAnyObjectByType<Player>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        float targetDistance = target.position.x - transform.position.x;
        if (Mathf.Abs(targetDistance) > 1 && Mathf.Abs(targetDistance) < 10)
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

    bool IsGrounded() => Physics2D.BoxCast(collider2d.bounds.center, collider2d.bounds.size, 0, Vector2.down, 0.1f, layerMask);
    float RandomDirection() => Mathf.Sign(Random.Range(-1, 1));
    public void SetInvencible()
    {
        invencible = false;
    }

    public void TakeDamage(float damage)
    {
        float damageAfterDefense = damage - Defense;
        if (damageAfterDefense > 0)
        {
            Health -= damageAfterDefense;
        }
        if (Health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("die");
    }
}
