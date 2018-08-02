using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempBoss : Enemy
{
    private const float BASE_STUN_TIME = 1.5f;
    private const float STUN_DECAY_INCREMENT = 0.5f;
    private const float RESET_NUM_HITS_TIME = 2.5f;

    private const float TRIGGER_LOWH = 0.25f;

    // hitstun decay
    // prevents you from permanently stunning boss
    private int numOfHits;
    private float timeSinceLastHit;
    private bool allowStun = true;
    private float stunTimer;


    private const float LOW_HEALTH_MODE_CD = 90f;
    private float lowHModeCDTimer = 0f;

    private Boss_Data bossData;
    Enemy_Movement mov;
    Animator anim;

    private const float IN_COMBAT_EXIT_TIME = 2f;
    private float inCombatTimer;
    //private bool isInCombat = false;
    


    private enum bossState { IN_COMBAT, IDLE, JUMP, LOW_HEALTH,RUN_AWAY };
    private enum inCombatState { ATTACK, CHASE, STUN };
    private enum attackSet { BASE_ATTACK = 0,RUN_ATT, TORNADO_ATT,EMPTY};
    
    private attackSet currAttSkill;
    private inCombatState currCombatState;
    private bossState currentState;

    private bool isLowHP = false;
    private bool isInLowHeaMode = false;
    
    private bool isUnderAttack = false;
    private bool isAttacking = false;
    

    private Boss_attack attackData;
    private GameObject player;
    private Player playerData;
    private BossFinish bossFinish;

    private float furiousRate;
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
        

        currAttSkill = attackSet.BASE_ATTACK;
        stunTimer = 0f;

        inCombatTimer = 0;
    }

    void Update()
    {
        if (timeSinceLastHit > RESET_NUM_HITS_TIME)
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

        if(currentState != bossState.RUN_AWAY)
            check_inCombat();


        Debug.Log("inCombaTimer = " + inCombatTimer);

        if (isInLowHeaMode) { low_health_mode(); }
        else
        {
            if (isLowHP && lowHModeCDTimer <= 0) { enter_lowHealth(); }
            else
            {
                bossData.auto_health_regen(inCombatTimer > 0);
                if (lowHModeCDTimer > 0) { lowHModeCDTimer -= Time.deltaTime; }
            }
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
            case bossState.RUN_AWAY:
                run_state();
                break;
            default:
                return;
        }
    }

    private void check_inCombat()
    {
        if (isUnderAttack || isAttacking)
        {
            set_inCombatTime();

        }
        else
        {
            if (inCombatTimer > 0)
            {
                currentState = bossState.IN_COMBAT;
                inCombatTimer -= Time.deltaTime;
            }
            else
            {
                currentState = bossState.IDLE;
                attackData.reset_att_CD((int)attackSet.BASE_ATTACK);
                isUnderAttack = false;
                isAttacking = false;
            }
        }

        if (inCombatTimer > 0)
            currentState = bossState.IN_COMBAT;
    }


    private void in_combat_state()
    {
        //can't been stunned while boss is punching
        if (isUnderAttack && allowStun && !(attackData.return_att_CD() <= 0.3f))
        {
            isUnderAttack = false;
            currCombatState = inCombatState.STUN;
        }
        else
        {
            if (check_skillSet())//if any skill is avaliable 
            {
                currCombatState = inCombatState.ATTACK;
            }
        }
        Debug.Log("currCombatState = " + currCombatState);
        switch (currCombatState)
        {
            
            case inCombatState.ATTACK:
                attack_state();
                break;
            case inCombatState.CHASE:
                chase_state();
                break;
            case inCombatState.STUN:
                stun_state();
                break;
        }
    }


    private void attack_state()
    {
        //avoid changing skill while doing basic attack
        if (currAttSkill != attackSet.BASE_ATTACK && (attackData.return_att_CD() <= 0.3f))
            currAttSkill = attackSet.BASE_ATTACK;
        Debug.Log("currAttSkill = " + currAttSkill);
        switch (currAttSkill)
        {
            case attackSet.BASE_ATTACK:
                in_base_attack();
                break;
            case attackSet.RUN_ATT:
                in_run_attack();
                break;
            case attackSet.TORNADO_ATT:
                in_tornado_attack();
                break;
        }
       
    }

    private bool check_skillSet()
    {
        switch (attackData.skill_CD_check())//check CoolDown
        {
            //check pre-condition
            case (int)attackSet.RUN_ATT:
                if (mov.rushAtt_prec())
                {
                    currAttSkill = attackSet.RUN_ATT;
                    return true;
                }
                return false;
            case (int)attackSet.TORNADO_ATT:
                currAttSkill = attackSet.TORNADO_ATT;
                return true;
            default:
                currAttSkill = attackSet.BASE_ATTACK;
                return false;
        }
    }

   

    private void in_base_attack()
    {
        if (attackData.startAttack())
        {
            isAttacking = true;
        }
        else
        {
            currCombatState = inCombatState.CHASE;
            isAttacking = false;
        }
    }


    private void in_run_attack()
    {
        Debug.Log("in rush attack");
     
         if (attackData.rush_attack())
         {
            isAttacking = true;
            mov.rush_att_mov();
            allowStun = false;
         }
         else
         {
            exit_skillAtt();
        }

    }

    private void in_tornado_attack()
    {
        if (attackData.tornado_attack())
        {
            isAttacking = true;
            mov.tornado_att_mov();
            allowStun = false;
        }
        else
        {
            exit_skillAtt();
        }
    }

    private void exit_skillAtt()
    {
        currCombatState = inCombatState.CHASE;
        isAttacking = false;
        allowStun = true;
        currAttSkill = attackSet.EMPTY;
    }

    private void chase_state()
    {
        if (attackData.attackCheck())
        {
            currCombatState = inCombatState.ATTACK;
            return;
        }

        
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
            set_inCombatTime();
        

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
        else if (remHealth <= TRIGGER_LOWH )
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
            
            currCombatState = inCombatState.CHASE;
        }

    }

    private void low_health_mode()
    {
        Debug.Log("In low health mode");
        if (bossData.inLowMode())
        {
            return;
        }
        else
        {
            exit_lowHealth();
            currentState = bossState.IDLE;
            return;
        }

    }

    private void enter_lowHealth()
    {
        if (!isInLowHeaMode)
        {
            isInLowHeaMode = true;
            allowStun = false;
            currentState = bossState.RUN_AWAY;
            stop_allAttack();

            attackData.enter_lowH_mode();
            

            set_inCombatTime(0);
            bossData.set_damRedRate();
            
        }

    }

    private void exit_lowHealth()
    {
        isLowHP = false;
        isInLowHeaMode = false;
        allowStun = true;
        lowHModeCDTimer = LOW_HEALTH_MODE_CD;
        attackData.exit_lowH_mode();
        bossData.set_damRedRate(0);
    }

    private void stop_allAttack()
    {
        
        isUnderAttack = false;
        isAttacking = false;
        switch (currAttSkill)
        {
            case attackSet.RUN_ATT:
                attackData.reset_att_CD((int)attackSet.RUN_ATT);
                break;
            case attackSet.TORNADO_ATT:
                attackData.reset_att_CD((int)attackSet.TORNADO_ATT);
                break;
            case attackSet.BASE_ATTACK:
                attackData.reset_att_CD((int)attackSet.BASE_ATTACK);
                break;
            default:
                return;
        }
    }

    private void run_state()
    {
        if (mov.runAway())
        {
            set_inCombatTime(0);
            return;
        }
        else
        {
            set_inCombatTime(0);
            currentState = bossState.IDLE;
        }

    }

    private void set_inCombatTime(float exitTime = IN_COMBAT_EXIT_TIME)
    {
        inCombatTimer = exitTime;
        
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        mov.OntriggerEnter(other);
    }

}