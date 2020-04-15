# intelligent-ar

AI/AR projects with the Entertainment Intelligence Lab at Georgia Tech

## ARFoundation Basics Tutorial Materials

* https://www.youtube.com/watch?v=Ml2UakwRxjk
* http://virtualxdesign.mit.edu/blog/2019/6/22/viewing-your-models-in-ar

## Setting Up Your Unity/Code Environment

1. Download code base
2. In Unity Hub with “Projects” selected click on “Add”
3. Navigate into the downloaded directory called “intelligent-ar-master”, then click on the “ARFoundationDemo” directory and click “Open”
4. Click on this newly added project in Unity Hub
5. Please wait while it imports and loads some files that were not included in the Git repository for storage size reasons
6. You should see the Unity Editor window now
7. Expand “Assets” in the “Project” tab, then click on “Scenes”
8. Double-click on the “SampleScene” scene to load this scene
9. go to Window -> Asset Store
10. search "Newtonsoft"
11. Click on the one that is free
12. Click on the "Import" button
13. a window on the left will appear titled "Import Unity Package". Leave everything checked and click the 'Import' button
14. The errors should go away after the importing from the previous step has completed

## Running the Demo Code on Android
1. Click on “File”->”Build Settings”
2. After ensuring your Android device is plugged into the laptop/computer, click on “Android” under the “Platform” section
3. Click on “Refresh” next to “Run Device” then choose your plugged-in device
4. Click on “Switch Platform”
5. Please wait while it imports assets
6. Click on “Build And Run”
7. Create a new folder called “Builds” when it prompts you to save the build.
8. In the “Builds” folder, name the build whatever you want and press “Save”

## Debugging
There is a script called Logging.cs. It's attached to a Game Object called "Logger". To log to a text file that's written to the device, follow these steps:
1. In your class, make a new variable like this: public GameObject loggerObj;
2. In the same class, write this code also: private Logging logger;
3. In the Unity editor, the corresponding script should have a new field called Logger Obj
4. Click and drag the "Logger" Game Object from the Hierarchy into the Logger Obj field
5. Going back to your code, get a reference to the Logging.cs script by writing this code: logger = loggerObj.GetComponent<Logging>();
6. Now you can log any string you want like this: logger.Log(<your string here>);
7. Go to your device's internal storage to see the file. It should be in a directory called debugLogs (On my Android device it's Internal storage > Android > data > tutorial.demo.ARFoundationDemo > files > debugLogs)