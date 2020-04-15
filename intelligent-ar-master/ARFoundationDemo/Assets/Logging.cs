using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

// todo: check if this works on IOS also
public class Logging : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text StateText;
    private string filename;
    private void Awake()
    {
        string logPath = Application.persistentDataPath + "/debugLogs";
        if(!Directory.Exists(logPath))
        {
            Directory.CreateDirectory(logPath);
        }
        filename = string.Format(logPath + "/log-{0:yyyy-MM-dd_hh-mm-ss-tt}.txt", DateTime.Now);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Log(string str)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(filename, true))
            {
                writer.WriteLine("> " + str);
            }
        }
        catch(Exception e)
        {
            StateText.text = "fileName: " + filename + ", Exception: " + e.ToString();
        }
    }
}
