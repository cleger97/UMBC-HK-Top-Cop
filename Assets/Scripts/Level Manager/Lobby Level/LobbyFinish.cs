using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LobbyFinish : MonoBehaviour {

    ObjectiveScript objScript;

	// Use this for initialization
	void Start () {
        objScript = ObjectiveScript.instance;
        if (objScript == null)
        {
            Debug.LogWarning("Objective Script doesn't exist; script will not do anything.");
        }

        if (this.GetComponent<Collider2D>().isTrigger == false)
        {
            Debug.LogWarning("Collider attached is not a trigger. Script will not work correctly.");
        }
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger entered");
        // no use if there is no level manager
        if (objScript == null)
        {
            return;
        }
        if (other.tag != "Player")
        {
            return;
        }

        objScript.ActivateFinish();
    }


}
