/*  By Matt Wong
 *  Revisited and Updated by Alex Leger
 *  This script holds the data of the player including health and other stats needed
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Data : MonoBehaviour {

    public int max_health = 100;
    private int healthStat;

    SpriteRenderer rend;
    Color defaultColor;
    bool isRed;
    float timeRed;
    float timeStamp;
    Animator anim;
    Player_Movement playerMovement;
    Player_Attack playerAttack;
    public Text enemyCounter;
    public ObjectiveScript objectiveHandle;
	public Transform healthBar;
	public Text hpbar_text;

	// ints of the children location
	private int hpbar_health = 1;


	int numEnemy = 0;

	bool isBlocking;

	// Awake: initialize objects
	void Awake() {
		playerMovement = GetComponent<Player_Movement>();
		anim = GetComponent<Animator>();
		playerAttack = GetComponent<Player_Attack>();
		healthStat = max_health;
		rend = gameObject.GetComponent<SpriteRenderer>();
		objectiveHandle = GameObject.Find("Objective Canvas").GetComponent<ObjectiveScript>();

		healthBar = GameObject.Find ("Objective Canvas").transform.GetChild (2);
		hpbar_text = healthBar.GetChild (2).GetComponent<Text> ();
	}



    void Start()
    {
        //SetCountText();
        
        // initialization of objective
        objectiveHandle.SetGoalType(0, 5);

		// initialization of hp bar
		healthBar.GetChild(hpbar_health).localScale = new Vector3(1, 1, 1);
		hpbar_text.text = "Health: 100%";


        isRed = false;
        timeRed = 0.5f;
        timeStamp = 0f;
    }

    void Update()
    {
		// Update hit color
        if (timeStamp > 0)
            timeStamp = timeStamp - Time.deltaTime;
        else
            rend.color = Color.white;

		// Update blocking
		isBlocking = Input.GetButton ("Fire2") == true || Input.GetKey(KeyCode.K) == true;
		playerMovement.isBlocking = isBlocking;
    }

    //getter for the current health
    public int getHealthStat()
    {
        return healthStat;
    }

    //sets the current health
    public void setHealthStat(int newHealthStat)
    {
        healthStat = newHealthStat;
    }

    //decreases from the current health
    private void decreaseHealth(int toDecr)
	{
		healthStat -= toDecr;
		rend.color = Color.red;
		isRed = true;
		timeStamp = timeRed;

		decimal percent = (decimal) healthStat / (decimal) max_health;

		healthBar.GetChild(hpbar_health).localScale = new Vector3((float)percent, 1, 1);

		percent *= 100;

		//Debug.Log (percent);

		hpbar_text.text = "Health: " + percent.ToString ("##") + "% ";
		//Freeze player movement
		if (healthStat <= 0) {
			hpbar_text.text = "Health: 0%";
			playerDead ();
		}
    }

	// precond: hitDamage is how much it would hit for,
	// facing is 1 for right player and -1 for left player
	public void getHit(int hitDamage, int facing) {
		if (isBlocking) {
			
			Vector2 hitForce = new Vector2((hitDamage) * facing, 0f);
			Debug.Log (hitForce.x);
			playerMovement.AddForce (hitForce, 5);

		} else {
			decreaseHealth (hitDamage);

		}

	}

    public bool isPlayerAlive()
    {
        if (healthStat > 0)
            return true;
        else
            return false;
    }

    public void updateKill()
    {
        objectiveHandle.UpdateKillCount(1);

		// post-kill things here
		// i.e. partial heal 
		healthStat += 25;
    }
		

    public void playerDead()
    {
        playerMovement.enabled = false;
        anim.SetBool("endGame", true);
        playerAttack.enabled = false;
        // #if UNITY_EDITOR
        //    UnityEditor.EditorApplication.isPlaying = false;
        // #else
        //     Application.Quit();
        // #endif


        GameObject.Find("UI Canvas").GetComponent<MenuUIHandle>().Defeat();
        // legit bricks the editor wtf
        Time.timeScale = 0.0001f;
    }

    

}
