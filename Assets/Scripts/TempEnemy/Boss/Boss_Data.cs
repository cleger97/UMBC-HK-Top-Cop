using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Data : Enemy {
    public float regenTimer = 1.0f;
    private float max_health;
    private float currHealth;
    public GameObject healthBar;

    float timeRed;
    public float TimeDisplayHurt;
    Color defColor;
    Animator anim;
    Enemy_Movement mov;
    private bool lowHealth;
    private bool underAttack;
    public override string returnName()
    {
        return "Boss";
    }
    // Use this for initialization
    void Start () {
        max_health = 400;
        currHealth = max_health;
        timeRed = TimeDisplayHurt;
        defColor = GetComponent<SpriteRenderer>().material.GetColor("_Color");
        anim = GetComponent<Animator>();
        mov = GetComponent<Enemy_Movement>();
        lowHealth = false;
        underAttack = false;
    }
	
	// Update is called once per frame
	void Update () {

        if (timeRed > 0)
        {
            timeRed -= Time.deltaTime;
        }
        else
            GetComponent<SpriteRenderer>().material.SetColor("_Color", defColor);

        if (lowHealth)
            enterLowMode();
    }
    public bool isAlive()
    {
        return (currHealth >= 0);
    }


    public override void takeDamage(int damage)
    {
        
        currHealth -= damage;
        underAttack = true;

        timeRed = TimeDisplayHurt;
        //SetHealthBar(currHealth);
        GetComponent<SpriteRenderer>().material.SetColor("_Color", Color.red);
        if (currHealth <= 0)
            dead();
        else if (currHealth <= 0.3 * max_health)
        {
            
            Debug.Log("enter low health mode");
            lowHealth = true;
            mov.setRunAway(lowHealth);
        }
    }

    void dead()
    {
      
        anim.SetBool("Death", true);
        //TEMPORARY WAY FOR AFTERMATH OF FIGHT
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<Rigidbody2D>().gravityScale = 0;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        //playerData.updateKill();
        this.enabled = false;
   
        if (currHealth <= 0)
            Destroy(gameObject, 0.25f);
    }
    public void SetHealthBar(float health)
    {
        float calHealth = health / max_health;
        healthBar.transform.localScale = new Vector3(calHealth, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }

    private void enterLowMode()
    {
        
        if (regenTimer > 0)
            regenTimer -= Time.deltaTime;
        else
        {
            currHealth += 0.02f * max_health;
            regenTimer = 1.0f;
            //SetHealthBar(currHealth);
            if (currHealth >= 0.7f * max_health)
                lowHealth = false;
            mov.setRunAway(lowHealth);
        }
    }
}
