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

		Physics2D.IgnoreCollision ((Collider2D) collider, (Collider2D) playerCollider, true);
		normalScript.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (this.transform.position.y < -3.0) {
			Physics2D.IgnoreCollision ((Collider2D) collider, (Collider2D) playerCollider, false);
			normalScript.enabled = true;

			this.GetComponent<Animator> ().enabled = true;

			Destroy (this);
		}
	}
}
