using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour {

    int health;
    protected static int num_enemy_alive = 0;
    public static int return_num_enemy()
    {
        return num_enemy_alive;
    }
    public abstract void takeDamage(int damage);
    public abstract string returnName();
    public static GameObject[] colliderTagSorter(string tagName, Collider2D[] toSort)
    {
        //for ease, use ArrayList to add items to array. Then convert back to array to send back. (MAY CHANGE LATER FOR MEMORY EFFICIENCY)
        ArrayList tempList = new ArrayList();
        for (int i = toSort.Length - 1; i >= 0; i--)
        {
            if (toSort[i].gameObject.tag == tagName)
            {
                tempList.Add(toSort[i].gameObject);
            }
        }
        return tempList.ToArray(typeof(GameObject)) as GameObject[];
    }

    public static Collider2D[] getAllAround(float radius, Transform center)
    {
        return Physics2D.OverlapCircleAll(center.position, radius);
    }
    //public abstract void attack();
}
