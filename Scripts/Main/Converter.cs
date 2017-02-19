using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Converter : Modifier {
    public TransformedProperties before = new TransformedProperties();
    string after;
    float timer;
    bool am_transformed;
    HitMe my_hitme;
    

    public bool Init(HitMe _hitme, float[] stats)
    {
        if (_hitme == null) { Debug.Log("WTF hitme is null\n"); }
        after = "";
        float roll = UnityEngine.Random.Range(0, 1f);
        float current = 0;
     //   Debug.Log("roll is " + roll + " stats: " + stats[0] + " % " + " timer " + stats[1] + "\n");
        timer = stats[1];        

        if (am_transformed) return true;


        float finisher_percent = (stats.Length == 3) ? stats[2] : 0;
        if (finisher_percent > 0 && UnityEngine.Random.RandomRange(0, 1) < finisher_percent)
        {
            after = "whale";
            timer = 99f;
        }
        else
        {
            foreach (TransformTo t in _hitme.transform_to)
            {
                current += t.percent * stats[0];
                //       Debug.Log("transform to " + t.name + " ? need < " + current + "\n");
                if (roll < current)
                {
                    after = t.name;
                    break;
                }
            }
        }


        if (after.Equals("") || !ValidateMe(after))
        {
       //     Debug.Log("Cancelling converter\n");
            return false;
        }
        

        if (my_hitme == null)
        {
            my_hitme = _hitme;
            
            before.sprite = my_hitme.sprite_renderer.sprite;
            before.sprite_size = my_hitme.sprite_renderer.gameObject.transform.localScale;
            getCollider(before);
            before.physics_material = my_hitme.getCollider().sharedMaterial.name;
            before.rotation_interval = my_hitme.my_ai.rotation_interval;
            before.rotation_inverse_speed_factor = my_hitme.my_ai.rotation_inverse_speed_factor;
            before.rotation_lerp_amount = my_hitme.my_ai.rotation_lerp_amount;
            before.speed = my_hitme.my_ai.speed;            
            before.defenses = CloneUtil.copyList<Defense>(my_hitme.init_defenses);
            
        }
        ChangeMe(false);

        return true;
    }

    public void ChangeMe(bool revert)
    {
        am_transformed = !revert;
        is_active = am_transformed;
        TransformedProperties tf = (revert) ? before : getProperties(after);
        setCollider(tf);


        my_hitme.sprite_renderer.sprite = tf.sprite;        
        my_hitme.my_ai.speed = tf.speed;

        my_hitme.my_ai.rotation_interval = tf.rotation_interval;
        my_hitme.my_ai.rotation_inverse_speed_factor = tf.rotation_inverse_speed_factor;
        my_hitme.my_ai.rotation_lerp_amount = tf.rotation_lerp_amount;
        my_hitme.getCollider().sharedMaterial = Get.getPhysicsMaterial(tf.physics_material);

        my_hitme.init_defenses = CloneUtil.copyList<Defense>(tf.defenses);
        my_hitme.defenses = CloneUtil.copyList<Defense>(tf.defenses);
        my_hitme.sprite_renderer.gameObject.transform.localScale = tf.sprite_size;
        Debug.Log("Finished converting " + this.name + " am transformed " + am_transformed + "\n");
    }


    void setCollider(TransformedProperties tf)
    {
        Collider2D collider = my_hitme.getCollider();

        if (collider is BoxCollider2D)
        {
            ((BoxCollider2D)collider).size = tf.collider_size;
        }
        else if (collider is CircleCollider2D)
        {
            
            ((CircleCollider2D)collider).radius = tf.collider_size.magnitude;
        }
        else
        {
            Debug.Log(this.gameObject.name + " has unknown collider type, converter does not know what to do with it:(\n");
        }
   //     Debug.Log("Set collider " + tf.collider_size.ToString() + "\n");
    }

    void getCollider(TransformedProperties tf)
    {
        
        Collider2D collider = my_hitme.getCollider();
        if (collider is BoxCollider2D)
        {
            tf.collider_size = ((BoxCollider2D)collider).size;
        }
        else if (collider is CircleCollider2D)
        {
         //   Debug.Log("Collider " + ((CircleCollider2D)collider).radius + "\n");
            tf.collider_size = ((CircleCollider2D)collider).radius * Vector2.one;
        }
        else
        {
            Debug.Log(this.gameObject.name + " has unknown collider type, converter does not know what to do with it:(\n");
        }

       // Debug.Log("Got collider " + tf.collider_size + "\n");
    }
    
    
    bool ValidateMe(string name)
    {
        return (name.Equals("toad") || name.Equals("stick_figure") || name.Equals("fruitfly") || name.Equals("whale"));
    }

    TransformedProperties getProperties(string name)
    {
        TransformedProperties tp = null;
        switch (name)
        {
            case "toad":
                tp = new TransformedProperties();
                tp.sprite_size = Vector3.one * 0.3432823f;
                tp.sprite = Get.getSprite("GUI/InGame/toad");
                tp.speed = 2.5f;
                tp.defenses = new List<Defense>();
                tp.defenses.Add(new Defense(EffectType.Force, .3f));
                tp.defenses.Add(new Defense(EffectType.Speed, .6f));
                tp.defenses.Add(new Defense(EffectType.VexingForce, .15f));                
                tp.rotation_interval = 0.2f;
                tp.rotation_inverse_speed_factor = 4;
                tp.rotation_interval = 0.05f;
                tp.physics_material = "toad";
                tp.collider_size = Vector2.one * 0.15f;
                
                
                break;
            case "stick_figure":
                tp = new TransformedProperties();
                tp.sprite_size = Vector3.one * 0.3053726f;
                tp.sprite = Get.getSprite("GUI/InGame/gingerbread_man");
                tp.speed = 3.5f;
                tp.defenses = new List<Defense>();
                tp.defenses.Add(new Defense(EffectType.Force, .4f));
                tp.defenses.Add(new Defense(EffectType.Speed, .5f));
                tp.defenses.Add(new Defense(EffectType.VexingForce, .28f));
                tp.rotation_interval = 0.2f;
                tp.rotation_inverse_speed_factor = 4;
                tp.rotation_interval = 0.05f;
                tp.physics_material = "soldier";
                tp.collider_size = Vector2.one * 0.15f;
                
                break;
            case "fruitfly":
                tp = new TransformedProperties();
                tp.sprite_size = Vector3.one * 0.08f;
                tp.sprite = Get.getSprite("GUI/InGame/fruitfly");
                tp.speed = 3.5f;
                tp.defenses = new List<Defense>();
                tp.defenses.Add(new Defense(EffectType.Force, .2f));
                tp.defenses.Add(new Defense(EffectType.Speed, .5f));
                tp.defenses.Add(new Defense(EffectType.VexingForce, .2f));
                tp.rotation_interval = 0.2f;
                tp.rotation_inverse_speed_factor = 4;
                tp.rotation_interval = 0.05f;
                tp.physics_material = "plane";
                tp.collider_size = Vector2.one * 0.15f;

                break;
            case "whale":
                tp = new TransformedProperties();
                tp.sprite_size = Vector3.one * 0.8f;
                tp.sprite = Get.getSprite("GUI/InGame/fruitfly");
                tp.speed = 0.05f;
                tp.defenses = new List<Defense>();
                tp.defenses.Add(new Defense(EffectType.Force, .05f));
                tp.defenses.Add(new Defense(EffectType.Speed, .95f));
                tp.defenses.Add(new Defense(EffectType.VexingForce, .05f));
                tp.rotation_interval = 0.2f;
                tp.rotation_inverse_speed_factor = 4;
                tp.rotation_interval = 0.05f;
                tp.physics_material = "whale";
                tp.collider_size = Vector2.one * 0.35f;

                break;
            default:
                Debug.Log("Coverter does not know properties for " + name + "\n");
                break;
        }
        if (tp != null) tp.defenses.Add(new Defense(EffectType.Transform, 1f));
        return tp;
        
    }


    protected override void SafeDisable()
    {
        if (am_transformed)
        {
            ChangeMe(true);
        }
    }

    protected override void YesUpdate()
    {

        timer -= Time.deltaTime;


        if (am_transformed && timer <= 0)
        {
            SafeDisable();
        }
    }

}

