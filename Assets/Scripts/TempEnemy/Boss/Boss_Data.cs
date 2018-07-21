using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Data : MonoBehaviour
{
    
    private float regenTimer;
    private static float max_health = 800f;
    private float currHealth;
    private GameObject healthBar;
    private GameObject currHealthLayer;

    private const int TOP_LAYERH = 4;
    private int currLayerH;

    private float timeRed;
    public static float TimeDisplayHurt = 0.5f;
    Color defColor;

    private const float COMBAT_EXIT_TIME = 3f;
    private const float OUTCOMBAT_REG_CD = 2.0f;
    private const float LOWH_REG_CD = 1.0f;

    private float inCombatTimer;

    private bool lowHealth;



    private bool underAttack;

    // Use this for initialization
    void Start()
    {
        currHealth = max_health;
        regenTimer = 1.0f;
        timeRed = TimeDisplayHurt;

        healthBar = this.transform.GetChild(3).gameObject;
        currHealthLayer = healthBar.transform.GetChild(TOP_LAYERH).gameObject;
        currLayerH = TOP_LAYERH;

        //defColor = GetComponent<SpriteRenderer>().material.GetColor("_Color");
        inCombatTimer = 0f;


        lowHealth = false;
        underAttack = false;



    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(currHealth);
        if (timeRed > 0)
        {
            timeRed -= Time.deltaTime;
        }
        else
            GetComponent<SpriteRenderer>().material.SetColor("_Color", Color.white);

        if (inCombatTimer > 0)
        {
            inCombatTimer -= Time.deltaTime;

        }
        else if (inCombatTimer <= 0 && !lowHealth)
        {
            underAttack = false;
            if (regenTimer > 0)
                regenTimer -= Time.deltaTime;
            else
            {
                regenHealth(0.005f);
                regenTimer = OUTCOMBAT_REG_CD;
            }
        }


    }

    public bool isAlive()
    {
        if (currHealth <= 0)
            return false;
        return true;
    }

    public float eneTakeDamage(int damage)
    {
        currHealth -= damage;
        underAttack = true;
        timeRed = TimeDisplayHurt;
        setCombatTimer();


        GetComponent<SpriteRenderer>().material.SetColor("_Color", Color.red);
        float calHealth = currHealth / max_health;
        SetHealthBar(calHealth);

        return calHealth;
    }

    public void setCombatTimer()
    {
        inCombatTimer = COMBAT_EXIT_TIME;
    }

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
        lowHealth = true;
        if (regenTimer > 0)
            regenTimer -= Time.deltaTime;
        else
        {
            regenHealth(0.01f);
            regenTimer = LOWH_REG_CD;
            if (currHealth >= 0.5f * max_health)
            {
                lowHealth = false;
                return false;
            }
        }

        /*if (inCombatTimer <= 0)
            return false;*/


        return true;
    }

    private void regenHealth(float amountPer)
    {
        if (currHealth == max_health)
            return;

        if (currHealth + (currHealth * amountPer) > max_health)
            currHealth = max_health;
        else
            currHealth += amountPer * max_health;


        SetHealthBar(currHealth / max_health);
    }

    private void reduceIncomingDamage()
    {

    }
}