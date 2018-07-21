using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempBoss : Enemy
{
    private const float BASE_STUN_TIME = 1.5f;
    private const float STUN_DECAY_INCREMENT = 0.3f;
    
    private const float LOW_HEALTH = 0.3f;

    // hitstun decay
    // prevents you from permanently stunning boss
    private int numOfHits;
    private float timeSinceLastHit;

    private const float LOW_HEALTH_MODE_CD = 90f;
    private float lowHModeTimer = 0f;

    private Boss_Data bossData;
    Enemy_Movement mov;
    Animator anim;

    private const float IN_COMBAT_EXIT_TIME = 2f;
    private float inCombatTimer;
    //private bool isInCombat = false;


    private enum bossState { IN_COMBAT, IDLE, JUMP, LOW_HEALTH, RUN_AWAY }
    private enum inCombatState { ATTACK, CHASE, STUN }
    private inCombatState currCombatState;
    private bossState currentState;
    private bool isLowHP;




    private float stunTimer;
    private bool isUnderAttack = false;
    private bool isAttacking = false;
    //private float underAttackTimer;

    private Boss_attack attackData;
    private GameObject player;
    private Player playerData;
    private BossFinish bossFinish;

    public override string returnName()
    {
        return "Boss";
    }

    // Use this for initialization


    void Start()
    {
        bossData = GetComponent<Boss_Data>();
        anim = GetComponent<Animator>();
        mov = GetComponent<Enemy_Movement>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerData = player.GetComponent<Player>();
        attackData = GetComponent<Boss_attack>();
        bossFinish = gameObject.GetComponent<BossFinish>();

        currentState = bossState.IDLE;
        currCombatState = inCombatState.CHASE;
        isLowHP = false;


        stunTimer = 0f;

        inCombatTimer = 0;
    }

    void Update()
    {
        if (timeSinceLastHit > 1f)
        {
            numOfHits = 0;
        }
        else
        {
            timeSinceLastHit += Time.deltaTime;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!playerData.IsPlayerAlive())
        {
            endGame();
            return;
        }

        check_inCombat();

        if (isLowHP && lowHModeTimer <= 0)
        {
            currentState = bossState.RUN_AWAY;
            low_health_mode();
        }
        else
        {
            if (lowHModeTimer > 0)
                lowHModeTimer -= Time.deltaTime;
        }

        Debug.Log(currentState);
        switch (currentState)
        {
            case bossState.IN_COMBAT:
                in_combat_state();
                break;
            case bossState.IDLE:
                idle_state();
                break;

            case bossState.LOW_HEALTH:
                //low_health_mode();
                break;

            case bossState.RUN_AWAY:
                temp_run_away();
                break;
            default:
                return;
        }
    }
    private void check_inCombat()
    {
        if (isUnderAttack || isAttacking)
        {
            inCombatTimer = IN_COMBAT_EXIT_TIME;

        }
        else
        {
            if (inCombatTimer > 0)
                inCombatTimer -= Time.deltaTime;
        }
    }


    private void in_combat_state()
    {

        if (isUnderAttack)
            currCombatState = inCombatState.STUN;

        switch (currCombatState)
        {
            case inCombatState.ATTACK:
                attack_state();
                break;
            case inCombatState.CHASE:
                move_state();
                break;
            case inCombatState.STUN:
                stun_state();
                break;
        }
    }


    private void attack_state()
    {

        if (attackData.startAttack())
        {
            isAttacking = true;
            return;
        }
        else
        {
            currCombatState = inCombatState.CHASE;
            isAttacking = false;
        }
        //currentState = bossState.MOVE;

    }

    private void move_state()
    {
        if (attackData.attackCheck())
        {
            //currentState = bossState.ATTACK;
            currCombatState = inCombatState.ATTACK;
            return;
        }

        //if (!mov.chasingPlayer())
        if (mov.inDectectRange() || inCombatTimer > 0)
        {
            mov.chasingPlayer();
        }
        else
        {
            if (inCombatTimer <= 0)
                currentState = bossState.IDLE;

        }

    }



    private void idle_state()
    {

        if (!mov.idleTime())
            currentState = bossState.IN_COMBAT;
        //currentState = bossState.MOVE;

    }


    public override void takeDamage(int damage)
    {
        float remHealth = bossData.eneTakeDamage(damage);
        isUnderAttack = true;

        float calculatedStunTimer = ((BASE_STUN_TIME - (numOfHits * STUN_DECAY_INCREMENT)));
        stunTimer = (calculatedStunTimer > 0) ? calculatedStunTimer : 0;
        numOfHits++;
        timeSinceLastHit = 0f;
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
        if (bossFinish != null)
        {
            bossFinish.FinishGame();
        }
        Destroy(gameObject, 0.25f);
    }



    private void endGame()
    {
        anim.SetBool("endGame", true);
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
            isUnderAttack = false;
            //currentState = bossState.IDLE;
            currCombatState = inCombatState.CHASE;
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
        if (mov.walk_away())
        {
            return;
        }
        else
            currentState = bossState.IN_COMBAT;

    }

    
}