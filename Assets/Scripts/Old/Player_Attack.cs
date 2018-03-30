/*  By Matt Wong
 *  This script controls the attack abilities of the player
 */

// Deprecated file
// All Player data is in Player.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Attack : MonoBehaviour
{

	public int base_damage;
	private int damage;

	public float base_attkRange;
	private float attkRange;

	private Animator anim;

	public int base_knockBackForce;
	private int knockBackForce;

	public float damageRadius;
	public Transform dmgArea;

	public bool isBlocking;

	private float attackCd = 0.08f;
	private float timer;

	private bool leftPunch;


	// Use this for initialization
	void Start ()
	{
		damage = base_damage;
		attkRange = base_attkRange;
		knockBackForce = base_knockBackForce;
		anim = GetComponent<Animator> ();
		isBlocking = false;

		leftPunch = true;
	}

	// Update is called once per frame
	void Update ()
	{
		anim.speed = 1f;
		anim.ResetTrigger ("isPunching");
		anim.ResetTrigger ("isPunching2");

		if ((Input.GetKey(KeyCode.J) == true || Input.GetButton ("Fire1") == true) && isBlocking == false) {
            
			if (leftPunch) {
				anim.SetTrigger ("isPunching");
			} else {
				anim.SetTrigger ("isPunching2");
			}

			anim.speed = 5f;
			GameObject[] checkInAttack = colliderTagSorter ("Enemy", getAllAround (damageRadius, dmgArea));
			if (timer > 0) {
				timer -= Time.deltaTime;
			} else if (checkInAttack.Length > 0) {
				for (int i = 0; i < checkInAttack.Length; i++) {
					Enemy enemyScript = checkInAttack [i].transform.GetComponent<Enemy> ();
					// Calculate Angle Between the collision point and the player
					Vector3 dir = checkInAttack [i].transform.position - transform.position;
					// We then get the opposite (-Vector3) and normalize it
					dir = dir.normalized;

					// Change: Only horizontal momentum on initial attacks.
					dir.y = 0;
					// And finally we add force in the direction of dir and multiply it by force. 
					checkInAttack [i].transform.GetComponent<Rigidbody2D> ().AddForce (dir * knockBackForce);
                    
					if (enemyScript != null) {
						//Check for attack cooldown
						if (timer <= 0) {
							float attackTimer = 2f;
							while (attackTimer > 0)
								attackTimer -= Time.deltaTime;
							enemyScript.takeDamage (damage);

							timer = attackCd;

							if (leftPunch) {
								leftPunch = false;
							} else {
								leftPunch = true;
							}
						}
					}    
				}
			}
			// finish punch so switch anims

		}

		if (Input.GetButton ("Fire2") == true || Input.GetKey(KeyCode.K) == true) {
			isBlocking = true;
			anim.SetBool ("isBlocking", true);

			//gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX;
		} else {
			isBlocking = false;
			anim.SetBool ("isBlocking", false);
			//gameObject.GetComponent<Rigidbody2D>().constraints &= ~RigidbodyConstraints2D.FreezePositionX;
			//gameObject.GetComponent<Rigidbody2D>().constraints &= ~RigidbodyConstraints2D.FreezePositionY;
			//gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
		}
        
	}
	//takes in an array of Collider2D objects and returns an array of GameObjects that have a certain tag
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


	void OnDrawGizmos ()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere (dmgArea.position, damageRadius);
	}
}
