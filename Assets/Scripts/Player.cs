/* By Alex Leger
 * Player script, consolidated 
 * Bits and pieces pulled from Matt Wong's files
 * Goal of this file is to be modular.
 * */

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {
	// global int
	private const float AVG_FRAME_TIME = 0.0212f;
	// state conditions
	// depending on the state, do something
	private enum State { ATTACK, BLOCK, MOVE, IDLE, JUMP }
	private State currentState;

	// keybinds
	private PlayerConfig keybinds;

	// Set if input has been put in
	// Handled by update
	private bool alreadyInput;
	private List<State> states;

	// Hit List
	private List<int> leftDamageList;
    private List<int> rightDamageList;

    private int leftTotalDamage;
    private int rightTotalDamage;
    // animator
    private Animator anim;

	// various state variables
	// blocking:
	private bool isBlocking;

	// attacking:
	public int baseDamage = 25;
	public float baseAtkRange = 3f;
	public int baseKBForce = 100;
	public float damageRadius = 0.5f; 
	public Transform dmgArea;

	// timers between attacking
	private float attackCD = 0;
	private float animTimer = 0;

	// combo count
	private int currentHits;
	private const int MAX_COMBO = 2;

	// movement:
	// rigidbody
	public Rigidbody2D rb2D;
	// direction : 1 for right, -1 for left
	private int faceDirection = 1;
	// speed
	public int speed = 5;
	public int jumpHeight = 250;

	// aerial movement
	private bool isGrounded = true;
	// detects ground
	public Transform groundPoint;
	public float radius;
	public LayerMask groundMask;

	// player data
	public int maxHealth;
	public float currentHealth;
	public float invulTime;
	public float currInvulTime = 0;

	private float healthRegen = 2.5f;

	public Transform healthBar;
	public Text hpbar_text;
	// location of the health bar among child objects
	public int hpbar_health = 1;
	// is dead?
	private bool isDead = false;

	// renderer stuff
	private SpriteRenderer rend;
	private bool isRed; // is red? used when hit.
	private float timeRed;

	// handle the objectives and goals
	public Text enemyCounter;
	public ObjectiveScript objectiveHandle;

	// Assign core objects in Awake()
	void Awake() {
		// objective stuff
		objectiveHandle = ObjectiveScript.instance;


		if (objectiveHandle != null) {
			enemyCounter = GameObject.Find ("Objective Canvas").transform.GetChild (0).GetComponent<Text> ();

			// health stuff
			healthBar = GameObject.Find ("Objective Canvas").transform.GetChild (2);
			hpbar_text = healthBar.GetChild (2).GetComponent<Text> ();
		}


		// attack tool
		dmgArea = transform.GetChild (2).transform;

		// rigidbody
		rb2D = GetComponent<Rigidbody2D>();

		// renderer tools
		rend = gameObject.GetComponent<SpriteRenderer>();

		// state list
		states = new List<State> ();
		leftDamageList = new List<int> ();
        rightDamageList = new List<int>();
		// animation tools
		anim = GetComponent<Animator> ();

		// keybinds
		keybinds = GetComponent<PlayerConfig>();

	}
	// Use this for initialization
	void Start () {


		// renderer aid
		isRed = false;

		// attack tool
		isBlocking = false;
		// health tools
		maxHealth = 250;
		currentHealth = maxHealth;

		// init objective
		// objectiveHandle.SetGoalType(0, 10);

	}

	// FixedUpdate for jumps
	// Hopefully stops the superjumping
	void FixedUpdate() {

		// Fire off the current states
		if (states.Contains (State.BLOCK)) {
			Block ();
		} else {
			Unblock ();
		}

		if (states.Contains (State.ATTACK)) {
			// the principle of combos come from canceling backswing - if the player attacks at this time, they should be able to hit again.
			float minAttackTime = 20 * AVG_FRAME_TIME;
			float baseAttackCooldown = 30 * AVG_FRAME_TIME;
			//Debug.Log ( (int) (attackCD / AVG_FRAME_TIME));
			if (attackCD <= minAttackTime) {
				//Debug.Log (currentHits);
				Attack ();
				attackCD = baseAttackCooldown;
			}

		}

		if (states.Count == 0) { 
			ResetAnimations ();
		}

		int horizInput = (int) Input.GetAxisRaw("Horizontal");
		int vertInput = (int) Input.GetAxisRaw("Vertical");

		if (horizInput != 0) {
			// can only move if not blocking
			if (!states.Contains(State.BLOCK) || states.Count == 0) {
				states.Add (State.MOVE);

			}
			if (horizInput != faceDirection) {
				//Debug.Log (horizInput);
				//Debug.Log (faceDirection);
				transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
				faceDirection = faceDirection * -1;
			}
		}

		if (Input.GetButtonDown ("Jump") || Input.GetKeyDown (keybinds.jumpkey) || vertInput > 0) {
			states.Add (State.JUMP);
		}

		if (states.Contains (State.JUMP) && isGrounded) {
			Jump ();
		}

		if (states.Contains (State.MOVE)) {
			Move ();

		} else {
			StopMoving ();
		}

		// checking if player is on the ground
		bool isGroundedNow = Physics2D.OverlapCircle(groundPoint.position, radius, groundMask);
		if (isGroundedNow != isGrounded) {
			this.ResetCollisions (isGroundedNow);
		}
		isGrounded = isGroundedNow;

		if (attackCD > 0) {
			//Debug.Log (attackCD);
			attackCD -= Time.deltaTime;
			if (attackCD < 0) {
				attackCD = 0;
				currentHits = 0;
			}
		}
			
		// check in air
		anim.SetBool("inAir", !isGrounded);

		// Update the renderer
		if (isRed) {
			timeRed -= Time.deltaTime;
			if (timeRed <= 0) {
				rend.color = Color.white;
				timeRed = 0;
				isRed = false;
			}
		}

		// Update invul frames
		if (currInvulTime > 0) {
			currInvulTime -= Time.deltaTime;
			if (currInvulTime < 0) {
				currInvulTime = 0;
			}

		}

		// Handle the damage input
		for (int i = 0; i < leftDamageList.Count; i++) {
			leftTotalDamage += leftDamageList [i];
		}
		for (int i = 0; i < rightDamageList.Count; i++)
		{
			rightTotalDamage += rightDamageList[i];
		}

		if (!isBlocking && (leftTotalDamage > 0|| rightTotalDamage > 0)) {
			int totalDamage = leftTotalDamage + rightTotalDamage;
			currentHealth -= totalDamage;
			UpdateHealth ();
		}
		else if (isBlocking){
			if (faceDirection == 1 && leftTotalDamage > 0)
				currentHealth -= leftTotalDamage;
			else if (faceDirection == -1 && rightTotalDamage > 0)
				currentHealth -= rightTotalDamage;

			if (leftTotalDamage > 0 || rightTotalDamage > 0) {
				UpdateHealth();
			}

		}

		leftTotalDamage = 0;
		rightTotalDamage = 0;
		leftDamageList.Clear ();
		rightDamageList.Clear();	
		// Clear the input list
		alreadyInput = false;
		states.Clear ();

		// if they died, then don't collect new input data
		if (isDead) {
			return;
		}

		// Get the input data
		if (Input.GetKey (keybinds.blockKey) || Input.GetButton("Fire2")) {
			//currentState = State.BLOCK;
			states.Add (State.BLOCK);
		}

		if (Input.GetKeyDown (keybinds.attackKey) || Input.GetButtonDown("Fire1")) {
			// if there's already been input (block has priority over attack)
			if (!states.Contains (State.BLOCK)) {
				states.Add (State.ATTACK);
			}
		}


	}

	// Update is called once per frame
	void Update () {
		
	



	}



	void Block() {
		isBlocking = true;
		anim.SetBool ("isBlocking", true);
		anim.SetBool ("idle", false);
	}

	void Unblock() {
		isBlocking = false;
		anim.SetBool ("isBlocking", false);
	}

	// Player function to attack
	// Preconditions: The attack is off cooldown. (attackCD < minAttackTime) 
	// Postcondition: Attacks.
	void Attack() {
		
		if (attackCD > 0) {
			currentHits++;

			if (currentHits > MAX_COMBO) {
				currentHits = 0;
			}

		} else {
			currentHits = 0;
		}
			
		//Debug.Log (currentHits);

		anim.SetBool ("idle", false);

		anim.ResetTrigger ("isPunching");

		// check people in the range

		anim.SetTrigger ("isPunching");
		anim.SetInteger ("combo", currentHits);

		GameObject[] checkInAttack = colliderTagSorter ("Enemy", getAllAround (damageRadius, dmgArea));

		anim.speed = 1f;

		for (int i = 0; i < checkInAttack.Length; i++) {
            
			Enemy enemyScript = checkInAttack [i].transform.GetComponent<Enemy> ();
			// Calculate Angle Between the collision point and the player
			Vector3 dir = checkInAttack [i].transform.position - transform.position;
			// We then get the opposite (-Vector3) and normalize it
			dir = dir.normalized;

			// Change: Only horizontal momentum on initial attacks.
			dir.y = 0;
			// And finally we add force in the direction of dir and multiply it by force. 
			checkInAttack [i].transform.GetComponent<Rigidbody2D> ().AddForce (dir * baseKBForce);

			if (enemyScript != null) {
                // Check for attack cooldown
                // animation timer should be set somewhere to make sure attacks hit at the same time

				enemyScript.takeDamage (baseDamage);
				animTimer = attackCD;


			}    
		}

	}
	void Move() {
		anim.SetBool ("idle", false);
		// set the animator speed for the movement
		int anim_speed = Mathf.Abs((int)Input.GetAxisRaw("Horizontal"));
		anim.SetInteger("anim_speed", anim_speed);
		anim.speed = (float) anim_speed;

		// handle the actual movement
	
		float movx = Input.GetAxisRaw ("Horizontal") * speed;
		if (isBlocking) {
			movx = 0;
		}
		float movy = rb2D.velocity.y;
		Vector2 moveDir = new Vector2 (movx, movy);
		rb2D.velocity = moveDir;

	}

	void StopMoving() {

		anim.SetInteger ("anim_speed", 0);
		anim.speed = 1;

		// preserve y velocity
		float yVel = rb2D.velocity.y;
		Vector2 newVel = new Vector2 (0, yVel);
		rb2D.velocity = newVel;

	}

	void Jump() {
		anim.SetBool ("idle", false);
		if (rb2D.velocity.y == 0) {
			rb2D.AddForce(new Vector2(0, jumpHeight));
		}

	}

	void ResetAnimations() {
		anim.SetInteger ("anim_speed", 0);
		anim.SetBool ("idle", true);

		//anim.ResetTrigger ("isPunching");
		//anim.ResetTrigger ("isPunching2");

	}

	void ResetCollisions(bool currentState) {
		if (currentState) {
			Physics2D.IgnoreLayerCollision (9, 10, false);
		} else {
			Physics2D.IgnoreLayerCollision (9, 10, true);
		}

	}
		

	// Get hit: damage and knockback value
	// Adds it to the damage list, to be calculated
	public void GetHit(int damage,int direction) {
		if (currInvulTime == 0) {
            if (direction == -1)
                rightDamageList.Add(damage);
            else
                leftDamageList.Add(damage);
			//currInvulTime = invulTime;
		}
	}
		
	void UpdateHealth() {
		//Debug.Log ("Update Health");
		//Debug.Log (currentHealth);
		//Debug.Log (maxHealth);
		rend.color = Color.red;
		// five frames to fix red
		isRed = true;
		timeRed = 1f;

		if (currentHealth > maxHealth) {
			currentHealth = maxHealth;
		}

		decimal percent = (decimal)currentHealth / (decimal)maxHealth;
		healthBar.GetChild(hpbar_health).localScale = new Vector3((float)percent, 1, 1);
		percent *= 100;

		hpbar_text.text = "Health: " + percent.ToString ("##") + "% ";
		//Freeze player movement
		if (currentHealth <= 0) {
			hpbar_text.text = "Health: 0%";
			playerDead ();
		}

	}

	// handle death
	public void playerDead()
	{
		isDead = true;
		anim.SetBool("endGame", true);

		GameObject.Find("UI Canvas").GetComponent<MenuUIHandle>().Defeat();
		// legit bricks the editor wtf
		// setting the time to 0 will completely kill the system

		Time.timeScale = 0.0001f;
	}

	// update the counters
	public void updateKill()
	{
		if (objectiveHandle != null) {
			objectiveHandle.UpdateKillCount(1);
		}
		// post-kill things here
		// i.e. partial heal 
		currentHealth += healthRegen;

		UpdateHealth ();
	}

	public bool IsPlayerAlive()
	{
		return !isDead;
	}
		
	// takes in an array of Collider2D objects and returns an array of GameObjects that have a certain tag
	private GameObject[] colliderTagSorter (string tagName, Collider2D[] toSort)
	{ 
		//for ease, use ArrayList to add items to array. Then convert back to array to send back. (MAY CHANGE LATER FOR MEMORY EFFICIENCY)
		ArrayList tempList = new ArrayList ();
		for (int i = toSort.Length - 1; i >= 0; i--) {
			if (toSort [i].gameObject.tag == tagName) {
				tempList.Add (toSort [i].gameObject);
			}
		}
		return tempList.ToArray (typeof(GameObject)) as GameObject[];
	}


	private Collider2D[] getAllAround (float radius, Transform center)
	{
		return Physics2D.OverlapCircleAll (center.position, radius);
	}

	// Debugging: Draw attack range.
	void OnDrawGizmos ()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere (dmgArea.position, damageRadius);
	}
}
