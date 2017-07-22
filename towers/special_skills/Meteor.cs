using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;



//basically same as telepoort cloud, should make it generic
public class Meteor : Interactable, IPointerClickHandler
{       
    public BoxCollider collider;
    public string meteor_lava;
    //bool am_active;
    
    int level;
    float lava_life;
    float lava_size = 1.5f;
    StatSum stats;
    private float speed;

    void Start()
    {
        Deactivate();
    }

    public override void Deactivate()
    {
    //    Debug.Log("plaguesummoner deactivating\n");
        collider.enabled = false;
        am_active = false;
        if (Monitor.Instance != null) Monitor.Instance.my_spyglass.DisableByDragButton(false);
    }

    public override void Activate(StatBit skill)
    {
        float[] _stats = skill.getStats();
        level = skill.level;

        my_firearm = Peripheral.Instance.getHeroFirearm(RuneType.Sensible);
     
        am_active = true;
        collider.enabled = true;

        speed = _stats[2];
        lava_size = _stats[4];
        lava_life = _stats[1];// / 5f;

        StatBit[] sb = new StatBit[2];        
        sb[0] = new StatBit(EffectType.Force, _stats[0], 1, false);        
        sb[0].very_dumb = true;
        sb[0].dumb = true;

        sb[1] = new StatBit(EffectType.Stun, speed, 1, false);
        sb[1].very_dumb = true;
        sb[1].dumb = true;

        if (Monitor.Instance != null) Monitor.Instance.my_spyglass.DisableByDragButton(true);
        stats = new StatSum(1, 0, sb, RuneType.Sensible);        
        stats.factor = 1;
        
    }



    public void OnPointerClick(PointerEventData eventData)
    {
        if (!am_active) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Fire(mousePos);

        if (Monitor.Instance != null) Monitor.Instance.my_spyglass.DisableByDragButton(false);
        Peripheral.Instance.my_skillmaster.UseSkill(EffectType.Meteor);

        Deactivate();

        Peripheral.Instance.my_skillmaster.UseSkill(EffectType.Meteor);

    }

    public override void Reset()
    {
        
    }



    bool Fire(Vector3 target)
    {
        target.z = 0f;
        Lava lava = Peripheral.Instance.zoo.getObject(meteor_lava, false).GetComponent<Lava>();
        Debug.Log("making a meteor lava " + meteor_lava + " @ " + target + " size " + lava_size + "\n");

        lava.Init(EffectType.Meteor, level, stats, lava_life, true, my_firearm);
        lava.SetLocation(this.transform, target, lava_size, Quaternion.identity);
        lava.SetFactor(1f);
        lava.gameObject.SetActive(true);
        
        List<HitMe> targets = lava.GetVictims();

        foreach (HitMe victim in targets)
        {
            float distance = Vector2.Distance(target, victim.transform.position);
            float force = 1f + 1 / distance;
            float mass = victim.my_rigidbody.mass;
            Vector3 direction = (victim.transform.position - target).normalized;
            //Debug.Log("FROM " + target + );
            victim.my_rigidbody.AddForce(15f*force * mass * direction , ForceMode2D.Impulse);
            victim.my_ai.Stunned = true;
        }

        Tracker.Log(PlayerEvent.SpecialSkillUsed, true,
customAttributes: new Dictionary<string, string>() { { "attribute_1", EffectType.Meteor.ToString() }, { "attribute_2", target.x + "_" + target.y } },
    customMetrics: new Dictionary<string, double>() { { "metric_1", level } });


        return true;
    }

    

    public override void Simulate(List<Vector2> positions)
    {
        Debug.LogError("Don't know how to simulate Meteor yet:(");
    }

}





