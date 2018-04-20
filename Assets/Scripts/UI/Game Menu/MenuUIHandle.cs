using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class MenuUIHandle : MonoBehaviour {

    // Attach this class to UI Canvas
    // This way it can access the other children

    GameObject text, button1, button2, button3;
	string menuSceneName = LevelManager.levels[0];
	string currSceneName = LevelManager.levels[1];
	int textNum = 0, button1Num = 1, button2Num = 2, button3Num = 3;
	bool paused = false;

	LevelManager levelManager;

    void Awake ()
    {

		DontDestroyOnLoad (this);
        text = transform.GetChild(textNum).gameObject;
        button1 = transform.GetChild(button1Num).gameObject;
        button2 = transform.GetChild(button2Num).gameObject;
		button3 = transform.GetChild(button3Num).gameObject;

		button1.transform.GetChild(0).GetComponent<Button>().onClick.AddListener (RestartLoad);
		button2.transform.GetChild(0).GetComponent<Button>().onClick.AddListener (ReturnLoad);
		button3.transform.GetChild (0).GetComponent<Button> ().onClick.AddListener (ResumeGame);

		levelManager = GameObject.Find ("Level Manager").GetComponent<LevelManager> ();

    }

	// Use this for initialization
	void Start () {
        text.SetActive(false);
        button1.SetActive(false);
        button2.SetActive(false);
		button3.SetActive(false);

		paused = false;
    }

	void Update() {
		if (Input.GetKeyDown (KeyCode.Escape)) {
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
    
		paused = true;

		Time.timeScale = 0;
	}

	public void Defeat() {
		text.SetActive(true);
		text.GetComponent<Text> ().text = "Defeat";
		text.GetComponent<Text> ().color = Color.red;
		button1.SetActive(true);
		button2.SetActive(true);

	}

	public void SetMenuTitle(string title) {
		text.GetComponent<Text>().text = title;
	}


	public void RestartLoad() 
	{
		Debug.Log ("Clicked!");
		SceneManager.LoadScene(currSceneName, LoadSceneMode.Single);
		Time.timeScale = 1;
	}

	public void ReturnLoad() 
	{
		Debug.Log ("Clicked!");
		SceneManager.LoadScene (menuSceneName, LoadSceneMode.Single);
		levelManager.UpdateLevel (0);
		Time.timeScale = 1;
	}

	public void ResumeGame() {
		Debug.Log ("Clicked!");
		button1.SetActive (false);
		button2.SetActive (false);
		button3.SetActive (false);
		text.SetActive (false);

		paused = false;

		Time.timeScale = 1;
	}

}
