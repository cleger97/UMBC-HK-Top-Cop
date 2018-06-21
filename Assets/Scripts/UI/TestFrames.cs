using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFrames : MonoBehaviour {

	float totalFrames = 0;
	float averageTime = 0;
	float totalTime = 0;
	
	// Update is called once per frame
	void Update () {
		totalFrames++;
		totalTime += Time.deltaTime;
		averageTime = totalTime / totalFrames;
		string text = "Average frame time is: " + averageTime.ToString ();
		Debug.Log (text);
	}
}
