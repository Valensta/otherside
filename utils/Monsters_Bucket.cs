using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class Monsters_Bucket : MonoBehaviour {


    private static Monsters_Bucket instance;
    public static Monsters_Bucket Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("monsters bucket got destroyeed");
            Destroy(gameObject);
        }
        Instance = this;
    }


}
