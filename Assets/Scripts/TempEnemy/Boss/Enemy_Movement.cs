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
    private const int RUN_AW_SPEED = 4;

    private const int RUSH_ATT_SPEED = 5;
    private const int TORN_ATT_SPEED = 4;

    private const float RUN_ATT_MAX_CR = 4f;
    private const float RUN_ATT_MIN_CR = 2f;


    private float left_end_pos;
    private float right_end_pos;
    private int faceDir; //-1 = left, 1 = right
    private const int FACE_RIGHT = 1;
    private const int FACE_LEFT = -1;
    private const float DECTECT_RANGE = 1f;

    private const float TURNING_TIME = 2f;
    private float turningTimer = 0f;
    private bool isTurning = false;
    


    private float detectionCircleRadius = 100;
    private bool isJumping = false;
    private bool isGrounded = true;
    public Transform groundPoint;
    private float groundRadius = 0.1f;
    public LayerMask groundMask;

    public Transform objectDectector;
    private float objectRadius = 0.5f;
    

    private const float RUN_AWAY_DUR = 5f;
    private float runAwayDurTimer;

    private bool hitEdge = false;
    private float leaveEdgeTimer = 0;
    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        
       
        rb2D = GetComponent<Rigidbody2D>();
        runAwayDurTimer = RUN_AWAY_DUR;
        cal_random_idleRange();

        turningTimer = TURNING_TIME;
        currSpeed = SLOW_SPEED;
        faceDir = FACE_LEFT;
        //transform.localScale = new Vector3(faceDir, transform.localScale.y, transform.localScale.z);
    }

    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if (faceDir == FACE_LEFT)
            Debug.Log("FACE_LEFT");
        else
            Debug.Log("FACE_RIGHT");
            */
        if (hitEdge)
        {
            if (leaveEdgeTimer > 0)
                leaveEdgeTimer -= Time.deltaTime;
            else
                hitEdge = false;
        }

    }

    public bool runAway()
    {
        if (runAwayDurTimer <= 0 || hitEdge) 
        {
            return false;
        }
        else
        {
            if (calc_distance() > 0)
                faceDir = FACE_RIGHT;
            else
                faceDir = FACE_LEFT;

            transform.localScale = new Vector3(-faceDir, transform.localScale.y, transform.localScale.z);
            movement(RUN_AW_SPEED);
            cal_random_idleRange();
            runAwayDurTimer -= Time.deltaTime;
            return true;
        }
    }


    public bool inDectectRange()
    {
        return (Mathf.Abs(player.transform.position.x - transform.position.x) <= DECTECT_RANGE);

    }

    public void chasingPlayer()
    {
        
        if (player.transform.position.x > transform.position.x)
        {
            faceDir = FACE_RIGHT;
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            faceDir = FACE_LEFT;
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
         }
        
        movement(CHASE_SPEED);
        cal_random_idleRange();
    }

    private void movement(int speed)
    {
        Vector2 toMove;
        anim.SetInteger("speed", speed);
        if (faceDir == FACE_RIGHT) //Moving right
            toMove = new Vector2(speed, GetComponent<Rigidbody2D>().velocity.y);
        else //Moving left
            toMove = new Vector2(-speed,GetComponent<Rigidbody2D>().velocity.y);

        GetComponent<Rigidbody2D>().velocity = toMove;

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
        
        if ((faceDir == FACE_LEFT))
        {
            if (transform.position.x >= left_end_pos && !isTurning)
            {
                movement(SLOW_SPEED);
            }

            else
            {
                turn_wait(FACE_RIGHT);
            }
        }
        else //face_RIGHT
        {
            if (transform.position.x <= right_end_pos && !isTurning)
            {
                movement(SLOW_SPEED);
            }

            else
            {
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
            //Debug.Log(turningTimer);
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


    private float calc_distance()
    {
        return (transform.position.x - player.transform.position.x);
    }
    public bool rushAtt_prec()
    {
        float distance = Mathf.Abs(calc_distance());
        if (distance < RUN_ATT_MIN_CR || distance > RUN_ATT_MAX_CR)
            return false;
        else
            return true;
    }

    public void rush_att_mov()
    {
        Vector2 toMove;
        int movSpeed = (faceDir == FACE_RIGHT)? RUSH_ATT_SPEED: -RUSH_ATT_SPEED;
        anim.SetInteger("speed", RUSH_ATT_SPEED);
        toMove = new Vector2(movSpeed, GetComponent<Rigidbody2D>().velocity.y);
        GetComponent<Rigidbody2D>().velocity = toMove;
    }
    public void tornado_att_mov()
    {
        Vector2 toMove;
        faceDir = -faceDir;
        transform.localScale = new Vector3(-faceDir, transform.localScale.y, transform.localScale.z);
        int movSpeed = ((calc_distance() > 0)) ? -TORN_ATT_SPEED : TORN_ATT_SPEED;
        toMove = new Vector2(movSpeed, GetComponent<Rigidbody2D>().velocity.y);
        GetComponent<Rigidbody2D>().velocity = toMove;
    }


    public void OntriggerEnter(Collider2D other)
    {
        Debug.Log("In collider");
        if(other.tag == "edge")
        {
            hitEdge = true;
            leaveEdgeTimer = 1.5f;
            change_dir();
        }
    }


    public void change_dir()
    {
        
        faceDir = -faceDir;
        transform.localScale = new Vector3(-faceDir, transform.localScale.y, transform.localScale.z);
    }

    private void cal_random_idleRange()
    {
        float distance = Random.Range(1f, 4f);
        right_end_pos = transform.position.x + distance;
        left_end_pos = transform.position.x - distance;
    }

    public void face_toward()
    {
        anim.SetInteger("speed", SLOW_SPEED);
        float distance = calc_distance();
        if (distance > 0 && faceDir == FACE_RIGHT)
            change_dir();
        else if (distance < 0 && faceDir == FACE_LEFT)
            change_dir();
        
    }
}