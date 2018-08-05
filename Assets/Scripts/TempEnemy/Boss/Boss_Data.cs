using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Data : MonoBehaviour
{
    
    

    private static float max_health = 1000f;
    private float currHealth;
    private GameObject healthBar;
    private GameObject currHealthLayer;

    private const int TOP_LAYERH = 4;
    private int currLayerH;

    private float timeRed;
    public static float TimeDisplayHurt = 0.5f;
    Color defColor;

    

    private float inCombatTimer;

    private bool lowHealth;

    private float inLowHealth_timer;
    private const float LOWHEALTH_DUR = 20f;

    private const float INCOM_REG_AMOUNT = 0.005f;
    private const float OUTCOM_REG_AMOUNT = 0.01f;
    private const float LOWHEAL_REG_AMOUNT = 0.07f;
    private const float LOWHEAL_REG_LIMIT = 0.6f;


    private float regenTimer = 0f;
    private const float HEALTH_REG_CD = 2f;

    

    private bool underAttack;

    private const float DEF_DAMAGE_RED = 0.2f;
    private float damageReduceRate = 0;

    // Use this for initialization
    void Start()
    {
        currHealth = max_health;
        
        timeRed = TimeDisplayHurt;

        healthBar = this.transform.GetChild(3).gameObject;
        currHealthLayer = healthBar.transform.GetChild(TOP_LAYERH).gameObject;
        currLayerH = TOP_LAYERH;

        //defColor = GetComponent<SpriteRenderer>().material.GetColor("_Color");
        inCombatTimer = 0f;

        inLowHealth_timer = LOWHEALTH_DUR;

        lowHealth = false;
        underAttack = false;

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(currHealth);
        if (timeRed > 0)
        {
            timeRed -= Time.deltaTime;
        }
        else
            GetComponent<SpriteRenderer>().material.SetColor("_Color", Color.white);
        //Debug.Log("current health = " + currHealth);

    }

    public bool isAlive()
    {
        if (currHealth <= 0)
            return false;
        return true;
    }

    public float eneTakeDamage(int damage)
    {
        currHealth -= (damage - (damageReduceRate * damage));
        underAttack = true;
        timeRed = TimeDisplayHurt;
        //setCombatTimer();


        GetComponent<SpriteRenderer>().material.SetColor("_Color", Color.red);
        float calHealth = currHealth / max_health;
        SetHealthBar(calHealth);

        return calHealth;
    }

    /*public void setCombatTimer()
    {
        inCombatTimer = COMBAT_EXIT_TIME;
    }*/

    public void SetHealthBar(float calHealth)
    {
        float calQuarterHealth = calHealth % 0.25f;
        float percQuaterH = calQuarterHealth * 4;
        float div = (calHealth / 0.25f);
        int calLayer = (int)div;
        //Debug.Log("div = " + layer + " remainder = " + calQuarterHealth);
        if (calLayer < currLayerH - 1 && calLayer >= 0)
        {
            currLayerH--;
            currHealthLayer.transform.localScale = new Vector3(0f, currHealthLayer.transform.localScale.y, currHealthLayer.transform.localScale.z);
            currHealthLayer = healthBar.transform.GetChild(currLayerH).gameObject;
        }
        else if (calLayer > currLayerH - 1 && calLayer < TOP_LAYERH)
        {
            currLayerH++;
            currHealthLayer.transform.localScale = new Vector3(1f, currHealthLayer.transform.localScale.y, currHealthLayer.transform.localScale.z);
            currHealthLayer = healthBar.transform.GetChild(currLayerH).gameObject;
        }


        currHealthLayer.transform.localScale = new Vector3(percQuaterH, currHealthLayer.transform.localScale.y, currHealthLayer.transform.localScale.z);

    }

    public bool inLowMode()
    {
        if (inLowHealth_timer > 0 && (currHealth < max_health* LOWHEAL_REG_LIMIT))
        {
            if (regenTimer > 0)
                regenTimer -= Time.deltaTime;
            else
            {
                regenHealth(LOWHEAL_REG_AMOUNT);
                regenTimer = HEALTH_REG_CD;
            }
            inLowHealth_timer -= Time.deltaTime;
            return true;
        }
        else
        {
            inLowHealth_timer = LOWHEALTH_DUR;
            return false;
            
        }
    }


    private void regenHealth(float amountPer)
    {
        if (currHealth >= max_health)
            return;

        if (currHealth + (currHealth * amountPer) > max_health)
            currHealth = max_health;
        else
            currHealth += amountPer * max_health;


        SetHealthBar(currHealth / max_health);
    }


    public void set_damRedRate(float rate = DEF_DAMAGE_RED)
    {
        if (rate >= 0 && rate <= 1)
            damageReduceRate = rate;
    }

    //Everytime changing health rengeration
    private void reset_regenTimer()
    {
        regenTimer = 0;
    }

    public void auto_health_regen(bool isInCombat)
    {
        
        if (regenTimer <= 0)
        {
            float regAmount;
            if (isInCombat)
                regAmount = INCOM_REG_AMOUNT;
            else
                regAmount = OUTCOM_REG_AMOUNT;

            regenHealth(regAmount);
            regenTimer = HEALTH_REG_CD;

        }
        else
            regenTimer -= Time.deltaTime;
    }

}