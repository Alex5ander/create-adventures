using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    CapsuleCollider2D capsuleCollider2D;
    Rigidbody2D body;
    [SerializeField] LayerMask layerMask;
    [SerializeField] Inventory inventory;

    float jumpPower = 15.0f;
    float speed = 5.0f;
    float horizontal = 0.0f;
    bool _jump = false;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        bool isGrounded = IsGrounded();
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            _jump = true;
        }
        if(Input.GetMouseButton(0) && !inventory.IsOpen())
        {
            animator.SetBool("Attack", true);
        }else
        {
            animator.SetBool("Attack", false);
        }
        if(horizontal != 0)
        {
            Quaternion rotation = transform.rotation;
            rotation.y = horizontal > 0 ? 180 : 0;
            transform.rotation = rotation;
        }
        animator.SetBool("Walk", horizontal != 0);
        animator.SetBool("Jump", !isGrounded);
    }

    private void FixedUpdate()
    {
        body.velocity = new(horizontal * speed, body.velocity.y);
        if(_jump)
        {
            Jump();
            _jump = false;
        }
    }

    void Jump()
    {
        body.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
    }

    bool IsGrounded() => Physics2D.CapsuleCast(capsuleCollider2D.bounds.center, capsuleCollider2D.bounds.size, CapsuleDirection2D.Vertical, 0, Vector2.down, 0.1f, layerMask);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Drop drop = collision.GetComponent<Drop>();
        if (drop != null)
        {
            if(inventory.Add(drop.type))
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
