﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempEnemy: Enemy
{
	public float maxHealth;
	public float currHealth;
	public int damage;
	public float moveSpeed;
	public float attkCoolDown;

	public float detectionCircleRadius = 500f;
	public float attackCircleRadius = 0.5f;

	public Transform dmgArea;
	GameObject player;
	Player playerData;
	private float timer;

	Animator anim;
	Color defColor;
	float timeRed;
	public float TimeDisplayHurt;


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
    void Start ()
	{
        num_enemy_alive++;
		maxHealth = 50f;
		currHealth = maxHealth;
		player = GameObject.FindGameObjectWithTag ("Player");
		playerData = player.GetComponent<Player> ();
		anim = GetComponent<Animator> ();
		defColor = GetComponent<SpriteRenderer> ().material.GetColor ("_Color");
		timeRed = TimeDisplayHurt;

        left_end_pos = transform.position.x - length; 
        right_end_pos = transform.position.x + length;
    }

	// Update is called once per frame
	void Update ()
	{
        //isGrounded = Physics2D.OverlapCircle(groundPoint.position, radius, groundMask);
        //Debug.Log("Current num of enemy:" + num_enemy_alive);
        if (timeRed > 0) {
			timeRed -= Time.deltaTime;
		} else
			GetComponent<SpriteRenderer> ().material.SetColor ("_Color", defColor);
		if (currHealth <= 0) {
			dead ();
		} else {
			//anim.ResetTrigger("Attack");
			anim.SetBool ("Attack", false);

            if (timer > 0)
            {
                timer -= Time.deltaTime;
                return;
            }

			GameObject[] check = colliderTagSorter ("Player", Enemy.getAllAround (detectionCircleRadius, transform));
			if (check.Length > 0) {
				GameObject player = check [0];
				GameObject[] checkInAttack = colliderTagSorter ("Player", Enemy.getAllAround (attackCircleRadius, dmgArea));
				if (player.GetComponent<Player> ().IsPlayerAlive () == false) {
					endGame ();
				}
				if (checkInAttack.Length > 0) {
					//anim.SetBool("InArea", false);
					if (timer <= 0) {
						// anim.SetTrigger("Attack");
						anim.SetBool ("Attack", true);
						float timer2 = 2f;
						// timer machine broke


						checkInAttack = colliderTagSorter ("Player", Enemy.getAllAround (attackCircleRadius, dmgArea));
						if (checkInAttack.Length > 0) {
							float distance = player.transform.position.x - this.transform.position.x;
							int direction = (int) Mathf.Sign (distance);
                            //Debug.Log("Direction = " + direction);
							// direction will be 1 for player on right side and -1 for player
							// on left side
							player.GetComponent<Player> ().GetHit (damage,direction);
						}
						timer = attkCoolDown;
					}
				} else {
					//anim.SetBool("InArea", true);
					anim.SetBool ("Attack", false);
					Vector2 toMove;
                    if (player.GetComponent<Player>().IsPlayerAlive()) {
                        //if (Mathf.Abs(player.transform.position.x - transform.position.x) <= DetLength) {
                            if (player.transform.position.x > transform.position.x) {
                                faceDir = FACE_RIGHT;
                                anim.SetInteger("speed", (int)moveSpeed);
                                toMove = new Vector2(moveSpeed, GetComponent<Rigidbody2D>().velocity.y);
							transform.localScale = new Vector3(-1 * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                            } else {
                                faceDir = FACE_LEFT;
                                anim.SetInteger("speed", (int)moveSpeed);
								transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                                toMove = new Vector2(-moveSpeed, GetComponent<Rigidbody2D>().velocity.y);
                            }
                            GetComponent<Rigidbody2D>().velocity = toMove;
                            left_end_pos = transform.position.x - length;
                            right_end_pos = transform.position.x + length;
                        //}
                        //else
                        /*{
                            
                            if ((faceDir == FACE_LEFT) && transform.position.x >= left_end_pos)
                            {

                                anim.SetInteger("speed", 1);
                                toMove = new Vector2(-1, GetComponent<Rigidbody2D>().velocity.y);
								transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
                                GetComponent<Rigidbody2D>().velocity = toMove;
                            }
                            else
                            {
                                faceDir = FACE_RIGHT;
                                //Jump();
                            }
                            if ((faceDir == FACE_RIGHT) && transform.position.x <= right_end_pos)
                            {
                                anim.SetInteger("speed", 1);
                                toMove = new Vector2(1, GetComponent<Rigidbody2D>().velocity.y);
								transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                                GetComponent<Rigidbody2D>().velocity = toMove;
                            }
                            else
                                faceDir = FACE_LEFT;
                        }*/

                    }
				}
			} else {
				anim.SetBool ("InArea", false);
			}
		}
        
	}
    public override string returnName()
    {
        return "normal";
    }
    public override void takeDamage (int damage)
	{
        
		currHealth -= damage;
		GetComponent<SpriteRenderer> ().material.SetColor ("_Color", Color.red);
		SetHealthBar (currHealth);
		timeRed = TimeDisplayHurt;

		// Apply hitstun
		timer = (timer > 1f) ? timer : 1f;
	}

	IEnumerator deathSequence ()
	{
		yield return new WaitForSeconds (10);
	}

	//takes in an array of Collider2D objects and returns an array of GameObjects that have a certain tag
	/*private GameObject[] colliderTagSorter (string tagName, Collider2D[] toSort)
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
    */
	void OnDrawGizmos ()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere (transform.position, detectionCircleRadius);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere (dmgArea.position, attackCircleRadius);
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

	public void SetHealthBar (float health)
	{
		float calHealth = health / maxHealth;

		healthBar.transform.localScale = new Vector3 (calHealth, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
	}
}
