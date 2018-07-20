using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Boss_Data))]
public class BossFinish : MonoBehaviour {

    public ObjectiveScript objScript;
    public Boss_Data bossData;

    void Start()
    {
        GameObject objScriptObject = GameObject.Find("Objective Canvas");
        if (objScriptObject == null)
        {
            Debug.LogWarning("No Objective Script! BossFinish will not work.");
            return;
        } else
        {
            objScript = objScriptObject.GetComponent<ObjectiveScript>();
        }

        if (objScript.GetGoalType() != 2)
        {
            Debug.LogWarning("Boss Finish is not set as the end goal!");
        }

        bossData = gameObject.GetComponent<Boss_Data>();
    }

    // Update is called once per frame
    void Update () {
        bool currentBossState = bossData.isAlive();

        if (currentBossState == false)
        {
            objScript.ActivateFinish();
        }
		
	}

    public void FinishGame()
    {
        objScript.ActivateFinish();
    }
}
