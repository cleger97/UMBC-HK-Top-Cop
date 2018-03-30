using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Player_Data playerData;
    public GameObject enemy;
    private float spawnTime = 7f;
    public Transform[] spawnPoints;
    private int totalEnemySpawned = 0;
    private int doubleEnemySize = 5;
    private int numToSpawn = 0;
    // Use this for initialization
    void Start()
    {
        InvokeRepeating("Spawn", spawnTime, spawnTime);
    }

    // Update is called once per frame
    void Spawn()
    {
        if (totalEnemySpawned % doubleEnemySize == 0)
        {
            numToSpawn++;
            spawnTime = 7f;
        }
        if (totalEnemySpawned % doubleEnemySize / 4 == 0 && spawnTime >3)
            spawnTime -= 1f;

        if (playerData.isPlayerAlive() == false)
        {
            return;
        }
        
        for (int i = 0; i < numToSpawn; i++) {
            int spawnPointIndex = Random.Range(0, spawnPoints.Length);
            Instantiate(enemy, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
        }
        totalEnemySpawned++;
    }
}
