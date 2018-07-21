using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISelection : MonoBehaviour {

	public GameObject RestartButton;
	public GameObject ReturnButton;
	public GameObject ContinueButton;
	public MenuUIHandle UIHandle;

	private int selectLocation = 0;

	private float delay = 0.8f;
	// 0 for restart, 1 for return, 2 for continue

	// active for either pause or defeat.
	public bool isPause;

	private bool vertAdjust;

	private int verticalInput;

	void OnEnable() {
		
		isPause = UIHandle.paused;

        if (isPause)
        {
            selectLocation = 0;
            this.transform.position = ContinueButton.transform.position;
        } else
        {
            selectLocation = 1;
            this.transform.position = RestartButton.transform.position;
        }
		
		delay = 0.8f;
		Debug.Log ("Is enable firing??");
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
                verticalInput = (input > 0) ? 1 : -1;
				vertAdjust = true;
			}
		}

		if (verticalInput > 0) {
			switch (selectLocation) {
			case 0:
				{
					
					selectLocation = 2;
				    this.transform.position = ReturnButton.transform.position;
					
					break;
				}
			case 1:
				{
                    if (isPause)
                        {
                            selectLocation = 0;
                            this.transform.position = ContinueButton.transform.position;
                        }
                    else
                        {
                            selectLocation = 2;
                            this.transform.position = ReturnButton.transform.position;
                        }
					
					break;
				}
			case 2:
				{
					selectLocation = 1;
					this.transform.position = RestartButton.transform.position;
					break;
				}
			} 
		} else if (verticalInput < 0) {
			switch (selectLocation) {
			case 0:
				{
                        selectLocation = 1;
                        this.transform.position = RestartButton.transform.position;
                        break;
				}
			case 1:
				{
					
					selectLocation = 2;
					this.transform.position = ReturnButton.transform.position;
					
					break;
				}
			case 2:
				{
                    if (isPause)
                        {
                            selectLocation = 0;
                            this.transform.position = ContinueButton.transform.position;
                        } else
                        {
                            selectLocation = 1;
                            this.transform.position = RestartButton.transform.position;
                        }
					
					break;
				}
			} 
		}
		// input delay of 0.8s
		if (delay > 0) {
			//Debug.Log (delay);
			delay -= Time.unscaledDeltaTime;
			return;
		}
		// don't allow input before at least 1.5 seconds
		if (Input.GetButtonDown ("Fire1") || Input.GetKeyDown (KeyCode.Return) || Input.GetButtonDown("Submit")) {
			switch (selectLocation) {
			case 0:
				UIHandle.ResumeGame ();
				break;
			case 1:
				UIHandle.RestartLoad ();
				break;
			case 2:
				UIHandle.ReturnLoad ();
				break;
			default:
				Debug.LogWarning ("Defaulted");
				break;
			}
		}

	}
}
