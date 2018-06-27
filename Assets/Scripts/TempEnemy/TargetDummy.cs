using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDummy : Enemy
{
    private int hpbarChild = 0;
    private int hpColorBar = 1;


    public override string returnName()
    {
        return "Target Dummy";
    }

    public override void takeDamage(int damage)
    {
        health -= damage;
    }

    private void Update()
    {
        if (health < 5000)
        {
            health = 9999;
        }

        UpdateHPBar();
    }

    private void UpdateHPBar()
    {
        float percent = ((float)health / (float)9999);
        transform.GetChild(hpbarChild).GetChild(hpColorBar).localScale = new Vector2(percent, 1);
    }

}
