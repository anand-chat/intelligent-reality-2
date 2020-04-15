using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;


[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
[RequireComponent(typeof(ARTapToPlaceObject))]
public class AvatarBasicControl : MonoBehaviour
{
    private Rigidbody rbody;
    private ARTapToPlaceObject aRTap;

    public float forwardMaxSpeed = 1f;
    public float turnMaxSpeed = 1f;

    private Ray belowDetectionRay;
    private Ray aboveDetectionRay;

    private RaycastHit belowHit;
    private RaycastHit aboveHit;

    //Useful if you implement jump in the future...
    public float jumpableGroundNormalMaxAngle = 45f;
    public bool closeToJumpableGround;
    private bool isGrounded;
    private Ray groundDetectionRay;
    private RaycastHit shootHit;
    private float range = 100.0f;
    private int touchableMask;
    private bool screenTapped;
    private ARPlaneManager arPlaneManager;

    [SerializeField] TMPro.TMP_Text StateText;
    [SerializeField] TMPro.TMP_Text Log1Text;
    [SerializeField] TMPro.TMP_Text Log2Text;
    [SerializeField] TMPro.TMP_Text Log3Text;
    [SerializeField] TMPro.TMP_Text Log4Text;

    private ARSessionOrigin arOrigin;

    private Vector3 currLoc;
    private bool changedPos;

    private int groundContactCount = 0;

	public bool IsGrounded
    {
        get
        {
            return groundContactCount > 0;
        }
    }


    void Awake()
    {
		changedPos = false;
        currLoc = transform.position;
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        arPlaneManager = arOrigin.GetComponent<ARPlaneManager>();
        belowDetectionRay = new Ray();
        aboveDetectionRay = new Ray();

        screenTapped = false;
        touchableMask = LayerMask.GetMask("touchable");
        groundDetectionRay = new Ray();
        rbody = GetComponent<Rigidbody>();

        if (rbody == null)
            Debug.Log("Rigid body could not be found");

        aRTap = GameObject.FindWithTag("Interaction").GetComponent<ARTapToPlaceObject>();//GetComponent<ARTapToPlaceObject>();

        if (aRTap == null)
            Debug.Log("ARTapToPlaceObject could not be found");

    }


    void Start()
    {
        TMPro.TMP_Text[] tt_arr = (TMPro.TMP_Text[])GameObject.FindObjectsOfType(typeof(TMPro.TMP_Text));
        for (int i = 0; i < tt_arr.Length; i++)
        {
            if (tt_arr[i].CompareTag("log1"))
            {
                Log1Text = tt_arr[i];
            }
            else if(tt_arr[i].CompareTag("log2"))
            {
                Log2Text = tt_arr[i];
            }
            else if(tt_arr[i].CompareTag("log3"))
            {
                Log3Text = tt_arr[i];
            }
            else if(tt_arr[i].CompareTag("log4"))
            {
                Log4Text = tt_arr[i];
            }
            else if(tt_arr[i].CompareTag("stateTag"))
            {
                StateText = tt_arr[i];
            }
        }

        GameObject levelGenObj = GameObject.Find("LevelGenerator");
        // set the json parser script to active (should initially be inactive)
        JsonParser jsonParser = levelGenObj.GetComponent<JsonParser>();
        // enable the json parser script
        jsonParser.enabled = true;
    }

    private void Update()
    {

        //testFuncs();
        /*if (isGrounded)
        {
            StateText.text = "grounded";
        }
        else
        {
            StateText.text = "NOT grounded";
        }*/
    }

    void FixedUpdate()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            screenTapped = true;
        }
        else
        {
            screenTapped = false;
        }

		float inputForward = 1.0f;
        Vector3 dirMove = new Vector3(0, 0, 0);

        if (aRTap.enabled)
        {
            if (aRTap.isPlacementPoseValid())
            {
                // set the direction
                dirMove = aRTap.getPlacementPose().position - gameObject.transform.position;
                dirMove.y = 0;
                dirMove = dirMove.normalized;
            }
            else
            {
                // don't move anywhere
                dirMove = new Vector3(0, 0, 0);
            }
        }

        groundDetectionRay.origin = transform.position;
        groundDetectionRay.direction = -transform.up;
        bool closeToGrnd = Physics.Raycast(groundDetectionRay, out shootHit, range, touchableMask);

        belowDetectionRay.origin = transform.position;
        belowDetectionRay.direction = transform.up;
        bool isBelow = Physics.Raycast(belowDetectionRay, out belowHit, touchableMask);

        aboveDetectionRay.origin = transform.position;
        aboveDetectionRay.direction = -transform.up;
        bool isAbove = Physics.Raycast(aboveDetectionRay, out aboveHit, touchableMask);

        //onCollisionXXX() doesn't always work for checking if the character is grounded from a playability perspective
        //Uneven terrain can cause the player to become technically airborne, but so close the player thinks they're touching ground.
        //Therefore, an additional raycast approach is used to check for close ground
        isGrounded = IsGrounded || CharacterCommon.CheckGroundNear(this.transform.position, jumpableGroundNormalMaxAngle, 50.0f, 0, out closeToJumpableGround);

        if (screenTapped && /*closeToGrnd &&*/ groundContactCount > 0)
        {
            rbody.AddForce(new Vector3(0, 5.0f, 0), ForceMode.Impulse);
            screenTapped = false;
		}

        //We use rbody.MovePosition() as it's the most efficient and safest way to directly control position in Unity's Physics
        rbody.MovePosition(rbody.position + dirMove * inputForward * Time.deltaTime * forwardMaxSpeed);
        //Most characters use capsule colliders constrained to not rotate around X or Z axis
        //However, it's also good to freeze rotation around the Y axis too. This is because friction against walls/corners
        //can turn the character. This errant turn is disorienting to players. 
        //Luckily, we can break the frozen Y axis constraint with rbody.MoveRotation()
        //BTW, quaternions multiplied has the effect of adding the rotations together
        //rbody.MoveRotation(rbody.rotation * Quaternion.AngleAxis(inputTurn * Time.deltaTime * turnMaxSpeed, Vector3.up));

        if (currLoc != rbody.position)
        {
            changedPos = true;
        }
        else
        {
            changedPos = false;
        }

        if (screenTapped)
        {
            screenTapped = false;
        }
    }

    //This is a physics callback
    void OnCollisionEnter(Collision collision)
    {

        if (collision.transform.gameObject.tag == "ground")
        {
            ++groundContactCount;
            //EventManager.TriggerEvent<PlayerLandsEvent, Vector3, float>(collision.contacts[0].point, collision.impulse.magnitude);
        }

    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.transform.gameObject.tag == "ground")
        {
            --groundContactCount;
        }
    }

    private (bool, string) on(GameObject go1, GameObject go2)
    {
        Ray belowDetectRay = new Ray();
        RaycastHit belowHt;
        belowDetectRay.origin = go1.transform.position;
        belowDetectRay.direction = -go1.transform.up;
        int touchMask = LayerMask.GetMask("touchable");
        bool isBelow = Physics.Raycast(belowDetectRay, out belowHt, touchMask);

        Collider go1Collider = go1.GetComponent<Collider>();
        Collider go2Collider = go2.GetComponent<Collider>();
        Collider cldr = belowHt.collider;
        if (go1Collider.bounds.Intersects(go2Collider.bounds) && cldr != null && cldr.gameObject == go2)
        {
            return (true, "bool");
        }
        /*
        if(groundContactCount > 0 && objectsOn.Contains(go2))
        {
            return true;
        }
        */
        return (false, "bool");
    }

    private (bool, string) touches(GameObject go1, GameObject go2)
    {
        Collider go1Collider = go1.GetComponent<Collider>();
        Collider go2Collider = go2.GetComponent<Collider>();
        if(go1Collider.bounds.Intersects(go2Collider.bounds))
        {
            return (true, "bool");
        }
        /*
        if (objectsTouching.Contains(go2))
        {
            return true;
        }
        */
        return (false, "bool");
    }

    private (bool, string) below(GameObject go1, GameObject go2)
    {
        // NOTE: can use this code if want to know if directly beneath or not
        /*Ray belowDetectRay = new Ray();
        RaycastHit belowHt;
        belowDetectRay.origin = go1.transform.position;
        belowDetectRay.direction = go1.transform.up;
        int touchMask = LayerMask.GetMask("touchable");
        bool isBelow = Physics.Raycast(belowDetectRay, out belowHt, touchMask);
        if(belowHt.collider.gameObject == go2)
        {
            return true;
        }
        return false;
        */

        float height1 = go1.GetComponent<Collider>().bounds.size.y;


        // get top of gameobject
        float yPos = go1.transform.position.y + (height1 / 2.0f);

        if(yPos < go2.transform.position.y)
        {
            return (true, "bool");
        }
        return (false, "bool");
    }

    private (bool, string) isMoving(GameObject go1)
    {
        Rigidbody rb = go1.GetComponent<Rigidbody>();
        Vector3 zeroVec = new Vector3(0, 0, 0);
        if (rb.velocity != zeroVec || changedPos)
        {
            return (true, "bool");
        }
        return (false, "bool");
    }

    private void testFuncs()
    {
        foreach (ARPlane plane in arPlaneManager.trackables)
        {
            if(on(this.gameObject, plane.gameObject).Item1)
            {
                Log1Text.text = "on ground";
            }
            else
            {
                Log1Text.text = "NOT on ground";
            }

            if(touches(this.gameObject, plane.gameObject).Item1)
            {
                Log2Text.text = "touching";
            }
            else
            {
                Log2Text.text = "NOT touching";
            }

            if(below(this.gameObject, plane.gameObject).Item1)
            {
                Log3Text.text = "below";
            }
            else
            {
                Log3Text.text = "NOT below";
            }

            if(isMoving(this.gameObject).Item1)
            {
                Log4Text.text = "moving";
            }
            else
            {
                Log4Text.text = "NOT moving";
            }

            // just get the first one for testing purposes
            break;
        }
    }
}