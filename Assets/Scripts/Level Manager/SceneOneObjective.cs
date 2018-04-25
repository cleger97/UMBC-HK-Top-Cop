using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneOneObjective : MonoBehaviour {

	public ObjectiveScript objectiveHandler;
	// Use this for initialization
	void Awake () {
		GameObject objectiveCanvas= GameObject.Find ("Objective Canvas");
		if (objectiveCanvas == null) {
			Debug.Log ("No Objective Canvas found!");
			SceneManager.LoadScene ("menu", LoadSceneMode.Single);
		} else {
			objectiveHandler = objectiveCanvas.GetComponent<ObjectiveScript> ();
			if (objectiveHandler == null) {
				Debug.LogError ("Scene handler did not find Objective Handle.");
			}
		}
	}

	void Start () {
		objectiveHandler.SetGoalType (0, 10);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
