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
    private const int ObjectLayer = 13;
	// state conditions
	// depending on the state, do something
	private enum State { ATTACK, BLOCK, MOVE, IDLE, JUMP, CROUCH }
	private State currentState;

	// keybinds
	private PlayerConfig keybinds;

	// Set if input has been put in
	// Handled by update
	private bool alreadyInput;
	private List<State> states;

	/* Damage List */
	private List<float> leftDamageList;
    private List<float> rightDamageList;

    private float leftTotalDamage;
    private float rightTotalDamage;
    /* Animator */
    private Animator anim;

	/* State Variables */
    // Substate: Blocking
	private bool isBlocking;

	// Substate: Attacking
	public int baseDamage = 25;
	public float baseAtkRange = 3f;
	public int baseKBForce = 100;
	public float damageRadius = 0.5f; 
	public Transform dmgArea;

	private float baseAttackCD = PlayerAnimationData.PlayerPunchTime * (4/5);
    public float attackCD = 0;
    private float animTimer = 0;
    private float timeSinceLastAttack = 0f;

	// Combo counter
    // "Shoryuken"
	private int currentHits;
	private const int MAX_COMBO = 2;

	// movement:
	// rigidbody
	private Rigidbody2D rb2D;
	// direction : 1 for right, -1 for left
	private int faceDirection = 1;
	// speed
	public int speed = 5;
	public int jumpHeight = 250;

	// aerial movement
	public bool isGrounded = true;
	// detects ground
	public Transform groundPoint;
	public float radius;
	public LayerMask groundMask;

	// Player data
	public int maxHealth;
	public float currentHealth;
    // Block Health
    private float blockHealth = 150f;
    public float maxBlockHealth = 250f;
    private float timeSinceLastBlock = 0f;

	private float invulTime = 0f;
	private float currInvulTime = 0;

    public bool isInvul = false;

	private float healthRegen = 0f;

	private Transform healthBar;

	// is dead?
	private bool isDead = false;

	// renderer stuff
	private SpriteRenderer rend;
	private bool isRed; // is red? used when hit.
	private float timeRed;

	// handle the objectives and goals
	public Text enemyCounter;
	public ObjectiveScript objectiveHandle;

    // item pickups
    private ThrowableObject carriedObject = null;
    private Transform collidedObject;
    private bool isCarrying = false;


	// Assign core objects in Awake()
	void Awake() {
		// objective stuff
		objectiveHandle = ObjectiveScript.instance;


		if (objectiveHandle != null) {
			enemyCounter = GameObject.Find ("Objective Canvas").transform.GetChild (0).GetComponent<Text> ();

			// health stuff
			healthBar = GameObject.Find ("Objective Canvas").transform.GetChild (2);
		}


		// attack tool
		dmgArea = transform.GetChild (2).transform;

		// rigidbody
		rb2D = GetComponent<Rigidbody2D>();

		// renderer tools
		rend = gameObject.GetComponent<SpriteRenderer>();

		// state list
		states = new List<State> ();
		leftDamageList = new List<float> ();
        rightDamageList = new List<float>();
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

		UpdateHealth (0);
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
            if (timeSinceLastBlock == -1) { timeSinceLastBlock = 0; }
		}

		if (states.Contains (State.ATTACK)) {
            Attack();
		} else
        {
            timeSinceLastAttack += Time.deltaTime;
        }

		if (states.Count == 0) { 
			ResetAnimations ();
		}

		int horizInput = (int) Input.GetAxisRaw("Horizontal");
		int vertInput = (int) Input.GetAxisRaw("Vertical");

        if (horizInput != 0)
        {
            // can only move if not blocking
            if (!states.Contains(State.BLOCK) || states.Count == 0)
            {
                states.Add(State.MOVE);

            }
            if (horizInput != faceDirection)
            {
                //Debug.Log (horizInput);
                //Debug.Log (faceDirection);
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                faceDirection = faceDirection * -1;
            }
        }

        UpdateCrouch(vertInput);
        if (vertInput < 0)
        {
            if (isGrounded) { states.Add(State.CROUCH); }          
        }

		if (Input.GetButtonDown ("Jump") || Input.GetKeyDown (keybinds.jumpkey) || vertInput > 0) {
            if (!states.Contains(State.CROUCH))
            {
                states.Add(State.JUMP);
            } 
		}

		if (states.Contains (State.JUMP) && isGrounded) {
			Jump ();
		}

		if (states.Contains (State.MOVE) && !states.Contains(State.CROUCH)) {
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
        // Handle block health
        if (timeSinceLastBlock != -1)
        {
            timeSinceLastBlock += Time.deltaTime;
        }
        

		// Handle the damage input
		for (int i = 0; i < leftDamageList.Count; i++) {
			leftTotalDamage += leftDamageList [i];
		}
		for (int i = 0; i < rightDamageList.Count; i++)
		{
			rightTotalDamage += rightDamageList[i];
		}

        float totalDamage = 0;
		if (!isBlocking && (leftTotalDamage > 0|| rightTotalDamage > 0)) {
			totalDamage = leftTotalDamage + rightTotalDamage;
			currentHealth -= totalDamage;
		}

		else if (isBlocking){
            
			totalDamage = 0;
            Debug.Log("Right total damage " + rightTotalDamage);
            Debug.Log("Left total damage " + leftTotalDamage);
            Debug.Log("Block health: " + blockHealth);
            Debug.Log("Face Direction: " + faceDirection);
			if (faceDirection == 1) {
                // right side block
                if (leftTotalDamage > 0)
                {
                    currentHealth -= leftTotalDamage;
                    totalDamage += leftTotalDamage;
                }
                if (rightTotalDamage > 0)
                {
                    if (blockHealth - rightTotalDamage < 0)
                    {
                        blockHealth = 0;
                        totalDamage += (rightTotalDamage - blockHealth);
                    } else {
                        blockHealth -= rightTotalDamage;
                    }
                }
				
			} else if (faceDirection == -1) {
                // left side block
                if (rightTotalDamage > 0)
                {
                    currentHealth -= rightTotalDamage;
                    totalDamage += rightTotalDamage;
                }
                if (leftTotalDamage > 0)
                {
                    if (blockHealth - leftTotalDamage < 0)
                    {
                        blockHealth = 0;
                        totalDamage += (leftTotalDamage - blockHealth);
                    }
                    else
                    {
                        blockHealth -= leftTotalDamage;

                    }
                }
			}
            currentHealth -= totalDamage;
		}
        UpdateHealth(totalDamage);

        if (totalDamage > 0 && isCarrying)
        {
            int damage = (int)((leftTotalDamage > rightTotalDamage) ?  leftTotalDamage : rightTotalDamage);
            DropItem(damage);
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

        //Debug.Log(timeSinceLastBlock);
        if (timeSinceLastBlock > 3.0f)
        {
            float incrementShield = 50f * Time.deltaTime; // should be 10 block/second
            if (blockHealth < maxBlockHealth)
            {
                blockHealth += incrementShield;
            }

            if (blockHealth > maxBlockHealth) { blockHealth = maxBlockHealth; }
            UpdateHealth(0);
        }

	}

	// Update is called once per frame
	void Update () {

        UpdateCrouch((int)Input.GetAxisRaw("Vertical"));

        // Get the input data
        if (Input.GetKey(keybinds.blockKey) || Input.GetButton("Fire2"))
        {
            //currentState = State.BLOCK;
            states.Add(State.BLOCK);
            timeSinceLastBlock = -1;
        }

        if (Input.GetButtonDown("ThrowItem") && !isBlocking)
        {
            if (isCarrying == false)
            {
                GrabItem();
            }
            else
            {
                ThrowItem(faceDirection, rb2D.velocity.x);
            }
        }

        if (Input.GetKeyDown(keybinds.attackKey) || Input.GetButtonDown("Fire1"))
        {
            // if there's already been input (block has priority over attack)
            if (!states.Contains(State.BLOCK))
            {
                states.Add(State.ATTACK);
            }
        }



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
        
        if (timeSinceLastAttack > 0.5f)
        {
            currentHits = 0;
        } else if (attackCD == 0) {
			currentHits++;

			if (currentHits > MAX_COMBO) {
				currentHits = 0;
			}

		} else
        {
            return;
        }

			
		//Debug.Log (currentHits);

		anim.SetBool ("idle", false);

		anim.ResetTrigger ("isPunching");

		// check people in the range

		anim.SetTrigger ("isPunching");
		anim.SetInteger ("combo", currentHits);

		GameObject[] checkInAttack = colliderTagSorter ("Enemy", getAllAround (damageRadius, dmgArea));

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

                enemyScript.takeDamage(baseDamage);

			}    
		}
        attackCD = baseAttackCD;
        timeSinceLastAttack = 0f;

	}

	void Move() {
		anim.SetBool ("idle", false);
		// set the animator speed for the movement
		int anim_speed = Mathf.Abs((int)Input.GetAxisRaw("Horizontal"));
		anim.SetInteger("anim_speed", anim_speed);
		//anim.speed = (float) anim_speed;

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

    void UpdateCrouch(int input)
    {
        if (input < 0)
        {
            anim.SetBool("isCrouching", true);
        } else
        {
            anim.SetBool("isCrouching", false);
        }
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

    void GrabItem()
    {
        int ThrowableObjectSpot = 4;

        if (collidedObject == null)
        {
            Debug.Log("No object to pick up");
            return;
        }
        
        ThrowableObject objToGrab = collidedObject.gameObject.GetComponent<ThrowableObject>();
        if (objToGrab == null)
        {
            Debug.Log("Object is not throwable");
            return;
        }
        carriedObject = objToGrab;

        Transform throwableSpot = this.transform.GetChild(ThrowableObjectSpot);

        objToGrab.Attach(throwableSpot);

        float offset = collidedObject.gameObject.GetComponent<Collider2D>().bounds.size.x + this.gameObject.GetComponent<Collider2D>().bounds.size.x;
        throwableSpot.localPosition = new Vector2(offset, 0);

        isCarrying = true;
        anim.SetBool("isHolding", true);
        return;
    }

    void ThrowItem(int direction, float initial)
    {
        carriedObject.Throw(direction, initial);
        carriedObject = null;

        anim.SetBool("isHolding", false);
        isCarrying = false;
    }

    void DropItem(int direction)
    {
        carriedObject.Drop((int)Mathf.Sign(direction));
        carriedObject = null;

        anim.SetBool("isHolding", false);
        isCarrying = false;
    }

    public void SetCollidedObject(Transform obj)
    {
        collidedObject = obj;
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
		
    public void SetInvul()
    {
        isInvul = true;
    }

	void UpdateHealth(float totalDamage) {
        // index of child objects
        int backindex = 0;
        int hpindex = 1;
        int shieldindex = 2;
        int text = 3;

        Text hpbar_text = null;

        if (isInvul)
        {
            return;
        }
		
		if (totalDamage > 0) {
			rend.color = Color.red;
            isRed = true;
            timeRed = 1f;
        } 
		// five frames to fix red
		

		if (currentHealth > maxHealth) {
			currentHealth = maxHealth;
		}

        if (healthBar != null)
        {
            hpbar_text = healthBar.GetChild(text).gameObject.GetComponent<Text>();

            decimal percent = (decimal)currentHealth / (decimal)maxHealth;
            healthBar.GetChild(hpindex).localScale = new Vector3((float)percent, 1, 1);
            percent *= 100;

            decimal shield = (decimal)blockHealth / (decimal)maxBlockHealth;
            healthBar.GetChild(shieldindex).localScale = new Vector3((float)shield, 1, 1);
            shield *= 100;

            hpbar_text.text = "Health: " + percent.ToString("##") + "% ";

        }
		//Freeze player movement
		if (currentHealth <= 0) {
            if (healthBar != null) { hpbar_text.text = "Health: 0%"; }
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

		UpdateHealth (-healthRegen);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Collision entered!");

        //Debug.Log(collision.contacts[0].normal);

        //if (collision.gameObject.layer == ObjectLayer)
        //{
            //Debug.Log("Object Collision - Updated");
        //   collidedObject = collision.transform;
            //Debug.Log(collision.gameObject);
        //}
        
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //Debug.Log("Collision exit");
        //if (collidedObject == collision.transform)
        //{
           // Debug.Log("Object Collision - Updated");
        //    collidedObject = null;
        //}
    }
}
