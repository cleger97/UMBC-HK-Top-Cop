using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_attack : MonoBehaviour {
    Boss_Data data;
    Animator anim;
    private float timer;
    GameObject player;
    Player playerData;

    public float detectionCircleRadius;
    public float attackCircleRadius;
    private float attkCoolDown;
    private int damage;
    public Transform dmgArea;

    public bool isAttacking;
    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        playerData = player.GetComponent<Player>();
        data = GetComponent<Boss_Data>();
        anim = GetComponent<Animator>();
        timer = 0f;
        isAttacking = false;
        damage = 20;
        attkCoolDown = 1f;
    }
	
	// Update is called once per frame
	void Update () {
        if (!data.isAlive() || !playerData.IsPlayerAlive())
        {
            anim.SetBool("endGame", true);
            this.enabled = false;
            return;
        }
        isAttacking = false;

        anim.SetBool("isAttack", false);
        if (timer > 0)
            timer -= Time.deltaTime;
   
        attackCheck();
       
        
    }

    public void attackCheck()
    {

        
        GameObject[] check = Enemy.colliderTagSorter("Player", Enemy.getAllAround(detectionCircleRadius, transform));
        if (check.Length > 0)
        {
            //anim.SetBool("Attack", false);
            GameObject player = check[0];
            GameObject[] checkInAttack = Enemy.colliderTagSorter("Player", Enemy.getAllAround(attackCircleRadius, dmgArea));
            /*if (player.GetComponent<Player>().IsPlayerAlive() == false)
            {
                endGame();
            }*/
            if (checkInAttack.Length > 0)
            {
                anim.SetBool("isAttack", true);
                //anim.SetBool("InArea", false);
                if (timer <= 0)
                {
                    
                    anim.SetBool("isAttack", true);
                    //anim.SetTrigger("Attack");
                    isAttacking = true;
                    Debug.Log(isAttacking);
                    Debug.Log("In here");
                    float timer2 = 3f;
                    //this is temporary. It will pause the damage dealt until Tobinator actually hits in the animation
                    while (timer2 > 0)
                    {
                        timer2 -= Time.deltaTime;
                    
                    }

                    checkInAttack = Enemy.colliderTagSorter("Player", Enemy.getAllAround(attackCircleRadius, dmgArea));
                    if (checkInAttack.Length > 0)
                    {
                        
                        float distance = player.transform.position.x - this.transform.position.x;
                        int direction = (int)Mathf.Sign(distance);
                        //Debug.Log("Direction = " + direction);
                        // direction will be 1 for player on right side and -1 for player
                        // on left side
                        player.GetComponent<Player>().GetHit(damage, direction);
                        
                    }
                    timer = attkCoolDown;
                    anim.SetBool("isAttack", false);
                }
            }
            else
            {
                //anim.SetBool("InArea", true);
                anim.SetBool("isAttack", false);
                
            }
            isAttacking = false;
        }
        else
        {
            isAttacking = false;
            //anim.SetBool("InArea", false);
            //anim.SetBool("isAttack", false);
        }
    }
}


//takes in an array of Collider2D objects and returns an array of GameObjects that have a certain tag
/*private GameObject[] colliderTagSorter(string tagName, Collider2D[] toSort)
{
    //for ease, use ArrayList to add items to array. Then convert back to array to send back. (MAY CHANGE LATER FOR MEMORY EFFICIENCY)
    ArrayList tempList = new ArrayList();
    for (int i = toSort.Length - 1; i >= 0; i--)
    {
        if (toSort[i].gameObject.tag == tagName)
        {
            tempList.Add(toSort[i].gameObject);
        }
    }
    return tempList.ToArray(typeof(GameObject)) as GameObject[];
}

private Collider2D[] getAllAround(float radius, Transform center)
{
    return Physics2D.OverlapCircleAll(center.position, radius);
}
}*/
