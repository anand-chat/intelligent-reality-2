using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;

public class ARTapToPlaceObject : MonoBehaviour
{
    public GameObject objectToPlace;
    //public GameObject testSurface;
    public GameObject coin;
    public GameObject placementIndicator;
    private ARPlaneManager arPlaneManager;
    //[SerializeField] TMPro.TMP_Text StateText;
    [SerializeField] TMPro.TMP_Text HitText;
    private bool hasPlacedPlayer;
    private Logging log;

    private ARSessionOrigin arOrigin;

    // describes position and rotation of a 3D point
    private Pose placementPose;

    // to keep track of whether raycast is hitting a flat surface or not
    private bool placementPoseIsValid = false;

    public bool isPlacementPoseValid()
    {
        return placementPoseIsValid;
    }

    public Pose getPlacementPose()
    {
        return placementPose;
    }

    void Start()
    {
        hasPlacedPlayer = false;
        arOrigin = FindObjectOfType<ARSessionOrigin>();
        arPlaneManager = arOrigin.GetComponent<ARPlaneManager>();

        // get access to logger
        log = GameObject.FindGameObjectWithTag("Logger").GetComponent<Logging>();
    }

    void Update()
    {
        // on every frame update, we want to ...
        // 1. check world around us
        // 2. find out where the camera is pointing
        // 3. identify if position where we can place virtual object
        // placementPose represents this position in space
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (Input.touchCount > 0)
        {
            // planemanager turn it off
            arPlaneManager.enabled = false;
        }

        // if position is valid to place object and if the first finger touch just began
        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !hasPlacedPlayer)
        {
            PlaceObject();
            hasPlacedPlayer = true;
        }
    }

    void FixedUpdate()
    {
        //StateText.text = "FixedUpdating";
        RaycastHit physHit;
        Debug.DrawRay(Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f)), new Vector3(1,1,1) * 1000, Color.blue, duration:10.0f, depthTest:false);
        /*if (Physics.Raycast(Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f)), Camera.current.transform.forward, out physHit))
        {
            HitText.text = "HIT!!!";
        }
        else
        {
            HitText.text = "no hit";
        }*/
    }

    private void PlaceObject()
    {
        //Instantiate(testSurface, placementPose.position + new Vector3(0, 0.5f, 0), placementPose.rotation);
        //Instantiate(coin, placementPose.position + new Vector3(1, 0.5f, 0), placementPose.rotation);
        Instantiate(objectToPlace, placementPose.position + new Vector3(0, 2, 0), placementPose.rotation);
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            // to show the object
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            // to hide the object
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        // want to shoot this raycast from the center of the screen
        var screenCenter = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));

        // represent points in physical space where this ray hits a physical surface
        var hits = new List<ARRaycastHit>();

        // the 3rd argument is for detecting all kinds of planes
        arOrigin.GetComponent<ARRaycastManager>().Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon);

        int touchMask = LayerMask.GetMask("touchable");
        Vector3 camCurrPos = Camera.current.transform.position;
        Vector3 forwardDir = Camera.current.transform.forward;
        RaycastHit[] rayHitsArr = Physics.RaycastAll(camCurrPos, forwardDir, Mathf.Infinity, touchMask);

        HitText.text = "CamCurrPos:" + camCurrPos + "physics #hit: " + rayHitsArr.Length;

        // check to see if hits array has at least 1 item in it
        int hitGeneratedCount = rayHitsArr.Length;
        placementPoseIsValid = /*hits.Count > 0 ||*/ hitGeneratedCount > 0;
        if (placementPoseIsValid)
        {
            float shortestDist = -1.0f;
            Vector3 shortestDistPoint = new Vector3(0,0,0);
            for (int i = 0; i < hitGeneratedCount; i++)
            {
                Vector3 hitPoint = rayHitsArr[i].point;
                float thisDist = Vector3.Distance(camCurrPos, hitPoint);
                if (thisDist < shortestDist || shortestDist < 0.0f)
                {
                    shortestDist = thisDist;
                    shortestDistPoint = hitPoint;
                }
            }
            placementPose = new Pose();//hits[0].pose;
            placementPose.position = shortestDistPoint;

            // to rotate the placement indicator with how the device is pointing

            // vector that acts like an arrow that describes the camera is facing along x, y, and z axes
            var cameraForward = Camera.current.transform.forward;

            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
}