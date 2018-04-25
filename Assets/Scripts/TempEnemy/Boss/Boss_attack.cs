using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_attack : MonoBehaviour {
    Animator anim;
    
    GameObject player;

    public float detectionCircleRadius;
    public float attackCircleRadius;
    private float attkCoolDown;
    private int damage;
    public Transform dmgArea;

    public bool isAttacking;

    
    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        isAttacking = false;
        damage = 20;
        attkCoolDown = 1f;
    }
	
	// Update is called once per frame
	void Update () {
    }

    public bool attackCheck()
    {
        GameObject[] check = Enemy.colliderTagSorter("Player", Enemy.getAllAround(detectionCircleRadius, transform));
        if (check.Length > 0)
        {
            GameObject[] checkInAttack = Enemy.colliderTagSorter("Player", Enemy.getAllAround(attackCircleRadius, dmgArea));

            GameObject player = check[0];
            if (checkInAttack.Length > 0)
                return true;
            else
            {
                isAttacking = false;
                anim.SetBool("isAttack", false);
                return false;
            }
            
        }
        else
        {
            isAttacking = false;
            return false;
        }
    }


    public void startAttack()
    {


            anim.SetBool("isAttack", true);
            isAttacking = true;
            Debug.Log(isAttacking);
            float timer2 = 3f;
            //this is temporary. It will pause the damage dealt until Tobinator actually hits in the animation
            while (timer2 > 0)
            {
                timer2 -= Time.deltaTime;
            }

            GameObject[] checkInAttack = Enemy.colliderTagSorter("Player", Enemy.getAllAround(attackCircleRadius, dmgArea));
            if (checkInAttack.Length > 0)
            {

                float distance = player.transform.position.x - this.transform.position.x;
                int direction = (int)Mathf.Sign(distance);
                //Debug.Log("Direction = " + direction);
                // direction will be 1 for player on right side and -1 for player
                // on left side
                player.GetComponent<Player>().GetHit(damage, direction);

            }
           
           
    }

    public float getAttcool()
    {
        return attkCoolDown;
    }
}

