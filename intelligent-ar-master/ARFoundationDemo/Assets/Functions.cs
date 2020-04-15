using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum aboveOrOnStatus
{
    Above,
    On,
    Neither
}

public class Functions : MonoBehaviour
{
    // for debugging
    [SerializeField] static TMPro.TMP_Text Log1Text;
    [SerializeField] static TMPro.TMP_Text Log2Text;
    [SerializeField] static TMPro.TMP_Text Log3Text;
    [SerializeField] static TMPro.TMP_Text Log4Text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static dynamic funcCall(ARGameObject obj1, ARGameObject obj2, string attribute, string type)
    {
        switch (type)
        {
            case "on":
                return on(obj1.go, obj2.go);
                break;
            case "touches":
                return touches(obj1.go, obj2.go);
                break;
            case "below":
                return below(obj1.go, obj2.go);
                break;
            case ".":
                return dot(obj1, attribute);
                break;
            case "++":
                return increment(obj1, attribute);
                break;
            case "--":
                return decrement(obj1, attribute);
                break;
            case "==":
                return isEqual(obj1.go, obj2.go);
                break;
            case "||":
                return or(obj1.go, obj2.go);
                break;
            case "&&":
                return and(obj1.go, obj2.go);
                break;
            default:
                Debug.Log("The function is unknown.");
                return false;
                break;
        }
    }

    public static dynamic funcCall(ARGameObject obj1, List<ARGameObject> obj2List, string type)
    {
        int len = obj2List.Count;
        List<dynamic> returnList = new List<dynamic>();
        bool retVal = false;
        for (int i = 0; i < len; i++)
        {
            switch (type)
            {
                case "on":
                    retVal = on(obj1.go, obj2List[i].go);
                    break;
                case "touches":
                    retVal = touches(obj1.go, obj2List[i].go);
                    break;
                case "below":
                    retVal = below(obj1.go, obj2List[i].go);
                    break;
                default:
                    Debug.Log("The function is unknown.");
                    return false;
                    break;
            }

            if (retVal)
            {
                return true;
            }
        }
        return false;
    }

    public static bool below(GameObject obj1, GameObject obj2)
    {
        float height1 = obj1.GetComponent<Collider>().bounds.size.y;


        // get top of gameobject
        float yPos = obj1.transform.position.y + (height1 / 2.0f);

        if (yPos < obj2.transform.position.y)
        {
            return true;
            //return (true, "bool");
        }
        return false;
        //return (false, "bool");
    }

    /*public static bool on(GameObject obj1, GameObject obj2)
    {
        setUpLogging();

        Ray belowDetectRay = new Ray();
        RaycastHit belowHt;
        belowDetectRay.origin = obj1.transform.position;
        belowDetectRay.direction = -obj1.transform.up;
        int touchMask = LayerMask.GetMask("touchable");
        bool isBelow = Physics.Raycast(belowDetectRay, out belowHt, touchMask);

        Collider go1Collider = obj1.GetComponent<Collider>();
        Collider go2Collider = obj2.GetComponent<Collider>();
        Collider cldr = belowHt.collider;

        bool boundsTouch = false;
        if (go1Collider.bounds.Intersects(go2Collider.bounds))
        {
            Log4Text.text = "touching bounds of: " + obj2.name.ToString();
            boundsTouch = true;
        }
        else
        {
            Log4Text.text = "NOT touching bounds of: " + obj2.name.ToString();
            boundsTouch = false;
        }

        if (boundsTouch && cldr != null && cldr.gameObject == obj2)
        {
            //return (true, "bool");
            return true;
        }
        //return (false, "bool");
        return false;
    }*/

    public static bool on(GameObject obj1, GameObject obj2)
    {
        setUpLogging();

        Ray belowDetectRay = new Ray();
        RaycastHit belowHt;
        belowDetectRay.origin = obj1.transform.position;
        belowDetectRay.direction = -obj1.transform.up;
        int touchMask = LayerMask.GetMask("touchable");
        bool isBelow = Physics.Raycast(belowDetectRay, out belowHt, touchMask);

        Collider go1Collider = obj1.GetComponent<Collider>();
        Collider go2Collider = obj2.GetComponent<Collider>();
        Collider cldr = belowHt.collider;

        bool boundsTouch = false;

        CollisionHandler obj2CollHandler = null;
        try
        {
            obj2CollHandler = obj2.GetComponent<CollisionHandler>();
        }
        catch(Exception e)
        {
            Log4Text.text = "on Exception: " + e.ToString();
        }

        if(obj2CollHandler == null)
        {
            Log4Text.text = "obj2CollHandler is null";
        }

        if(obj2CollHandler.getCollidingGameObjects().Contains(obj1))
        {
            boundsTouch = true;
        }

        if (boundsTouch && cldr != null && cldr.gameObject == obj2)
        {
            return true;
        }
        return false;
    }

