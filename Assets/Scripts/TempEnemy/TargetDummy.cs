using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDummy : Enemy
{
    public override string returnName()
    {
        return "Target Dummy";
    }

    public override void takeDamage(int damage)
    {
        throw new System.NotImplementedException();
    }

}
