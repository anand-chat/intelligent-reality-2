using System;
using Newtonsoft.Json.Linq;

public class GameObject
{
    static JObject json = JObject.Parse("vgddlObjects.json");
    public GameObject()
    {
        double height = 2;
        double width = 1;
        double radius = 0.5;
        int wallet = 0;
        double minArea = -1;
        double minHeight = -1;
        double maxAngleToFloor = 90;
        double minAngleToFloor = 0;
        bool isActive = false;
        bool isMoving = false;
        bool isGoal = false;
        bool isClimbable = false;
    }
    public GameObject(double height, double width, double radius, int wallet, bool isActive, bool isMoving)
    {
        this.height = height;
        this.width = width;
        this.radius = radius;
        this.wallet = wallet;
        this.isActive = isActive;
        this.isMoving = isMoving;
    }
    public GameObject(double minArea, double minAngleToFloor, double maxAngleToFloor, bool isGoal)
    {
        this.minAngleToFloor = minAngleToFloor;
        this.minArea = minArea;
        this.maxAngleToFloor = maxAngleToFloor;
        this.isGoal = isGoal;
    }
    public GameObject(double minHeight, double minAngleToFloor, double maxAngleToFloor)
    {
        this.minAngleToFloor = minAngleToFloor;
        this.minHeight = minHeight;
        this.maxAngleToFloor = maxAngleToFloor;
        //isClimbable = json["isClimbable"].Value<bool>();
    }
    public static GameObject makeAvatarGameObject()
    {
        double height = json["Objects"]["Avatar"]["height"].Value<double>();
        double width = json["Objects"]["Avatar"]["width"].Value<double>();
        double radius = json["Objects"]["Avatar"]["radius"].Value<double>();
        int wallet = json["Objects"]["Avatar"]["wallet"].Value<int>();
        bool isActive = json["Objects"]["Avatar"]["isActive"].Value<bool>();
        bool isMoving = json["Objects"]["Avatar"]["isMoving"].Value<bool>();
        return new GameObject(height, width, radius, wallet, isActive, isMoving);
    }
    public static GameObject makePlatformGameObject()
    {
        double minAngleToFloor = json["Objects"]["Platform"]["minAngleToFloor"].Value<double>();
        double minArea = json["Objects"]["Platform"]["minArea"].Value<double>();
        double maxAngleToFloor = json["Objects"]["Platform"]["maxAngleToFloor"].Value<double>();
        bool isGoal = json["Objects"]["Platform"]["isGoal"].Value<bool>();
        return new GameObject(minArea, minAngleToFloor, maxAngleToFloor, isGoal);

    }
    public static GameObject makeVerticalSurfaceGameObject()
    {
        double minAngleToFloor = json["Objects"]["VerticalSurface"]["minAngleToFloor"].Value<double>();
        double minHeight = json["Objects"]["VerticalSurface"]["minHeight"].Value<double>();
        double maxAngleToFloor = json["maxAngleToFloor"].Value<double>();
        return new GameObject(minHeight, minAngleToFloor, maxAngleToFloor);
    }
    public double height { get; set; }
    public double width { get; set; }
    public int wallet { get; set; }
    public double minArea { get; set; }
    public double minHeight { get; set; }
    public double maxAngleToFloor { get; set; }
    public double minAngleToFloor { get; set; }
    public bool isActive { get; set; }
    public bool isMoving { get; set; }
    public bool isGoal { get; set; }
}
