using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempBoss : Enemy {
    private Boss_Data bossData;
    Enemy_Movement mov;
    Animator anim;

    private enum State { ATTACK, BLOCK, MOVE, IDLE, JUMP }

    private bool isLowHP;
    private bool isAttacking;
    private float timer;

    Boss_attack attackData;
    GameObject player;
    Player playerData;
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
        timer = 0f;
        isLowHP = false;
    }
	

	// Update is called once per frame
	void Update () {
        if (!playerData.IsPlayerAlive())
            endGame();
        anim.SetBool("isAttack", false);
        if(timer > 0)
            timer -= Time.deltaTime;
        if (attackData.attackCheck())
        {
            if (timer <= 0)
            {
                anim.SetBool("isAttack", true);
                attackData.startAttack();
                timer = attackData.getAttcool();
            }
        }
        else
        {
            mov.handleMovement();
        }

	}




    public override void takeDamage(int damage)
    {
        int result = bossData.eneTakeDamage(damage);

        if (result == 1)
            dead();
        else if (result == 2)
            //enter low health mode
            isLowHP = true;
            enterlowHPMode();
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

    

}
