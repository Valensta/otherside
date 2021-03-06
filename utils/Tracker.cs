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
using UnityEngine.UI;

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

        if (label == PlayerEvent.Error && customAttributes != null)
        {
            customAttributes["attribute_1"] =
                Encoding.UTF8.GetString(Encoding.Default.GetBytes(customAttributes["attribute_1"])); 
            customAttributes["attribute_2"] =
                Encoding.UTF8.GetString(Encoding.Default.GetBytes(customAttributes["attribute_2"]));
            
        }

        if (customAttributes != null)
        {
            if (customAttributes.ContainsKey("attribute_2") && customAttributes["attribute_2"].Length > 255)
                customAttributes["attribute_2"] = customAttributes["attribute_2"].Substring(0, 254);
            if (customAttributes.ContainsKey("attribute_1") && customAttributes["attribute_1"].Length > 255)
                customAttributes["attribute_1"] = customAttributes["attribute_1"].Substring(0, 254);
        }
        
        customAttributes = (customAttributes == null) ? new Dictionary<string, string>() : customAttributes;
        customMetrics = (customMetrics == null) ? new Dictionary<string, double>() : customMetrics;
        Central.Instance.myAWSManager.logEvent(label, midlevel, customAttributes, customMetrics);
    
        
    }


    public static string getDate()
    {
        return DateTime.Now.ToString("HH:mm:ss yyyymmdd");
    }

}
	





	