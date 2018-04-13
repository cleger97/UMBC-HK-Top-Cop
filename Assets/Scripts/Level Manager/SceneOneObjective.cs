using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneOneObjective : MonoBehaviour {

	public ObjectiveScript objectiveHandler;
	// Use this for initialization
	void Awake () {
		objectiveHandler = GameObject.Find ("Objective Canvas").GetComponent<ObjectiveScript> ();
		if (objectiveHandler == null) {
			Debug.LogError ("Scene handler did not find Objective Handle.");
		}
	}

	void Start () {
		objectiveHandler.SetGoalType (0, 10);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
