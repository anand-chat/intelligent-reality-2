using System;


public class Functions
{
   public static dynamic funcCall(GameObject obj1, GameObject obj2, String attribute, String type)
    {
        switch (type)
        {
            case "on":
                return onit(obj1, obj2);
                break;
            case "touches":
                return touches(obj1, obj2);
                break;
            case "below":
                return below(obj1, obj2);
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
                return isEqual(obj1, obj2);
                break;
            case "||":
                return or(obj1, obj2);
                break;
            case "&&":
                return and(obj1, obj2);
                break;
            default:
                Console.WriteLine("The function is unknown.");
                return false;
                break;
        }
    }

    public static bool below(GameObject obj1, GameObject obj2)
    {
        // stub
        return false;
    }

    public static bool onit(GameObject obj1, GameObject obj2)
    {
        // stub
        return false;
    }

    public static bool touches(GameObject obj1, GameObject obj2)
    {
        // stub
        return false;
    }

    public static bool isEqual(GameObject obj1, GameObject obj2)
    {
        if (obj1 == obj2) {
            return true;
        }
        return false;
    }

    public static bool or(GameObject obj1, GameObject obj2)
    {
        if (obj1 || obj2) {
            return true;
        }
        return false;
    }

    public static bool and(GameObject obj1, GameObject obj2)
    {
        if (obj1 && obj2) {
            return true;
        }
        return false;
    }
    public static dynamic increment(GameObject obj1, String attribute)
    {
        switch (attribute)
        {
            case "height":
                return obj1.height++;
                break;
            case "width":
                return obj1.width++;
                break;
            case "radius":
                return obj1.radius++;
                break;
            case "wallet":
                return obj1.wallet++;
                break;
            case "minArea":
                return obj1.minArea++;
                break;
            case "minHeight":
                return obj1.minHeight++;
                break;
            case "minAngleToFloor":
                return obj1.minAngleToFloor++;
                break;
            case "maxAngleToFloor":
                return obj1.maxAngleToFloor++;
                break;
            default:
                Console.WriteLine("The attribute is unknown.");
                return -1;
                break;
        }
        return -1;
    }
    public static dynamic decrement(GameObject obj1, String attribute)
    {
        switch (attribute)
        {
            case "height":
                return obj1.height--:
                break;
            case "width":
                return obj1.width--;
                break;
            case "radius":
                return obj1.radius--;
                break;
            case "wallet":
                return obj1.wallet--;
                break;
            case "minArea":
                return obj1.minArea--;
                break;
            case "minHeight":
                return obj1.minHeight--;
                break;
            case "minAngleToFloor":
                return obj1.minAngleToFloor--;
                break;
            case "maxAngleToFloor":
                return obj1.maxAngleToFloor--;
                break;
            default:
                Console.WriteLine("The attribute is unknown.");
                return -1;
                break;
        }
        return -1;
    }
    public static dynamic dot(GameObject obj1, String attribute)
    {
        switch (attribute)
        {
            case "height":
                return obj1.height:
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
            case "isGoal":
                return obj1.isGoal;
                break;
            case "isMoving":
                return obj1.isMoving;
                break;
            case "isClimbable":
                return obj1.isClimbable;
                break;
            default:
                Console.WriteLine("The attribute is unknown.");
                return false;
                break;
        }
        return false;
    }
}
