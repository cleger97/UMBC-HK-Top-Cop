//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_attack : MonoBehaviour
{

    Animator anim;

    GameObject player;

    private const int BASIC_ATTACK = 0;
    private const int RUN_ATTACK = 1;
    private const int TORNADO_ATTACK = 2;


    //public float detectionCircleRadius;
    public float attackCircleRadius = 0.5f;

    private int damage;
    public Transform dmgArea;
    private float attackCD;

    private const float ATTACK_CD = 0.75f;
    private float basicAttCD;
    private const int ATTACK_INCREASE = 10;

    private bool attackUp;

    private const float RUN_ATT_CD = 30f;
    private float runAttCDTimer;

    private const float RUN_ATT_DUR = 1.5f; 
    private float runAtt_DurTimer;
    private float runAtt_gap;

    private const float TORNADO_ATT_CD = 40f;
    private float tornadoCDTimer;
    private const float TORNADO_DUR = 5f;
    private float tornado_DurTimer;
    private float torAtt_gap;

    private float skillGapTimer; //avoid casting all skills subsequently

    public int baseKBForce = 500;
    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();

        damage = 30;
        attackCD = 0.3f;

        tornado_DurTimer = TORNADO_DUR;
        runAtt_DurTimer = RUN_ATT_DUR;
        attackUp = false;
        runAttCDTimer = Time.time + 10f;
        tornadoCDTimer = Time.time + 15f;

        skillGapTimer = Time.time;

        torAtt_gap = 0.2f;
        runAtt_gap = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Time.time >= runAttCDTimer + RUN_ATT_CD)
            Debug.Log("1 ability is avaliable");
        else
            Debug.Log("1 ability is not avaliable");*/
    }

    public int skill_CD_check()
    {
        if (Time.time <= skillGapTimer)
            return BASIC_ATTACK;

        if (Time.time >= runAttCDTimer )
            return RUN_ATTACK;
        else if (Time.time >= tornadoCDTimer)
            return TORNADO_ATTACK;
        else
            return BASIC_ATTACK;
    }

    public void reduce_att_CD(int time = 10)
    {
        if (Time.time < runAttCDTimer)
            runAttCDTimer -= time;
        if (Time.time < tornadoCDTimer)
            tornadoCDTimer -= time;
    }

    public bool rush_attack()
    {
        
        if(runAtt_DurTimer < 0)
        {
            anim.SetBool("isAttack", false);
            runAttCDTimer = Time.time + RUN_ATT_CD;
            runAtt_DurTimer = RUN_ATT_DUR;
            set_skillGapTimer();
            return false;
        }
        else
        {
            if (runAtt_gap > 0)
            {
                runAtt_gap -= Time.deltaTime;
                if (runAtt_gap < 0.2)
                {
                    anim.SetBool("isAttack", true);
                    return true;
                }
                
            }
            else
            {
                GameObject[] checkInAttack = Enemy.colliderTagSorter("Player", Enemy.getAllAround(attackCircleRadius, dmgArea));
                if (checkInAttack.Length > 0)
                {
                    afflict_damage(checkInAttack[0]);
                }
                runAtt_gap = 0.5f;
            }
            runAtt_DurTimer -= Time.deltaTime;
         
            return true;
        }
       
    }


    public bool tornado_attack()
    {
        if (tornado_DurTimer < 0)
        {
            anim.SetBool("isAttack", false);
            tornadoCDTimer = Time.time + TORNADO_ATT_CD;
            tornado_DurTimer = TORNADO_DUR;
            set_skillGapTimer();
            return false;
        }
        else
        {
            anim.SetBool("isAttack", true);

            if(torAtt_gap < 0.2)
            {
                GameObject[] checkInAttack = Enemy.colliderTagSorter("Player", Enemy.getAllAround(attackCircleRadius, dmgArea));
                if (checkInAttack.Length > 0)
                {
                    afflict_damage(checkInAttack[0]);
                }
                torAtt_gap = 0.5f;
            }
            

            torAtt_gap -= Time.deltaTime;
            tornado_DurTimer -= Time.deltaTime;
            return true;
        }
    }


    public bool attackCheck()
    {
        anim.SetBool("isAttack", false);
        GameObject[] check = Enemy.colliderTagSorter("Player", Enemy.getAllAround(attackCircleRadius, transform));

        if (check.Length > 0)
        {
            Debug.Log("in attack check");
            return true;
        }
        else
            return false;
    }


    public bool startAttack()
    {
        //anim.SetBool("isAttack", true);
        GameObject[] checkInAttack = Enemy.colliderTagSorter("Player", Enemy.getAllAround(attackCircleRadius, dmgArea));
        if (checkInAttack.Length > 0)
        {
            Debug.Log("in start attack");
            //anim.SetInteger("speed", 0);
            if (attackCD > 0)
            {
                attackCD -= Time.deltaTime;
                if (attackCD <= 0.3f)
                {
                    anim.SetBool("isAttack", true);

                }
                return true;
            }
            afflict_damage(checkInAttack[0]);
            
            attackCD = random_attCD();
            anim.SetBool("isAttack", false);
            return true;
        }
        else
            return false;
    }

    private float random_attCD()
    {
        if(attackUp)
            return Random.Range(0.50f, 0.65f);
        else
            return Random.Range(0.65f, 0.8f);
    }


    private void afflict_damage(GameObject givenPlayer)
    {
        Player playerScript = givenPlayer.transform.GetComponent<Player>();
        float distance = givenPlayer.transform.position.x - this.transform.position.x;
        int direction = (int)Mathf.Sign(distance);
        playerScript.GetHit(damage, direction);

        Vector3 dir = givenPlayer.transform.position - transform.position;
        dir = dir.normalized;
        dir.y = 0;
        givenPlayer.transform.GetComponent<Rigidbody2D>().AddForce(dir * baseKBForce);
    }


    public void enter_lowH_mode()
    {
        if (!attackUp)
        {
            damage += ATTACK_INCREASE;
            attackUp = true;
        }
        reduce_att_CD();

    }

    public void exit_lowH_mode()
    {
        attackUp = false;
        damage -= ATTACK_INCREASE;
    }

    public void reset_att_CD(int selectSkill)
    {
        anim.SetBool("isAttack", false);
        switch (selectSkill)
        {
            case BASIC_ATTACK:
                attackCD = ATTACK_CD;
                break;
            case RUN_ATTACK:
                runAttCDTimer = Time.time + 15f;
                runAtt_DurTimer = RUN_ATT_DUR;
                runAtt_gap = 0.2f;
                break;
            case TORNADO_ATTACK:
                tornadoCDTimer = Time.time + 15f;
                tornado_DurTimer = TORNADO_DUR;
                torAtt_gap = 0.2f;
                break;
        }
        

    }

    

    public float return_att_CD()
    {
        return attackCD;
    }

    private void set_skillGapTimer()
    {
        float waitTime = Random.Range(5f, 15f);
        skillGapTimer = Time.time + waitTime;
    }


}