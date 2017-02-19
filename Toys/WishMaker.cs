using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class WishMaker: MonoBehaviour {

    private static WishMaker instance;
    public static WishMaker Instance { get; private set; }
    bool am_enabled = false;

    void Awake()
    {
  //      Debug.Log("WishMaker awake\n");
        if (Instance != null && Instance != this)
        {
            Debug.Log("WishMaker got destroyed\n");
            Destroy(gameObject);
        }
        Instance = this;
        HitMe.onMakeWish += onWishMake;
        GameAction.onMakeWish += onWishMake;
        
    }


    public void Init(StatSum stats, Vector3 target)
    {
      
        
    
    }

    void onWishMake(List<Wish> inventory, Vector3 pos)
    {
        
        float random = UnityEngine.Random.Range(0, 1f);

        
        bool make = false;
        Wish e = new Wish();
        string h = "";
        float random_place = 0;
        float strength = 0f;
        for (int i = 0; i < inventory.Count; i++)
        {
            
            WishType t = inventory[i].type;
            if (t == WishType.Null) { continue; }
            e = inventory[i];
            strength = e.Strength;
            h += "init strength " + strength;

            float percent = e.percent * Moon.Instance.getWishSpawnAdjustment(t);
            random_place += percent;
            
            if (random < random_place)
            {
                make = true;
        //        Debug.Log("make a " + e.type + "... " + random_place + " < " + random + ", strength " + strength + "\n");
                if (random < random_place * 1f / 5f)
                {
                    if (t == WishType.Sensible) strength += e.Strength;
                    else strength += e.Strength/2f;
                    //            Debug.Log("Plus 1 (1/4)\n");
                    h += " plus 1 (1/4)";
                }
                if (random < random_place * 1f / 3f)
                {
                    //    Debug.Log("Plus 1 (1/10)\n");
                    if (t == WishType.Sensible) strength += e.Strength;
                    else strength += e.Strength / 2f;
                    h += " plus 1 (1/10)";
                }
                break;
            }
            

        }

        if (make)
        {
            //if (strength > 3)Debug.Log("Want to make wish " + e.type + " strength " + strength + " " + h + "\n") ;

            GameObject wish = Peripheral.Instance.zoo.getObject("Wishes/" + e.type.ToString(), false);
            Effect_Button w = wish.GetComponent<Effect_Button>();
            if (w == null)
            {
          //      Debug.Log("What " + e.type.ToString() + "\n");
                Peripheral.Instance.zoo.returnObject(wish);
            }
        //    Debug.Log("Final strength " + strength + "\n");
            w.Init(e.type, strength); 
            wish.transform.localRotation = Quaternion.identity;
            wish.transform.parent = Wishes.Instance.transform;
            Vector3 p = Get.fixPosition(pos);
            p.z += 3f;
            wish.transform.position = p;
            
            wish.SetActive(true);
        }

    }


}
