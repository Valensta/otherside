
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine.UI;
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MyPlayerEvent : IComparable
{
    public PlayerEvent eventtype;
    public DateTime eventtime;
    public string attribute_1;
    public string attribute_2;
    public float metric_1;
    public float metric_2;
    public float wave_time;
    public bool event_complete = false;

    public string toString()
    {
        return eventtype + " | " + attribute_1 + " | " + attribute_2 + " | " + metric_1 + " | " + metric_2 + " | " + wave_time + " | " + event_complete;
    }

    

    public int CompareTo(object obj)
    {
        if (obj == null) return 1;
        MyPlayerEvent d = obj as MyPlayerEvent;
        return (this.wave_time == 0 || d.wave_time == 0) ?
                    this.eventtime.CompareTo(d.eventtime) : this.wave_time.CompareTo(d.wave_time);

    }
}

public class FakeRunLoader : MonoBehaviour
{
    private PlayerSimulator my_simulator;

    public Button button;
    public TextAsset file;
    public void showButton(bool show)
    {
        button.gameObject.SetActive(show);
    }

    

    List<MyPlayerEvent> rowList = new List<MyPlayerEvent>();
    bool isLoaded = false;

    public void setSimulator(PlayerSimulator ps)
    {
        my_simulator = ps;
    }

    public bool IsLoaded()
    {
        return isLoaded;
    }

    public List<MyPlayerEvent> GetRowList()
    {
        return rowList;
    }

    public void Load()
    {
        rowList.Clear();
        string[][] grid = CsvParser2.Parse(file.text);
        for (int i = 1; i < grid.Length; i++)
        {
            int j = 0;
            MyPlayerEvent row = new MyPlayerEvent();
            try
            {
                j = 0;
                row.eventtype = EnumUtil.EnumFromString<PlayerEvent>(grid[i][j++], PlayerEvent.Null);
                row.attribute_1 = grid[i][j++];
                row.attribute_2 = grid[i][j++];
                row.metric_1 = (grid[i][j].Equals(""))? 0 : float.Parse(grid[i][j]);
                j++;
                row.metric_2 = (grid[i][j].Equals("")) ? 0 : float.Parse(grid[i][j]);
                j++;
                row.wave_time = (grid[i][j].Equals("")) ? 0 : float.Parse(grid[i][j]);
                j++;
                row.eventtime = DateTime.Parse(grid[i][j]);
            }
            catch(Exception e)
            {
                Debug.Log("Could not parse " + String.Join(" | ",grid[i]) + " j " + j + " " + e.Message + "\n");
            }
            rowList.Add(row);
        }

        rowList.Sort();

        isLoaded = true;
    }



    public int NumRows()
    {
        return rowList.Count;
    }

    public MyPlayerEvent GetAt(int i)
    {
        if (rowList.Count <= i)
            return null;
        return rowList[i];
    }

    public MyPlayerEvent findEventType(PlayerEvent find, string attribute_1)
    {
        return rowList.Find(x => x.eventtype == find && x.attribute_1.Equals(attribute_1));
    }

    public MyPlayerEvent findEventType(PlayerEvent find)
    {
        return rowList.Find(x => x.eventtype == find);
    }
    public List<MyPlayerEvent> FindAllEventtype(PlayerEvent find)
    {
        return rowList.FindAll(x => x.eventtype == find);
    }
    public MyPlayerEvent Find_a_attribute_1(string find)
    {
        return rowList.Find(x => x.attribute_1 == find);
    }
    public List<MyPlayerEvent> FindAll_a_attribute_1(string find)
    {
        return rowList.FindAll(x => x.attribute_1 == find);
    }
    public MyPlayerEvent Find_a_attribute_2(string find)
    {
        return rowList.Find(x => x.attribute_2 == find);
    }
    public List<MyPlayerEvent> FindAll_a_attribute_2(string find)
    {
        return rowList.FindAll(x => x.attribute_2 == find);
    }
    public MyPlayerEvent Find_m_metric_1(float find)
    {
        return rowList.Find(x => x.metric_1 == find);
    }
    public List<MyPlayerEvent> FindAll_m_metric_1(float find)
    {
        return rowList.FindAll(x => x.metric_1 == find);
    }
    public MyPlayerEvent Find_m_metric_2(float find)
    {
        return rowList.Find(x => x.metric_2 == find);
    }
    public List<MyPlayerEvent> FindAll_m_metric_2(float find)
    {
        return rowList.FindAll(x => x.metric_2 == find);
    }
    public MyPlayerEvent Find_m_wave_time(float find)
    {
        return rowList.Find(x => x.wave_time == find);
    }
    public List<MyPlayerEvent> FindAll_m_wave_time(float find)
    {
        return rowList.FindAll(x => x.wave_time == find);
    }

}

