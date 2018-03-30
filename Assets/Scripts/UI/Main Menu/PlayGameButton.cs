using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class PlayGameButton : MonoBehaviour {

    string sceneName = "Scene 1 - beta";
    Button button;
    public void Awake()
    {
        
        button = transform.Find("Button").GetComponent<Button>();
        button.onClick.AddListener(Load);

    }

    public void Load()
    {
        Debug.Log("Clicked!");
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
