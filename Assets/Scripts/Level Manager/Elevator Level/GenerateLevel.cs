using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GenerateLevel : ScriptableObject {

	[MenuItem("AdditionalTools/Generate Elevator Level")]
	public void GenerateElevatorLevel() {
		GameObject elevatorContainer = GameObject.Find ("Elevator Container");
		GameObject backgroundContainer = GameObject.Find ("Background Container");

		// Simple rules for tile generation
		// 3 must have 4 under it or no more bounds
		// 5 should have 6 next to it and 7 / 8 under them 

		int elevTileSqSize = 128;

		int girdTileSqSize = 64;

		// 1 to 2 rows of black tiles per every row of girders

		bool [][] listOfTiles = new bool [10][];
		for (int i = 0; i < 10; i++) {
			listOfTiles [i] = new bool[10];
			for (int j = 0; j < 10; j++) {
				listOfTiles[i][j] = false;
			}
		}

		// 1280x1280
		for (int i = 0; i < 10; i++) {
			int posX = (640 - 64) + (i * 128);
			for (int j = 0; j < 10; j++) {
				int posY = (640 - 64) + (j * 128);

				Vector3 newPosition = new Vector3 (posX, posY, 0f);




			}
		}


	}


}
