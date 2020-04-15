using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System;

public enum PlatformType
{
    Start,
    End,
    Neither
}

public class ARGameObject : MonoBehaviour
{
    static JObject json;// = JObject.Parse("vgddlObjects.json");
    /*public static GameObject avatarPrefab;
    public static GameObject defaultPlane;
    public static GameObject defaultVertSurface;
    */

    private static GameObject player = null;

    // for debugging
    [SerializeField] TMPro.TMP_Text Log1Text;
    [SerializeField] TMPro.TMP_Text Log2Text;
    [SerializeField] TMPro.TMP_Text Log3Text;
    [SerializeField] TMPro.TMP_Text Log4Text;
    [SerializeField] static TMPro.TMP_Text PlaneText;

    private Collector collector;

    void Awake()
    {
        
    }

    /*public static void setUpPrefabs()
    {
        defaultPlane = GameObject.FindGameObjectWithTag("groundPrefab");
        defaultVertSurface = GameObject.FindGameObjectWithTag("verticalSurfacePrefab");
    }*/

    public ARGameObject(JObject jsonObj)
    {
        json = jsonObj;
        height = 1;
        width = 1;
        radius = 0.5;
        wallet = 0;
        minArea = -1;
        minHeight = -1;
        maxAngleToFloor = 90;
        minAngleToFloor = 0;
        isActive = false;
        isMoving = false;
        isGoal = false;
        isClimbable = false;
        go = null;
        arcs = null;
        v_xz_0_jump = 0.0f;
        v_y_0_jump = 0.0f;
        stepSize = 1.0f;
        isVirtPlat = false;
        platTp = PlatformType.Neither;
        grid = new List<List<Cell>>();

    }

    // this constructor is primarily for the AR planes only
    public ARGameObject(GameObject gameObj)
    {
        height = 1;
        width = 1;
        radius = 0.5;
        wallet = 0;
        minArea = -1;
        minHeight = -1;
        maxAngleToFloor = 90;
        minAngleToFloor = 0;
        isActive = true;
        isMoving = false;
        isGoal = false;
        isClimbable = false;

        go = gameObj;
        arcs = null;
        grid = new List<List<Cell>>();

    }
    // Avatar
    public ARGameObject(double height, double width, double radius, int wallet, bool isActive, bool isMoving)
    {
        this.height = height;
        this.width = width;
        this.radius = radius;
        //this.wallet = wallet;
        this.isActive = isActive;
        this.isMoving = isMoving;
        arcs = null;
        grid = new List<List<Cell>>();
    }
    // Platform
    public ARGameObject(double minArea, double minAngleToFloor, double maxAngleToFloor, bool isGoal)
    {
        this.minAngleToFloor = minAngleToFloor;
        this.minArea = minArea;
        this.maxAngleToFloor = maxAngleToFloor;
        this.isGoal = isGoal;
        arcs = null;
        grid = new List<List<Cell>>();
    }
    // Vertical Surface
    public ARGameObject(double minHeight, double minAngleToFloor, double maxAngleToFloor)
    {
        this.minAngleToFloor = minAngleToFloor;
        this.minHeight = minHeight;
        this.maxAngleToFloor = maxAngleToFloor;
        //isClimbable = json["isClimbable"].Value<bool>();
    }

