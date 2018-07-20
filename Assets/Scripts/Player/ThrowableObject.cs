using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class ThrowableObject : MonoBehaviour {

    private Transform parent = null;
    private Rigidbody2D rb2D;
    private BoxCollider2D bx2D;

    private enum State { Object, Carry, Thrown };

    public bool isLarge;

    public int damage = 1000;

    public float horizontalSpeed = 6f;

    private State currentState;

    private Player player;

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
        currentState = State.Carry;

        this.parent = parent;
        transform.parent = parent;
        transform.localPosition = new Vector2(0, 0);
        //todo: move object to a spot relative to the player

        rb2D.bodyType = RigidbodyType2D.Kinematic;

        bx2D.enabled = false;

        transform.rotation = Quaternion.identity;
        rb2D.velocity = Vector2.zero;
    }

    public void Throw(int direction, float initial)
    {
        Vector2 force = new Vector2(horizontalSpeed, -1f);
        currentState = State.Thrown;
        // TODO: Add arcing for large.
        force.x = force.x * direction;
        force.x += initial;
        Debug.Log(force.x);

        transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y + 0.5f);

        rb2D.bodyType = RigidbodyType2D.Dynamic;
        rb2D.gravityScale = 1f;

        parent = null;
        transform.parent = null;

        gameObject.layer = 15;

        bx2D.enabled = true;
       

        rb2D.velocity = force;
    }

    public void Drop(int direction)
    {
        parent = null;
        transform.parent = null;
        currentState = State.Object;

        bx2D.enabled = true;
        rb2D.bodyType = RigidbodyType2D.Dynamic;

        Vector2 dropAngle = new Vector2(2.5f * direction, 1f);
        rb2D.velocity = dropAngle;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision entered");
        if (collision.gameObject.layer == 8)
        {
            if (gameObject.layer == 15)
            {
                Destroy(this.gameObject);
            }
            
        }

        switch (currentState)
        {
            case State.Object:
                {
                    //Debug.Log("Collided with object");
                    break;
                }
            case State.Thrown:
                {
                    if (collision.transform.tag == "Enemy")
                    {
                        collision.gameObject.GetComponent<Enemy>().takeDamage(damage);
                    }
                    break;
                }
            case State.Carry:
                break;
            default:
                break;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger entered");
        Debug.Log(currentState);
        if (currentState != State.Carry)
        {
            if (currentState == State.Thrown)
            {
                if (other.transform.tag == "Enemy")
                {
                    other.gameObject.GetComponent<Enemy>().takeDamage(damage);
                }
            }
            if (other.tag == "Player")
            {
                Debug.Log("Updating");
                player = other.GetComponent<Player>();
                other.GetComponent<Player>().SetCollidedObject(this.transform);
               
            }
        }
        

    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Trigger exited");

        if (player != null)
        {
            player.SetCollidedObject(null);
        }
        

    }


}


