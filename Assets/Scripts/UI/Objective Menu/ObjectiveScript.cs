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
	private int timeGoal;

    // finish vars
    private string finishText;
    private string finishObjective;

	private bool isVictory = false;

    private Player player;

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
            GotoNext();
		}

	}

	public void ActivateObjects() {
		foreach (Transform objects in transform) {
			objects.gameObject.SetActive (true);
		}
		UIShow = true;

	}

    public void UpdatePlayer()
    {
        //player = GameObject.Find("Player").GetComponent<Player>();
    }

	public float GetPercentRemaining() {
		switch (currentGoal) {
		case 0:
			return 1f - (killCount / killGoal);
		case 1:
			return (timeLeft / timeGoal);
		// case 2 and 3: default
		default:
			return 0f;
		}
	}

    // set goal for numerical endings
    public void SetGoalType(int type, int goal)
    {
        currentGoal = type;
        killCount = 0;
        timeLeft = 0;
        didFinish = false;
		isVictory = false;

        ActivateObjects();

        switch (type)
        {
            case 0:
                {
					killCount = 0;
                    killGoal = goal;
                    goalText.text = "Goal: Kill " + killGoal + " enemies.";
                    progressText.text = "Enemies killed: 0 / " + killGoal;
                    break;

                }
            case 1:
                {
					timeGoal = goal;
					timeLeft = goal;
					goalText.text = "Goal: Survive for " + (goal/60).ToString("D2") + ":" + (goal%60).ToString("D2");
                    progressText.text = "Time left: ";
                    break;
                }
            case 2:
                Debug.LogWarning("Attempted to assign a finish goal with a count");
                break;
            default:
                break;
        }
    }

    private void GotoNext()
    {
        currentGoal = (int)TypeOfGoal.None;
        levelManager.NextLevel();
        
    }

    // set goal for finish - goal text
    public void SetGoalType(int type, string text1)
    {
        // Progress Text - Child # of Objective Canvas
        int progText = 0;

        currentGoal = type;
        killCount = 0;
        timeLeft = 0;
        didFinish = false;
        isVictory = false;

        ActivateObjects();

        transform.GetChild(progText).gameObject.SetActive(false);

        switch (type)
        {
            case 0:
                Debug.LogWarning("Attempted to assign a kill counter without a goal count");
                break;
            case 1:
                Debug.LogWarning("Attempted to assign a timed goal without a time");
                break;
            case 2:
                goalText.text = text1;
                break;
            default:
                break;
        }

    }

    public int GetGoalType()
    {
        return (int)currentGoal;
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
            GotoNext();
            // victory or whatever goes here
            // TODO: implement victory
        }
    }

    public void ActivateFinish()
    {
        if (currentGoal != 2)
        {
            Debug.LogWarning("Attempted to manually finish a level without the 'Finish' tag.");
            return;
        }

        GotoNext();
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
