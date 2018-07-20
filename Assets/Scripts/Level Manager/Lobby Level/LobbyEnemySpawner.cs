using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Note: This class (and ElevatorEnemySpawner) can be re-created to be child classes of a
// well built EnemySpawner class.
public class LobbyEnemySpawner : MonoBehaviour {

    private ObjectiveScript objective;
    private Player playerData;
    public GameObject enemyPrototype;

    public Transform[] spawnPoints;

    public Transform enemyContainer;

    private const float checkRate = 0.5f;
    private const float spawnRate = 4f;

    private const float maxEnemies = 8;

    private float spawnTimer;

    public Sprite enemyJump;

    bool oSpawn = false;

    void Start () {
        GameObject playerCharacter = GameObject.Find("Player");
        if (playerCharacter == null)
        {
            Debug.LogWarning("Player GameObject does not exist in scene!");
            playerData = null;
        }
        else
        {
            playerData = playerCharacter.GetComponent<Player>();
            if (playerData == null)
            {
                Debug.LogWarning("Player script does not exist in scene!");
            }
        }

        objective = ObjectiveScript.instance;
        if (objective == null)
        {
            Debug.LogWarning("No ObjectiveScript in scene!");
        }

        // start spawns 5 seconds in and continue every 2 seconds
        spawnTimer = 2f;
    }

    void Update()
    {
        if (spawnTimer > 0)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer < 0)
            {
                spawnTimer = 0;
            }
            return;
        }

        Spawn();

        spawnTimer = spawnRate;
    }

    // Update is called once per frame
    void Spawn()
    {
        oSpawn = !oSpawn;

        if (playerData == null || playerData.IsPlayerAlive() == false)
        {
            return;
        }

        if (enemyContainer.childCount < maxEnemies)
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                // ospawn -> spawn at odd points, don't spawn at even points
                if (oSpawn) { if (i % 2 == 0) continue; }
                else { if (i % 2 == 1) continue; }
                int spawnPoint = i;

                GameObject toSpawn = Instantiate(enemyPrototype, spawnPoints[spawnPoint].position, Quaternion.identity, enemyContainer);

                Setup(toSpawn);
            }
            

        }
    }

    void Setup(GameObject enemyToSet)
    {
        enemyToSet.GetComponent<Animator>().enabled = false;
        enemyToSet.AddComponent(typeof(EnemySpawnScript));

        enemyToSet.GetComponent<SpriteRenderer>().sprite = enemyJump;

        enemyToSet.GetComponent<NewEnemy>().SetIdleState(false);


    }
}
