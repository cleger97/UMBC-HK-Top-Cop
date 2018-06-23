using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ThrowableObject : MonoBehaviour {

    private Transform parent = null;
    private Rigidbody2D rb2D;
    private BoxCollider2D bx2D;

    private enum State { Object, Carry, Thrown };

    public bool isLarge;

    private State currentState;

    private void Start()
    {
        currentState = State.Object;
        rb2D = gameObject.GetComponent<Rigidbody2D>();
        bx2D = gameObject.GetComponent<BoxCollider2D>();
    }

    public Transform GetTransform()
    {
        return this.transform;
    }

    public void Attach(Transform parent)
    {
        this.parent = parent;
        transform.parent = parent;
        transform.localPosition = new Vector2(0, 0);
        //todo: move object to a spot relative to the player

        rb2D.bodyType = RigidbodyType2D.Kinematic;

        bx2D.enabled = false;

        transform.rotation = Quaternion.identity;

        rb2D.velocity = Vector2.zero;
    }

    public void Throw(int direction)
    {
        Vector2 force = new Vector2(3000f, 0f);
        currentState = State.Thrown;
        // TODO: Add arcing for large.
        force.x = force.x * direction;
        Debug.Log(force.x);

        transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y + 0.5f);

        rb2D.bodyType = RigidbodyType2D.Dynamic;
        rb2D.gravityScale = 1f;

        parent = null;
        transform.parent = null;

        bx2D.enabled = true;
       

        rb2D.AddForce(force);
    }

    public void Drop()
    {
        parent = null;
        transform.parent = null;
        currentState = State.Object;

        bx2D.enabled = true;
        rb2D.bodyType = RigidbodyType2D.Kinematic;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        switch (currentState)
        {
            case State.Object:
                {
                    //Debug.Log("Collided with object");
                    break;
                }
            case State.Thrown:
                {
                    if (collision.otherCollider.transform.tag == "Enemy")
                    {

                    }
                    break;
                }
            case State.Carry:
                break;
            default:
                break;
        }
       
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        //Debug.Log("Exited collision");
    }
}


