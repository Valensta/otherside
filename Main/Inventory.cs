using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;



[System.Serializable]
public class Wishlet
{
    public Wish my_wish;
    public MyLabel my_label;
    

    public Wishlet(Wish w, MyLabel l)
    {
        my_wish = w;        
        my_label = l;
    }
}

public class Inventory : MonoBehaviour {
    [SerializeField]

    public List<Wishlet> wishes = new List<Wishlet>();
    int last_button;
    int count = 0;
    public delegate void onWishChangedHandler(Wish w, bool added, bool visible, float delta);
    public static event onWishChangedHandler onWishChanged;
    public List<List_Panel> my_panels;


    public void InitWishes()
    {

        for (int i = 0; i < wishes.Count; i++)
        {
            _removeWishLabel(i);
        }

        wishes = new List<Wishlet>();
        wishes.Add(new Wishlet(new Wish(WishType.Sensible, 0, "sensible"), null));
    }


    public float GetWishScore()
    {
        //float i = 0f;
        //foreach (Wishlet w in wishes) i += w.my_wish.strength;
        return wishes[0].my_wish.Strength;            
        //return i;
    }

    void _removeWishLabel(int i)
    {
        if (i == 0) return;
        wishes[i].my_label.SetInteractable(false);
        wishes[i].my_label.SetActive(false);
        wishes[i].my_label.InitWish(null);
    }

    public void UseWish(Wish wish)
    {//broken

        Debug.Log("Using wish!!! " + wish.type + "\n");
        for (int i = 1; i < wishes.Count; i++)
        {
            if (wishes[i].my_wish.type == wish.type && wishes[i].my_wish.strength == wish.strength)
            {
                Debug.Log("Using wish " + i + "\n");
                if (DoTheThing(i))
                {
                    MyWishButton b = (MyWishButton)wishes[i].my_label.ui_button;
                    int count = wishes[i].my_wish.Count;
                    if (count > 1)
                    {
                        wishes[i].my_wish.Count--;
                        b.setCount(-1, false);
                    }
                    else
                    {
                        _removeWishLabel(i);
                        wishes.RemoveAt(i);
                    }


                }
                return;
            }
        }
        Debug.Log("Could not locate wish " + wish.type + " strength " + wish.strength + " to remove, trying to remove an invalid wish!!!!\n");
        return;
    }

    bool DoTheThing(int i)
    {
        bool status = false;
        switch (wishes[i].my_wish.type) {
            case WishType.MoreXP:
                status = Peripheral.Instance.xp_factor.AddEffect(wishes[i].my_wish.getEffect(), wishes[i].my_wish.getTime());
                break;
            case WishType.MoreDreams:
                status = Peripheral.Instance.dream_factor.AddEffect(wishes[i].my_wish.getEffect(), wishes[i].my_wish.getTime());
                break;
            case WishType.MoreDamage:
                status = Peripheral.Instance.damage_factor.AddEffect(wishes[i].my_wish.getEffect(), wishes[i].my_wish.getTime());
                break;
            case WishType.MoreHealth:
                Peripheral.Instance.AdjustHealth(wishes[i].my_wish.getEffect());
                status = true;
                break;
            default:
                Debug.Log("Inventory does not know how to deal with wishtype " + wishes[i].my_wish.type + "\n");
                break;
            }
        return status;
        
        //return status;
    }


    public bool HaveWish(WishType type, float strength)
    {
        if (type != WishType.Sensible)
        {
            Debug.Log("Using HaveWish on wish type " + type + ", not good\n");
            return false;
        }

        return (wishes[0].my_wish.Strength >= strength);

    }

    

    public float GetWish(WishType type)
    {
        if (type == WishType.Sensible) return wishes[0].my_wish.Strength;
        Debug.Log("Trying to getWish of type " + type + ", not good\n");
        return 0;
    }

    public bool SubtractWish(WishType type, float strength)
    {
        if (type != WishType.Sensible)
        {
            Debug.Log("Using SubtractWish on wish type " + type + ", not good\n");
            return false;
        }

        if (HaveWish(type, strength))
        {
            wishes[0].my_wish.Strength -= strength;

            if (onWishChanged != null) onWishChanged(wishes[0].my_wish, false, true, strength);

            

            return true;
        }
        else {
            return false;
        }

    }


    public List<Wish> getWishList()
    {
        List<Wish> wlist = new List<Wish>();
        foreach (Wishlet w in wishes)
        {
            if (w.my_wish != null) wlist.Add(w.my_wish);
        }
        return wlist;
    }

    public void setWishList(List<Wish> list) 
    {
        InitWishes();
    
        foreach (Wish w in list)
        {//should preagg them
       
            AddWish(w.type, w.Strength, w.count);
        }
    }

    MyLabel _getEmptyLabel()
    {
        foreach (List_Panel panel in my_panels)
        {
            foreach (MyLabel l in panel.list)
            {
                if (l.type.Equals("inventory") && !((MyWishButton)l.ui_button).interactable) return l;
            }
        }
        Debug.Log("Did not find an empty label!\n");
        return null;
    }

    MyWishButton _getWishButton(WishType type, float strength)
    {
    //    Debug.Log("Trying to get wishbutton for " + type + " " + strength + "\n");       
        foreach (List_Panel panel in my_panels)
        {
            foreach (MyLabel l in panel.list)
            {

                if (l.type.Equals("inventory")) {
                    MyWishButton checkme = (MyWishButton)l.ui_button;
                 //   Debug.Log("checking " + checkme.gameObject.name + " " + checkme.interactable + " " + checkme.my_wish.type + " " + checkme.my_wish.strength + "\n");
                    if (checkme.interactable && checkme.my_wish.type == type && Mathf.Approximately(checkme.my_wish.Strength, strength)) return checkme;
                }
            }
        }
      //  Debug.Log("DID NOT FIND wishbutton for " + type + " " + strength + "\n");
        return null;
    }

    public float AddWish(WishType type, float strength, int count)
    {
    //    Debug.Log("Adding wish " + type + " " + strength + " " + count + "\n");

        if (type == WishType.Sensible)
        {
            wishes[0].my_wish.Strength += strength;
            if (onWishChanged != null) onWishChanged(wishes[0].my_wish, (strength > 0), true, strength);
            
            EagleEyes.Instance.UpdateToyButtons("blah", ToyType.Temporary, false);
            //EagleEyes.Instance.WishUpdate(wishes[0], true);
            return wishes[0].my_wish.Strength;
        }
        else {

            MyWishButton button = _getWishButton(type, strength);
            Wish w = null;
            if (button)
            {
                button.setCount(count, false);
                w = button.my_wish;
                w.Count += count;
            }
            else
            {

                string w_name = count.ToString();
                w = new Wish(type, strength, w_name);
                w.Count = count;
                Wishlet wlet = new Wishlet(w, _getEmptyLabel());
                if (wlet.my_label == null) { Debug.Log("Could not add wish because ran out of inventory slots!\n"); return 0; }
                wlet.my_label.InitWish(w);
                wlet.my_label.SetActive(true);
                wlet.my_label.SetInteractable(true);
                //count++;
                wishes.Add(wlet);
                ((MyWishButton)wlet.my_label.ui_button).setCount(count, true);
            }
            if (onWishChanged != null) onWishChanged(w, true, true, strength);
            //EagleEyes.Instance.UpdateToyButtons("blah", ToyType.Temporary);

            return strength;
        }
    }

    public void OnEnable()
    {
        /*
 last_button = 0;
        foreach (Wishlet w in wishes)
        {
            w.my_label.InitWish(null);
        }
       */
    }
    

}
