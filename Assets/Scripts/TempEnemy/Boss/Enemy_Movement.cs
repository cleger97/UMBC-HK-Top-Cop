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
    private const int RUN_AW_SPEED = 2;

    private float left_end_pos;
    private float right_end_pos;
    private int faceDir; //-1 = left, 1 = right
    private const int FACE_RIGHT = 1;
    private const int FACE_LEFT = -1;
    private const int ILDE_RANGE = 2;
    private const float DECTECT_RANGE = 2;

    private const float TURNING_TIME = 2f;
    private float turningTimer = 0f;
    private bool isTurning = false;
    private bool isRunAway = false;


    private float detectionCircleRadius = 100;
    private bool isJumping = false;
    private bool isGrounded = true;
    public Transform groundPoint;
    private float groundRadius = 0.1f;
    public LayerMask groundMask;

    public Transform objectDectector;
    private float objectRadius = 0.5f;
    


    private const float WALK_AWAY_TIME = 2f;
    private float walkAwayTimer;
    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        right_end_pos = transform.position.x + ILDE_RANGE;
        left_end_pos = transform.position.x - ILDE_RANGE;
        walkAwayTimer = WALK_AWAY_TIME;
        rb2D = GetComponent<Rigidbody2D>();
        
        turningTimer = TURNING_TIME;
        currSpeed = SLOW_SPEED;
    }

    // Update is called once per frame
    void Update()
    {
    
      
    }


 
    public void runAway()
    {
        
        Debug.Log("In runAway here");
        Vector2 toMove;
        if (player.transform.position.x < transform.position.x)
        {
            anim.SetInteger("speed", RUN_AW_SPEED);
            toMove = new Vector2(RUN_AW_SPEED, GetComponent<Rigidbody2D>().velocity.y);
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            faceDir = FACE_LEFT;
        }
        else
        {
            anim.SetInteger("speed", RUN_AW_SPEED);
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            toMove = new Vector2(-RUN_AW_SPEED, GetComponent<Rigidbody2D>().velocity.y);
            faceDir = FACE_RIGHT;
        }
        GetComponent<Rigidbody2D>().velocity = toMove;
        //detectObject();
    }

    public void setRunAway(bool isRun)
    {
        isRunAway = isRun;
    }

    
    public bool inDectectRange()
    {
        return (Mathf.Abs(player.transform.position.x - transform.position.x) <= DECTECT_RANGE);

    }

    public bool chasingPlayer()
    {
        //if (Mathf.Abs(player.transform.position.x - transform.position.x) <= DECTECT_RANGE)
        //{
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
            right_end_pos = transform.position.x + ILDE_RANGE;
            left_end_pos = transform.position.x - ILDE_RANGE;
            return true;
       // }
        //else
        //{
           
        //    return false;
       // }
        //detectObject();
    }

    public bool idleTime()
    {
        if (inDectectRange())
            return false;
  
        idle_mov();
        return true;
    }

    private void idle_mov()
    {
        Vector2 toMove;
        if ((faceDir == FACE_LEFT))
        {
            if (transform.position.x >= left_end_pos && !isTurning)
            {
                anim.SetInteger("speed", SLOW_SPEED);
                toMove = new Vector2(-SLOW_SPEED, GetComponent<Rigidbody2D>().velocity.y);
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                GetComponent<Rigidbody2D>().velocity = toMove;
            }

            else
            {
                //anim.SetBool("isIdle",true);
                turn_wait(FACE_RIGHT);
            }
        }
        else //face_RIGHT
        {
            if (transform.position.x <= right_end_pos && !isTurning)
            {
                anim.SetInteger("speed", SLOW_SPEED);
                toMove = new Vector2(SLOW_SPEED, GetComponent<Rigidbody2D>().velocity.y);
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                GetComponent<Rigidbody2D>().velocity = toMove;
            }

            else
            {
                //anim.SetBool("isIdle", true);
                turn_wait(FACE_LEFT);
            }
        }
    }

    private void turn_wait(int faceDirection)
    {
        anim.SetInteger("speed", 0);
        anim.SetBool("isIdle", true);
       
        isTurning = true;
        if (turningTimer < 0)
        {
            turningTimer = Random.Range(2f,4f);
            Debug.Log(turningTimer);
            faceDir = faceDirection;
            isTurning = false;
            return;
        }
        else if (turningTimer <= TURNING_TIME / 2)
        {
            
            transform.localScale = new Vector3(-faceDirection, transform.localScale.y, transform.localScale.z);

        }
       

        turningTimer -= Time.deltaTime;
    }


    private bool checkIsGrounded()
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
        if (isGrounded && isJumping)
        {
            isGrounded = false;
            rb2D.AddForce(new Vector2(0, jumpForce));
        }
        //isJumping = false;
    }



    /*private void detectObject()
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
    }*/

    public void stop_Mov()
    {
        anim.SetInteger("speed",0);
    }

    public void avoidObstacle()
    {
        rb2D.AddForce(Vector2.up * 300f);
    }


    public bool walk_away()
    {

        if (walkAwayTimer > 0)
        {
            walkAwayTimer -= Time.deltaTime;
            runAway();

            return true;
        }
        else
        {
            walkAwayTimer = WALK_AWAY_TIME;
            return false;
        }
    }
}