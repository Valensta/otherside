using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Converter : Modifier {
    public TransformedProperties before = new TransformedProperties();
    TransformType after;
    float timer;
    bool am_transformed;
    HitMe my_hitme;
    


    public bool Init(HitMe _hitme, float[] stats, float factor)
    {
        if (_hitme == null) { Debug.Log("WTF hitme is null\n"); }
        after = TransformType.Null;
        float roll = UnityEngine.Random.Range(0, 1f);
        float current = 0;
    //    Debug.Log("roll is " + roll + " stats: " + stats[0] + " % " + " timer " + stats[1] + "\n");
        timer = stats[1] / factor;        

        if (am_transformed) return true;


        float finisher_percent = (stats.Length == StaticStat.StatLength(EffectType.Transform, true)) ? stats[2]/100f : 0;
        if (finisher_percent > 0 && UnityEngine.Random.RandomRange(0, 1) < finisher_percent)
        {
            after = TransformType.Whale;
            timer = 99f;
        }
        
        else
        {
            bool flying = (_hitme.gameObject.layer == Get.flyingProjectileLayer); 
            
            if (roll < stats[0])                
                after = (flying)? TransformType.FruitFly :  TransformType.StickFigure;        
        }


        if (!ValidateMe(after))
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
            before.defenses = CloneUtil.copyList<Defense>(my_hitme.stats.defenses);
            before.linear_drag = my_hitme.my_rigidbody.drag;
            before.angular_drag = my_hitme.my_rigidbody.angularDrag;
        }
        ChangeMe(false);

        return true;
    }

    public void AvgDefenses(List<Defense> from, List<Defense> to, float from_gets)
    {
        for (int i = 0; i < from.Count; i++) {
            float defense_to = Get.GetDefense(to, from[i].type);
            if (defense_to > 0) from[i].strength = (from[i].strength * from_gets + defense_to * (1 - from_gets));
        }
    }

    public void ChangeMe(bool revert)
    {
        am_transformed = !revert;
        is_active = am_transformed;
        TransformedProperties tf = (revert) ? before : getProperties(after);
        setCollider(tf);


        my_hitme.sprite_renderer.sprite = tf.sprite;        
        my_hitme.my_ai.speed = tf.speed;
        my_hitme.my_ai.current_speed = tf.speed;
        my_hitme.my_ai.rotation_interval = tf.rotation_interval;
        my_hitme.my_ai.rotation_inverse_speed_factor = tf.rotation_inverse_speed_factor;
        my_hitme.my_ai.rotation_lerp_amount = tf.rotation_lerp_amount;
        my_hitme.getCollider().sharedMaterial = Get.getPhysicsMaterial(tf.physics_material);

        
        if (revert)
        {
            my_hitme.init_defenses = CloneUtil.copyList<Defense>(tf.defenses);
            my_hitme.defenses = CloneUtil.copyList<Defense>(tf.defenses);
        }
        else
        {
            AvgDefenses(my_hitme.init_defenses, tf.defenses, 0.5f);
            AvgDefenses(my_hitme.defenses, tf.defenses, 0.5f);
        }     

        my_hitme.sprite_renderer.gameObject.transform.localScale = tf.sprite_size;
        my_hitme.my_rigidbody.drag = tf.linear_drag;
        my_hitme.my_rigidbody.angularDrag = tf.angular_drag;
        my_hitme.my_ai.AmHit();
        my_hitme.my_ai.ForceVelocity();

      //  Debug.Log("Finished converting " + this.name + " am transformed " + am_transformed + "\n");
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
          //  Debug.Log(this.gameObject.name + " has unknown collider type, converter does not know what to do with it:(\n");
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
            //Debug.Log(this.gameObject.name + " has unknown collider type, converter does not know what to do with it:(\n");
        }

       // Debug.Log("Got collider " + tf.collider_size + "\n");
    }
    
    
    bool ValidateMe(TransformType transform)
    {
        return transform != TransformType.Null;
        //return (name.Equals("toad") || name.Equals("stick_figure") || name.Equals("fruitfly") || name.Equals("whale"));
    }

    TransformedProperties getProperties(TransformType transform)
    {
        TransformedProperties tp = null;
        switch (transform)
        {
            case TransformType.Toad:
                tp = new TransformedProperties();
                tp.sprite_size = Vector3.one * 0.3432823f;
                tp.sprite = Get.getSprite("GUI/InGame/toad");
                tp.speed = 3.5f;
                tp.defenses = new List<Defense>();
                tp.defenses.Add(new Defense(EffectType.Force, 0f));
                tp.defenses.Add(new Defense(EffectType.Magic, 0f));               
                tp.rotation_interval = 0.2f;
                tp.rotation_inverse_speed_factor = 4;
                tp.rotation_interval = 0.05f;
                tp.physics_material = "toad";
                tp.collider_size = Vector2.one * 0.15f;
                tp.linear_drag = 3f;
                tp.angular_drag = 1f;

                break;
            case TransformType.StickFigure:
                tp = new TransformedProperties();
                tp.sprite_size = Vector3.one * 0.3053726f;
                tp.sprite = Get.getSprite("GUI/InGame/gingerbread_man");
                tp.speed = 3.5f;
                tp.defenses = new List<Defense>();
                tp.defenses.Add(new Defense(EffectType.Force, 0f));
                tp.defenses.Add(new Defense(EffectType.Magic, 0f));                
                tp.rotation_interval = 0.2f;
                tp.rotation_inverse_speed_factor = 4;
                tp.rotation_interval = 0.05f;
                tp.physics_material = "soldier";
                tp.collider_size = Vector2.one * 0.15f;
                tp.linear_drag = 5f;
                tp.angular_drag = 4f;
                break;
            case TransformType.FruitFly:
                tp = new TransformedProperties();
                tp.sprite_size = Vector3.one * 0.08f;
                tp.sprite = Get.getSprite("GUI/InGame/fruitfly");
                tp.speed = 3f;
                tp.defenses = new List<Defense>();
                tp.defenses.Add(new Defense(EffectType.Force, 0f));
                tp.defenses.Add(new Defense(EffectType.Magic, 0f));                
                tp.rotation_interval = 0.2f;
                tp.rotation_inverse_speed_factor = 4;
                tp.rotation_interval = 0.05f;
                tp.physics_material = "fruitfly";
                tp.collider_size = Vector2.one * 0.25f;
                tp.linear_drag = 5f;                
                tp.angular_drag = 8f;
                break;
            case TransformType.Whale:
                tp = new TransformedProperties();
                tp.sprite_size = Vector3.one * 0.8f;
                tp.sprite = Get.getSprite("GUI/InGame/whale");
                tp.speed = 0.01f;
                tp.defenses = new List<Defense>();
                tp.defenses.Add(new Defense(EffectType.Force, 0f));
                tp.defenses.Add(new Defense(EffectType.Magic, 0f));                
                tp.rotation_interval = 0.2f;
                tp.rotation_inverse_speed_factor = 4;
                tp.rotation_interval = 0.05f;
                tp.physics_material = "whale";
                tp.collider_size = Vector2.one * 0.35f;
                tp.linear_drag = 10f;                
                tp.angular_drag = 0f;
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

