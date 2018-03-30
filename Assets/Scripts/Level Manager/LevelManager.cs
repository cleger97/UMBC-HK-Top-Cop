using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		// Level Managers shouldn't be destroyed on load.
		DontDestroyOnLoad(this);
	}

	// Update is called once per frame
	void Update () {
		
	}
}
