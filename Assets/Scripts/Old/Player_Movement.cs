/*  By Matt Wong
 *  This script controls the movements for the player
 */

// Deprecated file
// All Player data is in Player.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour {


    public float speed;
    public float jumpHeight;

    //detects ground
    public Transform groundPoint;
    public float radius;
    public LayerMask groundMask;
    bool isGrounded;

	[HideInInspector]
	public bool isBlocking;

    //animator
    Animator anim;

    //object's rigidbody
    Rigidbody2D rb2D;

    //facing forward
    bool facingForward = true;

    int anim_speed;

	Vector2 addedVelocity;

	int framesToReset;
	int totalFrames;

    // Use this for initialization
    void Start () {
		addedVelocity = new Vector2 (0, 0);
        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

	// Update is called once per frame
	void Update () {
        anim.SetBool("inAir", !isGrounded);
        // moving

		// verify added velocity
		if (framesToReset == 0) {addedVelocity = new Vector2(0, 0);}


		// moving is more complicated now - added force from other scripts will cause the player to move a bit more
		float movx = Input.GetAxisRaw("Horizontal") * speed;

		// if isBlocking drop horizontal movement

		if (isBlocking) {movx = 0;}

		float movy = rb2D.velocity.y;
		Vector2 moveDir = new Vector2(movx, movy);
		rb2D.velocity = moveDir;
	

		// work on adding forces
		rb2D.AddForce (addedVelocity, ForceMode2D.Force);

		//Debug.Log (rb2D.velocity);

		//Debug.Log (moveDir);
        //checking if player is on the ground
        isGrounded = Physics2D.OverlapCircle(groundPoint.position, radius, groundMask);
        //Debug.Log(isGrounded);

        //switching faces
        if (Input.GetAxisRaw("Horizontal") == -1 && facingForward == true)
        {

            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            facingForward = false;

        }
        else if (Input.GetAxisRaw("Horizontal") == 1 && facingForward == false)
        {

            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            facingForward = true;
        }

        //start jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb2D.AddForce(new Vector2(0, jumpHeight));
        }

        anim_speed = Mathf.Abs((int)Input.GetAxisRaw("Horizontal"));
        anim.SetInteger("anim_speed", anim_speed);


		// reduce how many frames of bonus movement there are
		framesToReset--;
		addedVelocity = Vector2.zero;
		if (framesToReset > 0) {
			addedVelocity.x = addedVelocity.x * (framesToReset / totalFrames);
			addedVelocity.y = addedVelocity.y * (framesToReset / totalFrames);
		} else {
			totalFrames = 0;
		}

    }

	public void AddForce(Vector2 velocity, int frameCount) {
		addedVelocity.x += velocity.x;
		addedVelocity.y += velocity.y;

		framesToReset = frameCount;
		totalFrames = frameCount;
	}


    void OnCollisionEnter(Collision collision)
    {

        Debug.Log("Fire");
        if (collision.gameObject.tag == "Enemy")
        {
			Debug.Log ("Collided with enemy.");
            //Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        }

    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundPoint.position, radius);
    }

}

