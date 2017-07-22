using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class TinyCloud : Interactable, IPointerClickHandler
{
    float range = 0f;
    public EffectType type;
    private Vector2 mousePos;
    private Plane plane = new Plane(Vector3.up, Vector3.zero);
    public BoxCollider collider;
    public RuneType runeType;
    public string attack_lava;   
    StatSum stats;
    bool am_active;
    float initial_delay = 0.05f;
    int level;

    void Start()
    {
        Deactivate();
    }

    public override void Deactivate()
    {
        
        collider.enabled = false;
        am_active = false;
    }

    public override void Activate(StatBit skill)
    {
        float[] _stats = skill.getStats();
        level = skill.level;
        am_active = true;        
        collider.enabled = true;
        StatBit[] sb = new StatBit[1];

        sb[0] = new StatBit();
        sb[0].effect_type = type;
        sb[0].updateStat(_stats[0]);
        sb[0].dumb = true;
        //range = -s.stat * 2;
        range = _stats[1];

        if (Monitor.Instance != null) Monitor.Instance.my_spyglass.DisableByDragButton(true);
        stats = new StatSum(1, 0, sb, runeType);

       
    }



    public void OnPointerClick(PointerEventData eventData)
    {
        if (!am_active) return;
        Debug.Log("teleport onpointerup\n");
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Monitor.Instance != null) Monitor.Instance.my_spyglass.DisableByDragButton(false);
        Peripheral.Instance.my_skillmaster.UseSkill(EffectType.Teleport);
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
        lava.Init(type, level, stats, 3f, true, null);
        
        lava.gameObject.SetActive(true);


        Tracker.Log(PlayerEvent.SpecialSkillUsed, true,
            customAttributes: new Dictionary<string, string>() { { "attribute_1", type.ToString() }, { "attribute_2", mousePos.x + "_" + mousePos.y } },
            customMetrics: new Dictionary<string, double>() { { "metric_1", level } });
    }



    public override void Simulate(List<Vector2> positions)
    {
        Debug.LogError("Don't know how to simulate TinyCloud yet:(");
    }

}
