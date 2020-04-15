using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System;

public enum TemplateName
{
    CoinCollector,
    Platformer
}

public enum PlatformShape
{
    Square,
    Circle,
    Triangle,
    Pentagon,
    Hexagon
}

public enum PathPlanDistance
{
    Max,
    Avg,
    Min
}

public class Template : MonoBehaviour
{
    static JObject json;
    public GameObject loggerObj;
    private Logging logger;

    // Coin collector sample json
    private string jsonTemplateStr = @"{
'templateType': 'Coin Collection',
    'coinPointGoal': '95',
    'coinPointSparsity': '0.5',
    'coinToPlatformRatio': '3',
    'decayRateCoinPlatformRatio': '0.9',
    'surplusCoinAmt' : '0.2',
    'startPlatform' :
    {
        'minStartSize': '10',
        'makeStartPlatformVirtual': 'true'
    },
    'numRealPlatformsIncludeWithCoins': '2',
    'shapeVirtualPlatform': 'circle',
    'coinPointsMidAirToGroundRatio': '2.9'

}";

/*
    // Platformer sample json
    private string jsonTemplateStr = @"{
'templateType': 'Platformer',
'minStartSize':'10',
'makeStartPlatformVirtual':'false',
'minEndSize':'10',
'makeEndPlatformVirtual':'false',
'pathPlan': [{'distance':'maximize'}],
'optimalPathLen': '100',
'runJumpRatio': '6.5',
'decayRateRJRatio':'0.1',
'numRealPlatforms': '5',
'numVirtualPlatforms': '3',
'shapeVirtualPlatform':'square'
}";
    */

    // Start is called before the first frame update
    void Start()
    {
        logger = loggerObj.GetComponent<Logging>();
        logger.Log("Started Template Start function");

        try
        {
            json = JObject.Parse(jsonTemplateStr);
        }
        catch(Exception e)
        {
            logger.Log("Error parsinig json. Exception: " + e.ToString());
        }

        templateType = TemplateName.CoinCollector;

        // params common between Coin Collector and Platformer templates (defaults)
        minStartSize = 10;
        makeStartPlatformVirtual = false;

        // params for Coin Collector template (defaults)
        coinPointGoal = 100;
        coinPointSparsity = 0.5F;
        coinToPlatformRatio = 3;
        decayRateCoinPlatformRatio = 0.9F;
        surplusCoinAmt = 0.2F;
        numRealPlatformsIncludeWithCoins = 2;
        shapeVirtualPlatform = PlatformShape.Square;
        coinPointsMidAirToGroundRatio = 2.8F;

        // params for Platformer template (defaults)
        minEndSize = 10;
        makeEndPlatformVirtual = false;
        distance = PathPlanDistance.Avg;
        optimalPathLen = 100;
        runJumpRatio = 6.5F;
        decayRateRJRatio = 0.1F;
        numVirtualPlatforms = 3;
        numRealPlatforms = 5;

        try
        {
            // below code reads JSON and gets the values if given
            JToken templateTypeJson = (JToken)json["templateType"];
            if (templateTypeJson != null)
            {
                string val = templateTypeJson.Value<string>();
                if (val.Equals("Coin Collection"))
                {
                    templateType = TemplateName.CoinCollector;
                }
                else if (val.Equals("Platformer"))
                {
                    templateType = TemplateName.Platformer;
                }
            }

            coinPointGoal = parseValue<int>(json, "coinPointGoal", coinPointGoal);

            coinPointSparsity = parseValue<float>(json, "coinPointSparsity", coinPointSparsity);

            coinToPlatformRatio = parseValue<float>(json, "coinToPlatformRatio", coinToPlatformRatio);
            
            decayRateCoinPlatformRatio = parseValue<float>(json, "decayRateCoinPlatformRatio", decayRateCoinPlatformRatio);

            surplusCoinAmt = parseValue<float>(json, "surplusCoinAmt", surplusCoinAmt);

            if ((JToken)json["startPlatform"] != null)
            {
                JToken minStartSizeJson = (JToken)json["startPlatform"]["minStartSize"];
                if (minStartSizeJson != null)
                {
                    minStartSize = minStartSizeJson.Value<float>();
                }
            }

            if ((JToken)json["startPlatform"] != null)
            {
                JToken makeStartPlatformVirtualJson = (JToken)json["startPlatform"]["makeStartPlatformVirtual"];
                if (makeStartPlatformVirtualJson != null)
                {
                    makeStartPlatformVirtual = makeStartPlatformVirtualJson.Value<bool>();
                }
            }

            numRealPlatformsIncludeWithCoins = parseValue<int>(json, "numRealPlatformsIncludeWithCoins", numRealPlatformsIncludeWithCoins);

            JToken shapeVirtualPlatformJson = (JToken)json["shapeVirtualPlatform"];
            if (shapeVirtualPlatformJson != null)
            {
                string val = shapeVirtualPlatformJson.Value<string>();
                if (val.Equals("square"))
                {
                    shapeVirtualPlatform = PlatformShape.Square;
                }
                else if (val.Equals("circle"))
                {
                    shapeVirtualPlatform = PlatformShape.Circle;
                }
            }

            coinPointsMidAirToGroundRatio = parseValue<float>(json, "coinPointsMidAirToGroundRatio", coinPointsMidAirToGroundRatio);

            minEndSize = parseValue<float>(json, "minEndSize", minEndSize);

            makeEndPlatformVirtual = parseValue<bool>(json, "makeEndPlatformVirtual", makeEndPlatformVirtual);

            if ((JToken)json["pathPlan"] != null)
            {
                JToken distanceJson = (JToken)json["pathPlan"][0]["distance"];
                if (distanceJson != null)
                {
                    string val = distanceJson.Value<string>();
                    if (val.Equals("maximize"))
                    {
                        distance = PathPlanDistance.Max;
                    }
                    else if (val.Equals("average"))
                    {
                        distance = PathPlanDistance.Avg;
                    }
                    else if (val.Equals("minimize"))
                    {
                        distance = PathPlanDistance.Min;
                    }
                }
            }

            optimalPathLen = parseValue<float>(json, "optimalPathLen", optimalPathLen);

            runJumpRatio = parseValue<float>(json, "runJumpRatio", runJumpRatio);

            decayRateRJRatio = parseValue<float>(json, "decayRateRJRatio", decayRateRJRatio);

            numRealPlatforms = parseValue<int>(json, "numRealPlatforms", numRealPlatforms);

            numVirtualPlatforms = parseValue<int>(json, "numVirtualPlatforms", numVirtualPlatforms);
        }
        catch (Exception e)
        {
            logger.Log("Exception: " + e.ToString());
        }

        printVarVals();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // todo: remove this. Only for debugging purposes
    private void printVarVals()
    {
        logger.Log("=== Common ===");
        logger.Log("minStartSize: " + minStartSize);
        logger.Log("makeStartPlatformVirtual: " + makeStartPlatformVirtual);

        logger.Log("=== Coin Collector ===");
        logger.Log("templateType: " + templateType);
        logger.Log("coinPointGoal: " + coinPointGoal);
        logger.Log("coinPointSparsity: " + coinPointSparsity);
        logger.Log("coinToPlatformRatio: " + coinToPlatformRatio);
        logger.Log("decayRateCoinPlatformRatio: " + decayRateCoinPlatformRatio);
        logger.Log("surplusCoinAmt: " + surplusCoinAmt);
        logger.Log("numRealPlatformsIncludeWithCoins: " + numRealPlatformsIncludeWithCoins);
        logger.Log("shapeVirtualPlatform: " + shapeVirtualPlatform);
        logger.Log("coinPointsMidAirToGroundRatio: " + coinPointsMidAirToGroundRatio);

        logger.Log("=== platformer ===");
        logger.Log("minEndSize: " + minEndSize);
        logger.Log("makeEndPlatformVirtual: " + makeEndPlatformVirtual);
        logger.Log("distance: " + distance);
        logger.Log("optimalPathLen: " + optimalPathLen);
        logger.Log("runJumpRatio: " + runJumpRatio);
        logger.Log("decayRateRJRatio: " + decayRateRJRatio);
        logger.Log("numRealPlatforms: " + numRealPlatforms);
        logger.Log("numVirtualPlatforms: " + numVirtualPlatforms);
    }

    // Only works with fields that are not nested
    private T parseValue<T>(JObject json, string field, T defaultVal)
    {
        JToken jTok = (JToken)json[field];
        if(jTok != null)
        {
            return jTok.Value<T>();
        }

        // value was not passed. return null
        return defaultVal;
    }

    //public double height { get; set; }
    public TemplateName templateType { get; set; }
    public int coinPointGoal { get; set; }
    public float coinPointSparsity { get; set; }
    public float coinToPlatformRatio { get; set; }
    public float decayRateCoinPlatformRatio { get; set; }
    public float surplusCoinAmt { get; set; }
    public float minStartSize { get; set; }
    public bool makeStartPlatformVirtual { get; set; }
    public int numRealPlatformsIncludeWithCoins { get; set; }
    public PlatformShape shapeVirtualPlatform { get; set; }
    public float coinPointsMidAirToGroundRatio { get; set; }

    public float minEndSize { get; set; }
    public bool makeEndPlatformVirtual { get; set; }
    public PathPlanDistance distance { get; set; }
    public float optimalPathLen { get; set; }
    public float runJumpRatio { get; set; }
    public float decayRateRJRatio { get; set; }
    public int numVirtualPlatforms { get; set; }
    public int numRealPlatforms { get; set; }
}
