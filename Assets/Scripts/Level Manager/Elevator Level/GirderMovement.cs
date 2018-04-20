using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirderMovement : MonoBehaviour {

	private Transform container;
	private Transform[] tiles;
	private Vector3 velocity;

	// Use this for initialization
	void Awake () {
		velocity = new Vector3 (0f, -2.0f, 0f);

		container = this.gameObject.transform;
		tiles = new Transform[container.childCount];

		for (int i = 0; i < container.childCount; i++) {
			tiles [i] = container.GetChild (i);
		}
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < container.childCount; i++) {
			if (tiles [i].position.y <= -12.20f) {
				float difference = tiles [i].position.y + 12.20f;
				tiles [i].position = new Vector3 (tiles [i].position.x, 12.16f + difference, tiles [i].position.z);
			}

			tiles [i].gameObject.GetComponent<Rigidbody2D> ().velocity = velocity;
		}
	}
}