    public static bool touches(GameObject obj1, GameObject obj2)
    {
        setUpLogging();

        bool boundsTouch = false;
        CollisionHandler obj2CollHandler = null;
        try
        {
            obj2CollHandler = obj2.GetComponent<CollisionHandler>();
        }
        catch (Exception e)
        {
            Log4Text.text = "on Exception: " + e.ToString();
        }

        if (obj2CollHandler.getCollidingGameObjects().Contains(obj1))
        {
            boundsTouch = true;
        }

        if (boundsTouch)
        {
            return true;
        }
        return false;
    }

    public static bool isEqual(GameObject obj1, GameObject obj2)
    {
        if (obj1 == obj2)
        {
            return true;
        }
        return false;
    }

    public static bool or(GameObject obj1, GameObject obj2)
    {
        if (obj1 || obj2)
        {
            return true;
        }
        return false;
    }

    public static bool and(GameObject obj1, GameObject obj2)
    {
        if (obj1 && obj2)
        {
            return true;
        }
        return false;
    }

    // todo: set the actual go's attribute as well
    public static dynamic increment(ARGameObject obj1, string attribute)
    {
        switch (attribute)
        {
            case "height":
                obj1.setHeight(obj1.height + 1);
                return obj1.height;
                break;
            case "width":
                obj1.setWidth(obj1.width + 1);
                return obj1.width;
                break;
            case "radius":
                obj1.setRadius(obj1.radius + 1);
                return obj1.radius;
                break;
            case "wallet":
                obj1.wallet = obj1.wallet + 1;
                return obj1.wallet;
                break;
            case "minArea":
                return obj1.minArea++;
                break;
            case "minHeight":
                obj1.setMinHeight(obj1.minHeight + 1);
                return obj1.minHeight;
                break;
            case "minAngleToFloor":
                obj1.setAngle(obj1.minAngleToFloor + 1, obj1.maxAngleToFloor);
                return obj1.minAngleToFloor;
                break;
            case "maxAngleToFloor":
                obj1.setAngle(obj1.minAngleToFloor, obj1.maxAngleToFloor + 1);
                return obj1.maxAngleToFloor;
                break;
            default:
                Debug.Log("The attribute is unknown.");
                return -1;
                break;
        }
        return -1;
    }
    public static dynamic decrement(ARGameObject obj1, string attribute)
    {
        switch (attribute)
        {
            case "height":
                obj1.setHeight(obj1.height - 1);
                return obj1.height;
                break;
            case "width":
                obj1.setWidth(obj1.width - 1);
                return obj1.width;
                break;
            case "radius":
                obj1.setRadius(obj1.radius - 1);
                return obj1.radius;
                break;
            case "wallet":
                obj1.wallet = obj1.wallet - 1;
                return obj1.wallet;
                break;
            case "minArea":
                return obj1.minArea--;
                break;
            case "minHeight":
                obj1.setMinHeight(obj1.minHeight - 1);
                return obj1.minHeight;
                break;
            case "minAngleToFloor":
                obj1.setAngle(obj1.minAngleToFloor - 1, obj1.maxAngleToFloor);
                return obj1.minAngleToFloor;
                break;
            case "maxAngleToFloor":
                obj1.setAngle(obj1.minAngleToFloor, obj1.maxAngleToFloor - 1);
                return obj1.maxAngleToFloor;
                break;
            default:
                Debug.Log("The attribute is unknown.");
                return -1;
                break;
        }
        return -1;
    }
    public static dynamic dot(ARGameObject obj1, string attribute)
    {
        switch (attribute)
        {
            case "height":
                return obj1.height;
                break;
            case "width":
                return obj1.width;
                break;
            case "radius":
                return obj1.radius;
                break;
            case "wallet":
                return obj1.wallet;
                break;
            case "minArea":
                return obj1.minArea;
                break;
            case "minHeight":
                return obj1.minHeight;
                break;
            case "minAngleToFloor":
                return obj1.minAngleToFloor;
                break;
            case "maxAngleToFloor":
                return obj1.maxAngleToFloor;
                break;
            case "isActive":
                return obj1.isActive;
                break;
            case "isNotActive":
                return !obj1.isActive;
                break;
            case "isGoal":
                return obj1.isGoal;
                break;
            case "isMoving":
                return obj1.isMoving;
                break;
            /*case "isClimbable":
                return obj1.isClimbable;
                break;
                */
            default:
                Debug.Log("The attribute is unknown.");
                return false;
                break;
        }
        return false;
    }

