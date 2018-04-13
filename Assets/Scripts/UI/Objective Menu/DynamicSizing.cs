using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicSizing : MonoBehaviour {

	public Transform CounterBox;
	public Transform GoalBox;

	public Text CounterText;
	public Text GoalText;

	// Use this for initialization
	void Start () {
		if (CounterBox == null || GoalBox == null) {
			this.gameObject.GetComponent<DynamicSizing> ().enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		float countFontSize = CounterText.fontSize * 0.8f;
		float goalFontSize = GoalText.fontSize * 0.8f;

		int countTextLen = CounterText.text.Length;
		int goalTextLen = GoalText.text.Length;

		float countSizeNecessary = countFontSize * countTextLen;
		CounterBox.transform.GetComponent<RectTransform> ().sizeDelta = new Vector2 (countSizeNecessary, 25f);

		float goalSizeNecessary = goalFontSize * (0 + goalTextLen);
		GoalBox.transform.GetComponent<RectTransform> ().sizeDelta = new Vector2(goalSizeNecessary, 25f);

	}
}
