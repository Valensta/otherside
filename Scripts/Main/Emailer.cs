using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;


[System.Serializable]
public class Emailer : MonoBehaviour {
    private bool _capture = false;

    void onClick()
    {
        _capture = true;
    }

    void OnPostRenger()
    {
        string[] mail_to = new string[1];
        mail_to[0] = "imaginaryturn@gmail.com";
        
        if (_capture)
        {
            //SocialXT.Mail(mail_to, "screenshottest", "body test", false,
            _capture = false;
        }
    }
}//end of class

