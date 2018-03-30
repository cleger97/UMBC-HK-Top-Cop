using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour {

	// Use this for initialization
	string sceneName = "scene1";
	Button button;
	void Awake() {
		button = transform.Find ("Button").GetComponent<Button> ();
		button.onClick.AddListener (Load);
	}

	public void Load() 
	{
		Debug.Log ("Clicked!");
		SceneManager.LoadScene (this.sceneName, LoadSceneMode.Single);
		Time.timeScale = 1;
	}
}