    public GameObject makeAvatarARGameObject(GameObject gameObj)
    {
        //todo: remove after done debugging
        setUpLogging();

        //Log4Text.text = "<makeAvatarARGameObject> Started";

        double height = this.height;
        double width = this.width;
        double radius = this.radius;
        int wallet = this.wallet;
        bool isActive = true;
        bool isMoving = false;
        float v_xz_0 = this.v_xz_0_jump;
        float v_y_0 = this.v_y_0_jump;

        JObject avatarJson = (JObject)json["Objects"]["Avatar"];
        //Log4Text.text = "<makeAvatarARGameObject> Got the avatar json";

        JToken avatarHeightJson = (JToken)avatarJson["height"];
        if (avatarHeightJson != null)
        {
            height = avatarHeightJson.Value<double>();
        }

        //Log4Text.text = "<makeAvatarARGameObject> Handled the avatar's height";

        JToken avatarWidthJson = (JToken)avatarJson["width"];
        if (avatarWidthJson != null)
        {
            width = avatarWidthJson.Value<double>();
        }

        //Log4Text.text = "<makeAvatarARGameObject> Handled avatar's width";

        JToken avatarRadJson = (JToken)avatarJson["radius"];
        if (avatarRadJson != null)
        {
            radius = avatarRadJson.Value<double>();
        }

        JToken avatarWalletJson = (JToken)avatarJson["wallet"];
        if (avatarWalletJson != null)
        {
            wallet = avatarWalletJson.Value<int>();
        }

        JToken avatarIsActiveJson = (JToken)avatarJson["isActive"];
        if (avatarIsActiveJson != null)
        {
            isActive = avatarIsActiveJson.Value<bool>();
        }

        JToken avatarIsMovingJson = (JToken)avatarJson["isMoving"];
        if (avatarIsMovingJson != null)
        {
            isMoving = avatarIsMovingJson.Value<bool>();
        }

        JToken avatarVXZ0Json = (JToken)avatarJson["v_xz_0"];
        if (avatarVXZ0Json != null)
        {
            this.v_xz_0_jump = avatarVXZ0Json.Value<float>();
        }

        JToken avatarVY0Json = (JToken)avatarJson["v_y_0"];
        if (avatarVY0Json != null)
        {
            this.v_y_0_jump = avatarVY0Json.Value<float>();
        }

        JToken avatarStepSize = (JToken)avatarJson["stepSize"];
        if (avatarStepSize != null)
        {
            this.stepSize = avatarStepSize.Value<float>();
        }

        //Log4Text.text = "<makeAvatarARGameObject> finished with fetching json information";

        // set up the avatar's width and height
        Vector3 localScale = gameObj.transform.localScale;
        //Log4Text.text = "<makeAvatarARGameObject> got the gameObject's localScale";
        //localScale.y = transform.InverseTransformVector(new Vector3(0, (float)this.height, 0)).y;
        localScale.y = (float)height;
        localScale.x = (float)width;
        localScale.z = (float)width;
        gameObj.transform.localScale = localScale;

        //Log4Text.text = "<makeAvatarARGameObject> Set up avatar's width and height";

        //CapsuleCollider cc = gameObj.GetComponent<CapsuleCollider>();
        //Vector3 ccLocalScale = cc.transform.localScale;
        //ccLocalScale.y = (float)height;
        //gameObj.GetComponent<CapsuleCollider>().transform.localScale = ccLocalScale;
        //gameObj.GetComponent<CapsuleCollider>().radius = (float)radius;
        gameObj.SetActive(isActive);

        //Log4Text.text = "<makeAvatarARGameObject> Handled avatar's collider";

        this.go = gameObj;

        ////////////
        setWallet(wallet);
        /////////////

        //Log4Text.text = "<makeAvatarARGameObject> Completed";

        player = this.go;
        return this.go;
    }

    public GameObject makePlatformARGameObject(GameObject defaultPlane, bool isVirtualPlatform = false, PlatformType platformTp = PlatformType.Neither)
    {
        isVirtPlat = isVirtualPlatform;
        platTp = platformTp;

        double minAngleToFloor = this.minAngleToFloor;
        double minArea = this.minArea;
        double maxAngleToFloor = this.maxAngleToFloor;
        bool isGoal = this.isGoal;

        JObject platformJson = (JObject)json["Objects"]["Platform"];

        JObject platformMinAngleToFloor = (JObject)platformJson["minAngleToFloor"];
        if (platformMinAngleToFloor != null)
        {
            minAngleToFloor = platformMinAngleToFloor.Value<double>();
        }

        JToken platformMinArea = (JToken)platformJson["minArea"];
        if (platformMinArea != null)
        {
            minArea = platformMinArea.Value<double>();
        }

        JToken platformMaxAngleToFloor = (JToken)platformJson["maxAngleToFloor"];
        if (platformMaxAngleToFloor != null)
        {
            maxAngleToFloor = platformMaxAngleToFloor.Value<double>();
        }

        JToken platformIsGoal = (JToken)platformJson["isGoal"];
        if (platformIsGoal != null)
        {
            isGoal = platformIsGoal.Value<bool>();
        }

        GameObject platform = Instantiate(defaultPlane);
        platform.tag = "ground";
        platform.SetActive(true);

        this.go = platform;

        // todo: make a script for generated platforms, i.e. not the ones from real life surfaces, to handle min/maxes

        //platformConfig platConf = new platformConfig();
        //platformConfig platConf = platform.GetComponent<platformConfig>();

        // set the parameters
        this.setMaxAngle(maxAngleToFloor);
        this.setMinAngle(minAngleToFloor);
        this.setMinArea(minArea);

        // set the attributes of the platform based on parameters set
        //platConf.setAngleWithHorizontal();
        // only care about x and z rotation
        this.setAngle();

        //platConf.setWidthDepth();
        // todo: fix to calculate area correctly
        // todo: change so that it can take whatever area but clamp it if too small
        float len = Mathf.Sqrt((float)this.minArea);
        Vector3 locScale = platform.transform.localScale;
        locScale.x = len;
        locScale.z = len;
        platform.transform.localScale = locScale;

        // set isGoal
        this.isGoal = isGoal;

        return platform;

    }

