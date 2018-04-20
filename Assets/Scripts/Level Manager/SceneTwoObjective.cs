using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTwoObjective : MonoBehaviour {

	public ObjectiveScript objectiveHandler;


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
			objectiveHandler.SetGoalType (1, 60);
		}
	}

}
