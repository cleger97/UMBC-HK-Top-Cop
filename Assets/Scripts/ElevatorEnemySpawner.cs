using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorEnemySpawner : MonoBehaviour {


	private ObjectiveScript objective;
	private Player playerData;
	public GameObject enemyPrototype;

	public Transform[] spawnPoints;

	public Transform enemyContainer;

	public Sprite enemyJump;
	public Sprite enemyNormal;

	private const float checkRate = 0.5f;
	private const float spawnRate = 2f;

	private const float maxEnemies = 3;

	private float spawnTimer;
	// Use this for initialization
	void Start () {
		GameObject playerCharacter = GameObject.Find ("Player");
		if (playerCharacter == null) {
			Debug.LogWarning ("Player GameObject does not exist in scene!");
			playerData = null;
		} else {
			playerData = playerCharacter.GetComponent<Player> ();
			if (playerData == null) {
				Debug.LogWarning ("Player script does not exist in scene!");
			}
		}

		objective = ObjectiveScript.instance;

		// start spawns 5 seconds in and continue every 2 seconds
		spawnTimer = 2f;
	}

	void Update() {
		if (spawnTimer > 0) {
			spawnTimer -= Time.deltaTime;
			if (spawnTimer < 0) {
				spawnTimer = 0;
			}
			return;
		}

		Spawn ();

		if (objective != null) {
			spawnTimer = spawnRate * objective.GetPercentRemaining ();
		} else {
			spawnTimer = spawnRate;
		}
	}

	// Update is called once per frame
	void Spawn() {
		if (playerData == null || playerData.IsPlayerAlive () == false) {
			return;
		}

		if (enemyContainer.childCount < maxEnemies) {
			int spawnPoint = Random.Range (0, spawnPoints.Length);

			Enemy toSpawn = Enemy.InstantiateNew (spawnPoints [spawnPoint].position);

			Setup (toSpawn.gameObject);

		}
	}

	void Setup(GameObject enemyToSet) {
		enemyToSet.GetComponent<Animator> ().enabled = false;
		enemyToSet.AddComponent (typeof(EnemyFix));

		enemyToSet.GetComponent<SpriteRenderer> ().sprite = enemyJump;

		enemyToSet.transform.localScale = new Vector3 (2, 2, 2);

		enemyToSet.GetComponent<SpriteRenderer> ().sortingLayerName = "Elevator";
		enemyToSet.GetComponent<SpriteRenderer> ().sortingOrder = 5;

		enemyToSet.transform.GetChild (1).GetComponent<Canvas> ().sortingLayerName = "Elevator";
		enemyToSet.transform.GetChild (1).GetComponent<Canvas> ().sortingOrder = 5;

	}
}
