using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Data : MonoBehaviour{
    private float regenTimer;
    private static float max_health = 400f;
    private float currHealth;
    public GameObject healthBar;

    private float timeRed;
    public static float TimeDisplayHurt = 0.5f;
    Color defColor;
  
   
    private bool lowHealth;
    private bool isDead;

    private bool underAttack;
    
    // Use this for initialization
    void Start () {
        currHealth = max_health;
        regenTimer = 1.0f;
        timeRed = TimeDisplayHurt;

        defColor = GetComponent<SpriteRenderer>().material.GetColor("_Color");
        
        
        lowHealth = false;
        underAttack = false;
        isDead = false;
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
            inLowMode();
    }

    public bool isAlive()
    {
        return isDead;
    }

    public int eneTakeDamage(int damage)
    {
        
        currHealth -= damage;
        underAttack = true;
        timeRed = TimeDisplayHurt;


        //SetHealthBar(currHealth);
        GetComponent<SpriteRenderer>().material.SetColor("_Color", Color.red);
        if (currHealth <= 0)
        {
            isDead = true;
            return 1;
            
        }
        else if (currHealth <= 0.3 * max_health)
        {
            Debug.Log("enter low health mode");
            lowHealth = true;
            return 2;
        }
        return 3;
    }

    public void SetHealthBar(float health)
    {
        float calHealth = health / max_health;
        healthBar.transform.localScale = new Vector3(calHealth, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
    }

    private void inLowMode()
    {
        
        if (regenTimer > 0)
            regenTimer -= Time.deltaTime;
        else
        {
            currHealth += 0.02f * max_health;
            regenTimer = 1.0f;
            //SetHealthBar(currHealth);
            if (currHealth >= 0.5f * max_health)
                lowHealth = false;
        }
    }
}