    private static void setUpLogging()
    {
        TMPro.TMP_Text[] tt_arr = (TMPro.TMP_Text[])GameObject.FindObjectsOfType(typeof(TMPro.TMP_Text));
        for (int i = 0; i < tt_arr.Length; i++)
        {
            if (tt_arr[i].CompareTag("log1"))
            {
                Log1Text = tt_arr[i];
            }
            else if (tt_arr[i].CompareTag("log2"))
            {
                Log2Text = tt_arr[i];
            }
            else if (tt_arr[i].CompareTag("log3"))
            {
                Log3Text = tt_arr[i];
            }
            else if (tt_arr[i].CompareTag("log4"))
            {
                Log4Text = tt_arr[i];
            }
        }
    }

    // === functions for Coin Collector template ===
    // for coinPointGoal
    public GameObject[] getGameObjectsWithTag(string tagName)
    {
        return GameObject.FindGameObjectsWithTag(tagName);
    }

    public int getNumGameObjects(GameObject[] objects)
    {
        return objects.Length;
    }

    // todo: coinPointSparsity

    // coinToPlatformRatio
    public aboveOrOnStatus objectAboveGap(GameObject obj1, GameObject obj2, float gap)
    {
        // todo: may need to fix for vertical surfaces since they also have "touchable" as their layer
        aboveOrOnStatus aos = aboveOrOnStatus.Neither;

        // get height of obj1
        float obj1Height = obj1.GetComponent<Collider>().bounds.size.y;

        // raycast from object 1 downwards
        Ray aboveDetectRay = new Ray();
        RaycastHit aboveHt;
        aboveDetectRay.origin = (obj1.transform.position - new Vector3(0, (obj1Height / 2.0f), 0)); // starts at bottom of object 1
        aboveDetectRay.direction = -obj1.transform.up;
        int touchMask = LayerMask.GetMask("touchable");
        bool isAbove = Physics.Raycast(aboveDetectRay, out aboveHt, touchMask);

        // check if obj1 above obj2
        if (isAbove)
        {
            aos = aboveOrOnStatus.Above;
            // check if hit obj2
            Collider cldr = aboveHt.collider;
            if(cldr.gameObject == obj2)
            {
                // object1 is above object2
                // check if there's gap between obj1 and obj2
                if(aboveHt.distance < gap)
                {
                    aos = aboveOrOnStatus.On;
                }
            }
        }

        return aos;
    }

    public int getNumCoinsOnPlatform(GameObject[] coins, GameObject platform, float playerHeight)
    {
        int len = coins.Length;
        int count = 0;
        for (int i = 0; i < len; i++)
        {
            if (objectAboveGap(coins[i], platform, playerHeight) == aboveOrOnStatus.On)
            {
                count++;
            }
        }

        return count;
    }

    public aboveOrOnStatus coinOnAnyPlatform(GameObject obj1, float gap)
    {
        // todo: may need to fix for vertical surfaces since they also have "touchable" as their layer
        aboveOrOnStatus aos = aboveOrOnStatus.Neither;

        // get height of obj1
        float obj1Height = obj1.GetComponent<Collider>().bounds.size.y;

        // raycast from object 1 downwards
        Ray aboveDetectRay = new Ray();
        RaycastHit aboveHt;
        aboveDetectRay.origin = (obj1.transform.position - new Vector3(0, (obj1Height / 2.0f), 0)); // starts at bottom of object 1
        aboveDetectRay.direction = -obj1.transform.up;
        int touchMask = LayerMask.GetMask("touchable");
        bool isAbove = Physics.Raycast(aboveDetectRay, out aboveHt, touchMask);

        // check if obj1 above obj2
        if (isAbove)
        {
            aos = aboveOrOnStatus.Above;
            // check if hit obj2
            Collider cldr = aboveHt.collider;
            if (cldr.gameObject.tag == "ground")
            {
                // object1 is above a ground
                // check if there's gap between obj1 and obj2
                if (aboveHt.distance < gap)
                {
                    aos = aboveOrOnStatus.On;
                }
            }
        }

        return aos;
    }

