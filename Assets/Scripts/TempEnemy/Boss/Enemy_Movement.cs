using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
   
    public int jumpForce = 200;
    public Rigidbody2D rb2D;
    GameObject player;

    Animator anim;
    private float moveSpeed = 2;

    private float left_end_pos;
    private float right_end_pos;
    private int faceDir; //-1 = left, 1 = right
    private const int FACE_RIGHT = 1;
    private const int FACE_LEFT = -1;
    private const int length = 2;
    private const int slow_moveSpeed = 1;

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

    }

    // Update is called once per frame
    void Update()
    {
        isJumping = false;
        grounded = isGrounded();


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
        Debug.Log("In runAway here");
        Vector2 toMove;
        if (player.transform.position.x < transform.position.x)
        {
            anim.SetInteger("speed", (int)(moveSpeed + 3));
            toMove = new Vector2(moveSpeed, GetComponent<Rigidbody2D>().velocity.y);
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            faceDir = FACE_LEFT;
        }
        else
        {
            anim.SetInteger("speed", (int)(moveSpeed + 3));
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            toMove = new Vector2(-moveSpeed, GetComponent<Rigidbody2D>().velocity.y);
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
            chasingPlayer();
        else
            runAway();
    }

    private void chasingPlayer()
    {
        if (Mathf.Abs(player.transform.position.x - transform.position.x) <= 3)
        {
            Vector2 toMove;
            if (player.transform.position.x > transform.position.x)
            {
                faceDir = FACE_RIGHT;
                anim.SetInteger("speed", (int)moveSpeed);
                toMove = new Vector2(moveSpeed, GetComponent<Rigidbody2D>().velocity.y);
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            }
            else
            {
                faceDir = FACE_LEFT;
                anim.SetInteger("speed", (int)moveSpeed);
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                toMove = new Vector2(-moveSpeed, GetComponent<Rigidbody2D>().velocity.y);
             }
            GetComponent<Rigidbody2D>().velocity = toMove;
            right_end_pos = transform.position.x + length;
            left_end_pos = transform.position.x - length;
           
        }
        idleTime();
        detectObject();
    }

    private void idleTime()
    {
        
            if (randomJumpTimer <= 0)
            {
                jump();
                randomJumpTimer = Random.Range(1f, 5f);
            }
            Vector2 toMove;
            if ((faceDir == FACE_LEFT) && transform.position.x >= left_end_pos)
            {

                anim.SetInteger("speed", slow_moveSpeed);
                toMove = new Vector2(-1, GetComponent<Rigidbody2D>().velocity.y);
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                GetComponent<Rigidbody2D>().velocity = toMove;
            }
            else
            {
                faceDir = FACE_RIGHT;

            }
            if ((faceDir == FACE_RIGHT) && transform.position.x <= right_end_pos)
            {
                anim.SetInteger("speed", slow_moveSpeed);
                toMove = new Vector2(1, GetComponent<Rigidbody2D>().velocity.y);
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                GetComponent<Rigidbody2D>().velocity = toMove;
            }
            else
                faceDir = FACE_LEFT;
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
        isJumping = false;
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
                    jumpForce = 400;
                    jump();
                    jumpForce = 200;
                }

                return;
            }
            else if(colliders[i].gameObject.name == "background-v3 foreground")
            {
                Debug.Log("In here");
                faceDir = -faceDir;
                isRunAway = false;
            }
        }
    }
}