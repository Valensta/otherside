using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
[CreateAssetMenu()]
public class Actor : ScriptableObject
{
    public unitStats stats;
    public string actorName;
}

[System.Serializable]
public class Universe : MonoBehaviour {
    public List<Actor> enemyStore;
    public List<Actor> toyStore;


}