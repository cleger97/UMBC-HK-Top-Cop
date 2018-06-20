using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableObject : MonoBehaviour {

    private Transform parent = null;

    private enum State { Object, Carry, Thrown };

    public bool isLarge;

    private State currentState;

    private void Start()
    {
        currentState = State.Object;
    }

    public void Attach(Transform parent)
    {
        this.parent = parent;
        transform.parent = parent;
        //transform.localPosition = new Vector2(0, 0);
        //todo: move object to a spot relative to the player
    }

    public void Throw()
    {
        currentState = State.Thrown;
        if (isLarge)
        {
            // todo: velocity
            // arcing velocity
        } else
        {
            // todo: velocity
            // straight velocity
        }
    }

    public void Drop()
    {
        parent = null;
        transform.parent = null;
        currentState = State.Object;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        switch (currentState)
        {
            case State.Object:
                {
                    Debug.Log("Collided with object");
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
        Debug.Log("Exited collision");
    }
}


