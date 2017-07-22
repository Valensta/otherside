using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public enum EffectCloudType { SpecialSkill, PanicAttack };

//basically same as telepoort cloud, should make it generic
public class EffectCloud : Interactable, IPointerClickHandler
{
    float range = 0f;

    private Vector2 mousePos;
    //private Plane plane = new Plane(Vector3.up, Vector3.zero);
    public BoxCollider collider;
    public EffectType effect_type; // 
    public EffectCloudType cloud_type;
    public RuneType rune_type;
    public string attack_lava;
    float lava_life;
    StatSum stats;
  //  bool am_active;
    float initial_delay = 0.05f;
    int level;
    

    void Start()
    {
        Deactivate();
    }

    public override void Simulate(List<Vector2> positions)
    {
        Debug.LogError("Don't know how to simulate EffectCloud yet:(");
    }

    public override void Deactivate()
    {
        
        collider.enabled = false;
        am_active = false;
        if (Monitor.Instance != null) Monitor.Instance.my_spyglass.DisableByDragButton(false);
    }

    public override void Activate(StatBit skill)
    {
        float[] _stats = skill.getStats();
        level = skill.level;
        am_active = true;        
        collider.enabled = true;
        StatBit[] sb = new StatBit[1];

        sb[0] = new StatBit();
        sb[0].effect_type = effect_type;
        sb[0].updateStat(_stats[0]);
        sb[0].dumb = true;
        //range = -s.stat * 2;
        range = _stats[1];
        lava_life = _stats[3];
        if (Monitor.Instance != null) Monitor.Instance.my_spyglass.DisableByDragButton(true);
        stats = new StatSum(1, 0, sb, rune_type);

        
    }



    public void OnPointerClick(PointerEventData eventData)
    {
        if (!am_active) return;
        //Debug.LogError("!!!!! EffectCloud " + rune_type + " " + effect_type + " " + cloud_type + "\n");
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Monitor.Instance != null) Monitor.Instance.my_spyglass.DisableByDragButton(false);

        if (cloud_type == EffectCloudType.SpecialSkill) Peripheral.Instance.my_skillmaster.UseSkill(effect_type);
        if (cloud_type == EffectCloudType.PanicAttack) { Debug.Log("USE PANIC ATTACK WISH (not coded yet)\n"); }

        StartCoroutine(Fire());

        Deactivate();

    }

    public override void Reset()
    {
        StopAllCoroutines();
    }

    IEnumerator Fire()
    {


        yield return new WaitForSeconds(initial_delay);

        Lava lava = Peripheral.Instance.zoo.getObject(attack_lava, false).GetComponent<Lava>();

        lava.SetLocation(this.transform, mousePos, range, Quaternion.identity);
        lava.Init(effect_type, level, stats, lava_life, true, null);
        
        lava.gameObject.SetActive(true);

        Tracker.Log(PlayerEvent.SpecialSkillUsed, true,
        customAttributes: new Dictionary<string, string>() { { "attribute_1", effect_type.ToString() }, { "attribute_2", mousePos.x +"_"+mousePos.y} },
            customMetrics: new Dictionary<string, double>() { { "metric_1", level } });

    }

    


}
