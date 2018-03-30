using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour {

    int health;

    public abstract void takeDamage(int damage);

    //public abstract void attack();
}
