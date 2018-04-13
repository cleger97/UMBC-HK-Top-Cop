using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageTransparent : MonoBehaviour {

	public Image counterImage;
	public Image goalImage;

	// Use this for initialization
	// Runs literally once ever.

	// Only purpose of this is to add transparency to the background images.
	// Why is this not supported by the unity editor...?
	void Start () {
		Color ci = counterImage.color;
		ci.a = 0.4f;
		counterImage.color = ci;

		Color gi = goalImage.color;
		gi.a = 0.4f;
		goalImage.color = gi;
	}

}
