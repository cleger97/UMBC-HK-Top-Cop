using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
   
    public int jumpForce = 200;
    public Rigidbody2D rb2D;
    GameObject player;

    Animator anim;

    private float currSpeed;
    private const int CHASE_SPEED = 3;
    private const int SLOW_SPEED = 1;


    private float left_end_pos;
    private float right_end_pos;
    private int faceDir; //-1 = left, 1 = right
    private const int FACE_RIGHT = 1;
    private const int FACE_LEFT = -1;
    private const int length = 2;
    private const float detectLength = 4;
    

    private bool isRunAway;
    private float detectionCircleRadius = 100;
    private bool isJumping;
    private bool grounded;
    public Transform groundPoint;
    private float groundRadius = 0.1f;
    public LayerMask groundMask;

    public Transform objectDectector;
    private float objectRadius = 0.5f;
    private float randomJumpTimer;
    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        right_end_pos = transform.position.x + length;
        left_end_pos = transform.position.x - length;
        
        rb2D = GetComponent<Rigidbody2D>();
        isRunAway = false;
        randomJumpTimer = Random.Range(1f, 5f);
        currSpeed = SLOW_SPEED;
    }

    // Update is called once per frame
    void Update()
    {
        isJumping = false;
        grounded = isGrounded();
        if (!grounded)
            currSpeed = 1;

        if (randomJumpTimer > 0)
            randomJumpTimer -= Time.deltaTime;
    }


    /*void Jump()
    {
        anim.SetBool("idle", false);
        rb2D.AddForce(new Vector2(0, jumpHeight));
    }*/

    public void runAway()
    {
        if (!grounded)
            currSpeed = 1;
        Debug.Log("In runAway here");
        Vector2 toMove;
        if (player.transform.position.x < transform.position.x)
        {
            anim.SetInteger("speed", (int)(currSpeed));
            toMove = new Vector2(currSpeed, GetComponent<Rigidbody2D>().velocity.y);
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            faceDir = FACE_LEFT;
        }
        else
        {
            anim.SetInteger("speed", (int)(currSpeed + 3));
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            toMove = new Vector2(-currSpeed, GetComponent<Rigidbody2D>().velocity.y);
            faceDir = FACE_RIGHT;
        }
        GetComponent<Rigidbody2D>().velocity = toMove;
        detectObject();
    }

    public void setRunAway(bool isRun)
    {
        isRunAway = isRun;
    }

    

    public void handleMovement()
    {

        if (!isRunAway)
        {
            currSpeed = CHASE_SPEED;
            chasingPlayer();
        }
        else
        {
            currSpeed = CHASE_SPEED + 1;
            runAway();
        }
    }

    public bool chasingPlayer()
    {
        if (Mathf.Abs(player.transform.position.x - transform.position.x) <= detectLength)
        {
            Vector2 toMove;
            if (player.transform.position.x > transform.position.x)
            {
                faceDir = FACE_RIGHT;
                anim.SetInteger("speed", CHASE_SPEED);
                toMove = new Vector2(CHASE_SPEED, GetComponent<Rigidbody2D>().velocity.y);
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            }
            else
            {
                faceDir = FACE_LEFT;
                anim.SetInteger("speed", CHASE_SPEED);
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                toMove = new Vector2(-CHASE_SPEED, GetComponent<Rigidbody2D>().velocity.y);
            }
            GetComponent<Rigidbody2D>().velocity = toMove;
            right_end_pos = transform.position.x + length;
            left_end_pos = transform.position.x - length;
            return true;
        }
        else
        {
           
            return false;
        }
        //detectObject();
    }

    public bool idleTime()
    {
        if (Mathf.Abs(player.transform.position.x - transform.position.x) <= detectLength)
        {
            
            return false;
        }

        /*if (randomJumpTimer <= 0)
        {
            jump();
            randomJumpTimer = Random.Range(1f, 5f);
         }*/
         Vector2 toMove;
         if ((faceDir == FACE_LEFT) && transform.position.x >= left_end_pos)
        {
            anim.SetInteger("speed", SLOW_SPEED);
            toMove = new Vector2(-SLOW_SPEED, GetComponent<Rigidbody2D>().velocity.y);
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            GetComponent<Rigidbody2D>().velocity = toMove;
        }
        else
        {
            faceDir = FACE_RIGHT;

        }
        if ((faceDir == FACE_RIGHT) && transform.position.x <= right_end_pos)
        {
            anim.SetInteger("speed", SLOW_SPEED);
            toMove = new Vector2(SLOW_SPEED, GetComponent<Rigidbody2D>().velocity.y);
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            GetComponent<Rigidbody2D>().velocity = toMove;
        }
        else
            faceDir = FACE_LEFT;

        return true;
    }

    private bool isGrounded()
    {
        if (rb2D.velocity.y <= 0)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundPoint.position, groundRadius, groundMask);
            for(int i=0;i< colliders.Length;i++)
            {
                if (colliders[i].gameObject != gameObject)
                    return true;
            }
        }
        return false;
    }
    

    private void jump()
    {
        
        isJumping = true;
        if (grounded && isJumping)
        {
            grounded = false;
            rb2D.AddForce(new Vector2(0, jumpForce));
        }
        //isJumping = false;
    }

    private void detectObject()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(objectDectector.position,objectRadius);
        for (int i = 0; i < colliders.Length; i++)
        {
            Debug.Log(colliders[i]);
            if (colliders[i].gameObject.tag == "Enemy" && (colliders[i].gameObject !=gameObject))
            {
                if (!isRunAway)
                {
                    faceDir = -faceDir;
                    right_end_pos = transform.position.x + length;
                    left_end_pos = transform.position.x - length;
                }
                else
                {
                    currSpeed = 0;
                    jump();
                    
                }

                return;
            }
            else if(colliders[i].gameObject.name == "background-v3 foreground")
            {
                Debug.Log("In here");
                faceDir = -1*faceDir;
                isRunAway = false;
            }
        }
    }

    public void stop_Mov()
    {
        anim.SetInteger("speed",0);
    }
}