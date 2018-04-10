using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ObjectiveScript : MonoBehaviour {

    public Text goalText;
    public Text progressText;
    public enum TypeOfGoal { KillCounter, TimeSurvival, Finish };

	public MenuUIHandle MenuHandler;

	private LevelManager levelManager;

    // initiate to kill counter
    private int currentGoal = (int)TypeOfGoal.KillCounter;

    // progress vars
    private int killCount;
    private int timeSurvived;
    private bool didFinish;

    // goal vars
    private int killGoal;
    private int timeToSurvive;
    // finish doesn't really need one (yet)

	private bool isVictory = false;

	// UI handle
	public bool UIShow = false;

	void Awake() {
		// If a level manager and objective script already exist destroy this new one
		if (GameObject.FindGameObjectsWithTag ("GameController").Length > 2) {
			Destroy (this.gameObject);
		}
		// Otherwise, Objective Script shouldn't be destroyed on load.
		DontDestroyOnLoad (this);

		levelManager = GameObject.Find ("Level Manager").GetComponent<LevelManager> ();

		if (SceneManager.GetActiveScene () == SceneManager.GetSceneByName (LevelManager.levels [0])) {
			foreach (Transform objects in transform) {
				objects.gameObject.SetActive (false);

			}
			UIShow = false;
		}
	}

	// Use this for initialization
	void Start () {
		
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
        timeSurvived = 0;
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
                    timeToSurvive = goal; 
                    goalText.text = "Goal: Survive for " + goal + " minutes.";
                    progressText.text = "Time Survived: 0 minutes.";
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
        progressText.text = "Enemies Killed: " + killCount + " / " + killGoal;


        if (killCount >= killGoal && !isVictory)
        {
			levelManager.NextLevel ();
            // victory or whatever goes here
            // TODO: implement victory
        }
    }

	public void Victory() {
		isVictory = true;
		goalText.text = "Success!";
		progressText.text = "";

	}
}
