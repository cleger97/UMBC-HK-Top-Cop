using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempBoss : Enemy {
    private const float BASE_STUN_TIME = 0.75f;
    private const float LOW_HEALTH = 0.3f;

    private const float LOW_HEALTH_MODE_CD = 90f;
    private float lowHModeTimer = 0f;

    private Boss_Data bossData;
    Enemy_Movement mov;
    Animator anim;

    private enum bossState { ATTACK, BLOCK, MOVE, IDLE, JUMP,LOW_HEALTH,STUN,RUN_AWAY }
    private bossState currentState;
    private bool isLowHP;
    private bool isAttacking;

    private float stunTimer;
    private bool underAttack;

    //private float underAttackTimer;

    private Boss_attack attackData;
    private GameObject player;
    private Player playerData;
    
    public override string returnName()
    {
        return "Boss";
    }

    // Use this for initialization


    void Start () {
        bossData = GetComponent<Boss_Data>();
        anim = GetComponent<Animator>();
        mov = GetComponent<Enemy_Movement>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerData = player.GetComponent<Player>();
        attackData = GetComponent<Boss_attack>();

        
        currentState = bossState.IDLE;
        
        isLowHP = false;
        underAttack = false;
        stunTimer = 0f;
    }
	

	// Update is called once per frame
	void FixedUpdate () {
        if (!playerData.IsPlayerAlive())
        {
            endGame();
            return;
        }

        if (lowHModeTimer > 0)
            lowHModeTimer -= Time.deltaTime;
        


        if (isLowHP && lowHModeTimer <= 0)
        {
            //currentState = bossState.LOW_HEALTH;
            low_health_mode();
        }
        else
        {
            if (underAttack)
                currentState = bossState.STUN;
        }

        Debug.Log(currentState);
        switch (currentState) {
            case bossState.ATTACK:
                attack_state();
                break;
            case bossState.MOVE:
                move_state();
                break;
            case bossState.IDLE:
                idle_state();
                break;
            case bossState.STUN:
                stun_state();
                break;
            case bossState.LOW_HEALTH:
                //low_health_mode();
                break;

            case bossState.RUN_AWAY:

                break;
            default:
                return;
        }
	}

    private void attack_state()
    {
        
        if (attackData.startAttack())
        {
            
            return;
        }
        else
            currentState = bossState.MOVE;

    }

    private void move_state()
    {
        if (attackData.attackCheck())
        {
            currentState = bossState.ATTACK;
            return;
        }

        if (!mov.chasingPlayer())
            currentState = bossState.IDLE;   
            
    }

    private void idle_state()
    {
        if (!mov.idleTime())
            currentState = bossState.MOVE;
    }


    public override void takeDamage(int damage)
    {
        float remHealth = bossData.eneTakeDamage(damage);
        underAttack = true;

        stunTimer = BASE_STUN_TIME;
        if (remHealth <= 0)
            dead();
        else if (remHealth <= LOW_HEALTH)
        {
            //enter low health mode
            isLowHP = true;
        }
    }


    private void dead()
    {
        anim.SetBool("Death", true);
        //TEMPORARY WAY FOR AFTERMATH OF FIGHT
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<Rigidbody2D>().gravityScale = 0;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        //playerData.updateKill();
        this.enabled = false;
        Destroy(gameObject, 0.25f);
    }

    private void enterlowHPMode()
    {
        mov.setRunAway(isLowHP);
    }

    private void endGame()
    {
        anim.SetBool("endGame",true);
        this.enabled = false;
    }

    private void stun_state()
    {
        if (stunTimer >= 0)
        {
            //anim.SetBool("isIdle", true);
            stunTimer -= Time.deltaTime;
        
        }
        else
        {
            //anim.SetBool("isIdle", false);
            stunTimer = 0f;
            underAttack = false;
            currentState = bossState.IDLE;
        }
        
    }

    private void low_health_mode()
    {

        if (bossData.inLowMode())
        {
            attackData.enter_lowH_mode();
            return;
        }
        else
        {
            //currentState = bossState.IDLE;
            attackData.exit_lowH_mode();
            isLowHP = false;
            lowHModeTimer = LOW_HEALTH_MODE_CD;
        }
    }

   private void temp_run_away()
    {
        
    }
}
