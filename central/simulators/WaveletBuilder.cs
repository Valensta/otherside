using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class WaveletBuilder : MonoBehaviour {
    public List<MyToggle> enemyToggles;
    public Text summary;
    public EnemyType currentEnemyType;
    public int count = 0;
    public int path = 0;

    public InputField countInputField;
    public InputField pathInputField;


    public void toggleEnemyType(int number, string text, bool currentValue)
    {
        currentEnemyType = EnumUtil.EnumFromString<EnemyType>(text, EnemyType.Null);
        summary.text = toString();
    }

    public void setCount(string number)
    {
        if (number == null || number.Equals("")) return;
        count = int.Parse(number);
        summary.text = toString();
      //  Debug.Log("Setting count " + number + "\n");
    }

    public InitEnemyCount getSubWavelet()
    {
        InitEnemyCount enemies = new InitEnemyCount();
        enemies.name = currentEnemyType.ToString();
        enemies.c = count;
        enemies.p = path;

        return enemies;

    }

  
    public void ForcePath(int i)
    {
        pathInputField.text = i.ToString();
        setPath(i.ToString());
    }



    public bool Valid()
    {
        return (currentEnemyType != EnemyType.Null && count > 0 && path >= 0 && path <= WaypointMultiPathfinder.Instance.paths.Count);
    }

    public void setPath(string number)
    {
        if (number == null || number.Equals("")) return;
            path = int.Parse(number);
        summary.text = toString();
    //    Debug.Log("Setting Path " + number + "\n");
    }

    string toString()
    {
        if (Valid())
            return currentEnemyType.ToString() + "\nC: " + count + "\nP: " + path + "\n";
        else return "";
    }

    public void Init(List<EnemyType> enabled_enemies)
    {
        foreach (MyToggle toggle in enemyToggles)
        {
            if (!enemyEnabled(enabled_enemies, toggle.text))
            {
                toggle.gameObject.SetActive(false);
            }
            else
            {
                toggle.gameObject.SetActive(true);

                toggle.toggleAction += new MyToggle.ToggleAction(toggleEnemyType);
            }
        }
        
    }

    bool enemyEnabled(List<EnemyType> list, string s)
    {
        EnemyType type = EnumUtil.EnumFromString(s, EnemyType.Null);
        foreach (EnemyType t in list) if (type == t) return true;
        return false;
    }
       
}