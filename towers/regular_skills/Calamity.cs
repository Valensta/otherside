using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class Calamity : MonoBehaviour {
    public EffectType type;

    Transform my_target;
    public Lava my_lava;
    float TIME;
    float next_time_to_make_new_lava;
    float next_time_to_disable_lava;
    float lava_timer;
    StatSum lava_stats;
    float lava_size;

    void Update()
    {
        if (Time.timeScale == 0) return;
        TIME += Time.deltaTime;

    }


    public void Prepare(StatBit bit, Firearm firearm, EffectType type)
    {
        StatSum stats = firearm.toy.rune.GetStats(false);
        float[] calamity_stats = bit.getStats();

        if (TIME < next_time_to_make_new_lava || (my_lava != null && my_lava.gameObject.activeSelf)) return;

        //2 =make new timer, 3 = lava life
        next_time_to_disable_lava = TIME + calamity_stats[3];
        next_time_to_make_new_lava = TIME + calamity_stats[2];
        
  

        StatBit[] sb = new StatBit[1];
        if (type == EffectType.Foil) //Foil summons EMP lava, EMP does the thing
            sb[0] = new StatBit(EffectType.Foil, calamity_stats[3], 1, false);
        else
            sb[0] = new StatBit(EffectType.Force, calamity_stats[0], 1, false);

        sb[0].very_dumb = true;
        lava_stats = new StatSum(1, 0, sb, RuneType.Airy);

        lava_timer = calamity_stats[3];
        lava_stats.factor = 1;

        //  lava_size = (type == EffectType.Calamity) ? stats.getRange() * calamity_stats[1] / 2f : stats.getRange() * calamity_stats[1];
        //this shit should be handled by StatBit, the lazy bum
        lava_size = calamity_stats[1];
    }
    //CALAMITY
    public void Init(StatBit bit, HitMe target, Firearm firearm, EffectType type)
    {
        Prepare(bit, firearm, type);

        if (type == EffectType.Calamity)
            my_lava = Peripheral.Instance.zoo.getObject("Wishes/calamity_lava", true).GetComponent<Lava>();
        else
            my_lava = Peripheral.Instance.zoo.getObject("Wishes/foil_lava", true).GetComponent<Lava>();

        my_lava.auto_return = true;

        my_lava.my_firearm = firearm;
        my_lava.Init(type, bit.level, lava_stats, lava_timer, true, firearm);

        my_lava.SetLocation(target.transform, target.transform.position, lava_size, Quaternion.identity);
        my_lava.SetFactor(1);

        my_lava.transform.localPosition = Vector3.zero;
        Vector3 pos = my_lava.transform.position;
        pos.z = 3.75f;
        my_lava.transform.position = pos;
        my_lava.updateMyPosition = true;

        target.lavas.Add(my_lava);
    }
    //SWARM
    public void Init(StatBit bit, Transform target, Firearm firearm, EffectType type)
    {
        Prepare(bit, firearm, type);


        my_lava.Init(type, bit.level, lava_stats, lava_timer, true, firearm);
        my_lava.my_firearm = firearm;
        my_lava.SetLocation(target.transform, target.transform.position, lava_size, Quaternion.identity);
        my_lava.SetFactor(1);

        Vector3 pos = my_lava.transform.position;
        pos.z = 3.75f;
        my_lava.transform.position = pos;


        my_lava.gameObject.SetActive(true);


    }
	
	
	
}
