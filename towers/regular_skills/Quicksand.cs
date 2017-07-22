using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using System.Text;
public class Quicksand : Interactable, IPointerUpHandler, IPointerDownHandler
{
    
    private Plane plane = new Plane(Vector3.up, Vector3.zero);
    public BoxCollider collider;
    public string attack_lava;
    public DrawLine my_line;
    float lava_life;
    List<Lava> lavas;
    int bullets = 1;
    StatSum stats;
    int level = 0;
    
    float initial_delay = 0.05f;
    float intra_bullet_delay = 0.1f;
    float lava_size;
    void Start(){
        Deactivate();
	}

    public override void Simulate(List<Vector2> positions)
    {
        my_line.SimulateAttack(positions);
    }

    public override void Deactivate()
    {

        my_line.gameObject.SetActive(false);
        collider.enabled = false;
        am_active = false;
        if (Monitor.Instance != null) Monitor.Instance.my_spyglass.DisableByDragButton(false);
    }

    public override void Activate(StatBit skill)
    {
        float[] _stats = skill.getStats();
        level = skill.level;
        am_active = true;
        my_firearm = Peripheral.Instance.getHeroFirearm(RuneType.Airy);
            
        my_line.gameObject.SetActive(true);
        collider.enabled = true;
        StatBit[] sb = new StatBit[2];


        sb[0] = new StatBit(EffectType.Speed, _stats[0], 1, false);
        sb[0].effect_sub_type = EffectSubType.Freeze;
        sb[0].very_dumb = true;
        sb[0].dumb = true;      

        sb[1] = new StatBit(EffectType.Force, _stats[1], 1, false);
        sb[1].very_dumb = true;
        sb[1].dumb = true;        

        Debug.Log("Activating Quicksand! Speed " + _stats[0] + " force " + _stats[1] + "\n");

        if (Monitor.Instance != null) Monitor.Instance.my_spyglass.DisableByDragButton(true);
        stats = new StatSum(3, 0, sb, RuneType.Airy);
    //stats doesn't have a factor because of speed
        bullets = Mathf.CeilToInt(_stats[3]);
        stats.factor = 1;
        lava_life = _stats[2];
        lava_size = _stats[5];
        my_line.clearLine();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!am_active) return;
     //   Debug.Log("airattack onbegindrag\n");

        my_line.BeginLine();
    }
    
    

    public void OnPointerUp(PointerEventData eventData) {
        if (!am_active) return;
        my_line.EndLine();

        /*
        if (my_line.time_to_draw_line < 0.3f && my_line.getLine().Count < 3)
        {
            Deactivate();
            if (Monitor.Instance != null) Monitor.Instance.my_spyglass.DisableByDragButton(false);
            Peripheral.Instance.my_skillmaster.CancelSkill(EffectType.Quicksand);
            Debug.Log("Line too short, restart\n");
            return;
        }
        */
        if (Monitor.Instance != null) Monitor.Instance.my_spyglass.DisableByDragButton(false);
        Peripheral.Instance.my_skillmaster.UseSkill(EffectType.Frost);
        StartCoroutine(Fire());

        Deactivate();
             
    }

    public override void Reset()
    {
        StopAllCoroutines();
    }

    IEnumerator Fire()
    {        



        List<Vector3> targets = my_line.getFractions(bullets);
        
        StringBuilder line_string = new StringBuilder();
        yield return new WaitForSeconds(initial_delay);
        foreach (Vector3 target in targets)
        {

            Lava lava = Peripheral.Instance.zoo.getObject(attack_lava, false).GetComponent<Lava>();
            
            lava.SetLocation(this.transform, target, lava_size, Quaternion.identity);
            lava.Init(EffectType.Frost, level, stats, lava_life, true, null);
            lava.gameObject.SetActive(true);
            lava.SetFactor(1);

            line_string.Append("|");
            line_string.Append(target.x);
            line_string.Append("_");
            line_string.Append(target.y);

            yield return new WaitForSeconds(intra_bullet_delay);
        }

        Tracker.Log(PlayerEvent.SpecialSkillUsed, true,
            customAttributes: new Dictionary<string, string>() { { "attribute_1", EffectType.AirAttack.ToString() }, { "attribute_2", line_string.ToString() } },
            customMetrics: new Dictionary<string, double>() { { "metric_1", level } });

    }

}