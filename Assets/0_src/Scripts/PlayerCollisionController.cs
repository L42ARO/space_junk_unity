using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCollisionController : MonoBehaviour
{
     [Tooltip("Assign functions to be called when colliding with 'space junk'.")]
    public UnityEvent onSpaceJunkCollision;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the other object has the tag "space junk"
        if (other.CompareTag("SpaceJunk"))
        {
            // Destroy the space junk object
            Destroy(other.gameObject);
            // Invoke the assigned event functions
            onSpaceJunkCollision?.Invoke();
        }
    }
}
