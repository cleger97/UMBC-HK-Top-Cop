using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Scene Objective -> This is what should be used/reimplemented for other scenes.
public class SceneObjective : MonoBehaviour {

    public ObjectiveScript objectiveHandler;
    public Text text;
    public GameObject imageObj, textObj, text2obj;

    public AudioSource audio;

    public string textToDisplay;

    public int goalType;
    public int goalAmount;
    public string goalText;

    public bool hasStarted = false;

    private float delay;

    void Awake()
    {
        audio = GameObject.Find("Music").GetComponent<AudioSource>();

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

    void Start()
    {
        if (objectiveHandler != null)
        {
            if (goalType != 2)
            {
                objectiveHandler.SetGoalType(goalType, goalAmount);
            } else
            {
                objectiveHandler.SetGoalType(goalType, goalText);
            }
        }

        imageObj.SetActive(true);
        textObj.SetActive(true);
        text2obj.SetActive(false);

        text.text = System.Text.RegularExpressions.Regex.Unescape(textToDisplay);

        delay = 1.5f;

        Time.timeScale = 0;
    }


    void Update()
    {
        if (hasStarted) { return; }
        if (delay == 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                imageObj.SetActive(false);
                textObj.SetActive(false);
                text2obj.SetActive(false);
                Time.timeScale = 1;

                if (audio != null)
                {
                    audio.Play();
                }

                hasStarted = true;
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
