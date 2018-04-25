using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISelection : MonoBehaviour {

	public GameObject RestartButton;
	public GameObject ReturnButton;
	public GameObject ContinueButton;
	public MenuUIHandle UIHandle;

	private int selectLocation = 0;
	// 0 for restart, 1 for return, 2 for continue

	// active for either pause or defeat.
	public bool isPause;

	private bool vertAdjust;

	private int verticalInput;

	void OnEnable() {
		selectLocation = 0;
		isPause = UIHandle.paused;
		this.transform.position = RestartButton.transform.position;
	}
	// Update is called once per frame
	void Update () {
		float input = Input.GetAxisRaw ("Vertical");
		verticalInput = 0;
		if (input == 0) {
			vertAdjust = false;
			verticalInput = 0;
		}
		if (input != 0) {
			if (!vertAdjust) {
				verticalInput = (int) input;
				vertAdjust = true;
			}
		}



		//Debug.Log (isPause);

		if (Input.GetButtonDown ("Fire1") || Input.GetKeyDown (KeyCode.Return) || Input.GetButtonDown("Submit")) {
			switch (selectLocation) {
			case 0:
				UIHandle.RestartLoad ();
				break;
			case 1:
				UIHandle.ReturnLoad ();
				break;
			case 2:
				UIHandle.ResumeGame ();
				break;
			default:
				Debug.LogWarning ("Defaulted");
				break;
			}
		}

		//if (Input.GetKeyDown (KeyCode.Escape) || Input.GetButtonDown("Menu")) {
		//	if (isPause) {
		//		UIHandle.ResumeGame ();
		//	}
		//}

		if (verticalInput > 0) {
			switch (selectLocation) {
			case 0:
				{
					if (isPause) {
						selectLocation = 2;
						this.transform.position = ContinueButton.transform.position;
					} else {
						selectLocation = 1;
						this.transform.position = ReturnButton.transform.position;
					}
					break;
				}
			case 1:
				{
					selectLocation = 0;
					this.transform.position = RestartButton.transform.position;
					break;
				}
			case 2:
				{
					selectLocation = 1;
					this.transform.position = ReturnButton.transform.position;
					break;
				}
			} 
		} else if (verticalInput < 0) {
			switch (selectLocation) {
			case 0:
				{
					selectLocation = 1;
					this.transform.position = ReturnButton.transform.position;
					break;
				}
			case 1:
				{
					if (isPause) {
						selectLocation = 2;
						this.transform.position = ContinueButton.transform.position;
					} else {
						selectLocation = 0;
						this.transform.position = RestartButton.transform.position;
					}
					break;
				}
			case 2:
				{
					selectLocation = 0;
					this.transform.position = RestartButton.transform.position;
					break;
				}
			} 
		}

	}
}
