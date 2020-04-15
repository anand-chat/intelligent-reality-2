using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System;

public class GameParse : MonoBehaviour
{
    static ARGameObject avatar;
    static List<ARGameObject> ARPlatforms;
    static List<ARGameObject> ARVertSurfaces;
    static ARGameObject platform; // todo: remove this after done testing parsing
    static ARGameObject verticalsurface;
    static ARGameObject coin; // todo: remove this after done testing parsing

    // for debugging
    [SerializeField] static TMPro.TMP_Text Log1Text;
    [SerializeField] static TMPro.TMP_Text Log2Text;
    [SerializeField] static TMPro.TMP_Text Log3Text;
    [SerializeField] static TMPro.TMP_Text Log4Text;
    [SerializeField] static TMPro.TMP_Text PlaneText;

    void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static ARGameObject parsePlatformObject(JObject jObj, GameObject defaultPlane)
    {
        ARGameObject arPlatform = new ARGameObject(jObj);
        arPlatform.makePlatformARGameObject(defaultPlane, true, PlatformType.Neither);
        return arPlatform;
    }

    public static void parseObjects(JObject jObj, GameObject defaultPlane, GameObject defaultVertSurface, GameObject defaultCoin)
    {
        // todo: remove this after done debugging
        setUpLogging();

        // set up the prefabs before creating any ARGameObjects
        //ARGameObject.setUpPrefabs();

        avatar = new ARGameObject(jObj);

        // todo: remove this. Only for testing purposes
        GameObject pl;
        try
        {
            pl = GameObject.FindWithTag("Player");
            //pl = GameObject.Find("Player");
            //Log4Text.text = "<parseObjects> got the player";
        }
        catch (Exception e)
        {
            pl = null;
            //Log4Text.text = "<parseObjects> Exception: " + e.ToString();
        }

        /*if (pl == null)
        {
            Log3Text.text = "<parseObjects> pl is null";
        }*/

        try
        {
            avatar.makeAvatarARGameObject(pl/*GameObject.Find("Player")*/);
            //Log4Text.text = "makeAvatarARGameObject successful";
        }
        catch (Exception e)
        {
            //Log4Text.text = "call to <makeAvatarARGameObject> failed. Exception: " + e.ToString();
        }

        ARPlatforms = new List<ARGameObject>();
        ARVertSurfaces = new List<ARGameObject>();

        GameObject[] platforms = GameObject.FindGameObjectsWithTag("ground");
        int len = platforms.Length;
        for (int i = 0; i < len; i++)
        {
            //Log4Text.text = "<parseObjects> trying to add a platform";
            try
            {
                ARGameObject newPlatform = new ARGameObject(platforms[i]);
                ARPlatforms.Add(newPlatform);
            }
            catch (Exception e)
            {
                //Log4Text.text = "<parseObjects> Exception caught. Platform: " + platforms[i];
            }
        }

        //Log4Text.text = "<parseObjects> platforms length: " + len;

        // todo: remove this line. Only for testing purposes
        platform = ARPlatforms[0];

        //platform = new ARGameObject(jObj);
        //platform.makePlatformARGameObject(defaultPlane);

        //Log4Text.text = "about to call <makeVerticalSurfaceARGameObject>";
        verticalsurface = new ARGameObject(jObj);
        //try
        //{
            verticalsurface.makeVerticalSurfaceARGameObject(defaultVertSurface);

        /*}
        catch(Exception e)
        {
            Log4Text.text = "<parseObjects> Exception: " + e.ToString();
        }*/

        //PlaneText.text = "VertSurf angles: " + verticalsurface.go.transform.rotation.eulerAngles;

        coin = new ARGameObject(defaultCoin);
        coin.makeCoinARGameObject(defaultCoin);

        //Log4Text.text = "Completed parseObjects";

    }
    public static void parseActions()
    {
        return;
    }
    public static T parseFuncCall<T>(JObject jsonFunc)
    {
        setUpLogging();

        /*if (coin.go != null)
        {
            PlaneText.text = "coin NOT NULL";
        }
        else
        {
            PlaneText.text = "coin NULL";
        }
        */

        //PlaneText.text = "coin: " + coin.ToString();

        if (jsonFunc == null)
        {
            return default(T);
        }

        string type = (string)jsonFunc["type"];
        string strArg1 = jsonFunc["arg1"].ToString();
        string strArg2 = (string)jsonFunc["arg2"];
        System.Object arg1 = null;
        System.Object arg2 = null;
        string attribute = null;
        if (strArg1 == null)
        {
            return default(T);
        }
        /*if (strArg2 == null)
        {
            return default(T);
        }*/
        if (strArg1[0] == '{')
        {
            arg1 = parseFuncCall<T>(JObject.Parse(strArg1));
        }
        if (strArg2 != null && strArg2[0] == '{')
        {
            arg2 = parseFuncCall<T>(JObject.Parse(strArg1));
        }
        /*if (type == "==" || type == "<=" || type == ">=" || type == "<" || type == ">")
        {
            attribute = strArg2;
            arg2 = null;
            System.Object val = Functions.funcCall((ARGameObject)arg1, (ARGameObject)arg2, attribute, type);
            return (T)Convert.ChangeType(val, typeof(T));
        }*/
        switch (strArg1)
        {
            case "Avatar":
                arg1 = avatar;
                break;
            case "Platform":
                arg1 = platform;
                break;
            case "VerticalSurface":
                arg1 = verticalsurface;
                break;
            default:
                //PlaneText.text = "The first argument is unknown.";
                //Debug.Log("The first argument is unknown.");
                break;
        }
        switch (strArg2)
        {
            case "Avatar":
                arg2 = avatar;
                break;
            case "Platform":
                arg2 = platform;
                break;
            case "VerticalSurface":
                arg2 = verticalsurface;
                break;
            case "Coin":
                arg2 = coin;
                break;
            case "wallet":
                attribute = "wallet";
                arg2 = null;
                break;
            case "isGoal":
                attribute = "isGoal";
                arg2 = null;
                break;
            default:
                //PlaneText.text = "The secondd argument is unknown.";
                //Debug.Log("The secondd argument is unknown.");
                break;
        }

        System.Object value;
        if (strArg2 == "Platform")
        {
            value = Functions.funcCall((ARGameObject)arg1, ARPlatforms, type);
        }
        else if(strArg2 == "VerticalSurface")
        {
            // todo: need to populate ARVertSurfaces before this can be used
            value = Functions.funcCall((ARGameObject)arg1, ARVertSurfaces, type);
        }
        else
        {
            value = Functions.funcCall((ARGameObject)arg1, (ARGameObject)arg2, attribute, type);
        }
        return (T)Convert.ChangeType(value, typeof(T));

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
            else if(tt_arr[i].CompareTag("logPlane"))
            {
                PlaneText = tt_arr[i];
            }
        }
    }

    public static List<ARGameObject> getARPlatforms()
    {
        return ARPlatforms;
    }

    public static List<ARGameObject> getARVertSurfaces()
    {
        return ARVertSurfaces;
    }

    public static ARGameObject getARAvatar()
    {
        return avatar;
    }

    public static ARGameObject getVertSurface()
    {
        return verticalsurface;
    }
}
