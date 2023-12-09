using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

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
    int lastXdir;

    [Header("Speed")]
    float speed = 1;
    float maxSpeed = 10;
    float freno = 0.65f;
    float airFreno = 0.85f;

    float gravity = 5;

    [Header("Collisions")]
    public bool grounded = false;
    public bool wall = false;
    public bool roof = false;
    [SerializeField]
    LayerMask groundCollisionLayer;

    [Header("Jump")]
    public bool jump;
    float jumpForce = 15;
    public bool keepDashSpeed;

    [Header("Dash")]
    public GameObject dashObj;
    public bool dash;
    bool canDash;
    float dashSpeed = 25;
    Vector2 dashDirection = new Vector2();
    float dashTime = 0.2f;
    float dashTimer;

    [Header("Parry")]
    public GameObject parryObj;
    public bool parry;
    float parryTime = 0.25f;
    float parryTimer;
    public bool afterParry;
    float aParryTime = 0.5f;
    float aParryTimer;
    public bool parried;

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
    float parryPushForce = 50;
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

        sprite.color = Color.white;
        if (parry)
        {
            sprite.color = (Color.red + Color.yellow) / 2;
            rb.velocity *= 0.85f;

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
            sprite.color = Color.yellow;

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
            if ((!prevJump || jumpBuffer) && grounded)
            {
                jump = true;

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
        if (jump)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jump = false;
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
                rb.velocity *= 0.25f;

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
                compensate = 3;
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

        rb.gravityScale = grounded || dash /*|| knockback*/ ? 0 : gravity;

        // Turn plus speed
        if (!dash && !knockback)
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
            if (other != null)
            {
                // Te hacen Parry
                if (collision.tag == "Parry")
                {
                    if (dash)
                    {
                        dash = false;
                        jump = false;
                        knockback = true;
                        parried = true;

                        other.parry = false;
                        other.afterParry = false;
                        other.canDash = true;
                    }
                }

                // Te golpea un Dash
                if (collision.tag == "Dash")
                {
                    Debug.Log(this.name + " Hit by Dash " + other.dashDirection);

                    dash = false;
                    jump = false;
                    knockback = true;
                    rb.AddForce(other.dashDirection * parryPushForce, ForceMode2D.Impulse);
                    StartCoroutine(StopKnockBack());

                    other.dash = false;
                    other.canDash = true;
                    other.rb.velocity = Vector2.zero;
                }
            }
        }
        else
        {
            if (other != null)
            {
                if (collision.tag == "Player")
                {
                    /*if (dash)
                    {
                        rb.velocity = Vector2.zero;
                        canDash = true;

                        other.parried = false;
                        other.knockback = false;
                    }*/
                }
            }
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
    }
}
