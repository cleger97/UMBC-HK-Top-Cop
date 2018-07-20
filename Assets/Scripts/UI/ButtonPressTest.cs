using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ButtonPressTest : MonoBehaviour {

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Fire1");
        }
        if (Input.GetButtonDown("Fire2"))
        {
            Debug.Log("Fire2");
        }
        if (Input.GetButtonDown("Fire3"))
        {
            Debug.Log("Fire3");
        }
        if (Input.GetButtonDown("ThrowItem"))
        {
            Debug.Log("ThrowItem");
        }
    }
}


