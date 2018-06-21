using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewEnemy : Enemy {

	// state machine for enemies
	private enum EnemyState { STUNNED, ATTACK, MOVE, IDLE }
	private EnemyState currentState;


	public float maxHealth = 50f;
	public float currHealth = 50f;
	public const float damage = 10f;
	public float moveSpeed;
	public float attkCooldown;

	public float detectionCircleRadius = 500f;
	public float attackCircleRadius = 0.5f;

	public Transform dmgArea;
	GameObject player;
	Player playerData;
	private float stunTime;
	private const float baseStunTime = 0.25f;
	private const float attackSwingTime = 0.6f;

	private float attackSwingCurrentTime;

	Animator anim;
	Color defColor;
	float timeRed = 1f;
	public float TimeDisplayHurt = 1f;


	private const float DetLength = 3f;
	private float left_end_pos;
	private float right_end_pos;
	private const float length = 2;
	private int faceDir;
	private const int FACE_RIGHT = 1;
	private const int FACE_LEFT = -1;


	public Transform groundPoint;
	public float radius = 0.1f;
	public LayerMask groundMask;
	private bool isGrounded;

	// Use this for initialization
	void Start () {
		num_enemy_alive++;
		player = GameObject.FindGameObjectWithTag ("Player");
		playerData = player.GetComponent<Player> ();
		anim = GetComponent<Animator> ();
		defColor = GetComponent<SpriteRenderer> ().material.GetColor ("_Color"); 

		left_end_pos = transform.position.x - length; 
		right_end_pos = transform.position.x + length;

		currentState = EnemyState.MOVE;
	}
	
	// Update is called once per frame
	void Update () {
		// if paused don't do anything
		if (MenuUIHandle.instance.paused == true) {
			return;
		}
		// if player dead then remove this object
		if (player.GetComponent<Player> ().IsPlayerAlive () == false) {
			endGame ();
		}

		if (timeRed > 0) {
			timeRed -= Time.deltaTime;
			if (timeRed <= 0) {
				GetComponent<SpriteRenderer> ().material.SetColor ("_Color", defColor);
			}
		}

		// if stunned then reduce the timer and return;
		if (stunTime > 0) {
			stunTime -= Time.deltaTime;
			if (stunTime <= 0) {
				stunTime = 0;
				currentState = EnemyState.MOVE;
			}
			return;
		}

		Debug.Log (currentState);

		// if not stunned then do something with it's current state
		switch (currentState) {
		case EnemyState.MOVE:
				ChasePlayer ();
				return;
		case EnemyState.ATTACK:
				AttackPlayer ();
				return;
        case EnemyState.IDLE:
                idleState();
                return;
		default:
			return;
		}

	}

	public void ChasePlayer() {
		anim.SetBool ("Attack", false);
		GameObject[] check = colliderTagSorter ("Player", Enemy.getAllAround (attackCircleRadius, transform));
        if (check.Length > 0) {
            attackSwingCurrentTime = attackSwingTime;
            currentState = EnemyState.ATTACK;

            return;
        } else {

            if (Mathf.Abs(player.transform.position.x - transform.position.x) <= DetLength)
            {
                Vector2 toMove;
                if (player.transform.position.x > transform.position.x)
                {
                    faceDir = FACE_RIGHT;
                    anim.SetInteger("speed", (int)moveSpeed);
                    toMove = new Vector2(moveSpeed, GetComponent<Rigidbody2D>().velocity.y);
                    transform.localScale = new Vector3(-1 * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
                else
                {
                    faceDir = FACE_LEFT;
                    anim.SetInteger("speed", (int)moveSpeed);
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    toMove = new Vector2(-moveSpeed, GetComponent<Rigidbody2D>().velocity.y);
                }
                GetComponent<Rigidbody2D>().velocity = toMove;
                left_end_pos = transform.position.x - length;
                right_end_pos = transform.position.x + length;
            }
            else
            {
                currentState = EnemyState.IDLE;
            }
		}
	}

	public void AttackPlayer() {
		GameObject[] checkInAttack = colliderTagSorter ("Player", Enemy.getAllAround (attackCircleRadius, dmgArea));
		if (checkInAttack.Length <= 0) {
			currentState = EnemyState.MOVE;
			return;
		} else {
			// Wait for attack backswing
			if (attackSwingCurrentTime > 0) {
				attackSwingCurrentTime -= Time.deltaTime;
				if (attackSwingCurrentTime < 0.3f) {
					anim.SetBool ("Attack", true);
				}
				if (attackSwingCurrentTime < 0) {
					attackSwingCurrentTime = 0;
				}
				return;
			}
			// If the attack backswing is over then continue onto the attack
			float distance = player.transform.position.x - this.transform.position.x;
			int direction = (int) Mathf.Sign (distance);
			player.GetComponent<Player> ().GetHit ((int)damage,direction);
			attackSwingCurrentTime = attackSwingTime;
			anim.SetBool ("Attack", false);
		}
	}

    public void idleState()
    {
        Vector2 toMove;
        if ((faceDir == FACE_LEFT) && transform.position.x >= left_end_pos)
        {

            anim.SetInteger("speed", (int)moveSpeed - 1);
            toMove = new Vector2(-1, GetComponent<Rigidbody2D>().velocity.y);
            transform.localScale = new Vector3(1 * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            GetComponent<Rigidbody2D>().velocity = toMove;
        }
        else
        {
            faceDir = FACE_RIGHT;

        }
        if ((faceDir == FACE_RIGHT) && transform.position.x <= right_end_pos)
        {
            anim.SetInteger("speed", (int)moveSpeed - 1);
            toMove = new Vector2(1, GetComponent<Rigidbody2D>().velocity.y);
            transform.localScale = new Vector3(-1 * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            GetComponent<Rigidbody2D>().velocity = toMove;
        }
        else
            faceDir = FACE_LEFT;

        if (Mathf.Abs(player.transform.position.x - transform.position.x) <= DetLength)
            currentState = EnemyState.MOVE;

    }


	public override void takeDamage (int damage)
	{

		currHealth -= damage;
		GetComponent<SpriteRenderer> ().material.SetColor ("_Color", Color.red);
		SetHealthBar (currHealth, maxHealth);
		timeRed = TimeDisplayHurt;

		if (currHealth <= 0) {
			dead ();
		}

		// Apply hitstun
		stunTime = (stunTime > baseStunTime) ? stunTime : baseStunTime;
		currentState = EnemyState.STUNNED;
	}

	public override string returnName()
	{
		return "normal";
	}

	void dead ()
	{
		//anim.SetBool("InArea", false);
		anim.SetBool ("Death", true);
		//Destroy(gameObject, 2f);
		//TEMPORARY WAY FOR AFTERMATH OF FIGHT
		GetComponent<BoxCollider2D> ().enabled = false;
		GetComponent<Rigidbody2D> ().gravityScale = 0;
		GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, 0);
		playerData.updateKill ();
		this.enabled = false;
		num_enemy_alive--;
		if (currHealth <= 0)
			Destroy (gameObject, 0.25f);
	}

	void endGame ()
	{
		anim.SetBool ("Death", true);
		this.enabled = false;
	}


}
