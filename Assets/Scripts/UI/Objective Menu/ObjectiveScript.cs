using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveScript : MonoBehaviour {

    public Text goalText;
    public Text progressText;
    public enum TypeOfGoal { KillCounter, TimeSurvival, Finish };

	public MenuUIHandle MenuHandler;

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

	// Use this for initialization
	void Start () {
		
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
			Victory();
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
