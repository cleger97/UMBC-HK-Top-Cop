using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetEnemyVisibility : MonoBehaviour {
		
	// Update is called once per frame
	void Update () {
		GameObject[] list = GameObject.FindGameObjectsWithTag ("Enemy");
		foreach (GameObject e in list) {
			e.GetComponent<SpriteRenderer> ().sortingLayerName = "Elevator";
			e.GetComponent<SpriteRenderer> ().sortingOrder = 5;

			e.transform.localScale = new Vector3 (2, 2, 2);
		}
	}
}