    public GameObject makeVerticalSurfaceARGameObject(GameObject defaultVertSurface)
    {
        setUpLogging();
        //Log3Text.text = "Started makeVerticalSurfaceARGameObject";

        double minAngleToFloor = this.minAngleToFloor;
        double minHeight = this.minHeight;
        double maxAngleToFloor = this.maxAngleToFloor;

        JToken verticalJson = (JToken)json["Objects"]["VerticalSurface"];

        JToken vertMinAngleToFloorJson = (JToken)verticalJson["minAngleToFloor"];
        if (vertMinAngleToFloorJson != null)
        {
            minAngleToFloor = vertMinAngleToFloorJson.Value<double>();
        }

        JToken vertMinHeight = (JToken)verticalJson["minHeight"];
        if (vertMinHeight != null)
        {
            minHeight = vertMinHeight.Value<double>();
        }

        JToken vertMaxAngleToFloor = (JToken)verticalJson["maxAngleToFloor"];
        if (vertMaxAngleToFloor != null)
        {
            maxAngleToFloor = vertMaxAngleToFloor.Value<double>();
        }

        GameObject vertSurface = Instantiate(defaultVertSurface);
        vertSurface.tag = "verticalSurface";
        vertSurface.SetActive(true);

        this.go = vertSurface;

        //vertSurfaceConfig vertConf = new vertSurfaceConfig();

        //vertSurfaceConfig vertConf = vertSurface.GetComponent<vertSurfaceConfig>();

        // set the parameters
        this.setMaxAngle(maxAngleToFloor);
        this.setMinAngle(minAngleToFloor);

        //try
        //{
        this.setMinHeight(minHeight);
        /*}
        catch (Exception e)
        {
            Log4Text.text = "<makeVerticalSurfaceARGameObject> Exception: " + e.ToString();
        }*/

        // set the attributes of the vertical surface based on parameters set
        // set the angle with the floor
        // only care about x and z rotation
        this.setAngle();

        //Log3Text.text = "Completed makeVerticalSurfaceARGameObject, vertSurface tag: " + vertSurface.tag + ", vertSurface active: " + vertSurface.activeSelf;
        return vertSurface;
    }

    public GameObject makeCoinARGameObject(GameObject defaultCoin)
    {
        setUpLogging();
        //Log3Text.text = "Started makeVerticalSurfaceARGameObject";

        Vector3 pos = new Vector3(0.0f, 0.0f, 0.0f);
        if(player != null)
        {
            pos = player.transform.position;
        }

        JToken coinJson = (JToken)json["Objects"]["Coin"];

        JToken radiusJson = (JToken)coinJson["radius"];
        if (radiusJson != null)
        {
            radius = radiusJson.Value<double>();
        }

        GameObject coin = Instantiate(defaultCoin, pos + new Vector3(2, -0.4f, 0), new Quaternion());

        this.go = coin;

        // set the parameters
        //this.setRadius(this.radius);

        //Log3Text.text = "Completed makeVerticalSurfaceARGameObject, vertSurface tag: " + vertSurface.tag + ", vertSurface active: " + vertSurface.activeSelf;
        //Collider cldr = this.go.GetComponent<Collider>();
        //Gizmos.DrawWireCube(this.go.transform.position, cldr.bounds.size);
        return coin;
    }

    private void getPlatformParams()
    {
        this.minAngleToFloor = json["Objects"]["Platform"]["minAngleToFloor"].Value<double>();
        this.minArea = json["Objects"]["Platform"]["minArea"].Value<double>();
        this.maxAngleToFloor = json["Objects"]["Platform"]["maxAngleToFloor"].Value<double>();
        this.isGoal = json["Objects"]["Platform"]["isGoal"].Value<bool>();
    }

    private float getRandInRange()
    {
        System.Random rng = new System.Random();

        //PlaneText.text = "max: " + this.maxAngleToFloor + ", min: " + this.minAngleToFloor;

        // first generate # between 0 and 1 to indicate if we are doing negative or positive angle
        double maxAgl = this.maxAngleToFloor;
        double newAngle;
        double randNegPos = rng.NextDouble();
        if (randNegPos < 0.5)
        {
            maxAgl = -maxAgl;
        }

        if (maxAgl < 0)
        {
            // then calculate the angle between min and max (for negative angle)
            newAngle = rng.NextDouble() * (-this.minAngleToFloor - (maxAgl)) + maxAgl;
        }
        else
        {
            // then calculate the angle between min and max (for positive angle)
            newAngle = rng.NextDouble() * (maxAgl - (this.minAngleToFloor)) + this.minAngleToFloor;
        }

        //PlaneText.text = "newAngle: " + (float)newAngle;

        return (float)newAngle;
    }

