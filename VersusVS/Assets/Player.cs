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
    float speed = 10;

    [Header("Gravity")]
    float gravity = 2.25f;

    [Header("Collisions")]
    public bool grounded = false;
    public bool wall = false;
    public bool roof = false;
    [SerializeField]
    LayerMask groundCollisionLayer;

    [Header("Jump")]
    public bool jump;
    public bool endJump;
    float jumpSpeed = 15;
    float jumpHeight = 2.25f;
    float jumpTop;
    public bool keepDashSpeed;

    [Header("Dash")]
    public bool dash;
    bool canDash;
    float dashSpeed = 20;
    Vector2 dashDirection = new Vector2();
    float dashTime = 0.3f;
    float dashTimer;
    bool verticalDash;

    [Header("Parry")]
    public GameObject parryObj;
    public bool parry;
    float parryTime = 0.25f;
    float parryTimer;
    public bool afterParry;
    float aParryTime = 0.5f;
    float aParryTimer;

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
    public bool vulnerableState = false;
    public float vulnerableStateTime = 1.0f;
    public float parryPushForce = 1500;
    public float pushForce = 10.0f;
    public float pushCD;
    private IEnumerator corutine;
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
        lastXdir = sprite.flipX ? 1 : -1;

        #region Parry
        if (keyParry)
        {
            if (!prevParry && !parry && !afterParry)
            {
                parry = true;
                parryTimer = 0;

                // Stop everything
                rb.velocity = Vector2.zero;
                dash = false;
                jump = false;
                endJump = false;
                keepDashSpeed = false;
                jumpTop = transform.position.y;
                dir.x = 0;
            }
        }

        sprite.color = Color.white;
        if (parry)
        {
            sprite.color = (Color.red + Color.yellow) / 2;

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
                // Setup max jump height
                jumpTop = transform.position.y + jumpHeight;

                // If was on a dash, keep the horizontal speed
                if (dash)
                {
                    dash = false;
                    keepDashSpeed = true;
                }
                jumpBuffer = false;
            }
        }
        // Jumping
        if (jump)
        {
            // Go up
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);

            // Reached the max jump height
            if (transform.position.y >= jumpTop)
            {
                jump = false;
                endJump = keyJump;
                rb.velocity *= new Vector2(1, 0.75f);
            }

            // Key is not pressed anymore
            if (!keyJump || roof)
            {
                jump = false;
                rb.velocity *= new Vector2(1, 0.5f);
            }
        }
        // Jump reached max height, if key is pressed, lower gravity
        if (endJump)
        {
            if (!keyJump)
            {
                endJump = false;
            }
        }
        #endregion

        #region Dash
        // Dash Event
        if (!prevDash && keyDash && canDash && !dash && !parry && !afterParry && !vulnerableState)
        {
            dash = true;
            canDash = false;
            verticalDash = false;
            dashTimer = 0;
            dashDirection = dir;

            // Dash horizontally if not input dir
            if (dir.x == 0 && dir.y == 0)
            {
                dashDirection.x = sprite.flipX ? 1 : -1;
            }
            // Dash was vertical
            else if (dir.x == 0 && dir.y != 0)
            {
                verticalDash = true;
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

                if (!grounded)
                {
                    keepDashSpeed = true;
                }
            }
        }
        #endregion
        // Normal Movement
        else
        {
            if (!vulnerableState) { 
                float s = keepDashSpeed ? dashSpeed : speed;
                float d = keepDashSpeed && !grounded && !verticalDash ? lastXdir : dir.x;

                if (parry) d = 0;

                rb.velocity = new Vector2(s * d, rb.velocity.y);
            }
        }

        // Apply gravity
        if (!grounded && !jump && !dash && !parry && !vulnerableState)
        {
            if (endJump)
            {
                rb.velocity -= new Vector2(0, gravity/2);
            }
            else
            {
                rb.velocity -= new Vector2(0, gravity);
            }
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

        anim.SetBool("Jump", jump);
        anim.SetBool("Ground", grounded);
        anim.SetBool("Run", dir.x != 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Parry" && dash)
        {
            
            vulnerableState = true;
            collision.transform.parent.GetComponent<Player>().afterParry = false;
            collision.transform.parent.GetComponent<Player>().parry = false;
            EnterVulnerableState(collision.transform.parent.GetComponent<Player>());
            LaunchCharacter(new Vector2(-dir.x, 1), parryPushForce);

        }
    }

    private bool GroundCheck()
    {
        bool prevGround = grounded;

        RaycastHit2D r;
        r = Physics2D.BoxCast(box.bounds.center, box.size * new Vector2(1, 0.5f), 0, Vector2.down, box.size.y/2, groundCollisionLayer);

        bool ground = (r.collider != null && r.collider.tag == "Ground" && r.normal.y > 0);
        
        if (ground)
        {
            if (!dash)
            {
                canDash = true;
            }
            endJump = false;

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
        r = Physics2D.BoxCast(box.bounds.center, box.size * new Vector2(0.5f, 1), 0, Vector2.right*dir.x, box.size.x/2, groundCollisionLayer);
        return (r.collider != null && r.normal.x != dir.x && r.normal.x != 0);
    }

    private bool RoofCheck()
    {
        RaycastHit2D r;
        r = Physics2D.BoxCast(box.bounds.center, box.size * new Vector2(1, 0.5f), 0, Vector2.up, box.size.y/2, groundCollisionLayer);

        return r.collider != null;
    }


    private void LaunchCharacter(Vector2 dir, float force)
    {
        rb.isKinematic = false;
        rb.AddForce(dir * force);
    }

    IEnumerator vulnerableStateCD()
    {
        yield return new WaitForSeconds(vulnerableStateTime);
        rb.isKinematic = false;
        vulnerableState = false;
        
        
    }

    IEnumerator pushCDCorrutine(Player player)
    {

        yield return new WaitForSeconds(pushCD);
        vulnerableState = false;


    }
    public void EnterVulnerableState(Player player)
    {
        vulnerableState = true;
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        dash = false;
        corutine = vulnerableStateCD();
        StartCoroutine(corutine);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Player") { 
            Player controller = collision.gameObject.GetComponent<Player>();
            if (controller.dash) { 
                 if (vulnerableState)
                 {
                    StopCoroutine(corutine);
                    corutine = vulnerableStateCD();
                    StartCoroutine(corutine);
                    rb.velocity = Vector2.zero;
                    LaunchCharacter(controller.dir, pushForce);                    
                 }
                 else
                 {
                    vulnerableState = true;
                    StartCoroutine (pushCDCorrutine(controller));
                    LaunchCharacter(new Vector2(controller.dir.x, -1), pushForce);
                }
            }
        }
    }


}
