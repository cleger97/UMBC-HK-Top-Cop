using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReturnToMain : MonoBehaviour {

	public LevelManager levelManager;
	public GameObject button;

	private float timer = 3f;

	void Awake() {
		levelManager = LevelManager.instance;
		timer = 3f;
	}

	//void Start() {
	//	Debug.Log (button);
	//}

	// Update is called once per frame
	public void Load() {
		levelManager.LoadSpecificLevel (0);
	}

	private void EnableLoad() {
		button.GetComponent<Button> ().onClick.AddListener (Load);
	}

	void Update() {
		if (timer > 0) {
			timer -= Time.deltaTime;
			if (timer <= 0) {
				timer = 0;
				EnableLoad();
			}
			return;
		}

		if (Input.GetButtonDown ("Fire1") || Input.GetButtonDown ("Submit") || Input.GetButtonDown("Menu")) {
			Load ();
		}
	}
}
