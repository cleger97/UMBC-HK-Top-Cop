using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class PlayGameButton : MonoBehaviour {

	string sceneName = LevelManager.levels[1];
	LevelManager levelManager;
    Button button;
    public void Awake()
    {
        button = transform.Find("Button").GetComponent<Button>();
        button.onClick.AddListener(Load);

    }

	void Start() {
		levelManager = LevelManager.instance;
	}

	void Update() {
		// Controller start buttons/All Fire Buttons/Computer Enter and Return buttons
		if (Input.GetButtonDown ("Fire1") || Input.GetButtonDown("Submit")) {
			Load ();
		}
	}

    public void Load()
    {
        Debug.Log("Loaded game!");
		levelManager.UpdateLevel (1);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

    }
}
