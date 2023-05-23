using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    // Deðiþkeni SerializeField olarak iþaretledikten sonra bu Script'i kullanan Gameplay nesnesinde ilgili deðiþkenler de inspector aracýnda görülebilir
    [SerializeField] private float speed;    
    [SerializeField] private float jumpPower;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;



    private void Awake()
    {
        // Referans almak için
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        // Player nesnesi için  yatay girdisi
        horizontalInput = Input.GetAxis("Horizontal");

        // Player nesnesi için sað ve sola dönüp hareket etme
        if (horizontalInput > 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        //
        anim.SetBool("Run", horizontalInput != 0);
        anim.SetBool("Grounded", isGrounded());

        if (wallJumpCooldown > 0.2f)
        {
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            if (onWall() && !isGrounded())
            {
                body.gravityScale = 0;
                body.velocity = Vector2.zero;
            }
            else
            {
                body.gravityScale = 3;
            }

            if (Input.GetKey(KeyCode.Space))
                Jump();
        }
        else
        {
            wallJumpCooldown += Time.deltaTime;
        }
        // WallGround nesnesinden Ground nesnesine indiðinde oluþan eksen (90 derece yan dönme) hatasý çözümü
        transform.rotation = Quaternion.identity;
    }

    private void Jump()
    {
        if (isGrounded())
        {
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            anim.SetTrigger("Jump");
        }
        // Duvarda zýplamaya devam edebilmek için (Yukarý doðru itme kuvveti)
        else if (onWall() && !isGrounded())
        {
            if(horizontalInput == 0)
            {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
                transform.localScale = new Vector3(-Mathf.Sign(transform.position.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);
            }
            wallJumpCooldown = 0;
        
        }


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       
    }

    private bool isGrounded()
    {
        // Oluþturduðumuz sanal çizgi çarpýþtýrýcýya sahip bir nesne ile kesiþirse false döndürücek 
        RaycastHit2D raycasHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        //
        return raycasHit.collider != null;
    }

    private bool onWall()
    {
        RaycastHit2D raycasHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);

        return raycasHit.collider != null;
    }
}
// karakterin merkez noktasý wallun ne kadar içinde kontrolü ile wall'a yapýþýp kalmamasýný saðlama