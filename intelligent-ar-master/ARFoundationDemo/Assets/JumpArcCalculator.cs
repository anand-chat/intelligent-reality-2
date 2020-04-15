using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpArcCalculator : MonoBehaviour
{
    //private AvatarBasicControl basicControlScript;
    public float v_y_0;
    public float v_xz_0;
    private float v_0;

    public float t_inc;
    public float t_max;
    public int arc_rot_times;

    public int num_edge_rots;
    public float inc_amt;

    private bool foundPlatformOn;

    private List<ARGameObject> platforms;
    ARGameObject arAvatar;

    // todo: make this private after done testing
    public List<List<GameObject>> arcPointList;

    // prefab for point
    public GameObject arcPoint;

    // prefab for edge point
    public GameObject edgePoint;

    // for decreasing v_0 for calculating subarcs
    public int v_0_subArcNumTimes;

    [SerializeField] TMPro.TMP_Text StateText;
    [SerializeField] TMPro.TMP_Text Log1Text;
    [SerializeField] TMPro.TMP_Text Log2Text;
    [SerializeField] TMPro.TMP_Text Log3Text;
    [SerializeField] TMPro.TMP_Text Log4Text;

    private void Awake()
    {
        //Log3Text.text = "JumpArcCalc Awake starting!";
        v_xz_0 = 0;
        v_y_0 = 0;
        v_0 = 0;

        arcPointList = new List<List<GameObject>>();

        //Log3Text.text = "JumpArcCalc Awake called!";
    }

    // Start is called before the first frame update
    void Start()
    {
        Log1Text.text = "JumpArcCalc Start called!";

        foundPlatformOn = false;

        // set the Level Generator script 'JsonParser.cs' to active (should initially be inactive)
        //Log1Text.text = "JumpArcCalc, getting the JsonParser!";
        JsonParser jsonParser = GetComponent<JsonParser>();
        //Log1Text.text = "JumpArcCalc Start COMPLETE!";
        arAvatar = jsonParser.getAvatar();

        // set the v_0 values
        v_xz_0 = arAvatar.v_xz_0_jump;
        v_y_0 = arAvatar.v_y_0_jump;

        // get reference to Basic Controller
        //GameObject player = GameObject.Find("Player");
        //Log1Text.text = "Player get!";
        //basicControlScript = player.GetComponent<AvatarBasicControl>();
        //Log1Text.text = "Avatar Basic Control get!";

        // get the velocity forward magnitude
        //v_xz_0 = basicControlScript.forwardMaxSpeed;
        // get the velocity y magnitude
        //v_y_0 = basicControlScript.v_y_0;

        // calculate v_0
        float v_sq_sum = (v_xz_0 * v_xz_0) + (v_y_0 * v_y_0);
        v_0 = Mathf.Sqrt(v_sq_sum);

        //Log1Text.text = "v_0 calculated = " + v_0;

        // angle to rotate arc at
        float rot_angle = (360.0f / arc_rot_times);
        float x_loc = 0;

        // v_0 decrease amount for subarcs
        float v_0_decr = v_0 / v_0_subArcNumTimes;
        float v_xz_decr = v_xz_0 / v_0_subArcNumTimes;

        //Debug.Log("Physics gravity y: " + Physics.gravity.y);
        float grav_y = - Physics.gravity.y;

        Log1Text.text = "About to do for loops!";

        // take in a setting to tell it the increment of t values
        // calculate arc multiple times for incrementing value of t
        for (int v_0_subArcIdx = 0; v_0_subArcIdx < v_0_subArcNumTimes; v_0_subArcIdx++)
        {
            for (float t = 0.0F; t <= t_max; t += t_inc)
            {
                // calculate y part of arc
                double y_loc = ( (v_0 - (v_0_subArcIdx * v_0_decr)) * t * Mathf.Sin(Mathf.PI / 2)) - (0.5 * grav_y * t * t);

                // rotate this part of arc arc_rot times
                for (int rot_times = 0; rot_times < arc_rot_times; rot_times++)
                {
                    //Debug.Log("t: " + t + ", rot_times: " + rot_times);
                    //Debug.Log("arcPointList: " + arcPointList);
                    x_loc = (v_xz_0 - (v_xz_decr * v_0_subArcIdx)) * t;
                    GameObject aPoint = Instantiate(arcPoint, new Vector3(x_loc, (float)y_loc, 0), new Quaternion());
                    aPoint.transform.RotateAround(new Vector3(0, 0, 0), Vector3.up, rot_angle * rot_times);

                    // todo: set the parent for the point here probably if go this route

                    // if outer array empty, then add elements to outer array
                    if (arcPointList.Count <= arc_rot_times)
                    {
                        // add List element to outer array
                        arcPointList.Add(new List<GameObject>());
                    }
                    // add elements to inner array
                    arcPointList[rot_times].Add(aPoint);

                }
            }
        }

        platforms = GameParse.getARPlatforms();

        // associate arcs with the ARPlatforms
        associateArcsWithPlatforms(platforms);
    }

    public List<List<List<GameObject>>> setArcs(GameObject pltfrm, List<List<GameObject>> arcPointList_arg)
    {
        // find the edges of each platform
        Vector3 edgeOrigin = new Vector3(0, 0, 0);
        Vector3 edgeDir = new Vector3(0, -2, 0);
        Ray edgeDetectRay = createRay(edgeOrigin, edgeDir);

        // set all layers to 'ignore' for the platforms
        GameObject[] singlePltfrm = { pltfrm };

        setLayers(singlePltfrm, 2);

        List<Vector3> edgePoints = getEdgePoints(singlePltfrm, edgeDetectRay, 8, 2);
        /*string edgeLocs = "";
        for (int i = 0; i < edgePoints.Count; i++)
        {
            edgeLocs = edgeLocs + "," + edgePoints[i].ToString();
        }
        Log3Text.text = "edges: " + edgeLocs;
        */

        // put the arcs at these edge points
        List<List<List<GameObject>>> edgeArcsList = placeJumpArcsAtEdges(arcPointList_arg, edgePoints);

        setLayers(singlePltfrm, 8);

        return edgeArcsList;
    }

    public List<List<List<GameObject>>> setArcsGen(GameObject pltfrm, List<List<GameObject>> arcPointList_arg)
    {
        // find the edges of each platform
        Vector3 edgeOrigin = new Vector3(0, 0, 0);
        Vector3 edgeDir = new Vector3(0, -2, 0);
        Ray edgeDetectRay = createRay(edgeOrigin, edgeDir);

        // set all layers to 'ignore' for the platforms
        GameObject[] singlePltfrm = { pltfrm };

        setLayers(singlePltfrm, 2);

        List<Vector3> edgePoints = getEdgePointsGen(singlePltfrm, edgeDetectRay, 8, 2);
        /*string edgeLocs = "";
        for (int i = 0; i < edgePoints.Count; i++)
        {
            edgeLocs = edgeLocs + "," + edgePoints[i].ToString();
        }
        Log3Text.text = "edges: " + edgeLocs;
        */

        // put the arcs at these edge points
        List<List<List<GameObject>>> edgeArcsList = placeJumpArcsAtEdges(arcPointList_arg, edgePoints);

        setLayers(singlePltfrm, 8);

        return edgeArcsList;
    }

    // Update is called once per frame
    void Update()
    {
        //GameObject pl = GameObject.FindWithTag("Player");
        //StateText.text = "Player Collider info AFTER arc setting: " + pl.GetComponent<Collider>().bounds.size;

        // check which platform the avatar is on. That is the start platform.

        if (!foundPlatformOn)
        {
            for (int i = 0; i < platforms.Count; i++)
            {
                if (Functions.on(arAvatar.go, platforms[i].go))
                {
                    // found the platform we are on
                    foundPlatformOn = true;
                    // set the Level Generator script 'JsonParser.cs' to active (should initially be inactive)
                    LevelGenMain lgm = GetComponent<LevelGenMain>();
                    lgm.startPlatform = platforms[i];
                    lgm.enabled = true;
                    break;
                }
            }
        }
    }

    public Ray createRay(Vector3 pt1, Vector3 pt2)
    {
        Ray aRay = new Ray(pt1, pt2);
        //aRay.origin = pt1;
        //aRay.direction = pt2;

        return aRay;
    }

    // function to raycast from one point to another
    // returns true if intersection, false otherwise
    bool raycastHit(Vector3 pt1, Vector3 pt2)
    {
        Ray adjPointRay = createRay(pt1, pt2);
        RaycastHit hits;
        int touchMask = LayerMask.GetMask("touchable");
        bool isRayHit = Physics.Raycast(adjPointRay, out hits, touchMask);

        return isRayHit;
    }

    // function to raycast from one point to another
    // returns true if intersection, false otherwise
    bool raycastHit(Vector3 pt1, Vector3 pt2, Ray adjPointRay)
    {
        adjPointRay.origin = pt1;
        adjPointRay.direction = pt2;
        RaycastHit hits;
        int touchMask = LayerMask.GetMask("touchable");
        bool isRayHit = Physics.Raycast(adjPointRay, out hits, touchMask);

        return isRayHit;
    }

    // function to raycast from one point to another
    // returns the hit structure
    RaycastHit raycastHit(Ray adjPointRay)
    {
        /*Ray belowDetectRay = new Ray();
        RaycastHit belowHt;
        belowDetectRay.origin = go1.transform.position;
        belowDetectRay.direction = -go1.transform.up;
        int touchMask = LayerMask.GetMask("touchable");
        bool isBelow = Physics.Raycast(belowDetectRay, out belowHt, touchMask);
        */

        RaycastHit hits;
        int touchMask = LayerMask.GetMask("touchable");
        //Debug.Log("<raycast Hit> ray origin: " + adjPointRay.origin);
        //Debug.Log("<raycast Hit> ray direction: " + adjPointRay.direction);
        bool isRayHit = Physics.Raycast(adjPointRay, out hits, touchMask);

        return hits;
    }

    // function to raycast from one point to another
    // returns the hit structure
    RaycastHit[] raycastHitWhich(Ray adjPointRay)
    {
        RaycastHit[] rayHitsArr;
        int touchMask = LayerMask.GetMask("touchable");
        rayHitsArr = Physics.RaycastAll(adjPointRay, Mathf.Infinity, touchMask);

        return rayHitsArr;
    }

    // function that raycasts between center of two game objects
    // returns true if a hit, false otherwise
    bool touchableHitLine(GameObject go1, GameObject go2)
    {
        return raycastHit(go1.transform.position, go2.transform.position);
    }

    // function that starts from first point in an arc and checks if any
    // connecting lines in the arc intersect any "touchable" game objects
    // returns true if intersection/touch, false otherwise
    bool arcTouches(List<GameObject> arcPoints)
    {
        int arcPointsLen = arcPoints.Count;

        bool hitArc = false;

        // return false if less than 2 points in the arc
        if (arcPointsLen < 2)
        {
            return false;
        }

        for (int i = 1; i < arcPointsLen; i++)
        {
            hitArc = touchableHitLine(arcPoints[i - 1], arcPoints[i]);
            if (hitArc)
            {
                return hitArc;
            }
        }

        return false;
    }

    GameObject[] getAllPlatforms()
    {
        return GameObject.FindGameObjectsWithTag("ground");
    }

    private void setLayers(GameObject[] gameObjs, int layerId)
    {
        
        int gameObjsLen = gameObjs.Length;
        for (int i = 0; i < gameObjsLen; i++)
        {
            // 2 is 'Ignore Raycast'
            gameObjs[i].layer = layerId;
        }
    }

    // gets the edge points of a platform
    public List<Vector3> getEdgePoints(GameObject[] platforms, Ray aRay, int touchableId, int ignoreId)
    {
        List<Vector3> hitPoints = new List<Vector3>();

        Vector3 dir;
        int platformsLen = platforms.Length;
        GameObject platform;
        Vector3 tempVec3;
        Vector3 lastHitLoc = new Vector3(0,0,0);
        RaycastHit[] hits;
        float angle_edge_rot = 360.0f / num_edge_rots;
        RaycastHit rch;

        for (int i = 0; i < platformsLen; i++)
        {
            dir = Vector3.right;

            platform = platforms[i];
            platform.layer = touchableId;

            // put it at xz center
            aRay.origin = platform.transform.position;

            // set the origin y coordinate of the ray
            tempVec3 = aRay.origin;
            tempVec3.y = platform.transform.position.y + 2.1f;
            aRay.origin = tempVec3;

            Vector3 originalOrigin = aRay.origin;

            Log3Text.text = "platform layer: " + platform.layer + ", platform pos: " + platform.transform.position + ", platform locscale: " + platform.transform.localScale + "platform collider: " + platform.GetComponent<Collider>() + ", aRay origin: " + aRay.origin + ", aRay dir: " + aRay.direction;

            // rotate the ray around the y axis
            for (int j = 0; j < num_edge_rots; j++)
            {
                int inc_count = 0;
                dir = Quaternion.AngleAxis(angle_edge_rot, Vector3.up) * dir;
                // increment the ray's xz position
                do
                {
                    // increment/expand the ray trace outwards in dir direction
                    aRay.origin = originalOrigin + (dir * (inc_amt * inc_count));
                    hits = raycastHitWhich(aRay);
                    //Log4Text.text = "# hits: " + hits.Length;
                    rch = colliderContained(platform.GetComponent<Collider>(), hits);
                    if (rch.collider != null)
                    {
                        // record the hit location
                        lastHitLoc = rch.point;
                    }
                    else
                    {
                        // get the last hit location and add it to the list
                        hitPoints.Add(lastHitLoc);
                    }
                    inc_count++;
                }
                while (rch.collider != null);

            }

            platform.layer = ignoreId;
        }

        return hitPoints;
    }

    // gets the edge points of a platform (specifically for the generated ones since the other version does not work for these)
    public List<Vector3> getEdgePointsGen(GameObject[] platforms, Ray aRay, int touchableId, int ignoreId)
    {
        List<Vector3> hitPoints = new List<Vector3>();

        Vector3 dir;
        int platformsLen = platforms.Length;
        GameObject platform;
        Vector3 tempVec3;
        Vector3 lastHitLoc = new Vector3(0, 0, 0);
        RaycastHit[] hits;
        float angle_edge_rot = 360.0f / num_edge_rots;
        RaycastHit rch;

        for (int i = 0; i < platformsLen; i++)
        {
            dir = Vector3.right;

            platform = platforms[i];
            platform.layer = touchableId;

            // put it at xz center
            aRay.origin = platform.transform.position;

            // set the origin y coordinate of the ray
            tempVec3 = aRay.origin;
            tempVec3.y = platform.transform.position.y + 2.1f;
            aRay.origin = tempVec3;

            Vector3 originalOrigin = aRay.origin;

            Log3Text.text = "platform layer: " + platform.layer + ", platform pos: " + platform.transform.position + ", platform locscale: " + platform.transform.localScale + "platform collider: " + platform.GetComponent<Collider>() + ", aRay origin: " + aRay.origin + ", aRay dir: " + aRay.direction;

            // todo: just get the four sides of the square
            Vector3 pltfrmPos = platform.transform.position;
            Vector3 right = new Vector3(platform.transform.localScale.x / 2.0f, 0.0f, 0.0f);
            Vector3 left = new Vector3(-platform.transform.localScale.x / 2.0f, 0.0f, 0.0f);
            Vector3 forward = new Vector3(0.0f, 0.0f, platform.transform.localScale.z / 2.0f);
            Vector3 backward = new Vector3(0.0f, 0.0f, -platform.transform.localScale.z / 2.0f);
            hitPoints.Add(pltfrmPos + right);
            hitPoints.Add(pltfrmPos + left);
            hitPoints.Add(pltfrmPos + forward);
            hitPoints.Add(pltfrmPos + backward);
            platform.layer = ignoreId;
        }

        return hitPoints;
    }

    private RaycastHit colliderContained(Collider col, RaycastHit[] hits)
    {
        int len = hits.Length;
        for (int i = 0; i < len; i++)
        {
            if(hits[i].collider == col)
            {
                return hits[i];
            }
        }
        RaycastHit rch = new RaycastHit();
        return rch;
    }

    // creates copies of the original arc list and places at edge points
    public List<List<List<GameObject>>> placeJumpArcsAtEdges(List<List<GameObject>> arcPoints, List<Vector3> edgePoints)
    {
        List<List<List<GameObject>>> edgeArcsList = new List<List<List<GameObject>>>();
        int edgePointsLen = edgePoints.Count;
        for (int i = 0; i < edgePointsLen; i++)
        {
            Vector3 edgePt = edgePoints[i];

            // make a copy of the arcPoints
            //List<List<GameObject>> anArcList = new List<List<GameObject>>(arcPoints);
            List<List<GameObject>> anArcList = makeDeepCopyArcPoints(arcPoints);

            int anArcListLen = anArcList.Count;

            // get the vector between first element in arcPoints copy and the edgePoint
            Vector3 posFirstPoint = anArcList[0][0].transform.position;
            Vector3 translationVector = edgePt - posFirstPoint;

            // use that vector to calculate the new positions for the points in the arcs
            for (int j = 0; j < anArcListLen; j++)
            {
                List<GameObject> anArc = anArcList[j];
                int anArcLen = anArc.Count;
                for (int k = 0; k < anArcLen; k++)
                {
                    // move this point to the correct position relative to the edge point location
                    anArc[k].transform.position += translationVector;
                }
            }

            // add this group of arcs to our list
            edgeArcsList.Add(anArcList);
        }

        return edgeArcsList;
    }

    // makes a deep copy of the arc list
    List<List<GameObject>> makeDeepCopyArcPoints(List<List<GameObject>> arcPointLis)
    {
        List<List<GameObject>> arcPointsDeepCopy = new List<List<GameObject>>();

        int len = arcPointLis.Count;
        for (int i = 0; i < len; i++)
        {
            List<GameObject> anArc = arcPointLis[i];
            int anArcLen = anArc.Count;

            List<GameObject> anArcDeepCopy = new List<GameObject>();

            for (int j = 0; j < anArcLen; j++)
            {
                anArcDeepCopy.Add(Instantiate(arcPoint, anArc[j].transform.position, new Quaternion()));
            }

            // add this arc list to the outer list
            arcPointsDeepCopy.Add(anArcDeepCopy);
        }

        return arcPointsDeepCopy;
    }

    public void associateArcsWithPlatforms(List<ARGameObject> platforms)
    {
        int len = platforms.Count;
        for (int i = 0; i < len; i++)
        {
            //Log4Text.text = "<parseObjects> trying to add a platform";
            try
            {
                ARGameObject thisPlatform = platforms[i];
                thisPlatform.arcs = setArcs(thisPlatform.go, arcPointList);
            }
            catch (Exception e)
            {
                //Log4Text.text = "<parseObjects> Exception caught. Platform: " + platforms[i];
            }
        }
    }

    public void deleteArcCluster(List<List<GameObject>> arcCluster)
    {
        int len = arcCluster.Count;
        for (int i = 0; i < len; i++)
        {
            List<GameObject> currArc = arcCluster[i];
            int lenArc = currArc.Count;

            // for each element, destroy the point
            for (int j = 0; j < lenArc; j++)
            {
                Destroy(currArc[j]);
            }

            // clear the lists
            currArc.Clear();
        }
    }

    public void deleteListArcClusters(List<List<List<GameObject>>> arcClusters)
    {
        int len = arcClusters.Count;
        for (int k = 0; k < len; k++)
        {
            deleteArcCluster(arcClusters[k]);
        }

        // clear the outermost list
        arcClusters.Clear();

    }

}
