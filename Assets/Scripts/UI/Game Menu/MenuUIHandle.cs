using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class MenuUIHandle : MonoBehaviour {

    // Attach this class to UI Canvas
    // This way it can access the other children
	public static MenuUIHandle instance = null;

    GameObject text, select, button1, button2, button3, gameOver, controls;
	string menuSceneName = LevelManager.levels[0];
	public string currSceneName;
	int textNum = 0, gameOverScreen = 1, selection = 2, button1Num = 3, button2Num = 4, button3Num = 5, buttonDiagram = 6;
	public bool paused = false;

	ObjectiveScript objective;

	LevelManager levelManager;

    void Awake ()
    {
		// If a level manager and objective script and a menu UI already exist destroy this new one
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		DontDestroyOnLoad (this);



        text = transform.GetChild(textNum).gameObject;
		select = transform.GetChild (selection).gameObject;
        button1 = transform.GetChild(button1Num).gameObject;
        button2 = transform.GetChild(button2Num).gameObject;
		button3 = transform.GetChild(button3Num).gameObject;
		gameOver = transform.GetChild (gameOverScreen).gameObject;
		controls = transform.GetChild (buttonDiagram).gameObject;



    }

	// Use this for initialization
	void Start () {
		levelManager = LevelManager.instance;
		objective = ObjectiveScript.instance;

		button1.transform.GetChild(0).GetComponent<Button>().onClick.AddListener (RestartLoad);
		button2.transform.GetChild(0).GetComponent<Button>().onClick.AddListener (ReturnLoad);
		button3.transform.GetChild (0).GetComponent<Button> ().onClick.AddListener (ResumeGame);
		MainMenu ();
        
    }

	void MainMenu() {
		text.SetActive(false);
		select.SetActive (false);
		button1.SetActive(false);
		button2.SetActive(false);
		button3.SetActive(false);
		controls.SetActive (true);

		paused = false;

	}

	public void EnableControls() {
		controls.SetActive (true);
	}

	public void DisableControls() {
		controls.SetActive (false);
	}

	// GUI input detects input as soon as it fires
	void Update() {
		Debug.Log (levelManager.currentLevel);
		if (levelManager.currentLevel == 0) {
			return;
		}
		currSceneName = LevelManager.levels [levelManager.currentLevel];
		if (Input.GetKeyDown (KeyCode.Escape) || Input.GetButtonDown("Menu")) {
			if (!paused) {
				Pause ();
			} else {
				ResumeGame ();

			}
		}
	}


	// No need to update.
    public void Pause()
    {
        text.SetActive(true);
		text.GetComponent<Text> ().text = "Paused";
		text.GetComponent<Text>().color = Color.blue;

		button1.SetActive(true);
        button2.SetActive(true);
		button3.SetActive(true);
		controls.SetActive (true);
    
		paused = true;

		select.SetActive (true);

		Time.timeScale = 0;
	}

	public void Defeat() {
		gameOver.SetActive (true);

		objective.Defeat ();
		text.SetActive(true);
		text.GetComponent<Text> ().text = "Defeat";
		text.GetComponent<Text> ().color = Color.red;
		button1.SetActive(true);
		button2.SetActive(true);
		paused = false;

		select.SetActive (true);

	}

	public void SetMenuTitle(string title) {
		text.GetComponent<Text>().text = title;
	}

	public void DisableObjects() {
		select.SetActive (false);
		gameOver.SetActive (false);
		button1.SetActive (false);
		button2.SetActive (false);
		button3.SetActive (false);
		text.SetActive (false);
		controls.SetActive (false);
	}

	public void RestartLoad() 
	{
		DisableObjects ();

		objective.ActivateObjects ();
		Debug.Log ("Clicked!");
		SceneManager.LoadScene(currSceneName, LoadSceneMode.Single);
		paused = false;
		Time.timeScale = 1;
	}

	public void ReturnLoad() 
	{
		DisableObjects ();

		Debug.Log ("Clicked!");
		SceneManager.LoadScene (menuSceneName, LoadSceneMode.Single);
		levelManager.UpdateLevel (0);
		paused = false;
		Time.timeScale = 1;
	}

	public void ResumeGame() {
		Debug.Log ("Clicked!");
		DisableObjects ();

		paused = false;

		Time.timeScale = 1;
	}

}
