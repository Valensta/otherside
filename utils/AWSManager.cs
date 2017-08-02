using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.MobileAnalytics;
using Amazon.MobileAnalytics.MobileAnalyticsManager;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.SecurityToken.Model;
using Amazon.Util.Internal.PlatformServices;


public class AWSManager : MonoBehaviour {

	//private static AWSManager instance;
	//public static AWSManager Instance { get; private set; }
    private const string MobileAnaylticsAppId = "6d0189d126c44beab7e7434f84dd1ae5";
    private MobileAnalyticsManager analyticsManager;
    private BasicAWSCredentials credentials;
    private bool firstFocus = true;
    private static readonly RegionEndpoint _mobileAnalyticsRegion = null;
    private RegionEndpoint MobileAnalyticsRegion { get { return _mobileAnalyticsRegion != null ? _mobileAnalyticsRegion : AWSConfigs.RegionEndpoint; } }

    public bool ENABLED = true;
    
    void Awake(){
        
      //  Instance = this;
#if UNITY_EDITOR
ENABLED = false;
#else
ENABLED = true;
#endif
        
        if (!ENABLED) return;
        UnityInitializer.AttachToGameObject(this.gameObject);
    
    }

    private AWSCredentials getCredentials
    {
        get
        {
            
            if (!ENABLED) return null;
            
            if (credentials == null)
            {
                
                credentials = new BasicAWSCredentials("AKIAIHTVSK4DE7HQYDJA", "atN5AKIbmCCAz4YeahopfJYETKL33r/ltb+y");
                               
            }
            return credentials;
        }
    }


    public void logEvent(PlayerEvent label, bool midlevel, Dictionary<string, string> customAttributes = null, Dictionary<string, double> customMetrics = null)
    {

        if (!ENABLED) return;
        //Debug.Log("Logging custom event\n");

        CustomEvent customEvent = new CustomEvent("custom_event");

        
        customEvent.AddAttribute("eventtype", label.ToString());

        customEvent.AddMetric("level", Central.Instance.current_lvl);

        if (midlevel)
        {
            customEvent.AddMetric("health", Peripheral.Instance.GetHealth());
            customEvent.AddMetric("wave_time", Sun.Instance.current_time_of_day);
            customEvent.AddAttribute("difficulty", Peripheral.Instance.difficulty.ToString());
        }else
        {
            customEvent.AddMetric("wave_time", -1);
        }

        customEvent.AddAttribute("app_version", Application.version);

        customEvent.AddAttribute("device",
            SystemInfo.deviceType + " " + SystemInfo.deviceName + " " + SystemInfo.deviceModel);
        customEvent.AddAttribute("device_id", SystemInfo.deviceUniqueIdentifier);


        foreach (string key in customAttributes.Keys)
        {
            customEvent.AddAttribute(key, customAttributes[key].ToString());            
        }

        foreach (string key in customMetrics.Keys)
        {
            customEvent.AddMetric(key, customMetrics[key]);
        }
        AnalyticsManager.RecordEvent(customEvent);
        
        
        
    }



    private MobileAnalyticsManager AnalyticsManager
    {
        
        
        get
        {
            if (!ENABLED) return null;
            if (analyticsManager == null)
            {

                Debug.Log("Device " + SystemInfo.deviceType + " " + SystemInfo.deviceName + " " + SystemInfo.deviceUniqueIdentifier + " " + SystemInfo.deviceModel + "\n");

                MobileAnalyticsManagerConfig config = new MobileAnalyticsManagerConfig();
                config.AllowUseDataNetwork = true;
                config.DBWarningThreshold = 0.9f;
                config.MaxDBSize = 5242880;
                config.MaxRequestSize = 102400;
                config.SessionTimeout = 5;
                    
                
                analyticsManager = MobileAnalyticsManager.GetOrCreateInstance(MobileAnaylticsAppId, 
                            new CognitoAWSCredentials("us-east-1:c7528789-2aa8-49db-b8b6-f87320fa8a5a", RegionEndpoint.USEast1),
                            Amazon.RegionEndpoint.USEast1, config);
                ApplicationInfo info = new ApplicationInfo();
                
                
            }
            
            return analyticsManager;
        }
    }

    /*
    void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            if (firstFocus)
            {
                // if this is the first time the application focuses, we do not resume,
                // because creation of the AnalyticsManager object fires the session start
                // event instead.
                firstFocus = false;
            }
            else
            {
                AnalyticsManager.ResumeSession();
            }
        }
        else
        {
            AnalyticsManager.PauseSession();
        }
    }
    */
   
}