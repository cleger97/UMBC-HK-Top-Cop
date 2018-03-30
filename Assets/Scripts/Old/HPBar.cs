using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{

    public float barDisplay; //current progress
    public Vector2 pos = new Vector2(20, 40);
    public Vector2 size = new Vector2(200, 50);
    public Texture2D emptyTex;
    public Texture2D fullTex;
    public Player_Data data;
    public GUIStyle progress_empty, progress_full;

    void Awake()
    {
        data = GameObject.Find("Player").GetComponent<Player_Data>();
    }

    void OnGUI()
    { 
        //draw the background:
        GUI.BeginGroup(new Rect(pos.x, pos.y, size.x, size.y));
        DrawQuad(new Rect(0, 0, size.x, size.y), emptyTex);
        

        //draw the filled-in part:
        GUI.BeginGroup(new Rect(0, 0, size.x * barDisplay, size.y));
        DrawQuad(new Rect(0, 0, size.x * barDisplay , size.y), fullTex);
        GUI.EndGroup();

        GUI.EndGroup();

    }   

    void DrawQuad(Rect position, Texture2D text)
    {
        GUI.skin.box.normal.background = text;
        GUI.Box(position, GUIContent.none);
    }

    void Update()
    {
        //for this example, the bar display is linked to the current time,
        //however you would set this value based on your desired display
        //eg, the loading progress, the player's health, or whatever.
        barDisplay = (float)data.getHealthStat() / (float)data.max_health;
        //        barDisplay = MyControlScript.staticHealth;
    }
}