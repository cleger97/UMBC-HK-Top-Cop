using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A lot of Enemy now contains various statics to help initalize and modify new enemies
// This is to aid in fixing health bars and also allow new and dynamic behaviors to occur
public abstract class Enemy : MonoBehaviour {

	// these are assigned in the editor
	// click the "Enemy" script then check the defaults in inspector to verify
	public static GameObject enemyPrototype;
	public static GameObject healthPrototype;

    public static GameObject bossPrototype;
    public static GameObject bossHealthPro;



    private static Vector3 hpPos = new Vector3 (0f, 0.7f, 0f);
	private static Vector2 hpSize = new Vector2 (0.5f, 0.25f);

	protected GameObject healthBar = null;
    public int health;
    protected static int num_enemy_alive = 0;

	public static GameObject enemyContainer;

    

    public static int return_num_enemy()
    {
        return num_enemy_alive;
    }

	public static void resetEnemyCount() {
		num_enemy_alive = GameObject.FindGameObjectsWithTag ("Enemy").Length;
	}
    public abstract void takeDamage(int damage);
    public abstract string returnName();

	public void AlignHealthBar(GameObject HealthBar) {
		this.healthBar = HealthBar;
	}

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
   

	public static Enemy InstantiateNew(Vector3 position) {
		enemyContainer = GameObject.Find ("Enemy Container");

		enemyPrototype = Resources.Load("Prefabs/Enemy") as GameObject;
		healthPrototype = Resources.Load("Prefabs/EnHealth") as GameObject;



		GameObject toSpawn;
		if (enemyContainer == null) {
			toSpawn = Instantiate (enemyPrototype, position, Quaternion.identity);
		} else {
			toSpawn = Instantiate (enemyPrototype, position, Quaternion.identity, enemyContainer.transform);
		}

		GameObject healthBar = Instantiate (healthPrototype, hpPos, Quaternion.identity, toSpawn.transform);
		healthBar.GetComponent<RectTransform> ().sizeDelta = hpSize;
		healthBar.GetComponent<RectTransform> ().localPosition = hpPos;

		toSpawn.GetComponent<Enemy> ().AlignHealthBar (healthBar.transform.GetChild(1).gameObject);

		return toSpawn.GetComponent<Enemy> ();
	}
    public static void InstantiateBoss(Vector3 position)
    {
        Debug.Log("Before...");
        bossPrototype = Resources.Load("Prefabs/BigBoss") as GameObject;
        bossHealthPro = Resources.Load("Prefabs/BossHealth") as GameObject;

        GameObject toSpawn = Instantiate(bossPrototype, position, Quaternion.identity);
        GameObject healthBar = Instantiate(bossHealthPro, hpPos, Quaternion.identity, toSpawn.transform);
        healthBar.GetComponent<RectTransform>().sizeDelta = hpSize;
        healthBar.GetComponent<RectTransform>().localPosition = hpPos;
        toSpawn.GetComponent<Enemy>().AlignHealthBar(healthBar.transform.GetChild(4).gameObject);




    }

    public void SetHealthBar (float currHealth, float maxHealth)
	{
		if (healthBar == null) {
			return;
		}
		float calHealth = currHealth / maxHealth;

		healthBar.transform.localScale = new Vector3 (calHealth, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
	}
}
