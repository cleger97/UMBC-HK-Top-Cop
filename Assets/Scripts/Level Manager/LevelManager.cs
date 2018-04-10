using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

	public static string[] levels = { "menu", "Scene 1", "Victory" };
	public int currentLevel;

	private ObjectiveScript objectiveHandle;

	// Use this for initialization
	void Awake () {
		// If a level manager and objective script already exist destroy this new one
		if (GameObject.FindGameObjectsWithTag ("GameController").Length > 2) {
			Destroy (this.gameObject);
		}
		// Otherwise, Level Managers shouldn't be destroyed on load.
		DontDestroyOnLoad(this);

		objectiveHandle = GameObject.Find ("Objective Canvas").GetComponent<ObjectiveScript>();
		// Start at level zero - menu level
		currentLevel = 0;
	}

	// Level Manager shouldn't need to update
	// Call each function it needs accordingly

	// use this when manually changing level
	public void UpdateLevel(int level) {
		currentLevel = level;
		if (objectiveHandle.UIShow == false && currentLevel != 0) {
			objectiveHandle.ActivateObjects ();
		}
	}

	public void NextLevel() {
		currentLevel++;
		if (currentLevel > levels.Length) {
			// that's all folks
			// todo: throw some victory stuff here
		} else {
			SceneManager.LoadScene (levels [currentLevel], LoadSceneMode.Single);
		}

		if (levels [currentLevel] == "Victory") {
			objectiveHandle.Victory ();
		}
	}

	public void LoadSpecificLevel(int level) {
		currentLevel = level;
		SceneManager.LoadScene (levels [level], LoadSceneMode.Single);
	}

	public void LoadSpecificLevel(string level) {
		int oldLevel = currentLevel;
		for (int i = 0; i < levels.Length; i++) {
			if (level.ToLower () == levels [i].ToLower ()) {
				currentLevel = i;
				break;
			}
		}
		if (oldLevel != currentLevel) {
			// new level selected
			SceneManager.LoadScene(levels[currentLevel], LoadSceneMode.Single);
		}
	}

}