    // todo: decayRateCoinPlatformRatio

    // surplusCoinAmt
    public float getSurplusCoinAmt(int coinPointGoal)
    {
        int numCoins = getGameObjectsWithTag("coin").Length;
        return ((float)numCoins / (float)coinPointGoal) - 1.0f;
    }

    // todo: minStartSize

    // makeStartPlatformVirtual
    public void makeStartPlatformVirtual(ARGameObject arPlatform)
    {
        arPlatform.platTp = PlatformType.Start;
    }

    // assuming only one Start platform
    public bool checkIfStartPlatformVirtual(List<ARGameObject> arPlatforms)
    {
        int len = arPlatforms.Count;
        for (int i = 0; i < len; i++)
        {
            ARGameObject currARGameObject = arPlatforms[i];
            if (currARGameObject.platTp == PlatformType.Start && currARGameObject.isVirtPlat)
            {
                return true;
            }
        }
        return false;
    }

    // numRealPlatformsIncludeWithCoins
    public int numRealPlatformsIncludedWithCoins(GameObject[] coins, List<ARGameObject> ARPlatforms, float playerHeight)
    {
        int count = 0;
        int platformsLen = ARPlatforms.Count;
        int coinsLen = coins.Length;
        for (int i = 0; i < platformsLen; i++)
        {
            for (int j = 0; j < coinsLen; j++)
            {
                ARGameObject currARGameObj = ARPlatforms[i];
                // check if this coin is on this platform and it's a real platform
                if (objectAboveGap(coins[j], currARGameObj.go, playerHeight) == aboveOrOnStatus.On && !currARGameObj.isVirtPlat)
                {
                    // if so, increment count and break
                    count++;
                    break;
                }
            }
        }

        return count;
    }
    
    // todo: shapeVirtualPlatform

    // coinPointsMidAirToGroundRatio
    public float getCoinFloatingToGroundRatio(GameObject[] coins, GameObject[] platforms, float playerHeight)
    {
        // get the number of coins floating and on the ground
        int coinsOnGround = 0;
        int coinsFloating = 0;
        int coinsCount = coins.Length;
        int platformsCount = platforms.Length;
        for (int i = 0; i < coinsCount; i++)
        {
            aboveOrOnStatus aos = coinOnAnyPlatform(coins[i], playerHeight);
            if(aos == aboveOrOnStatus.On)
            {
                coinsOnGround++;
            }
            else
            {
                coinsFloating++;
            }
        }

        // get the ratio
        return (float)coinsFloating / (float)coinsOnGround;
    }

    // === functions for Platformer template ===
    // todo: minEndSize

    // makeEndPlatformVirtual
    public void makeEndPlatformVirtual(ARGameObject arPlatform)
    {
        arPlatform.platTp = PlatformType.End;
    }

    // assuming only one End platform
    public bool checkIfEndPlatformVirtual(List<ARGameObject> arPlatforms)
    {
        int len = arPlatforms.Count;
        for (int i = 0; i < len; i++)
        {
            ARGameObject currARGameObject = arPlatforms[i];
            if (currARGameObject.platTp == PlatformType.End && currARGameObject.isVirtPlat)
            {
                return true;
            }
        }
        return false;
    }

    // todo: distance

    // todo: optimalPathLen

    // todo: runJumpRatio

    // todo: decayRateRJRatio

    // numRealPlatforms
    public int getNumRealPlats(List<ARGameObject> platforms)
    {
        int count = 0;
        int len = platforms.Count;
        for (int i = 0; i < len; i++)
        {
            if (!platforms[i].isVirtPlat)
            {
                count++;
            }
        }
        return count;
    }

    // numVirtualPlatforms
    public int getNumVirtPlats(List<ARGameObject> platforms)
    {
        int count = 0;
        int len = platforms.Count;
        for (int i = 0; i < len; i++)
        {
            if(platforms[i].isVirtPlat)
            {
                count++;
            }
        }
        return count;
    }

    // todo: shapeVirtualPlatform
}
