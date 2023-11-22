using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /*public class MyPlayerKeys
    {
        public PlayerKeys.PlayerKeysType playerKeys;

        public KeyCode leftKey;
        public KeyCode rightKey;
        public KeyCode downKey;
        public KeyCode upKey;

        public KeyCode jumpKey;
        public KeyCode dashkey;
        public KeyCode attackKey;

        public MyPlayerKeys()
        {
            playerKeys = PlayerKeys.PlayerKeysType.Player1_Keyboard;
        }

        public MyPlayerKeys(PlayerKeys.PlayerKeysType p)
        {
            playerKeys = p;
        }
    }*/
    //public MyPlayerKeys myKeys;
    public PlayerKeys.PlayerKeysType playerKeysIndex;
    PlayerKeys pKeyScript;

    [Header("Keys")]
    public bool keyLeft;
    public bool keyRight;
    public bool keyDown;
    public bool keyUp;
    public bool keyJump;
    public bool keyDash;
    public bool keyShoot;

    public bool releasedJump;
    public bool releasedDash;

    Rigidbody2D rb;
    BoxCollider2D box;
    Animator anim;
    SpriteRenderer sprite;

    [Header("Direction")]
    public Vector2 dir = new Vector2(1, 0);
    bool noX;

    [Header("Speed")]
    float speed = 0;
    float maxSpeed = 12;
    [Header("Acceleration")]
    float acceleration = 1;
    float deceleration = 2;
    float airDeceleration = 1;

    [Header("Gravity")]
    float gravity = 1.5f;
    [Header("Collisions")]
    public bool grounded = false;
    public bool wall = false;
    public bool roof = false;
    [SerializeField]
    LayerMask groundCollisionLayer;

    [Header("Jump")]
    public bool jumpUp;
    public float jumpEnd;
    float jumpHeight = 1.2f;
    float jumpSpeed = 12;

    [Header("Dash")]
    public bool groundDash;
    public bool airDash;
    public bool canAirDash;
    float dashSpeed = 35;
    float dashTime = 0.1f;
    float dashTimer;
    public bool keepDashSpeed;
    bool verticalDash;
    float diagonalDashMod = 0.6f;

    [Header("Afterimage")]
    public GameObject afterImage;
    float spawnAfterimageTime = 0.0175f;
    float spawnAfterimageTimer;

    [Header("Coyote")]
    public bool coyote = false;
    float coyoteTime = 0.2f;
    float coyoteTimer;

    [Header("Jump Buffer")]
    public bool jumpBuffer = false;
    float jBufferTime = 0.15f;
    float jBufferTimer = 0;

    [Header("Keep Dash Buffer")]
    public bool keepDashBuffer = false;
    float kdBufferTime = 0.1f;
    float kdBufferTimer = 0;

    // Start
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        pKeyScript = new PlayerKeys();
        //myKeys = new MyPlayerKeys(playerKeysIndex);
        //pKeyScript.SetPlayerKeys(ref myKeys);

        // Setup AfterImage Objects
    }

    // Update
    void FixedUpdate()
    {
        // Check collisions
        roof = RoofCheck();
        wall = WallCheck();
        grounded = GroundCheck();
        anim.SetBool("Ground", grounded);

        #region Coyote
        if (coyote)
        {
            coyoteTimer += Time.fixedDeltaTime;
            if (coyoteTimer > coyoteTime)
            {
                coyote = false;
            }
        }
        #endregion
        #region Jump Buffer
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
        #endregion
        #region Keep Dash Buffer
        if (keepDashBuffer)
        {
            kdBufferTimer += Time.fixedDeltaTime;
            if (kdBufferTimer > kdBufferTime)
            {
                keepDashBuffer = false;
            }
        }
        #endregion
        #region Afterimage
        // Spawn Afterimages
        if (groundDash || airDash || keepDashSpeed)
        {
            spawnAfterimageTimer += Time.fixedDeltaTime;
            if (spawnAfterimageTimer >= spawnAfterimageTime)
            {
                spawnAfterimageTimer = 0;

                GameObject a = Instantiate(afterImage, transform.position, transform.rotation, null);
                a.name = "a";
                SpriteRenderer s = a.GetComponent<SpriteRenderer>();
                s.sprite = sprite.sprite;
                s.sortingOrder = -1;
                if (groundDash || airDash)
                {
                    s.color = Color.red;
                }
            }
        }
        #endregion

        /*#region Inputs
        if (myKeys.playerKeys == PlayerKeys.PlayerKeysType.Player1_Keyboard || 
            myKeys.playerKeys == PlayerKeys.PlayerKeysType.Player2_Keyboard)
        {
            // Player 1/2 - Teclado
            keyRight = Input.GetKey(myKeys.rightKey);
            keyLeft = Input.GetKey(myKeys.leftKey);
            keyUp = Input.GetKey(myKeys.upKey);
            keyDown = Input.GetKey(myKeys.downKey);
        }
        else
        {
            // Player 1 - Mando
            if (myKeys.playerKeys == PlayerKeys.PlayerKeysType.Player1_Gamepad)
            {
                keyRight = Input.GetAxis(pKeyScript.g1PadX) >= pKeyScript.minStickValue  || Input.GetAxis(pKeyScript.g1StickX) >= pKeyScript.minStickValue;
                keyLeft =  Input.GetAxis(pKeyScript.g1PadX) <= -pKeyScript.minStickValue || Input.GetAxis(pKeyScript.g1StickX) <= -pKeyScript.minStickValue;
                keyUp =    Input.GetAxis(pKeyScript.g1PadY) >= pKeyScript.minStickValue  || Input.GetAxis(pKeyScript.g1StickY) >= pKeyScript.minStickValue;
                keyDown =  Input.GetAxis(pKeyScript.g1PadY) <= -pKeyScript.minStickValue || Input.GetAxis(pKeyScript.g1StickY) <= -pKeyScript.minStickValue;
            }
            // Player 2 - Mando
            else if (myKeys.playerKeys == PlayerKeys.PlayerKeysType.Player2_Gamepad)
            {
                keyRight = Input.GetAxis(pKeyScript.g2PadX) >= pKeyScript.minStickValue  || Input.GetAxis(pKeyScript.g2StickX) >= pKeyScript.minStickValue;
                keyLeft =  Input.GetAxis(pKeyScript.g2PadX) <= -pKeyScript.minStickValue || Input.GetAxis(pKeyScript.g2StickX) <= -pKeyScript.minStickValue;
                keyUp =    Input.GetAxis(pKeyScript.g2PadY) >= pKeyScript.minStickValue  || Input.GetAxis(pKeyScript.g2StickY) >= pKeyScript.minStickValue;
                keyDown =  Input.GetAxis(pKeyScript.g2PadY) <= -pKeyScript.minStickValue || Input.GetAxis(pKeyScript.g2StickY) <= -pKeyScript.minStickValue;
            }
        }
        keyJump = Input.GetKey(myKeys.jumpKey);
        keyDash = Input.GetKey(myKeys.dashkey);
        #endregion*/

        #region Jump Key
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

                // Mantener velocidad del Dash
                if (groundDash || keepDashBuffer)
                {
                    groundDash = false;
                    anim.SetBool("Dash", false);
                    dashTimer = dashTime;
                    keepDashSpeed = true;
                    rb.velocity *= diagonalDashMod;
                }
            }

            // Salto Buffer
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
        #endregion

        #region Dash Key
        if (keyDash)
        {
            if (releasedDash)
            {
                verticalDash = false;
                if (grounded)
                {
                    groundDash = true;
                }
                else if (canAirDash)
                {
                    airDash = true;
                    canAirDash = false;
                    keepDashSpeed = true;

                    jumpUp = false;
                    releasedJump = false;
                }

                if (groundDash || airDash)
                {
                    dashTimer = 0;
                    anim.SetBool("Dash", true);
                }
                releasedDash = false;
            }
        }
        else
        {
            if (groundDash || airDash)
            {
                keepDashBuffer = true;
                kdBufferTimer = 0;
                rb.velocity *= new Vector2(1, rb.velocity.y > 0 ? diagonalDashMod : 1);
            }

            releasedDash = true;
            groundDash = airDash = false;
            anim.SetBool("Dash", false);
            dashTimer = dashTime;
        }
        #endregion

        #region Movement
        if (keyLeft && !keyRight)
        {
            // Izquierda
            if (!airDash || dir.x == -1)
            {
                dir.x = -1;
            }

            if (noX && !airDash)
            {
                verticalDash = false;
            }
            noX = false;
        }
        else if (!keyLeft && keyRight)
        {
            // Derecha
            if (!airDash || dir.x == 1)
            {
                dir.x = 1;
            }

            if (noX && !airDash)
            {
                verticalDash = false;
            }
            noX = false;
        }
        else
        {
            noX = true;
        }

	    if (!keyUp && keyDown)
        {
            if(!airDash || dir.y == -1)
            {
                dir.y = -1;
            }
        }
        else if (!keyDown && keyUp)
        {
            if (!airDash || dir.y == 1)
            {
                dir.y = 1;
            }
        }
        else
        {
            dir.y = 0;
        }

        // Parao
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
        // Moviendose
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

        // Flip Sprite
        GetComponent<SpriteRenderer>().flipX = dir.x == 1;

        #region Jump
        // Chocao
        if (roof)
        {
            jumpUp = false;
        }

        // Salto parriba
        if (jumpUp)
        {
            if (transform.position.y > jumpEnd)
            {
                jumpUp = false;
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            }
        }
        // Apply Gravity
        else
        {
            rb.velocity -= new Vector2(0, gravity);
        }
        anim.SetBool("Jump", jumpUp);
        #endregion

        #region Dash
        if (groundDash || airDash)
        {
            speed = dashSpeed;

            if (airDash)
            {
                Vector2 d = new Vector2();
                if (dir.y == 0)
                {
                    d = new Vector2(dir.x, 0);
                }
                else
                {
                    if (noX)
                    {
                        d = new Vector2(0, dir.y);
                        verticalDash = true;
                    }
                    else
                    {
                        d = dir*diagonalDashMod;
                    }
                }

                rb.velocity = d * dashSpeed;
            }

            dashTimer += Time.fixedDeltaTime;
            if (dashTimer >= dashTime)
            {
                if (groundDash)
                {
                    keepDashBuffer = true;
                    kdBufferTimer = 0;
                }
                if (airDash)
                {
                    keepDashSpeed = true;
                }

                groundDash = airDash = false;
                anim.SetBool("Dash", false);
                rb.velocity *= new Vector2(1, rb.velocity.y > 0 ? diagonalDashMod : 1);
            }
        }
        
        // Set Dash Speed
        if (keepDashSpeed && (keyLeft || keyRight))
        {
            speed = dashSpeed;
            if (!canAirDash)
            {
                speed *= diagonalDashMod;
            }
        }
        #endregion

        // Parate no
        if (wall)
        {
            speed = 0;
        }

        // Ponle la Velosida
        if (!airDash)
        {
            rb.velocity = new Vector2((verticalDash ? 0 : dir.x) * speed, rb.velocity.y);
        }
    }

    private bool GroundCheck()
    {
        bool prevGround = grounded;

        RaycastHit2D r;
        r = Physics2D.BoxCast(box.bounds.center, box.size * new Vector2(1, 0.5f), 0, Vector2.down, box.size.y/2, groundCollisionLayer);

        bool yeahyeahperdonenkamekamekadespuesdeltemadeltetrisvieneeldragonballrap 
             = (r.collider != null && r.collider.tag == "Ground" && r.normal.y > 0);
        
        if (yeahyeahperdonenkamekamekadespuesdeltemadeltetrisvieneeldragonballrap)
        {
            anim.SetBool("Jump", false);
            if (airDash)
            {
                keepDashBuffer = true;
                kdBufferTimer = 0;
            }
            airDash = false;
            canAirDash = true;
            spawnAfterimageTimer = 0;
            if (prevGround)
            {
                keepDashSpeed = false;
            }
        }
        else
        {
            if (prevGround && !jumpUp)
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
        r = Physics2D.BoxCast(box.bounds.center, box.size, 0, Vector2.right*dir.x, 0, groundCollisionLayer);
        return (r.collider != null && r.normal.x != dir.x && r.normal.x != 0);
    }

    private bool RoofCheck()
    {
        RaycastHit2D r;
        r = Physics2D.BoxCast(box.bounds.center, box.size, 0, Vector2.up, 0, groundCollisionLayer);

        return r.collider != null;
    }
}
