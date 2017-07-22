using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;



//basically same as telepoort cloud, should make it generic
public class PlagueSummoner : Interactable, IPointerClickHandler
{       
    public BoxCollider collider;
  //  bool am_active;
    float[] stats;
    int level;

    void Start()
    {
        Deactivate();
    }

    public override void Deactivate()
    {
    //    Debug.Log("plaguesummoner deactivating\n");
        collider.enabled = false;
        am_active = false;
    }

    public override void Activate(StatBit skill)
    {
        float[] _stats = skill.getStats();
        level = skill.level;

        my_firearm = Peripheral.Instance.getHeroFirearm(RuneType.Airy);
        stats = _stats;
        am_active = true;
        collider.enabled = true;
    //    Debug.Log("plaguesummoner activated\n");
    }



    public void OnPointerClick(PointerEventData eventData)
    {
        if (!am_active) return;

        if (Fire())
        {
            Peripheral.Instance.my_skillmaster.UseSkill(EffectType.Plague);
        }else
        {
            Peripheral.Instance.my_skillmaster.CancelSkill(EffectType.Plague);
            Noisemaker.Instance.Click(ClickType.Error);
        }
        Deactivate();

    }

    
    bool Fire()
    {
        MyArray<HitMe> enemies = Peripheral.Instance.targets;
        int how_many = Mathf.Min(Mathf.FloorToInt(stats[0]), enemies.max_count);
        Debug.Log("PlagueSummoner how many? " + how_many + "\n");
        if (how_many < 1) return false;
        int[] selected_ids = new int[how_many];

        int current = 0;
        int max_tries = 100;
        
        while (current < how_many && max_tries > 0)
        {
            int id = Mathf.FloorToInt(UnityEngine.Random.RandomRange(0, enemies.max_count));
            while (max_tries > 0 && (enemies.array[id] == null || enemies.array[id].amDying() || !enemies.array[id].gameObject.activeSelf || already_selected(selected_ids, id)))
            {
                id = Mathf.FloorToInt(UnityEngine.Random.RandomRange(0, enemies.max_count));
                max_tries--;
            }
            selected_ids[current] = id;
            current++;
        }
  //      Debug.Log("plague selected " + selected_ids.Length + " victims\n");
        foreach (int i in selected_ids)
        { //1 how many;  2 min % damage; 3 max % damage; 4 % speed and defense decrease
            HitMe victim = enemies.array[i];
            if (victim == null) continue;
            StatSum sum = new StatSum();
            sum.runetype = RuneType.Airy;
            sum.level = -1;

            StatBit[] sb = new StatBit[3];
            sum.stats = sb;
            
            float percent = getDamage();
            float damage = Get.bullshit_damage_factor * percent * victim.GetInitMass()/2f;
            Debug.Log("Plague hurting victim " + victim.gameObject.name + " with " + victim.GetInitMass() + " init mass for " + percent + " healthl, damage: " + damage + "\n");

            sb[0] = new StatBit();
            sb[0].effect_type = EffectType.DirectDamage; //% time
            sb[0].very_dumb = true;
            sb[0].dumb = true;
            sb[0].updateStat(damage);

            sb[1] = new StatBit();
            sb[1].effect_type = EffectType.Speed;//%, time to normal, lifetime
            sb[1].Level = -1;
            sb[1].very_dumb = true;
            sb[1].dumb = true;
            sb[1].updateStat(stats[3]);

            sb[2] = new StatBit(); 
            sb[2].effect_type = EffectType.Weaken;//%, time
            sb[2].Level = -1;
            sb[2].very_dumb = true;
            sb[2].dumb = true;
            sb[2].updateStat(stats[3]);
            
            victim.EnableVisuals(MonsterType.Plague, 2f);
            victim.HurtMe(sum, my_firearm, EffectType.Plague, level);
            
        }

        Tracker.Log(PlayerEvent.SpecialSkillUsed, true,
            customAttributes: new Dictionary<string, string>() { { "attribute_1", EffectType.Plague.ToString() } },
            customMetrics: new Dictionary<string, double>() { { "metric_1", level } });


        //  Central.Instance.my_spyglass.DisableByDragButton(true);
        return true;
    }
    public override void Reset()
    {
     
    }

    float getDamage()
    {
        return UnityEngine.Random.RandomRange(stats[1], stats[2]);
    }

    bool already_selected(int[] list, int me)
    {
        foreach (int i in list)
        {
            if (me == i) return true;
        }
        return false;
    }

    public override void Simulate(List<Vector2> positions)
    {
        Debug.LogError("Don't know how to simulate Plague yet:(");
    }

}





