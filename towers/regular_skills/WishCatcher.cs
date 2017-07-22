using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class WishCatcher : Modifier
{
    public HitMe my_hitme;
    public float lifetime;
    float my_time;
    float percent_increase;
    float finisher_percent;

    List<Wish> original_wish_list;

    public float Init(HitMe _hitme, float[] stats)
    {
        percent_increase = Get.getPercent(stats[0]);
        lifetime = stats[1];
        my_time = 0;
        _hitme.EnableVisuals(MonsterType.Wishful, lifetime);

        if (is_active) return percent_increase;

        my_hitme = _hitme;
        is_active = true;
        

       // Debug.Log("initializing wish catcher: percent increase " + percent_increase + " for " + lifetime + "\n");
        original_wish_list = new List<Wish>();

        

        finisher_percent = (stats.Length == StaticStat.StatLength(EffectType.WishCatcher, true)) ? stats[2] : 0;

        bool finisher = (finisher_percent > 0 && UnityEngine.Random.RandomRange(0, 1) < finisher_percent);
        if (finisher) lifetime = stats[1]*2f;//some other stuff as well like add health and other nice wishes

        original_wish_list = CloneUtil.copyList(_hitme.stats.inventory);

        bool have_health = false;
        bool have_damage = false;
        bool have_dreams = false;
        Debug.Log("Wishcatcher " + stats[0] + " -> " + percent_increase + "\n");
        foreach (Wish w in _hitme.stats.inventory)
        {
            w.percent *= (1 + percent_increase);
          //  Debug.Log("Setting " + _hitme.gameObject.name + " " + w.type + " to " + w.strength + "\n");
            if (finisher) {
                w.percent *= (1 + finisher_percent);
                if (w.type == WishType.MoreHealth) have_health = true;
                if (w.type == WishType.MoreDreams) have_dreams = true;
                if (w.type == WishType.MoreDamage) have_damage = true;
            }                        
        }

        if (finisher && !have_health) _hitme.stats.inventory.Add(new Wish(WishType.MoreHealth, 1, finisher_percent));
        if (finisher && !have_dreams) _hitme.stats.inventory.Add(new Wish(WishType.MoreDreams, 0.25f, finisher_percent));
        if (finisher && !have_damage) _hitme.stats.inventory.Add(new Wish(WishType.MoreDamage, 0.25f, finisher_percent));
        
        return percent_increase;


    }



    private void _ReturnToNormal()
    {//probably going overboard with copy
        my_hitme.stats.inventory = CloneUtil.copyList(original_wish_list);
    }

    protected override void SafeDisable()
    {
        _ReturnToNormal();

    }
    protected override void YesUpdate()
    {

        my_time += Time.deltaTime;

        if (my_time > lifetime)
        {

            _ReturnToNormal();

            is_active = false;
            return;
        }

    }
}
