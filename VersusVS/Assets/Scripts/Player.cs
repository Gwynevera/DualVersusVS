using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class Player : MonoBehaviour
{
    public class MyPlayerKeys
    {
        public PlayerKeys.PlayerKeysType playerKeys;

        public KeyCode leftKey;
        public KeyCode rightKey;
        public KeyCode downKey;
        public KeyCode upKey;

        public KeyCode jumpKey;
        public KeyCode dashkey;
        public KeyCode attackKey;
        public KeyCode parryKey;

        public MyPlayerKeys()
        {
            playerKeys = PlayerKeys.PlayerKeysType.Player1_Keyboard;
        }

        public MyPlayerKeys(PlayerKeys.PlayerKeysType p)
        {
            playerKeys = p;
        }
    }
    public MyPlayerKeys myKeys;
    public PlayerKeys.PlayerKeysType playerKeysIndex;
    PlayerKeys pKeyScript;

    [Header("Keys")]
    public bool keyLeft;
    public bool keyRight;
    public bool keyDown;
    public bool keyUp;
    public bool keyJump;
    public bool keyDash;
    public bool keyAttack;
    public bool keyParry;

    public bool prevJump;
    public bool prevDash;
    public bool prevParry;

    Rigidbody2D rb;
    BoxCollider2D box;
    Animator anim;
    SpriteRenderer sprite;

    [Header("Direction")]
    public Vector2 dir = new Vector2();
    public int lastXdir;

    [Header("Speed")]
    float speed = 1;
    float maxSpeed = 10;
    float freno = 0.65f;
    float airFreno = 0.85f;

    float gravity = 5;
    float low_gravity = 1;

    int turnCompensationPlus = 3;

    [Header("Collisions")]
    public bool grounded = false;
    public bool wall = false;
    public bool roof = false;
    [SerializeField]
    LayerMask groundCollisionLayer;

    [Header("Jump")]
    public bool jump;
    public bool doubleJump;
    public bool canDoubleJump;
    float jumpForce = 15;
    public bool keepDashSpeed;

    [Header("Dash")]
    public GameObject dashObj;
    public bool dash;
    public bool clash;
    bool canDash;
    float dashSpeed = 25;
    Vector2 dashDirection = new Vector2();
    float dashTime = 0.2f;
    float dashTimer;
    float endDashSlowMult = 0.25f;

    [Header("Parry")]
    public GameObject parryObj;
    public bool parry;
    float parryTime = 0.25f;
    float parryTimer;
    public bool afterParry;
    float aParryTime = 0.5f;
    float aParryTimer;
    public bool parried;
    float parrySlowMult = 0.75f;

    [Header("Afterimage")]
    public GameObject afterImage;
    bool spawnAfterImage;
    float spawnAfterimageTime = 0.0175f;
    float spawnAfterimageTimer;

    [Header("Coyote")]
    public bool coyote = false;
    float coyoteTime = 0.25f;
    float coyoteTimer;

    [Header("Jump Buffer")]
    public bool jumpBuffer = false;
    float jBufferTime = 0.25f;
    float jBufferTimer = 0;

    [Header("Keep Dash Buffer")]
    public bool keepDashBuffer = false;
    float kdBufferTime = 0.25f;
    float kdBufferTimer = 0;

    [Header("Vulnerable state")]
    public bool knockback = false;

    float knockbackTime = 0.45f;
    float smallKnockBackTime = 0.1f;
    float bigKnockBackTime = 0.8f;

    float upForceParryKnockbackTime = 0.4f;

    float superPushForce = 30;
    float dashPushForce = 20;
    float clashPushForce = 10;
    float parryPushForce = 7.5f;

    // Start
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        pKeyScript = new PlayerKeys();
        myKeys = new MyPlayerKeys(playerKeysIndex);
        pKeyScript.SetPlayerKeys(ref myKeys);
    }

    // Update
    void FixedUpdate()
    {
        // Check collisions
        roof = RoofCheck();
        grounded = GroundCheck();
        anim.SetBool("Ground", grounded);

        #region Coyote
        // Can jump briefly after leaving the ground
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
        // Can jump if press the button a bit after touching ground
        if (jumpBuffer)
        {
            jBufferTimer += Time.fixedDeltaTime;
            if (jBufferTimer > jBufferTime)
            {
                jumpBuffer = false;
            }
        }
        #endregion
        #region Keep Dash Buffer
        // Can keep dash speed after jumping even a bit after dash ends
        if (keepDashBuffer)
        {
            kdBufferTimer += Time.fixedDeltaTime;
            if (kdBufferTimer > kdBufferTime)
            {
                keepDashBuffer = false;
            }
        }
        #endregion
        
        #region Inputs
        prevDash = keyDash;
        prevJump = keyJump;
        prevParry = keyParry;

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
                keyUp =    Input.GetAxis(pKeyScript.g1PadY) >= pKeyScript.minStickValue  || Input.GetAxis(pKeyScript.g1StickY) <= -pKeyScript.minStickValue;
                keyDown =  Input.GetAxis(pKeyScript.g1PadY) <= -pKeyScript.minStickValue || Input.GetAxis(pKeyScript.g1StickY) >= pKeyScript.minStickValue;
            }
            // Player 2 - Mando
            else if (myKeys.playerKeys == PlayerKeys.PlayerKeysType.Player2_Gamepad)
            {
                keyRight = Input.GetAxis(pKeyScript.g2PadX) >= pKeyScript.minStickValue  || Input.GetAxis(pKeyScript.g2StickX) >= pKeyScript.minStickValue;
                keyLeft =  Input.GetAxis(pKeyScript.g2PadX) <= -pKeyScript.minStickValue || Input.GetAxis(pKeyScript.g2StickX) <= -pKeyScript.minStickValue;
                keyUp =    Input.GetAxis(pKeyScript.g2PadY) >= pKeyScript.minStickValue  || Input.GetAxis(pKeyScript.g2StickY) <= -pKeyScript.minStickValue;
                keyDown =  Input.GetAxis(pKeyScript.g2PadY) <= -pKeyScript.minStickValue || Input.GetAxis(pKeyScript.g2StickY) >= pKeyScript.minStickValue;
            }
        }
        keyJump = Input.GetKey(myKeys.jumpKey);
        keyDash = Input.GetKey(myKeys.dashkey);
        keyParry = Input.GetKey(myKeys.parryKey);
        #endregion

        // Left & Right
        if (keyLeft && !keyRight)
        {
            dir.x = -1;
        }
        else if (!keyLeft && keyRight)
        {
            dir.x = 1;
        }
        else
        {
            dir.x = 0;
        }

        wall = WallCheck();
        if (wall)
        {
            // Stop player
            dir.x = 0;
        }

        // Up & Down
        if (keyUp && !keyDown)
        {
            dir.y = 1;
        }
        else if (!keyUp && keyDown)
        {
            dir.y = -1;
        }
        else
        {
            dir.y = 0;
        }
        
        // Sprite flip
        if (dir.x == 1)
        {
            sprite.flipX = true;
        }
        else if (dir.x == -1)
        {
            sprite.flipX = false;
        }
        else
        {
            // Slow down
            if (!keepDashSpeed && !knockback)
            {
                rb.velocity *= new Vector2(grounded ? freno : airFreno, 1);
            }
        }
        lastXdir = sprite.flipX ? 1 : -1;

        #region Parry
        if (keyParry)
        {
            if (!prevParry && !parry && !afterParry)
            {
                parry = true;
                parryTimer = 0;

                // Stop everything
                dash = false;
                jump = false;
                keepDashSpeed = false;
                dir.x = 0;
            }
        }

        sprite.color = Color.white; ///
        if (parry)
        {
            sprite.color = (Color.red + Color.yellow) / 2; ///
            rb.velocity *= parrySlowMult;

            parryTimer += Time.fixedDeltaTime;
            if (parryTimer > parryTime)
            {
                parry = false;
                afterParry = true;
                aParryTimer = 0;
            }
        }
        
        parryObj.SetActive(parry);

        if (afterParry)
        {
            sprite.color = Color.yellow; ///

            aParryTimer += Time.fixedDeltaTime;
            if (aParryTimer > aParryTime)
            {
                afterParry = false;
            }
        }
        #endregion

        #region Jump
        if (keyJump)
        {
            // Just pressed jump, not in the ground yet
            if (!prevJump && !grounded && !parry)
            {
                jumpBuffer = true;
                jBufferTimer = 0;
            }

            // Jump event
            if ((!prevJump || jumpBuffer) && (grounded || canDoubleJump))
            {
                jump = true;
                if (!grounded && canDoubleJump)
                {
                    jump = false;
                    doubleJump = true;
                    canDoubleJump = false;
                }

                // If was on a dash, keep the horizontal speed
                if (dash || keepDashBuffer)
                {
                    dash = false;
                    keepDashBuffer = false;
                    keepDashSpeed = true;
                }
                jumpBuffer = false;
            }
        }
        
        // Jumping
        if (jump || doubleJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jump = false;
            doubleJump = false;
        }
        #endregion

        #region Dash
        // Dash Event
        if (!prevDash && keyDash && canDash && !dash && !parry && !afterParry && !knockback)
        {
            dash = true;
            canDash = false;
            dashTimer = 0;
            dashDirection = dir;

            // Dash horizontally if not input dir
            if (dir.x == 0 && dir.y == 0)
            {
                dashDirection.x = sprite.flipX ? 1 : -1;
            }

            if (dashDirection.x != 0 && dashDirection.y != 0)
            {
                dashDirection = dashDirection.normalized;
            }

            jump = false;
        }

        // Dashing
        if (dash)
        {
            rb.velocity = dashSpeed * dashDirection;

            dashTimer += Time.fixedDeltaTime;
            if (dashTimer >= dashTime)
            {
                dash = false;
                rb.velocity *= endDashSlowMult;

                if (!grounded)
                {
                    keepDashSpeed = true;
                }
            }

            sprite.color = Color.red;
        }
        dashObj.SetActive(dash);
        #endregion

        // Normal Movement
        if (!dash && !knockback)
        {
            int compensate = 1;
            if (dir.x == 1 && rb.velocity.x < 0
                || dir.x == -1 && rb.velocity.x > 0)
            {
                compensate = turnCompensationPlus;
            }

            rb.AddForce(speed * compensate * new Vector2(dir.x, 0), ForceMode2D.Impulse);
        }

        #region Afterimage
        // Spawn Afterimages
        spawnAfterImage = dash || keepDashSpeed;
        if (spawnAfterImage)
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
                if (dash)
                {
                    s.color = Color.red;
                }
            }
        }
        #endregion

        // Gravity
        if (grounded || dash)
        {
            rb.gravityScale = 0;
        }
        else if (parried)
        {
            rb.gravityScale = low_gravity;
        }
        else
        {
            rb.gravityScale = gravity;
        }

        // Limit speed
        if (!dash /*&& !keepDashSpeed*/ && !knockback)
        {
            if (Mathf.Abs(rb.velocity.x) > maxSpeed)
            {
                rb.velocity = new Vector2(dir.x*maxSpeed, rb.velocity.y);
            }
        }

        anim.SetBool("Jump", jump);
        anim.SetBool("Ground", grounded);
        anim.SetBool("Run", dir.x != 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player other = collision.gameObject.GetComponentInParent<Player>();

        if (collision.isTrigger)
        {
            if (other != null && !knockback)
            {
                // Te hacen Parry
                if (collision.tag == "Parry")
                {
                    if (dash)
                    {
                        // Parreado y mini lanzado patras
                        dash = false;
                        jump = false;
                        doubleJump = false;
                        knockback = true;
                        parried = true;
                        rb.velocity = Vector2.zero;
                        rb.AddForce(new Vector2(-lastXdir, 1.25f) * parryPushForce, ForceMode2D.Impulse);
                        StartCoroutine(StopKnockBack());

                        // Recuperar dash y doble salto
                        other.parry = false;
                        other.afterParry = false;
                        other.canDash = true;
                        other.canDoubleJump = true;
                        other.doubleJump = false;
                    }
                }

                // Te golpea un Dash
                if (collision.tag == "Dash" && !parry)
                {
                    Debug.Log(this.name + " Hit by Dash " + other.dashDirection);

                    clash = (dash && other.dash) || other.clash;

                    dash = false;
                    jump = false;
                    doubleJump = false;
                    knockback = true;
                    rb.velocity = Vector2.zero;

                    Vector2 dashPushDir = other.dashDirection;
                    // Downwards vertical dash on grounded opponent
                    if (grounded && dashPushDir.x != 0)
                    {
                        dashPushDir.y = 1;
                    }

                    if (clash)
                    {
                        Debug.Log(this.name + " CLASH ");

                        canDash = true;

                        // Push Player in dash opposite direction
                        rb.AddForce(new Vector2(-lastXdir , 1) * clashPushForce, ForceMode2D.Impulse);
                        StartCoroutine(StopSmallKnockBack());
                    }
                    else
                    {
                        // Push Player in received dash direction
                        if (parried)
                        {
                            rb.AddForce(dashPushDir * superPushForce, ForceMode2D.Impulse);
                            StartCoroutine(StopBigKnockBack());
                        }
                        else
                        {
                            rb.AddForce(dashPushDir * dashPushForce, ForceMode2D.Impulse);
                            StartCoroutine(StopKnockBack());
                        }
                    }

                    if (!clash)
                    {
                        other.rb.velocity = Vector2.zero;
                        other.dash = false;
                        other.canDash = true;
                    }
                }
            }
        }
        else
        {
            /*if (other != null)
            {
                if (collision.tag == "Player")
                {
                    if (dash)
                    {
                        rb.velocity = Vector2.zero;
                        canDash = true;

                        other.parried = false;
                        other.knockback = false;
                    }
                }
            }*/
        }
    }

    private bool GroundCheck()
    {
        bool prevGround = grounded;

        RaycastHit2D r;
        r = Physics2D.BoxCast(box.bounds.center, box.size * new Vector2(1, 0.5f), 0, Vector2.down, box.size.y/3.25f, groundCollisionLayer);

        bool ground = (r.collider != null && r.collider.tag == "Ground" && r.normal.y > 0);
        
        if (ground)
        {
            if (!dash)
            {
                canDash = true;
            }
            canDoubleJump = true;

            // Just touched the ground
            if (!prevGround)
            {
                if (!jump)
                {
                    keepDashSpeed = false;
                }
            }
        }
        else
        {
            // Just left the ground
            if (prevGround)
            {
                if (!jump)
                {
                    coyote = true;
                    coyoteTimer = 0;
                }
            }
        }
        return ground;
    }

    private bool WallCheck()
    {
        RaycastHit2D r;
        r = Physics2D.BoxCast(box.bounds.center, box.size, 0, Vector2.right*dir.x, box.size.x, groundCollisionLayer);
        return (r.collider != null && r.normal.x != dir.x && r.normal.x != 0);
    }

    private bool RoofCheck()
    {
        RaycastHit2D r;
        r = Physics2D.BoxCast(box.bounds.center, box.size, 0, Vector2.up, box.size.y, groundCollisionLayer);

        return r.collider != null;
    }

    IEnumerator StopKnockBack()
    {
        yield return new WaitForSeconds(knockbackTime);
        knockback = false;
        parried = false;
        clash = false;
        Debug.Log("Recovered (N) " + this.name);
    }
    IEnumerator StopSmallKnockBack()
    {
        yield return new WaitForSeconds(smallKnockBackTime);
        knockback = false;
        parried = false;
        clash = false;
        Debug.Log("Recovered (S) " + this.name);
    }
    IEnumerator StopBigKnockBack()
    {
        yield return new WaitForSeconds(bigKnockBackTime);
        knockback = false;
        parried = false;
        clash = false;
        Debug.Log("Recovered (B) " + this.name);
    }

    IEnumerator StopLowGravityAfterParry()
    {
        yield return new WaitForSeconds(upForceParryKnockbackTime);
        
    }
}
