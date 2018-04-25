using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFix : MonoBehaviour {

	public TempEnemy normalScript;
	public BoxCollider2D collider;
	public BoxCollider2D playerCollider;
	// Use this for initialization
	void Start () {
		GameObject player = GameObject.Find ("Player");
		if (player == null) {
			Debug.LogWarning ("Player not found in scene!");
		} else {
			playerCollider = player.GetComponent<BoxCollider2D> ();
			if (playerCollider == null) {
				Debug.LogError ("Player has no collider! Removing script...");
				Destroy (this);
			}
		}

		collider = this.GetComponent<BoxCollider2D> ();
		normalScript = this.GetComponent<TempEnemy> ();
		// Falling enemy - doesn't collide w/ enemy or player
		this.gameObject.layer = 11;
		normalScript.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (this.transform.position.y < -3.0) {
			// Not-falling anymore - will collide with player and enemy
			this.gameObject.layer = 10;
			normalScript.enabled = true;

			this.GetComponent<Animator> ().enabled = true;

			Destroy (this);
		}
	}
}
