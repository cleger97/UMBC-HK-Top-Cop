﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
	
	public static LevelManager instance = null;

    // live version
    public static string[] levels = {"menu", "Scene 1", "Lobby", "Scene 2", "Scene 3", "Victory"};

    // experimental version
    //public static string[] levels = { "menu", "Scene 3", "Victory"};
	public int currentLevel = 0;

	public static int VictoryLevel = 5;

	public ObjectiveScript objectiveHandle;

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

        SceneManager.sceneLoaded += OnSceneLoaded;

	}

	void Start() {
		objectiveHandle = ObjectiveScript.instance;
	}


    // Level Manager shouldn't need to update
    // Call each function it needs accordingly

    private void DoWhenUpdateLevel() {

		Enemy.resetEnemyCount ();

		if (currentLevel != 0) {
			MenuUIHandle.instance.DisableControls ();
		} else {
			MenuUIHandle.instance.EnableControls ();
		}
        Debug.Log(objectiveHandle);
        if (objectiveHandle.UIShow == false && currentLevel != 0)
        {
            objectiveHandle.ActivateObjects();
        }

        if (levels [currentLevel] == "Victory") {
			objectiveHandle.Victory ();
		}

        if (currentLevel != 0 && currentLevel != VictoryLevel)
        {
            Debug.Log(SceneManager.GetActiveScene().name);
            objectiveHandle.UpdatePlayer();
        }
	}

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == levels[0] || scene.name == levels[VictoryLevel]) { return; }
        DoWhenUpdateLevel();
    }

	// use this when manually changing level
	public void UpdateLevel(int level) {
		
		currentLevel = level;
    }

	public void NextLevel() {
		currentLevel++;
		if (currentLevel > levels.Length) {
			// that's all folks
			// todo: throw some victory stuff here
		} else {
			SceneManager.LoadScene (levels [currentLevel], LoadSceneMode.Single);
		}
	}

    public void RestartLevel()
    {
        SceneManager.LoadScene(levels[currentLevel], LoadSceneMode.Single);
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
