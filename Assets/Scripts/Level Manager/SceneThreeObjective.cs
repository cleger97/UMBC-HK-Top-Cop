using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/*  Scene 3 Manager 
 *  Note regarding the Scene Managers - they seem to be capable of being merged into one file
 *  Would just need to alter certain variables to make sure everything is working as intended.
 */
public class SceneThreeObjective : MonoBehaviour {

    public ObjectiveScript objectiveHandler;
    public Text text;
    public GameObject imageObj, textObj, text2obj;

    public float delay;

    void Awake()
    {
        GameObject objectiveCanvas = GameObject.Find("Objective Canvas");
        if (objectiveCanvas == null)
        {
            objectiveHandler = null;
            Debug.LogWarning("Scene handler did not find Objective Canvas.");
            SceneManager.LoadScene("menu", LoadSceneMode.Single);
        }
        else
        {
            objectiveHandler = objectiveCanvas.GetComponent<ObjectiveScript>();
            if (objectiveHandler == null)
            {
                Debug.LogWarning("Scene handler did not find Objective Handle.");
            }
        }
    }

    // Use this for initialization
    void Start () {
        if (objectiveHandler != null)
        {
            objectiveHandler.SetGoalType(2, "Defeat the Boss");
        }

        imageObj.SetActive(true);
        textObj.SetActive(true);
        text2obj.SetActive(false);

        text.text = "SCENE 3\n\nBEAT THE BOSS";

        delay = 1.5f;

        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (delay == 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                imageObj.SetActive(false);
                textObj.SetActive(false);
                text2obj.SetActive(false);
                Time.timeScale = 1;
            }
        }

        else if (delay > 0)
        {
            delay -= Time.unscaledDeltaTime;
            if (delay < 0)
            {
                delay = 0;
                text2obj.SetActive(true);
            }
        }
    }
}
