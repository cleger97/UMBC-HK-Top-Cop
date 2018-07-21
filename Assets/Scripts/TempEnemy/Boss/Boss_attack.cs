using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_attack : MonoBehaviour
{

    Animator anim;

    GameObject player;

    //public float detectionCircleRadius;
    public float attackCircleRadius;

    private int damage;
    public Transform dmgArea;
    private float attackCD;

    private const float ATTACK_CD = 0.5f;
    private const int ATTACK_INCREASE = 10;

    private bool attackUp;
    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();

        damage = 50;
        attackCD = 0.3f;

        attackUp = false;
    }

    // Update is called once per frame
    void Update()
    {

    }


    public bool attackCheck()
    {
        anim.SetBool("isAttack", false);
        GameObject[] check = Enemy.colliderTagSorter("Player", Enemy.getAllAround(attackCircleRadius, transform));

        if (check.Length > 0)
            return true;
        else
            return false;
    }


    public bool startAttack()
    {
        //anim.SetBool("isAttack", true);
        GameObject[] checkInAttack = Enemy.colliderTagSorter("Player", Enemy.getAllAround(attackCircleRadius, dmgArea));
        if (checkInAttack.Length > 0)
        {
            anim.SetInteger("speed", 0);
            if (attackCD > 0)
            {
                attackCD -= Time.deltaTime;
                if (attackCD <= 0.3f)
                {
                    anim.SetBool("isAttack", true);

                }
                return true;
            }

            float distance = player.transform.position.x - this.transform.position.x;
            int direction = (int)Mathf.Sign(distance);
            player.GetComponent<Player>().GetHit(damage, direction);
            attackCD = ATTACK_CD;
            anim.SetBool("isAttack", false);
            return true;
        }
        else
            return false;
    }

    public void enter_lowH_mode()
    {
        if (!attackUp)
        {
            damage += ATTACK_INCREASE;
            attackUp = true;
        }

    }

    public void exit_lowH_mode()
    {
        attackUp = false;
        damage -= ATTACK_INCREASE;
    }



}