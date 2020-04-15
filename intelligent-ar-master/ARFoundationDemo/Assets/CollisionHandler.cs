using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    private List<GameObject> collidingGameObjects;
    // Start is called before the first frame update
    void Start()
    {
        collidingGameObjects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<GameObject> getCollidingGameObjects()
    {
        return collidingGameObjects;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // add the game object being collided with to the list
        collidingGameObjects.Add(collision.gameObject);
    }

    private void OnCollisionExit(Collision collision)
    {
        // remove the game object stopped colliding with from the list
        collidingGameObjects.Remove(collision.gameObject);
    }
}
