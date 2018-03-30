using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainScButton : MonoBehaviour {

	// Use this for initialization
	string sceneName = "menu";
	Button button;
	void Start () {
		button = transform.Find ("Button").GetComponent<Button> ();
		button.onClick.AddListener (Load);
	}
	
	public void Load() 
	{
		Debug.Log ("Clicked!");
		SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
		Time.timeScale = 1;
	}
}
