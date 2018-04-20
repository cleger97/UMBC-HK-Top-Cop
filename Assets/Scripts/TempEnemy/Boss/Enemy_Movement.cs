using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
    private bool isGrounded = true;
    public int jumpHeight = 400;
    public Rigidbody2D rb2D;
    Boss_attack attackData;
    Boss_Data data;
    GameObject player;
    Player playerData;
    Animator anim;
    private float moveSpeed = 2;
    private float move_left;
    private float move_right;
    private int faceDir = -1; //-1 = left, 1 = right
    private bool isRunAway;
    public Transform groundPoint;
    public float radius =  0.1f;
    public LayerMask groundMask;
    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerData = player.GetComponent<Player>();
        data = GetComponent<Boss_Data>();
        anim = GetComponent<Animator>();
        move_right = transform.position.x + 2;
        move_left = transform.position.x - 2;
        attackData = GetComponent<Boss_attack>();
        rb2D = GetComponent<Rigidbody2D>();
        isRunAway = false;


    }

    // Update is called once per frame
    void Update()
    {
        if (!data.isAlive() || !playerData.IsPlayerAlive())
        {
            anim.SetBool("endGame",true);
            this.enabled = false;
            return;
        }

        if (isRunAway)
        {
            Debug.Log("Run away is called");
            
            runAway();
        }
        else
        {
            /*isGrounded = Physics2D.OverlapCircle(groundPoint.position, radius, groundMask);
            // check in air
            anim.SetBool("Kick", !isGrounded);*/

            if (Mathf.Abs(player.transform.position.x - transform.position.x) <= 3)
            {
                Vector2 toMove;
                if (!attackData.isAttacking)
                {
                    if (player.transform.position.x > transform.position.x)
                    {
                        anim.SetInteger("speed", (int)moveSpeed);
                        toMove = new Vector2(moveSpeed, GetComponent<Rigidbody2D>().velocity.y);
                        transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                    }
                    else
                    {
                        anim.SetInteger("speed", (int)moveSpeed);
                        transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                        toMove = new Vector2(-moveSpeed, GetComponent<Rigidbody2D>().velocity.y);
                    }
                    GetComponent<Rigidbody2D>().velocity = toMove;
                    move_right = transform.position.x + 1;
                    move_left = transform.position.x - 1;
                }
                else
                    anim.SetInteger("speed", (int)1);
            }
            else
            {
               
                Vector2 toMove;
                if ((faceDir == -1) && transform.position.x >= move_left)
                {

                    anim.SetInteger("speed", 1);
                    toMove = new Vector2(-1, GetComponent<Rigidbody2D>().velocity.y);
                    transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                    GetComponent<Rigidbody2D>().velocity = toMove;
                }
                else
                {
                    faceDir = 1;
                    //Jump();
                }
                if ((faceDir == 1) && transform.position.x <= move_right)
                {
                    anim.SetInteger("speed", 1);
                    toMove = new Vector2(1, GetComponent<Rigidbody2D>().velocity.y);
                    transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                    GetComponent<Rigidbody2D>().velocity = toMove;
                }
                else
                    faceDir = -1;

            }
        }
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
            anim.SetInteger("speed", (int)(moveSpeed + 2));
            toMove = new Vector2(moveSpeed, GetComponent<Rigidbody2D>().velocity.y);
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            anim.SetInteger("speed", (int)(moveSpeed + 2));
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            toMove = new Vector2(-moveSpeed, GetComponent<Rigidbody2D>().velocity.y);
        }
        GetComponent<Rigidbody2D>().velocity = toMove;
    }
    public void setRunAway(bool isRun)
    {
        isRunAway = isRun;
    }
}