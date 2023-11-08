using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Que hace este codigo?
    //Tu padre maricon tiene una polla ah no es tu madre ya la quisiera yo,
    //me dio pena por tu padre el dia que se entero,
    //que fue la noche de bodas quien lo iba a imaginar que iba a ser a tu padre al que iban a empalar ah no encular,
    //transesual transesual
    //Gabriel Garcia Marquez.
    #endregion

    bool keyLeft, keyRight, keyDown, keyUp;
    bool keyJump, keyDash, keyShoot;

    bool releasedJump, releasedDash;

    Rigidbody2D rb; 
    BoxCollider2D box;
    Animator anim; //Te da fuerza de voluntad
    SpriteRenderer sprite;

    int dir = -1;

    float speed = 0; //Droga
    float maxSpeed = 9; //Mucha droga
    float acceleration = 0.70f; //Lo rapido que sube (Soy turbo... y me masturbo)
    float deceleration = 0.95f; //Gatillazo
    float airDeceleration = 0.95f; //Gatillazo aereo

    float gravity = 1.25f; //Newton XD
    bool grounded = false; //No se que hace
    bool wall = false; //Pues un muro
    bool roof = false; //Pues un perro ladrando (Ingles: Ruffy)
    [SerializeField]
    LayerMask groundCollisionLayer;

    bool jumpUp; //España
    float jumpEnd; //Yo ya
    float jumpHeight = 1.25f; //Salntado en altura vertical pa arrbia
    float jumpSpeed = 16.5f;

    bool airDash, groundDash;
    bool canAirDash;
    float dashSpeed = 12;
    float dashTime = 0.35f;
    float dashTimer;
    bool keepDashSpeed; //Que buena esta funcion me gusta

    float spawnAfterimageTime = 0.035f; //Que?
    float spawnAfterimageTimer;

    bool coyote = false; //Una especie de perro
    float coyoteTime = 0.55f;
    float coyoteTimer;

    bool jumpBuffer = false; //Ok
    float jBufferTime = 0.35f;
    float jBufferTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        wall = WallCheck();
        grounded = GroundCheck();
        anim.SetBool("Ground", grounded);
        roof = RoofCheck();

        if (coyote)
        {
            coyoteTimer += Time.fixedDeltaTime;
            if (coyoteTimer > coyoteTime)
            {
                coyote = false;
            }
        }

        if (jumpBuffer)
        {
            releasedJump = true;
            jBufferTimer += Time.fixedDeltaTime;
            if (jBufferTimer > jBufferTime)
            {
                jumpBuffer = false;
                releasedJump = false;
            }
        }

        #region Inputs
        keyUp = Input.GetKey(KeyCode.W);
        keyLeft = Input.GetKey(KeyCode.A);
        keyDown = Input.GetKey(KeyCode.S);
        keyRight = Input.GetKey(KeyCode.D);
        keyJump = Input.GetKey(KeyCode.K);
        keyDash = Input.GetKey(KeyCode.LeftShift);

        if (keyJump)
        {
            if ((grounded || coyote) && releasedJump)
            {
                releasedJump = false;
                grounded = false;

                coyote = false;
                jumpBuffer = false;

                jumpUp = true;
                jumpEnd = transform.position.y + jumpHeight;
                anim.SetBool("Jump", true);

                if (groundDash)
                {
                    groundDash = false;
                    anim.SetBool("Dash", false);
                    dashTimer = dashTime;
                    keepDashSpeed = true;
                }
            }

            if (!grounded && !jumpBuffer && jBufferTimer == 0)
            {
                jumpBuffer = true;
            }
        }
        else
        {
            releasedJump = false;
            jBufferTimer = 0;

            if (grounded)
            {
                releasedJump = true;
            }

            if (jumpUp)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y/2);
            }
            jumpUp = false;
        }

        if (keyDash)
        {
            if (releasedDash)
            {
                releasedDash = false;
                if (grounded)
                {
                    groundDash = true;
                }
                else if (canAirDash)
                {
                    airDash = true;
                    canAirDash = false;
                    keepDashSpeed = true;
                }

                if (groundDash || airDash)
                {
                    dashTimer = 0;
                    spawnAfterimageTimer = 0;
                    anim.SetBool("Dash", true);
                }
            }
        }
        else
        {
            releasedDash = true;
            groundDash = airDash = false;
            anim.SetBool("Dash", false);
            dashTimer = dashTime;
        }
        #endregion

        #region Movement
        if (keyLeft && !keyRight)
        {
            if (!airDash || dir == -1)
            {
                dir = -1;
            }
        }
        if (!keyLeft && keyRight)
        {
            if (!airDash || dir == 1)
            {
                dir = 1;
            }
        }

        if (!keyLeft && !keyRight)
        {
            if (speed > 0)
            {
                if (grounded)
                {
                    speed -= deceleration;
                }
                else
                {
                    speed -= airDeceleration;
                }
            }
            else if (speed < 0)
            {
                speed = 0;
            }
            anim.SetBool("Run", false);
        }
        else
        {
            if (speed < maxSpeed)
            {
                speed += acceleration;
            }
            else if (speed > maxSpeed)
            {
                speed = maxSpeed;
            }
            anim.SetBool("Run", true);
        }
        #endregion

        GetComponent<SpriteRenderer>().flipX = dir == 1;

        #region Jump
        if (roof)
        {
            jumpUp = false;
        }

        if (!jumpUp)
        {
            float extraGravity = rb.velocity.y > 0 ? gravity/2 : 0;
            rb.velocity -= new Vector2(0, gravity /*+ extraGravity*/);
        }
        else
        {
            if (jumpUp)
            {
                if (transform.position.y > jumpEnd)
                {
                    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
                    jumpUp = false;
                }
                else
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
                }
            }
        }
        anim.SetBool("Jump", jumpUp);
        #endregion

        #region Dash
        if (groundDash || airDash)
        {
            speed = dashSpeed;

            if (airDash)
            {
                rb.velocity *= new Vector2(1, 0);
            }

            dashTimer += Time.fixedDeltaTime;
            if (dashTimer >= dashTime)
            {
                groundDash = airDash = false;
                anim.SetBool("Dash", false);
            }
        }
        if (keepDashSpeed && (keyLeft || keyRight))
        {
            speed = dashSpeed;
        }
        if (groundDash || airDash || keepDashSpeed)
        {

            spawnAfterimageTimer += Time.fixedDeltaTime;
            if (spawnAfterimageTimer > spawnAfterimageTime)
            {
                spawnAfterimageTimer = 0;

                GameObject afterImage = new GameObject();
                afterImage.transform.position = transform.position;
                SpriteRenderer spTMP = afterImage.AddComponent<SpriteRenderer>();
                spTMP.sprite = sprite.sprite;
                spTMP.flipX = sprite.flipX;
                spTMP.color = Color.red;
                spTMP.sortingOrder = -1;
                afterImage.AddComponent<DestroyAfter>();
                afterImage.GetComponent<DestroyAfter>().timeToDie = 0.5f;
            }
        }
        #endregion

        if (wall)
        {
            speed = 0; //Ta bueno
        }

        rb.velocity = new Vector2(dir * speed, rb.velocity.y);
    }

    private bool GroundCheck()
    {
        bool prevGround = grounded;

        RaycastHit2D r;
        r = Physics2D.BoxCast(box.bounds.center, new Vector2(box.size.x, box.size.y/2), 0, Vector2.down, 1, groundCollisionLayer);

        bool yeahyeahperdonenkamekamekadespuesdeltemadeltetrisvieneeldragonballrap = (r.collider != null && r.collider.tag == "Ground" && r.normal.y > 0);
        
        if (yeahyeahperdonenkamekamekadespuesdeltemadeltetrisvieneeldragonballrap)
        {
            if (!prevGround)
            {
                anim.SetBool("Jump", false);
                canAirDash = true;
                if (!jumpUp)
                {
                    keepDashSpeed = false;
                }
            }
        }
        else
        {
            if (prevGround)
            {
                coyote = true;
                coyoteTimer = 0;
            }
        }
        return yeahyeahperdonenkamekamekadespuesdeltemadeltetrisvieneeldragonballrap;
    }

    private bool WallCheck()
    {
        RaycastHit2D r;
        r = Physics2D.BoxCast(box.bounds.center, box.size, 0, Vector2.right*dir, 1, groundCollisionLayer);
        return (r.collider != null && r.normal.x != dir && r.normal.x != 0);
    }

    private bool RoofCheck()
    {
        RaycastHit2D r;
        r = Physics2D.BoxCast(box.bounds.center, new Vector2(box.size.x, box.size.y / 2), 0, Vector2.up, 1, groundCollisionLayer);

        return r.collider != null;
    }
}
