using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour {

    int health;
    protected static int num_enemy_alive = 0;
    public abstract int return_num_enemy();
    public abstract void takeDamage(int damage);

    //public abstract void attack();
}
