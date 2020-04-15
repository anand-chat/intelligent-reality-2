using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.IO;
using System;

public class JsonParser : MonoBehaviour
{
    public GameObject avatarPrefab;
    public GameObject defaultPlane;
    public GameObject defaultVertSurface;
    public GameObject defaultCoin;

    private ARGameObject avatar;

    private string jsonStr = @"{
    'Actions': {
        'Jump': {
            'Preconds': [
                {
                    'type': 'on',
                    'arg1': 'Avatar',
                    'arg2': 'Platform'
                }
            ],
            'Effects': [
                {
                    'type': 'on',
                    'arg1': 'Avatar',
                    'arg2': 'Platform'
                }
            ]
        },
        'Collect': {
            'Preconds': [
                {
                    'type': 'touches',
                    'arg1': 'Avatar',
                    'arg2': 'Coin'
                }
            ],
            'Effects': [
                {
                    'type': '++',
                    'arg1': 'Avatar',
                    'arg2': 'wallet'
                },
                {
                    'type': 'isNotActive',
                    'arg1': 'Coin'
                }
            ]
        }
    },
    'Objects': {
        'Avatar': {
            'height': '1',
            'radius': '1',
            'wallet': '5',
            'v_xz_0': '1',
            'v_y_0': '4.803798',
            'stepSize': '2.0'
        },
        'Platform': {
            'minArea': '4',
            'maxAngleToFloor': '45',
            'isGoal': 'true'
        },
        'VerticalSurface': {
            'minHeight': '1',
            'minArea': '10',
            'maxAngleToFloor': '60',
            'minAngleToFloor': '45'
        },
        'Coin': {
            'radius':  '0.5'
        }
    }
}";

    private Dictionary<string, JObject> actionsDict = new Dictionary<string, JObject>();

    // todo: remove this. Only for testing
    private bool retActionCollectPrecondValueBool;

    // todo: remove this. Only for testing
    private bool didCollectIncTest;

    // for debugging
    [SerializeField] TMPro.TMP_Text PlaneText;
    [SerializeField] TMPro.TMP_Text Log1Text;
    [SerializeField] TMPro.TMP_Text Log2Text;
    [SerializeField] TMPro.TMP_Text Log3Text;
    [SerializeField] TMPro.TMP_Text Log4Text;

    // Start is called before the first frame update
    void Start()
    {
        JumpArcCalculator jac = GetComponent<JumpArcCalculator>();

        try
        {
            GameParse.parseObjects(JObject.Parse(jsonStr), defaultPlane, defaultVertSurface, defaultCoin);
        }
        catch (Exception e)
        {
            Log3Text.text = "parseObjects Exception: " + e.ToString();
        }
        //Log1Text.text = "Start JsonParser called!";

        // todo: remove this. Only for testing
        retActionCollectPrecondValueBool = false;

        // todo: remove this. Only for testing
        didCollectIncTest = false;

        storeActions(jsonStr/*"Assets/exampleGame.json"*/);

        avatar = GameParse.getARAvatar();

        // enable the JumpArcCalculator here
        jac.enabled = true;

        Log1Text.text = "Completed JsonParser Start";

    }

    // Update is called once per frame
    void Update()
    {
        //Log1Text.text = "Update called";
        // todo: only for testing purposes. Remove after done testing!

        // Jump Precondition "on"
        //Log2Text.text = "about to call parseFuncCall dict len: " + actionsDict.Count;

        string keyList = "";

        foreach (KeyValuePair<string, JObject> pair in actionsDict)
        {
            keyList = keyList + ", " + pair.Key;
        }

        //Log2Text.text = keyList;
        /*
        try
        {
            var actionJumpPrecondVals = parseFuncCall((JObject)(actionsDict["Jump"])["Preconds"][0]);
            //Log1Text.text = "completed parseFuncCall";
            System.Object actionJumpPrecondVal = actionJumpPrecondVals.Item1;
            bool retActionJumpPrecondValueBool = (bool)Convert.ChangeType(actionJumpPrecondVal, typeof(bool));
            Log1Text.text = "Jump Precond Bool: " + retActionJumpPrecondValueBool;
        }
        catch (Exception e)
        {
            Log1Text.text = "Exception: " + e.ToString();
        }

        // Jump Effect "on"
        var actionJumpEffectVals = parseFuncCall((JObject)(actionsDict["Jump"])["Effects"][0]);
        System.Object actionJumpEffectVal = actionJumpEffectVals.Item1;
        bool retActionJumpEffectValueBool = (bool)Convert.ChangeType(actionJumpEffectVal, typeof(bool));
        Log2Text.text = "Jump Effect Value Bool: " + retActionJumpEffectValueBool;

        // Collect Precond touch coin
        //if (!retActionCollectPrecondValueBool)
        //{
        // if statement so it will just stay as true after coin collected
        try
        {
            var actionCollectPrecondVals = parseFuncCall((JObject)(actionsDict["Collect"])["Preconds"][0]);
            System.Object actionCollectPrecondVal = actionCollectPrecondVals.Item1;
            retActionCollectPrecondValueBool = (bool)Convert.ChangeType(actionCollectPrecondVal, typeof(bool));
        }
        catch (Exception e)
        {
            Log3Text.text = "Collect Exception: " + e.ToString();
        }
        */

        //}
        //Log3Text.text = "touched? " + retActionCollectPrecondValueBool;

        /*if (!didCollectIncTest)
        {
            // Collect Effect ++
            var actionCollectEffectVals = parseFuncCall((JObject)(actionsDict["Collect"])["Effects"][0]);
            System.Object actionCollectEffectVal = actionCollectEffectVals.Item1;
            double retCollectEffectValueDbl = (double)Convert.ChangeType(actionCollectEffectVal, typeof(double));
            Log4Text.text = "In wallet: " + retCollectEffectValueDbl;
            didCollectIncTest = true;
        }*/

        // todo: make a 5th logging thing or comment out collect one above and use Log4Text in this test
        // Collect Effect isNotActive
        /*var actionCollectEffect2Vals = parseFuncCall((JObject)(actionsDict["Collect"])["Effects"][1]);
        System.Object actionCollectEffect2Val = actionCollectEffect2Vals.Item1;
        bool retCollectEffectValue2Bool = (bool)Convert.ChangeType(actionCollectEffect2Val, typeof(bool));
        PlaneText.text = "collect Effect Value Bool: " + retCollectEffectValue2Bool;
        */

    }

    private void storeActions(string jsonData/*string jsonFileName*/)
    {
        //Log4Text.text = "starting storeActions";
        /*try
        {
            Log1Text.text = "Json: " + File.ReadAllText("exampleGame.json");
        } catch(Exception e)
        {
            Log1Text.text = "Exception: " + e.ToString();
        }*/

        JObject json = JObject.Parse(jsonData/*File.ReadAllText("Assets/exampleGame.json")*/);
        //Log4Text.text = "read json file";
        JObject actionsJson = (JObject)json["Actions"];
        //Log4Text.text = "actions gotten: " + actionsJson.ToString();


        foreach (JProperty prop in actionsJson.Properties())
        {
            string propName = prop.Name;
            JObject propVal = (JObject)prop.Value;
            //Debug.Log(name + " - " + val);
            actionsDict.Add(propName, propVal);
            //Log4Text.text = "added an element to actions dict";
            //Debug.Log("Preconds: " + val["Preconds"]);
            //Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

            // todo: remove this. This is only for testing purposes
            /*var actionVals = parseFuncCall((JObject)propVal["Effects"][0]);
            System.Object actionVal = actionVals.Item1;
            int type = actionVals.Item2;
            if (type == 0)
            {
                bool retActionValueBool = (bool)Convert.ChangeType(actionVal, typeof(bool));
                //Debug.Log("+_+_+_+_+_+_+ " + propName + ": " + retActionValueBool);
            }
            else if (type == 1)
            {
                double retActionValDbl = (double)Convert.ChangeType(actionVal, typeof(double));
                //Debug.Log("+_+_+_+_+_+_+ " + propName + ": " + retActionValDbl);
            }*/
        }

        /*
        Debug.Log("actionsJson: " + actionsJson.ToString());
        Debug.Log("actionsJson children: " + actionsJson.Children());
        IEnumerator ie = actionsJson.Children().GetEnumerator();
        ie.MoveNext();
        string curr = ie.Current.ToString();
        Debug.Log("first element: " + curr);
        ie.MoveNext();
        curr = ie.Current.ToString();
        Debug.Log("2nd element: " + curr);
        */
    }

    private (dynamic, int) parseFuncCall(JObject jsonObj)
    {
        //Debug.Log("about to get type.");
        string type = jsonObj["type"].ToString();
        //Debug.Log("type is: " + type);
        if (type == "on" || type == "touches" || type == "below" || type == "==" || type == "||" || type == "&&")
        {
            return (GameParse.parseFuncCall<bool>(jsonObj), 0);
        }
        else
        {
            return (GameParse.parseFuncCall<double>(jsonObj), 1);
        }

    }

    public ARGameObject parsePlatform()
    {
        return GameParse.parsePlatformObject(JObject.Parse(jsonStr), defaultPlane);
    }

    public ARGameObject getAvatar()
    {
        return this.avatar;
    }
}