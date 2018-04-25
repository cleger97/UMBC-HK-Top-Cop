using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReturnToMain : MonoBehaviour {

	public LevelManager levelManager;
	public GameObject button;


	void Awake() {
		levelManager = GameObject.Find ("Level Manager").GetComponent<LevelManager> ();
		//button = this.transform.GetChild (0).gameObject;

	}
	void Start() {
		button.GetComponent<Button> ().onClick.AddListener (Load);
	}

	// Update is called once per frame
	void Load() {
		levelManager.UpdateLevel (0);
	}
}
