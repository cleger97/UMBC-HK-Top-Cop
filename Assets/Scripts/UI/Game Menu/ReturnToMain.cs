using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReturnToMain : MonoBehaviour {

	public LevelManager levelManager;
	public GameObject button;


	void Awake() {
		levelManager = LevelManager.instance;
		//button = this.transform.GetChild (0).gameObject;

	}
	void Start() {
		button.GetComponent<Button> ().onClick.AddListener (Load);
		Debug.Log (button);
	}

	// Update is called once per frame
	public void Load() {
		levelManager.LoadSpecificLevel (0);
	}

	void Update() {
		if (Input.GetButtonDown ("Fire1") || Input.GetButtonDown ("Submit") || Input.GetButtonDown("Menu")) {
			Load ();
		}
	}
}
