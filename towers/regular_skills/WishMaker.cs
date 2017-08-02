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

 //       Debug.Log("On make Wish\n");
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
            
            percent = (LevelBalancer.Instance.am_enabled)
                ? percent * LevelBalancer.Instance.currentPointPercent
                : percent;
            
            random_place += percent;
            
            if (random < random_place)
            {
                make = true;
                if (t != WishType.Sensible) break;
            
                if (random < random_place * 1f / 10f) strength += e.Strength;                                    
                if (random < random_place * 1f / 5f)  strength += e.Strength;                    
                
                break;
            }
            

        }

        if (make)
        {
            //if (strength > 3)Debug.Log("Want to make wish " + e.type + " strength " + strength + " " + h + "\n") ;

            GameObject wish = Peripheral.Instance.zoo.getObject("Wishes/" + e.type.ToString(), false);
            Effect_Button w = wish.GetComponent<Effect_Button>();
            if (w == null) Peripheral.Instance.zoo.returnObject(wish);            
        

             
            wish.transform.localRotation = Quaternion.identity;
            wish.transform.SetParent(Wishes.Instance.transform);
            
            Vector3 p = Get.fixPosition(pos);
            
            p.z = 5f;
            wish.transform.position = p;
            w.Init(e.type, strength);
            wish.SetActive(true);
        }

    }


}
