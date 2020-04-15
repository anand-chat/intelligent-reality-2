Using System;

public class GameParse
    {
        GameObject avatar;
        GameObject platform;
        GameObject verticalsurface;

    public static void parseObjects()
   {
    avatar = makeAvatarGameObject();
    platform = makePlatformGameObject();
    verticalsurface = makeVerticalSurfaceGameObject();
   }
   public static void parseActions()
   {
    return;
   }
   public static T parseFuncCall<T>(JObject jsonFunc)
   {
    if (jsonFunc == null) {
        return null;
    }
    String type = jsonFunc["type"];
    String arg1 = jsonFunc["arg1"];
    String arg2 = jsonFunc["arg2"];
    String attribute = null;
    if (arg1 == null) {
        return;
    }
    if (arg2 == null) {
        return;
    }
    if (arg1.charAt(0) == '{') {
        arg1 = parseFuncCall(JObject.Parse(arg1));
    }
    if (arg2.charAt(0) == '{') {
        arg2 = parseFuncCall(JObject.Parse(arg1));
    }
    if (type == "==" || type == "<=" || type == ">=" || type == "<" || type == ">") {
        attribute = arg2;
        arg2 = null;
        Object value = funcCall(arg1, arg2, attribute, type);
        return (T) Convert.ChangeType(value, typeof(T));
    }
    switch (arg1)
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
            Console.WriteLine("The argument is unknown.");
            break;
    }
    switch (arg2)
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
        case "Wallet":
            attribute = "wallet";
            arg2 = null;
            break;
        case "isGoal":
            attribute = "isGoal";
            arg2 = null;
            break;
        default:
            Console.WriteLine("The argument is unknown.");
            break;
    }
    Object value = funcCall(arg1, arg2, attribute, type);
        return (T) Convert.ChangeType(value, typeof(T));

   }


}
