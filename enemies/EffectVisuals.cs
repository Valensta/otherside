using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public enum MonsterType{Null, Speed, Mass, SummonDecoy, Weaken,Teleport, Burning, Fear, Wishful, Plague, Regeneration, Freeze, DOT}; 

public class EffectVisuals : MonoBehaviour {
    
	[SerializeField]
	public AuraType[] auras;
	public AuraType default_aura;    
    public bool auto_stabilize;
    private bool am_enabled; 
    int auras_count = 0;
    float timer = 0f;
	// Use this for initialization
	void Start () {
        //Debug.Log("Disabling sprites for effectvisual\n");
        DisableAll();
	    
	}

	public void Enable(MonsterType type, float time){
	    am_enabled = true;
	    setRotation();
        AuraType aura = GetAura(type);	   
        if (aura == null) { return;  }
		Set (aura, time, true, false);
        
	}

    public void setRotation()
    {
        if (!auto_stabilize || !am_enabled) return;
        transform.rotation = Quaternion.identity;
    }
    
    public void DisableAll()
    {
        foreach (AuraType a in auras)
        {
            Set(a, 0, false, true);
        }
    }

    public void Set(AuraType auratype, float time, bool on, bool instantly)
    {
        
        auratype.timer = time;
        if (on && on == auratype.am_running) return;

        float alpha = (on) ? 1f : 0f;
        auratype.am_running = on;

        if (instantly)
        {
            Show.SetAlpha(auratype.aura, alpha);
            auratype.aura.gameObject.SetActive(on);
        }
        else {
            StartCoroutine(_Set(auratype, true, alpha));
            return;
        }
    }

    IEnumerator _Set(AuraType auratype, bool on, float alpha)
    {
        float how_fast = 0.2f;

        if (on) auratype.aura.gameObject.SetActive(on);
        LeanTween.cancel(auratype.aura.gameObject);
        LeanTween.alpha(auratype.aura.gameObject, alpha, how_fast).setEase(LeanTweenType.easeInQuad);

        if (!on)
        {
            yield return new WaitForSeconds(how_fast);
            auratype.aura.gameObject.SetActive(on);
        }

        yield return null;


    }

    void OnEnable()
    {
        auras_count = auras.Length;
        foreach (AuraType a in auras)
        {
            a.aura.color = new Color(a.aura.color.r, a.aura.color.g, a.aura.color.b, 0);
        }
    }

    IEnumerator DisableAura(AuraType auratype, float time)
    {
        while (time > 0)
        {
            time -= 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
        //s.enabled = false;
        auratype.aura.gameObject.SetActive(false);
    }

    public void Die()
    {
        StopAllCoroutines();
        DisableAll();
    }

    public void Update()
    {
        if (Time.timeScale == 0) return;

        timer += Time.deltaTime;
        if (timer <= 0.25f) return;

        am_enabled = false;
        for (int i = 0; i < auras_count; i++)
        {
            AuraType aura = auras[i];
            if (!aura.am_running) continue;
            aura.timer -= timer;
            am_enabled = true;
            if (aura.timer < 0) Set(aura, 0, false, false);
        }

        timer = 0f;
    }


   


    AuraType GetAura(MonsterType type)
    {
        AuraType aura = default_aura;
        for (int i = 0; i < auras_count; i++)
        {
            if (auras[i].type == type) aura = auras[i];
        }
        return aura;
    }
	
}

[System.Serializable]
public class AuraType{
	public MonsterType type;
	public SpriteRenderer aura;
    public Color color;
    public float timer;
    public bool am_running;
    
}