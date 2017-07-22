using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using System.Text;
public class AirAttack : Interactable, IPointerUpHandler, IPointerDownHandler
{

    private Vector3 v3OrgMouse;
    //private Plane plane = new Plane(Vector3.up, Vector3.zero);
    public BoxCollider collider;
    public string attack_lava;
    public DrawLine my_line;
    float lava_life;
    List<Lava> lavas;
    int bullets = 5;
    int level;
    StatSum stats;
    
    float initial_delay = 0f;
    float intra_bullet_delay = 0.1f;

    private float draw_line_time; //how long it take ya to draw it, too short, no good cuz it's probably a mistake

    float lava_size = 1.5f;
    void Start(){
        Deactivate();
	}



    public override void Deactivate()
    {
        Debug.Log("KILLING AIR ATTACK\n");
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

        my_firearm = Peripheral.Instance.getHeroFirearm(RuneType.Sensible);


        
        my_line.gameObject.SetActive(true);
        collider.enabled = true;
        StatBit[] sb = new StatBit[1];
        //sb[0] = new StatBit(EffectType.Force, _stats[0], 1, false);
        sb[0] = new StatBit(EffectType.Force, _stats[0], 1, false);
        lava_life = _stats[1];// / 5f;
        sb[0].very_dumb = true;
        bullets = Mathf.CeilToInt(_stats[2]);
        if (Monitor.Instance != null) Monitor.Instance.my_spyglass.DisableByDragButton(true);
        stats = new StatSum(1, 0, sb, RuneType.Sensible);

        


        stats.factor = 1;// / (lava_life * lava_size);
    //    Debug.Log("Activating Air Attack! Damage " + _stats[0] + " factor " + stats.factor + " bullets " + bullets + " lava_life " + lava_life + "\n");

        my_line.clearLine();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!am_active) return;
      //  Debug.Log("airattack onbegindrag\n");
        
        my_line.BeginLine();
    }
    
    

    public void OnPointerUp(PointerEventData eventData) {
        if (!am_active) return;
        my_line.EndLine();
        if (my_line.time_to_draw_line < 0.3f && my_line.getLine().Count < 3)
        {
            Deactivate();
            if (Monitor.Instance != null) Monitor.Instance.my_spyglass.DisableByDragButton(false);
            Peripheral.Instance.my_skillmaster.CancelSkill(EffectType.AirAttack);
            Debug.Log("Line too short, restart\n");
            return;
        }
            

        if (Monitor.Instance != null) Monitor.Instance.my_spyglass.DisableByDragButton(false);
        Peripheral.Instance.my_skillmaster.UseSkill(EffectType.AirAttack);
        StartCoroutine(Fire());

        Deactivate();
             
    }



    IEnumerator Fire()
    {        

        //List<Vector2> pointsList = my_line.getLine();

        List<Vector3> targets = my_line.getFractions(bullets);

        yield return new WaitForSeconds(initial_delay);


        StringBuilder line_string = new StringBuilder(); 

        foreach (Vector3 target in targets)
        {

            Lava lava = Peripheral.Instance.zoo.getObject(attack_lava, false).GetComponent<Lava>();
            
                        
            lava.Init(EffectType.AirAttack, level, stats, lava_life, true,my_firearm);
            lava.SetLocation(this.transform, target, lava_size, Quaternion.identity);
            lava.SetFactor(1f);
            lava.gameObject.SetActive(true);
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

    public override void Simulate(List<Vector2> positions)
    {
        my_line.SimulateAttack(positions);

        my_line.EndLine();
        //   Debug.Log("airattack onenddrag\n");
        if (Monitor.Instance != null) Monitor.Instance.my_spyglass.DisableByDragButton(false);
        Peripheral.Instance.my_skillmaster.UseSkill(EffectType.AirAttack);
        StartCoroutine(Fire());

        Deactivate();
    }

    public override void Reset()
    {
     StopAllCoroutines();
    }
}