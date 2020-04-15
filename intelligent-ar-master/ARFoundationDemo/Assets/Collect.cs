using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider c)
    {
        Collector collector = c.attachedRigidbody.gameObject.GetComponent<Collector>();
        if(c.attachedRigidbody.gameObject.tag == "Player" && collector != null)
        {
            // increment player's coin count
            collector.pickedCoinUp();
            Destroy(this.gameObject);
        }
    }
}
