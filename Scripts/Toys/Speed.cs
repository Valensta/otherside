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

    bool stun;



    public float Init(HitMe _hitme, float[] stats, bool _stun)
    {
        my_ai = _hitme.my_ai;
    //    Debug.Log("Stats " + stats[0] + " " + stats[1] + " " + stats[2] + "\n");
        my_time = 0;
        start = false;
        bool was_already_active = is_active;
        is_active = true;
        float xp = 0f;
        if (orig_rotation_inverse_speed_factor == -1) orig_rotation_inverse_speed_factor = my_ai.rotation_inverse_speed_factor;
        stun = _stun;
        //float aff = stats[0];
        //float _lifetime = stats[1];
        float aff = (1 - stats[0]);
        if (stun)
        {
            //Stun
            my_ai.Stunned = true;
            //	Debug.Log("Stun aff " + aff + " lifetime " + _lifetime +  "\n");
            my_ai.current_speed = my_ai.speed * aff;
            my_ai.rotation_inverse_speed_factor = my_ai.rotation_inverse_speed_factor / 2f;
            lifetime = stats[2];
            time_to_normal = stats[1];
            xp = time_to_normal * (my_ai.speed - my_ai.current_speed) / my_ai.speed;
        }
        else {
            //Speed
            lifetime = stats[2];
            time_to_normal = stats[1];
            float final = my_ai.speed * aff;
            if (was_already_active) {                
                float current_speed_factor = my_ai.current_speed / my_ai.speed;
                float new_factor = (2* aff + current_speed_factor) / 3f;
                final = new_factor * my_ai.speed;
             //   Debug.Log("Speed acting again, current speed factor " + current_speed_factor + " new_factor " + new_factor + " instead of " + aff + "\n");
            }
            //	Debug.Log(my_ai.gameObject.name + " SETTING SPEED FROM " + my_ai.speed + " TO " + final + " AFF " + aff + " FOR " + lifetime + "\n");
            my_ai.Stunned = true;
            my_ai.current_speed = final;
            xp = lifetime * (my_ai.speed - my_ai.current_speed) / my_ai.speed;
        }

        _hitme.EnableVisuals(MonsterType.Speed, lifetime);
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
            }
            else { return; }
        }
        if (my_ai.speed - my_ai.current_speed <= 2 * Time.deltaTime / time_to_normal)
        {
            //	Debug.Log("Returning speed to normal from " + my_ai.speed + " to " + init_speed + " cuz <= " + 2*Time.deltaTime/time_to_normal + "\n");
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

        my_ai.Stunned = false;
    }

    protected override void SafeDisable()
    {
        _ReturnToNormal();

    }

}