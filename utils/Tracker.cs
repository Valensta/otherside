using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Fabric.Crashlytics;
using Fabric.Answers;
using UnityEngine.Analytics;


using System;
using Amazon;
using Amazon.S3;
using Amazon.SecurityToken.Model;

public static class Tracker {
    public static bool track = true;
    public static bool crashlytics = true;

    static StringBuilder sb = new StringBuilder();

    public static void LogDump(string what)
    {
        sb.Append(what);
        
    }

    public static void EndLog()
    {
        System.IO.File.WriteAllText(Get.savegame_location + "log_dump.txt", sb.ToString());

#if !UNITY_EDITOR
        System.IO.File.WriteAllText(Get.savegame_location + "snapshot", sb.ToString());
#else
        Debug.Log(sb.ToString());
#endif
    }

    

    

    public static void ThrowNonFatal(string error)
    {
        Crashlytics.RecordCustomException("ERROR", error, error);
    
        Crashlytics.ThrowNonFatal();
    }

    public static void Log(PlayerEvent label, bool midlevel, Dictionary<string, string> customAttributes = null,
        Dictionary<string, double> customMetrics = null)
    
    {
        if (!track) return;
        
        customAttributes = (customAttributes == null) ? new Dictionary<string, string>() : customAttributes;
        customMetrics = (customMetrics == null) ? new Dictionary<string, double>() : customMetrics;
        Central.Instance.myAWSManager.logEvent(label, midlevel, customAttributes, customMetrics);
    
        
    }


    public static string getDate()
    {
        return DateTime.Now.ToString("HH:mm:ss yyyymmdd");
    }

}
	





	