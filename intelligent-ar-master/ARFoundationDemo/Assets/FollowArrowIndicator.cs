using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody), typeof(CapsuleCollider))]
public class FollowArrowIndicator : MonoBehaviour
{
    private Rigidbody rbody;
    private GameObject arrowIndicator;

    public float forwardMaxSpeed = 1f;
    public float turnMaxSpeed = 1f;

    //Useful if you implement jump in the future...
    public float jumpableGroundNormalMaxAngle = 45f;
    public bool closeToJumpableGround;

    public bool isGrounded;



    void Awake()
    {
        rbody = GetComponent<Rigidbody>();

        if (rbody == null)
            Debug.Log("Rigid body could not be found");

        arrowIndicator = GameObject.Find("Placement Indicator");
    }


    // Use this for initialization
    void Start()
    {
        isGrounded = false;

        //never sleep so that OnCollisionStay() always reports for ground check
        rbody.sleepThreshold = 0f;
    }




    void FixedUpdate()
    {

        float inputForward = 0f;
        float inputTurn = 0f;

        //this.transform.Translate(Vector3.forward * cinput.Forward * Time.deltaTime * forwardMaxSpeed);
        //this.transform.Rotate(Vector3.up, cinput.Turn * Time.deltaTime * turnMaxSpeed);

        // todo: get normalized vector between center of object and center of arrow indicator
        Vector3 toIndicatorDir = arrowIndicator.transform.position - this.transform.position;
        toIndicatorDir = toIndicatorDir.normalized;

        //It's supposed to be safe to not scale with Time.deltaTime (e.g. framerate correction) within FixedUpdate()
        //If you want to make that optimization, you can precompute your velocity-based translation using Time.fixedDeltaTime
        //We use rbody.MovePosition() as it's the most efficient and safest way to directly control position in Unity's Physics
        rbody.MovePosition(rbody.position + toIndicatorDir * 1 * Time.deltaTime * forwardMaxSpeed);
        //Most characters use capsule colliders constrained to not rotate around X or Z axis
        //However, it's also good to freeze rotation around the Y axis too. This is because friction against walls/corners
        //can turn the character. This errant turn is disorienting to players. 
        //Luckily, we can break the frozen Y axis constraint with rbody.MoveRotation()
        //BTW, quaternions multiplied has the effect of adding the rotations together
        //rbody.MoveRotation(rbody.rotation * Quaternion.AngleAxis(inputTurn * Time.deltaTime * turnMaxSpeed, Vector3.up));

        //clear for next OnCollisionStay() callback      
        isGrounded = false;

    }




    //This is a physics callback
    void OnCollisionStay(Collision collision)
    {
        isGrounded = true;

    }

    //This is a physics callback
    void OnCollisionEnter(Collision collision)
    {

        /*if (collision.transform.gameObject.tag == "ground")
        {
            EventManager.TriggerEvent<MinionLandsEvent, Vector3, float>(collision.contacts[0].point, collision.impulse.magnitude);
        }*/

    }






}
