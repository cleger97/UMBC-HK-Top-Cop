using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTwoObjective : MonoBehaviour {

	public ObjectiveScript objectiveHandler;
	public Text text;
	public GameObject imageObj, textObj, text2obj;

	public float delay;

	void Awake () {
		GameObject objectiveCanvas = GameObject.Find ("Objective Canvas");

		if (objectiveCanvas == null) {
			objectiveHandler = null;
			Debug.LogWarning ("Scene handler did not find Objective Canvas.");
			SceneManager.LoadScene ("menu", LoadSceneMode.Single);
		} else { 
			objectiveHandler = objectiveCanvas.GetComponent<ObjectiveScript> ();
			if (objectiveHandler == null) {
				Debug.LogWarning ("Scene handler did not find Objective Handle.");
			}
		}
	}

	void Start () {
		if (objectiveHandler != null) {
			objectiveHandler.SetGoalType (1, 90);
		}

		imageObj.SetActive (true);
		textObj.SetActive (true);
		text2obj.SetActive (false);

		text.text = "SCENE 2\n\nSURVIVE FOR 1:30";

		delay = 1.5f;

		Time.timeScale = 0;
	}

	void OnGUI() {
		if (delay == 0) {
			if (Input.GetButtonDown ("Fire1")) {
				imageObj.SetActive (false);
				textObj.SetActive (false);
				text2obj.SetActive (false);
				Time.timeScale = 1;
			}
		}
	}

	void Update() {
		if (delay > 0) {
			delay -= Time.unscaledDeltaTime;
			if (delay < 0) {
				delay = 0;
				text2obj.SetActive (true);
			}
		}
	}
}
