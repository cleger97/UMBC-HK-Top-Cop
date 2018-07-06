using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    private Player playerData = null;
    public GameObject enemy;
    private Enemy enemyData = null;

    private const float checkRate = 0.5f;
    private const float spawnRate = 2f;

    private float repeatRate = 3f;
    private float startSpawn = 5f;
    public Transform[] spawnPoints;
    private int numWaves = 1;
    private int doubleEnemySize = 5;
    private int numToSpawn = 1;
    // Use this for initialization
    void Start()
    {
        playerData = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        GameObject foundEnemy = GameObject.FindGameObjectWithTag("Enemy");
        if (foundEnemy == null)
        {
            Debug.LogWarning("No enemy initalized");
        }
        else
        {
            enemyData = foundEnemy.GetComponent<Enemy>();
        }
        InvokeRepeating("Spawn", startSpawn, repeatRate);

    }

    // Update is called once per frame
    void Spawn()
    {


        if (playerData.IsPlayerAlive() == false)
        {
            return;
        }

        if (numWaves == 1)
        {

            int spawnPointIndex = Random.Range(0, spawnPoints.Length);
            Enemy.InstantiateBoss(spawnPoints[spawnPointIndex].position);
        }


        if (Enemy.return_num_enemy() <= numToSpawn)
        {
            int spawnPointIndex = Random.Range(0, spawnPoints.Length);
		    Enemy spawned = Enemy.InstantiateNew (spawnPoints [spawnPointIndex].position);
            if (enemyData == null)
            {
                enemyData = spawned;
            }
            numWaves++;
		    repeatRate = spawnRate * ObjectiveScript.instance.GetPercentRemaining();
        }
        else
            repeatRate = checkRate;

        


    }
}