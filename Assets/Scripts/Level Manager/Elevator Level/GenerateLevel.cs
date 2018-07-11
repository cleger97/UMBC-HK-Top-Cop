using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class GenerateLevel : ScriptableObject {

	[MenuItem("Tools/Generate Elevator Level")]
	public static void GenerateElevatorLevel() {
		GameObject elevatorContainer = GameObject.Find ("Elevator Container");
		GameObject backgroundContainer = GameObject.Find ("Background Container");

		// Simple rules for tile generation
		// 3 must have 4 under it or no more bounds
		// 5 should have 6 next to it and 7 / 8 under them 

		//Object[] assets = AssetDatabase.LoadAllAssetsAtPath ("Assets/Sprites/stage - elevator/");
		ArrayList assets = new ArrayList();


		string[] dataFiles = Directory.GetFiles(Application.dataPath + "/Sprites/stage - elevator/", "*.png", SearchOption.AllDirectories);

		foreach(string asset in dataFiles)
		{
			string assetPath = "Assets" + asset.Replace(Application.dataPath, "").Replace('\\', '/');
			Sprite sprite = (Sprite)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite));
			assets.Add (sprite);

		}


		foreach (Object asset in assets) {
			Debug.Log (assets);
		}

		Object elevatorPrototype =  AssetDatabase.LoadAssetAtPath<Object> ("Assets/Resources/Prefabs/Elevator.prefab");

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

		// 896 * 896
		for (int i = 0; i < 7; i++) {
			float posX = ((-384) + (i * 128)) / 100f;
			for (int j = 0; j < 7; j++) {
				float posY = ((-384) + (j * 128)) / 100f;

				Vector3 newPosition = new Vector3 (posX, posY, 0f);

				GameObject newPanel = (GameObject) Instantiate (elevatorPrototype, newPosition, Quaternion.identity, elevatorContainer.transform);

				newPanel.name = "Panel x: " + posX + " y: " + posY;

				newPanel.GetComponent<SpriteRenderer> ().sprite = (Sprite)assets [3];
				newPanel.GetComponent<SpriteRenderer> ().sortingLayerName = "Elevator";
				newPanel.GetComponent<SpriteRenderer> ().sortingOrder = 3;

			}
		}
	}

	[MenuItem("Tools/Clear Elevator Assets")]
	public static void ClearElevatorAssets() {
		GameObject elevatorContainer = GameObject.Find ("Elevator Container");
		Transform container = elevatorContainer.transform;

		while (container.childCount > 0) {
			Transform t = container.GetChild (0);
			t.SetParent (null);
			DestroyImmediate (t.gameObject);
		}

	}

	[MenuItem("Tools/Cleanup Elevator")] 
	public static void CleanupElevator() {
		
		GameObject elevatorContainer = GameObject.Find ("Elevator Container");
		Transform container = elevatorContainer.transform;

		for (int i = 0; i < container.childCount; i++) {
			GameObject p = container.GetChild (i).gameObject;
			p.name = "Elevator x: " + p.transform.position.x + " y: " + p.transform.position.y;

		}
	}
		

	[MenuItem("Tools/Generate Girders")] 
	public static void GenerateElevatorGirders() {
		GameObject backgroundContainer = GameObject.Find ("Background Container");
		Transform container = backgroundContainer.transform;

		ArrayList assets = new ArrayList();

		string[] dataFiles = Directory.GetFiles(Application.dataPath + "/Sprites/stage - elevator/", "*.png", SearchOption.AllDirectories);

		foreach(string asset in dataFiles)
		{
			string assetPath = "Assets" + asset.Replace(Application.dataPath, "").Replace('\\', '/');
			Sprite sprite = (Sprite)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite));
			assets.Add (sprite);

		}


		foreach (Object asset in assets) {
			Debug.Log (assets);
		}

		Object elevatorPrototype =  AssetDatabase.LoadAssetAtPath<Object> ("Assets/Prefabs/Elevator.prefab");

		// 2560 x 2560
		// A lot of girders.
		for (int i = 0; i < 40; i++) {
			float posX = ((64 - 1280) + (i * 64)) / 100f;
			for (int j = 0; j < 40; j++) {
				float posY = ((64 - 1280) + (j * 64)) / 100f;

				Vector3 newPosition = new Vector3 (posX, posY, 0f);

				GameObject newPanel = (GameObject) Instantiate (elevatorPrototype, newPosition, Quaternion.identity, backgroundContainer.transform);

				newPanel.name = "Girder x: " + posX + " y: " + posY;

				// girder in assets
				// every 3rd girder should be a normal otherwise black tile
				int num = (j % 4 == 0) ? 2 : 0;
				newPanel.GetComponent<SpriteRenderer> ().sprite = (Sprite)assets [num];
				newPanel.GetComponent<SpriteRenderer> ().sortingLayerName = "Elevator";
				newPanel.GetComponent<SpriteRenderer> ().sortingOrder = 2;

			}
		}

	}

	[MenuItem("Tools/Clear Girder Assets")]
	public static void ClearGirderAssets() {
		GameObject backgroundContainer = GameObject.Find ("Background Container");
		Transform container = backgroundContainer.transform;

		while (container.childCount > 0) {
			Transform t = container.GetChild (0);
			t.SetParent (null);
			DestroyImmediate (t.gameObject);
		}

	}

	[MenuItem("Tools/Count Elevator Tiles")]
	public static void DisplayTiles() {
		GameObject elevatorContainer = GameObject.Find ("Elevator Container");
		Transform container = elevatorContainer.transform;


		Debug.Log (container.childCount);
	}

	[MenuItem("Tools/Generate Flooring")] 
	public static void GenerateFlooring() {

		GameObject floorContainer = GameObject.Find ("Wall Container");

		Object floorPrototype =  AssetDatabase.LoadAssetAtPath<Object> ("Assets/Prefabs/Flooring.prefab");

		for (int j = 0; j < 7; j++) {
			float posX = ((-384) + (j * 128)) / 100f;
			float posY = -4.8f;

			Vector3 newPosition = new Vector3 (posX, posY, 0f);

			GameObject newPanel = (GameObject) Instantiate (floorPrototype, newPosition, Quaternion.identity, floorContainer.transform);

			newPanel.name = "Floor x: " + posX + " y: " + posY;

			//newPanel.GetComponent<SpriteRenderer> ().sortingLayerName = "Elevator";
			//newPanel.GetComponent<SpriteRenderer> ().sortingOrder = 3;

		}
	}

	[MenuItem("Tools/Clear Floor Assets")]
	public static void ClearFloorAssets() {
		GameObject backgroundContainer = GameObject.Find ("Wall Container");
		Transform container = backgroundContainer.transform;

		while (container.childCount > 0) {
			Transform t = container.GetChild (0);
			t.SetParent (null);
			DestroyImmediate (t.gameObject);
		}

	}
}
#endif