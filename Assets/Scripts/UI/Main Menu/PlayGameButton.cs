using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class PlayGameButton : MonoBehaviour {

	string sceneName = LevelManager.levels[1];
	LevelManager levelManager;
    Button button;
    public void Awake()
    {
        button = transform.Find("Button").GetComponent<Button>();
        button.onClick.AddListener(Load);
		levelManager = GameObject.Find ("Level Manager").GetComponent<LevelManager> ();
    }

    public void Load()
    {
        Debug.Log("Clicked!");
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
		levelManager.UpdateLevel (1);
    }
}
