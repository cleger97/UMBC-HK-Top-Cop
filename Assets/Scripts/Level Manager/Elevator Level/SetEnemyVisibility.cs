using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// deprecated class

public class SetEnemyVisibility : MonoBehaviour {
		
	// Update is called once per frame
	void Update () {
		GameObject[] list = GameObject.FindGameObjectsWithTag ("Enemy");
		foreach (GameObject e in list) {
			

			e.transform.localScale = new Vector3 (2, 2, 2);
		}
	}
}
