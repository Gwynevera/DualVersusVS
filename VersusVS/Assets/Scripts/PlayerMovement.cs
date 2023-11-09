using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
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

        public MyPlayerKeys()
        {
            playerKeys = PlayerKeys.PlayerKeysType.Player1_Keyboard;
        }
    }
    public MyPlayerKeys myKeys;
    PlayerKeys pKeyScript;

    bool keyLeft, keyRight, keyDown, keyUp;
    bool keyJump, keyDash, keyShoot;

    bool releasedJump, releasedDash;

    Rigidbody2D rb;
    BoxCollider2D box;
    Animator anim;
    SpriteRenderer sprite;

    int dir = -1;

    float speed = 0;
    float maxSpeed = 12;
    float acceleration = 0.50f;
    float deceleration = 0.95f;
    float airDeceleration = 0.95f;

    float gravity = 1;
    bool grounded = false;
    bool wall = false;
    bool roof = false;
    [SerializeField]
    LayerMask groundCollisionLayer;

    bool jumpUp;
    float jumpEnd;
    float jumpHeight = 2;
    float jumpSpeed = 18;

    bool airDash, groundDash;
    bool canAirDash;
    float dashSpeed = 20;
    float dashTime = 0.2f;
    float dashTimer;
    bool keepDashSpeed;

    float spawnAfterimageTime = 0.035f;
    float spawnAfterimageTimer;
    bool keepSpawn = false;
    float keepSpawnTime = 0.25f;
    float keepSpawnTimer = 0;

    bool coyote = false;
    float coyoteTime = 0.2f;
    float coyoteTimer;

    bool jumpBuffer = false;
    float jBufferTime = 0.15f;
    float jBufferTimer = 0;

    bool keepDashBuffer = false;
    float kdBufferTime = 0.25f;
    float kdBufferTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        pKeyScript = new PlayerKeys();
        myKeys = new MyPlayerKeys();
        pKeyScript.SetPlayerKeys(ref myKeys);
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

        if (keepDashBuffer)
        {
            kdBufferTimer += Time.fixedDeltaTime;
            if (kdBufferTimer > kdBufferTime)
            {
                keepDashBuffer = false;
            }
        }

        #region Inputs
        if (myKeys.playerKeys == PlayerKeys.PlayerKeysType.Player1_Keyboard || 
            myKeys.playerKeys == PlayerKeys.PlayerKeysType.Player2_Keyboard)
        {
            keyRight = Input.GetKey(myKeys.rightKey);
            keyLeft = Input.GetKey(myKeys.leftKey);
            keyUp = Input.GetKey(myKeys.upKey);
            keyDown = Input.GetKey(myKeys.downKey);
        }
        else
        {
            if (myKeys.playerKeys == PlayerKeys.PlayerKeysType.Player1_Gamepad)
            {
                keyRight = Input.GetAxis(pKeyScript.g1PadX) >= pKeyScript.minStickValue  || Input.GetAxis(pKeyScript.g1StickX) >= pKeyScript.minStickValue;
                keyLeft =  Input.GetAxis(pKeyScript.g1PadX) <= -pKeyScript.minStickValue || Input.GetAxis(pKeyScript.g1StickX) <= -pKeyScript.minStickValue;
                keyUp =    Input.GetAxis(pKeyScript.g1PadY) >= pKeyScript.minStickValue  || Input.GetAxis(pKeyScript.g1StickY) >= pKeyScript.minStickValue;
                keyDown =  Input.GetAxis(pKeyScript.g1PadY) <= -pKeyScript.minStickValue || Input.GetAxis(pKeyScript.g1StickY) <= -pKeyScript.minStickValue;
            }
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
        #endregion

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

                if (groundDash || keepDashBuffer)
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
        #endregion

        #region Dash Key
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
            if (groundDash)
            {
                keepDashBuffer = true;
                kdBufferTimer = 0;

                keepSpawn = true;
                keepSpawnTimer = 0;
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
                if (groundDash)
                {
                    keepDashBuffer = true;
                    kdBufferTimer = 0;
                }

                groundDash = airDash = false;
                anim.SetBool("Dash", false);

                keepSpawn = true;
                keepSpawnTimer = 0;
            }
        }
        
        if (keepDashSpeed && (keyLeft || keyRight))
        {
            speed = dashSpeed;
        }

        if (keepSpawn)
        {
            keepSpawnTimer += Time.fixedDeltaTime;
            if (keepSpawnTimer > keepSpawnTime)
            {
                keepSpawn = false;
            }
        }

        if (groundDash || airDash || keepDashSpeed || keepSpawn)
        {

            spawnAfterimageTimer += Time.fixedDeltaTime;
            if (spawnAfterimageTimer > spawnAfterimageTime)
            {
                spawnAfterimageTimer = 0;

                GameObject afterImage = new GameObject();
                afterImage.transform.position = transform.position;
                afterImage.AddComponent<SpriteRenderer>();
                afterImage.GetComponent<SpriteRenderer>().sprite = sprite.sprite;
                afterImage.GetComponent<SpriteRenderer>().flipX = sprite.flipX;
                afterImage.GetComponent<SpriteRenderer>().color = Color.red;
                afterImage.GetComponent<SpriteRenderer>().sortingOrder = -1;
                afterImage.AddComponent<DestroyAfter>();
                afterImage.GetComponent<DestroyAfter>().timeToDie = 0.5f;
                afterImage.AddComponent<FadeOut>();
                afterImage.GetComponent<FadeOut>().timeToFade = 0.5f;
                afterImage.GetComponent<FadeOut>().CalculateFadeRate();
            }
        }
        #endregion

        if (wall)
        {
            speed = 0;
        }

        rb.velocity = new Vector2(dir * speed, rb.velocity.y);
    }

    private bool GroundCheck()
    {
        bool prevGround = grounded;

        RaycastHit2D r;
        r = Physics2D.BoxCast(box.bounds.center, box.size, 0, Vector2.down, box.size.y/2, groundCollisionLayer);

        bool yeah = (r.collider != null && r.collider.tag == "Ground" && r.normal.y > 0);
        
        if (yeah)
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
            if (prevGround && !jumpUp)
            {
                coyote = true;
                coyoteTimer = 0;
            }
        }
        return yeah;
    }

    private bool WallCheck()
    {
        RaycastHit2D r;
        r = Physics2D.BoxCast(box.bounds.center, box.size, 0, Vector2.right*dir, box.size.x/2, groundCollisionLayer);
        return (r.collider != null && r.normal.x != dir && r.normal.x != 0);
    }

    private bool RoofCheck()
    {
        RaycastHit2D r;
        r = Physics2D.BoxCast(box.bounds.center, box.size, 0, Vector2.up, box.size.y/2, groundCollisionLayer);

        return r.collider != null;
    }
}