    public void setMinAngle(double angle)
    {
        this.minAngleToFloor = angle;
    }

    public void setMaxAngle(double angle)
    {
        this.maxAngleToFloor = angle;
    }

    public void setAngle()
    {
        Quaternion rotation = this.go.transform.rotation;
        float xVal = this.getRandInRange();
        float zVal = this.getRandInRange();
        Vector3 eulerAgl = new Vector3(xVal, 0, zVal);
        //rotation.x = this.getRandInRange();
        //rotation.z = this.getRandInRange();
        rotation.eulerAngles = eulerAgl;
        //PlaneText.text = "eulerAgl:" + eulerAgl + ", rot: " + rotation.eulerAngles;
        this.go.transform.rotation = rotation;
	}

	public void setAngle(double minAngle, double maxAngle)
    {
        this.minAngleToFloor = minAngle;
        this.maxAngleToFloor = maxAngle;
        setAngle();
    }

    // mainly so player can walk
    public void setMinArea(double area)
    {
        this.minArea = area;
    }

    public void setMinHeight()
    {
        // todo: change code to take in a y such that it will set it to the height unless it's less than minHeight in which it clamps it to minHeight
        // set the surface height
        Vector3 locScale = this.go.transform.localScale;
        locScale.y = (float)this.minHeight;
        this.go.transform.localScale = locScale;
    }

    public void setMinHeight(double height)
    {
        this.minHeight = height;

        setMinHeight();
    }

    public void setHeight()
    {
        Vector3 localScale = this.go.transform.localScale;
        //localScale.y = transform.InverseTransformVector(new Vector3(0, (float)this.height, 0)).y;
        localScale.y = (float)this.height;
        this.go.transform.localScale = localScale;

        CapsuleCollider cc = this.go.GetComponent<CapsuleCollider>();
        Vector3 ccLocalScale = cc.transform.localScale;
        ccLocalScale.y = (float)this.height;
        this.go.GetComponent<CapsuleCollider>().transform.localScale = ccLocalScale;
    }
    public void setHeight(double h)
    {
        this.height = h;
        setHeight();
    }

    // note: it sets the radius of the capsule collider to half of the width
    public void setWidth()
    {
        // set up the avatar's width and height
        Vector3 localScale = this.go.transform.localScale;
        //localScale.y = transform.InverseTransformVector(new Vector3(0, (float)this.height, 0)).y;
        localScale.x = (float)width;
        localScale.z = (float)width;
        this.go.transform.localScale = localScale;

        CapsuleCollider cc = this.go.GetComponent<CapsuleCollider>();
        Vector3 ccLocalScale = cc.transform.localScale;
        this.go.GetComponent<CapsuleCollider>().transform.localScale = ccLocalScale;
        this.go.GetComponent<CapsuleCollider>().radius = (float)width/2.0f;
    }

    public void setWidth(double w)
    {
        this.width = w;
        setWidth();
    }

    public void setRadius()
    {
        // set up the avatar's width and height
        Vector3 localScale = this.go.transform.localScale;
        localScale.x = (float)radius;
        localScale.z = (float)radius;
        this.go.transform.localScale = localScale;

        //this.go.GetComponent<CapsuleCollider>().radius = (float)radius;
    }

    public void setRadius(double r)
    {
        this.radius = r;
        setRadius();
    }

    public void setWallet(int w)
    {
        this.wallet = w;

        collector = this.go.GetComponent<Collector>();
        collector.setWallet(w);
    }

    private void setUpLogging()
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
            else if (tt_arr[i].CompareTag("logPlane"))
            {
                PlaneText = tt_arr[i];
            }
        }
    }

    public double height { get; set; }
    public double width { get; set; }
    public double radius { get; set; }
    public int wallet { get; set; }
    public double minArea { get; set; }
    public double minHeight { get; set; }
    public double maxAngleToFloor { get; set; }
    public double minAngleToFloor { get; set; }
    public bool isActive { get; set; }
    public bool isMoving { get; set; }
    public bool isGoal { get; set; }
    public bool isClimbable { get; set; }
    public float v_xz_0_jump { get; set; }
    public float v_y_0_jump { get; set; }
    public float stepSize { get; set; }
    public GameObject go { get; set; }
    public List<List<List<GameObject>>> arcs { get; set; }
    public bool isVirtPlat { get; set; }
    public PlatformType platTp { get; set; }
    public List<List<Cell>> grid { get; set; }
}
