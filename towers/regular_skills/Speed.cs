using UnityEngine;
using System.Collections;

public class Speed : Modifier
{
    public AI my_ai;
    public float lifetime;
    public float time_to_normal;
    float my_time;
    float orig_rotation_inverse_speed_factor = -1;

    bool start;
    
    



    public float Init(HitMe _hitme, float[] stats, EffectType type, EffectSubType sub_type)
    {
        if (_hitme.my_body.amHidden()) return 0;

        my_ai = _hitme.my_ai;
        //    Debug.Log("Stats " + stats[0] + " " + stats[1] + " " + stats[2] + "\n");
        my_time = 0;
        start = false;
        bool was_already_active = is_active;
        is_active = true;
        float xp = 0f;
        if (orig_rotation_inverse_speed_factor == -1)
            orig_rotation_inverse_speed_factor = my_ai.rotation_inverse_speed_factor;

        
        float aff = Get.getPercent(stats[0]);
        //    Debug.Log("SPEED AFF " + stats[0] + " -> " + aff + "\n");

        float original_aff = aff;
        if (aff < 0) aff = 0;
        if (aff > 0.99f) aff = 0.99f;
        bool froze = false;
        if (sub_type == EffectSubType.Freeze)
        {
            float pct_freeze = stats[3];
            float roll = UnityEngine.Random.Range(0f, 100f);
            if (roll < pct_freeze)
            {
                Debug.Log("FREEZING " + my_ai.gameObject.name + " for " + stats[5] + "\n");
                aff = 0f;
                froze = true;
                StatBit[] sb = new StatBit[1];
                lifetime = stats[5];
                time_to_normal = stats[5];

                sb[0] = new StatBit(EffectType.Weaken, stats[4], 1, false);
                sb[0].effect_sub_type = EffectSubType.Freeze;
                sb[0].very_dumb = true;
                sb[0].dumb = true;

                StatSum weaken = new StatSum(3, 0, sb, RuneType.Airy);

                _hitme.HurtMe(weaken, null, EffectType.Null);
            }
        }
        if (froze)
            _hitme.EnableVisuals(MonsterType.Freeze, lifetime);
        else
            _hitme.EnableVisuals(MonsterType.Speed, lifetime);
        

        //Speed
        lifetime = stats[2];
        time_to_normal = stats[1];
        float final = my_ai.speed * aff;


        /*  this makes speed lavas ineffective
        if (was_already_active)
        {
            float current_speed_factor = my_ai.current_speed / my_ai.speed;
            float new_factor = (2 * aff + current_speed_factor) / 3f;
            final = new_factor * my_ai.speed;
            Debug.Log("Speed acting again, current speed factor " + current_speed_factor + " new_factor " + new_factor + " instead of " + aff + "\n");
        }*/


      // 	Debug.Log(my_ai.gameObject.name + " SETTING SPEED FROM (" + original_aff + ") " + my_ai.speed + " TO " + final + " AFF " + aff + " FOR " + lifetime + "\n");
        my_ai.Stunned = true;
        my_ai.current_speed = final;
        xp = lifetime * (my_ai.speed - my_ai.current_speed) / my_ai.speed;

        //   Debug.Log(stats[0] + " " + aff + " time to normal " + time_to_normal + "\n");            
        //Debug.Log("stat " + stats[0] + " aff " + aff + " xp " + xp + " lifetime " + lifetime + " speed " + my_ai.speed + " current_speed " + my_ai.current_speed + "\n");

        

        
        return xp / 10f;
        //return 0;
    }

    protected override void YesUpdate()
    {

        my_time += Time.deltaTime;
        if (!start)
        {
            if (my_time > lifetime)
            {
                start = true;
                my_ai.Stunned = false;
            }
            else { return; }
        }
        if (my_ai.speed - my_ai.current_speed <= 2 * Time.deltaTime / time_to_normal)
        {
            //	Debug.Log("Returning speed to normal from " + my_ai.speed + " to " + init_speed + " cuz <= " + 2*Duration.deltaTime/time_to_normal + "\n");
            _ReturnToNormal();
            is_active = false;
            return;
        }
        my_ai.current_speed += 2 * Time.deltaTime / time_to_normal;
    }

    private void _ReturnToNormal()
    {
        my_ai.current_speed = my_ai.speed;
        my_ai.rotation_inverse_speed_factor = orig_rotation_inverse_speed_factor;
        is_active = false;
        my_ai.Stunned = false;
    }

    private void OnDisable()
    {
        _ReturnToNormal();
    }

    protected override void SafeDisable()
    {
        _ReturnToNormal();

    }

}