using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GenerateLevel : MonoBehaviour {

	[MenuItem("AdditionalTools/Generate Elevator Level")]
	public void GenerateElevatorLevel() {
		GameObject elevatorContainer = GameObject.Find ("Elevator Container");
		GameObject backgroundContainer = GameObject.Find ("Background Container");

		// Simple rules for tile generation
		// 3 must have 4 under it or no more bounds
		// 5 should have 6 next to it and 7 / 8 under them 



	}


}
