using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnScript : MonoBehaviour {

    public Enemy normalScript;
    public BoxCollider2D collider;

	void Start () {
        GameObject player = GameObject.Find("Player");
        if (player == null)
        {
            Debug.LogWarning("Player not found in scene!");
        }

        collider = this.GetComponent<BoxCollider2D>();
        normalScript = this.GetComponent<Enemy>();
        // Falling enemy - doesn't collide w/ enemy or player
        this.gameObject.layer = 11;
        normalScript.enabled = false;

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Enemy landed");
        if (other.gameObject.layer == 8)
        {
            // Not-falling anymore - will collide with player and enemy
            this.gameObject.layer = 10;
            normalScript.enabled = true;

            this.GetComponent<Animator>().enabled = true;

            Destroy(this);
        }

    }
}
