using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

public class AirAttack : Interactable, IPointerUpHandler, IPointerDownHandler
{

    private Vector3 v3OrgMouse;
    private Plane plane = new Plane(Vector3.up, Vector3.zero);
    public BoxCollider collider;
    public string attack_lava;
    public DrawLine my_line;
    float lava_life;
    List<Lava> lavas;
    int bullets = 5;
    StatSum stats;
    bool am_active;
    float initial_delay = 0f;
    float intra_bullet_delay = 0.1f;

    void Start(){
        Deactivate();
	}

    public override void Deactivate()
    {

        my_line.gameObject.SetActive(false);
        collider.enabled = false;
        am_active = false;
        if (Monitor.Instance != null) Monitor.Instance.my_spyglass.DisableByDragButton(false);
    }

    public override void Activate(float[] _stats)
    {
        am_active = true;
        
    //    Debug.Log("Activating Air Attack!\n");
        my_line.gameObject.SetActive(true);
        collider.enabled = true;
        StatBit[] sb = new StatBit[1];
        sb[0] = new StatBit(EffectType.Force, _stats[0], 1, false);
        //sb[0].updateStat(_stats[0]);
        //sb[0].effect_type = EffectType.Force;
        lava_life = _stats[1];// / 5f;
        //bullets = Mathf.CeilToInt(sb[0].stat * 4f);
        bullets = Mathf.CeilToInt(_stats[2]);
        if (Monitor.Instance != null) Monitor.Instance.my_spyglass.DisableByDragButton(true);
        stats = new StatSum(1, 0, sb, RuneType.Sensible);
        

        

        stats.factor = 1 / (lava_life * (float)bullets);
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
        //   Debug.Log("airattack onenddrag\n");
        if (Monitor.Instance != null) Monitor.Instance.my_spyglass.DisableByDragButton(false);
        Peripheral.Instance.my_skillmaster.UseSkill(EffectType.AirAttack);
        StartCoroutine(Fire());

        Deactivate();
             
    }



    IEnumerator Fire()
    {        

        List<Vector2> pointsList = my_line.getLine();

        List<Vector3> targets = my_line.getFractions(bullets);


   //     Debug.Log("AirAttack got " + targets.Count + " targets\n");

        yield return new WaitForSeconds(initial_delay);
        foreach (Vector3 target in targets)
        {

            Lava lava = Peripheral.Instance.zoo.getObject(attack_lava, false).GetComponent<Lava>();
            lava.SetLocation(this.transform, target, 1, Quaternion.identity);
            //lava.transform.parent = this.transform;
            //lava.transform.position = target;
            //lava.transform.localScale = Vector3.one;
            //lava.transform.localRotation = Quaternion.identity;
            lava.Init(stats, lava_life, true, null);
            lava.gameObject.SetActive(true);
            yield return new WaitForSeconds(intra_bullet_delay);
        }

    }

}