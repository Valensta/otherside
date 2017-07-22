using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TemporarySaver
{
    public float percent;
    public float remaining_time;
    public string my_name;
    public WishType type;
}

[System.Serializable]
public class Temporary
{
    
    public float percent;
    float remaining_time;
    public MyLabel label; //potion in use label
    public string my_name;

    public TemporarySaver getSaver(WishType type)
    {
        TemporarySaver saver = new TemporarySaver();
        saver.percent = this.percent;
        saver.remaining_time = this.remaining_time;
        saver.my_name = this.my_name;
        saver.type = type;
        return saver;
    }

    public Temporary(GenericPanel panel, WishType type, float _percent, float _time, string label_name, bool has_label)
    {
        percent = _percent;
        remaining_time = _time;
        if (has_label)
        {
            getLabel(panel);
            label.content = label_name;
            label.gameObject.name = label_name;
            Sprite sp = Resources.Load("GUI/Inventory/" + type.ToString() + "_image", typeof(Sprite)) as Sprite;
            label.image.sprite = sp;

            panel.AddLabel(label, true, true);
            panel.UpdatePanel();
            Blink();
        }
        else
        {
            Debug.Log("Has no label!\n");
        }
    }

    public void getLabel(GenericPanel panel)
    {
        foreach (MyLabel l in panel.list)
        {
            if (!l.gameObject.activeSelf){
                label = l;
         //       Debug.Log("reusing label\n");
                l.SetActive(true);
                return;
            }
        }
      //  Debug.Log("getting new label\n");
        label = Zoo.Instance.getObject("GUI/VariableStatVertical", true).GetComponent<MyLabel>();
    }
    
    public float GetRemainingTime()
    {
        return remaining_time;    
    }

    public void SetRemainingTime(float _r)
    {
        remaining_time = _r;
        if (label != null) SetLabels();
    }
    public void RemoveLabel(GenericPanel my_panel)
    {        
        my_panel.RemoveLabel(label.content);
        //label.SetActive(false);
        
        
        Zoo.Instance.returnObject(label.gameObject, true);
    }

    public void Blink()
    {
        if (label != null && label.tweener != null)
        {
          //  label.tweener.duration = EagleEyes.Instance.tween_duration;
            label.tweener.Init();
        }
    }

    public void SetLabels()
    {
        
        label.getText(LabelName.SkillStrength).setText(Show.ToPercent(percent));
        label.getText(LabelName.TimeRemaining).setText(Mathf.CeilToInt(remaining_time).ToString());
        
    }
}
[System.Serializable]
public class VariableStat : MonoBehaviour
{
    public bool has_label;
    int count;
    public float init_stat;
    public List<Temporary> effects; // in action
    public GenericPanel my_panel;
    public WishType type;

   public List<TemporarySaver> getTemporarySavers()
    {
        List<TemporarySaver> savers = new List<TemporarySaver>();
        foreach (Temporary effect in effects)
        {
            savers.Add(effect.getSaver(type));
        }
        return savers;
    }

    public VariableStat(float init)
    {
        init_stat = init;
        effects = new List<Temporary>();
    }

    public float getStat()
    {
        float stat = init_stat;
        foreach(Temporary t in effects)
        {
            stat += init_stat * t.percent;
        }
    //    Debug.Log("Getting stat " + stat + "\n");
        return stat;
    }

    public bool AddEffect(float percent, float time)
    {
        if (effects.Count < 1)
        {
            effects.Add(new Temporary(my_panel, type, percent, time, type.ToString() + count.ToString(), true));
            count++;
            return true;
        }
        else
        {
            //do a scale bounce visual thing on the effects that are already in place
            Debug.Log("Already using 2 effects on " + this.name + "\n");
            foreach(Temporary t in effects)
            {
                t.Blink();                    
            }

            return false;
        }
    }

    public void Reset()
    {
     //   Debug.Log("Resetting " + type + "\n");
        for (int i = 0; i < effects.Count; i++)
        {
            RemoveEffect(i);
        }
    }

    void RemoveEffect(int i)
    {
    //    Debug.Log("removing effect " + effects[i].my_name + "\n");
        effects[i].RemoveLabel(my_panel);
        effects.RemoveAt(i);

    }

    void Update()
    {
        if (Time.timeScale == 0) return;
        if (effects.Count == 0) return;

        
        for (int i = 0; i < effects.Count; i++)
        {
            //     Debug.Log("Checking " + effects[i].remaining_time + "\n");
            if (effects[i].GetRemainingTime() <= 0) { RemoveEffect(i); }
            else effects[i].SetRemainingTime(effects[i].GetRemainingTime() - Time.deltaTime);
        }
    }
}

