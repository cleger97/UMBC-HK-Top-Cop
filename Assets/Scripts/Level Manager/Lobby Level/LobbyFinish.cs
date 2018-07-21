using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LobbyFinish : MonoBehaviour {

    ObjectiveScript objScript;
    GameObject enemySpawners;

    bool part2 = false;
    bool finished = false;

	// Use this for initialization
	void Start () {
        objScript = ObjectiveScript.instance;
        if (objScript == null)
        {
            Debug.LogWarning("Objective Script doesn't exist; script will not do anything.");
        }

        if (this.GetComponent<Collider2D>().isTrigger == false)
        {
            Debug.LogWarning("Collider attached is not a trigger. Script will not work correctly.");
        }

        enemySpawners = GameObject.Find("Enemy Spawners");

        
	}

    void Update()
    {
        Debug.Log(Enemy.return_num_enemy());
        if (finished)
        {
            return;
        }
        if (part2)
        {
            if (Enemy.return_num_enemy() == 0)
            {
                objScript.ActivateFinish();
                finished = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger entered");
        // no use if there is no level manager
        if (objScript == null)
        {
            return;
        }
        if (other.tag != "Player")
        {
            return;
        }

        objScript.SetGoalType(2, "Clear out the Lobby");
        
        enemySpawners.SetActive(false);

        part2 = true;

        Enemy.resetEnemyCount();
    }


}
