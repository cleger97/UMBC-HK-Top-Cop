using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	private static bool paused;
	private static float lastPause;

	void Awake() {
		paused = false;
	}

	public static void Pause() {
		paused = true;
		lastPause = Time.timeScale;
		Time.timeScale = 0;
	}

	public static void Unpause() {
		paused = false;
		Time.timeScale = lastPause;
	}

	public static bool IsPaused() {
		return paused;
	}

}
