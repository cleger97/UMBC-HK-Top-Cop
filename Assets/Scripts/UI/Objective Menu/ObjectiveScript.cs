using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ObjectiveScript : MonoBehaviour {

	public static ObjectiveScript instance = null;

    public Text goalText;
    public Text progressText;
    public enum TypeOfGoal { KillCounter, TimeSurvival, Finish, None };

	MenuUIHandle MenuHandler;

	LevelManager levelManager;

	private int healthBar = 2;

    // initiate to kill counter
    private int currentGoal = (int)TypeOfGoal.KillCounter;

    // progress vars
    private int killCount;
    private float timeLeft;
    private bool didFinish;

    // goal vars
    private int killGoal;
    // finish doesn't really need one (yet)

	private bool isVictory = false;

	// UI handle
	public bool UIShow = false;

	void Awake() {
		// If a level manager and objective script already exist destroy this new one
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
		// Otherwise, Objective Script shouldn't be destroyed on load.
		DontDestroyOnLoad (this);



		if (SceneManager.GetActiveScene () == SceneManager.GetSceneByName (LevelManager.levels [0])) {
			foreach (Transform objects in transform) {
				objects.gameObject.SetActive (false);

			}
			UIShow = false;
		}
	}

	void Start() {
		levelManager = LevelManager.instance;
		MenuHandler = MenuUIHandle.instance;
	}

	void Update() {
		// objective UI only needs to update by itself when it's a timed level
		if (currentGoal != 1) {
			return;
		}
		timeLeft -= Time.deltaTime;
		if (timeLeft > 0) {
			int minutes = (int)timeLeft / 60;
			int seconds = (int)timeLeft % 60;

			progressText.text = "Time Left: " + minutes.ToString("D2") + ":" + seconds.ToString("D2");
		} else {
			levelManager.NextLevel ();
			currentGoal = (int) TypeOfGoal.None;
		}

	}

	public void ActivateObjects() {
		foreach (Transform objects in transform) {
			objects.gameObject.SetActive (true);
		}
		UIShow = true;

	}

    public void SetGoalType(int type, int goal)
    {
        currentGoal = type;
        killCount = 0;
        timeLeft = 0;
        didFinish = false;
		isVictory = false;
        switch (type)
        {
            case 0:
                {
                    killGoal = goal;
                    goalText.text = "Goal: Kill " + killGoal + " enemies.";
                    progressText.text = "Enemies killed: 0 / " + killGoal;
                    break;

                }
            case 1:
                {
					timeLeft = goal;
					goalText.text = "Goal: Survive for " + (goal/60).ToString("D2") + ":" + (goal%60).ToString("D2");
                    progressText.text = "Time left: ";
                    break;
                }
            case 2:
            default:
                break;
        }
    }
	
    public void UpdateKillCount(int howMany)
    {


        killCount += howMany;

		if (currentGoal != (int)TypeOfGoal.KillCounter) {
			return;
		}

        progressText.text = "Enemies Killed: " + killCount + " / " + killGoal;


        if (killCount >= killGoal && !isVictory)
        {
			levelManager.NextLevel ();
			currentGoal = (int) TypeOfGoal.None;
            // victory or whatever goes here
            // TODO: implement victory
        }
    }

	public void Defeat() {
		goalText.gameObject.transform.parent.gameObject.SetActive (false);
		progressText.gameObject.transform.parent.gameObject.SetActive (false);
		UIShow = false;
	}

	public void Victory() {
		isVictory = true;
		goalText.text = "Success!";
		progressText.text = "";

		UIShow = false;

		goalText.gameObject.transform.parent.gameObject.SetActive (false);
		progressText.gameObject.transform.parent.gameObject.SetActive (false);

		this.transform.GetChild (healthBar).gameObject.SetActive (false);
	}
}
