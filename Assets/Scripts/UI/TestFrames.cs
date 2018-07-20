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
        int averageFPS = (int)(totalFrames / totalTime);
		string text = "Average FPS is: " + averageFPS.ToString ();
		Debug.Log (text);
	}
}
