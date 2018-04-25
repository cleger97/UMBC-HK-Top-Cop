﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
	
	public static LevelManager instance = null;

	public static string[] levels = { "menu", "Scene 1", "Scene 2", "Victory" };
	public int currentLevel = 0;

	public static int VictoryLevel = 3;


	ObjectiveScript objectiveHandle;

	// Use this for initialization
	void Awake () {
		// If a level manager and objective script already exist destroy this new one
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
		// Otherwise, Level Managers shouldn't be destroyed on load.
		DontDestroyOnLoad(this);

	}

	void Start() {
		objectiveHandle = ObjectiveScript.instance;
	}

	// Level Manager shouldn't need to update
	// Call each function it needs accordingly

	// use this when manually changing level
	public void UpdateLevel(int level) {
		if (currentLevel == 0 && level != 0) {
			if (MenuUIHandle.instance == null) {
				Debug.LogError ("Menu UI handle not initalized!");
				return;
			} else {
				MenuUIHandle.instance.StartGame ();
			}
		}
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
		if (objectiveHandle.UIShow == false && currentLevel != 0) {
			objectiveHandle.ActivateObjects ();
		}
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
