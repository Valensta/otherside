using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class Sparkles : Modifier {
	
	public float fire_time; //how often to shoot bursts of sparkles
    public int bullets;
	public GameObject myTarget;
    public bool am_firing;
    Color[] colors = null;
    bool lets_make_sparkles = false;

    float TIME_TO_FIRE;
    StatSum statsum;
    public Firearm my_firearm;

    

    public void initStats(float[] stats, int ID)
    {        
        bullets = Mathf.RoundToInt(stats[0]);
        fire_time = stats[1];
        
        if (!am_firing)
        {
            statsum = new StatSum();
            statsum.towerID = ID;
            statsum.runetype = RuneType.Sensible;
            StatBit[] statbits;

            float finisher_percent = (stats.Length == 4) ? stats[3] : 0;
            if (finisher_percent > 0 && UnityEngine.Random.Range(0, 1) < finisher_percent)
            {
                statbits = new StatBit[3];
                statbits[2].effect_type = EffectType.Sparkles;
                statbits[2].Base_stat = stats[4];
            }
            else
            {
                statbits = new StatBit[2];
            }            
                       
            statbits[0] = new StatBit();
            statbits[0].effect_type = EffectType.Force;
            statbits[1] = new StatBit();
            statbits[1].effect_type = EffectType.Range;
            statbits[1].Base_stat = 2f;
            statsum.stats = statbits;
        }

        //lets_make_sparkles = false;
        statsum.stats[0].Base_stat = stats[2];
        Debug.Log("Initializing sparkles with bullets: " + bullets + " firetime: " + stats[1] + " damage: " + stats[2] + "\n");

        am_firing = true;
        is_active = true;

        if (colors == null) InitColors();        
    }
    

	protected override void YesUpdate () {
        if (!am_firing) return;
        if (lets_make_sparkles) return;
        TIME_TO_FIRE -= Time.deltaTime; 
		if (TIME_TO_FIRE <= 0)
        {
            lets_make_sparkles = true;
        }			
	}

    public void AskSparkles(HitMe hitme)
    {
        if (lets_make_sparkles)
        {
            StartCoroutine(MakeSparkes(hitme));
            lets_make_sparkles = false;
            TIME_TO_FIRE = fire_time;
        }
    }

    
    protected override void SafeDisable() {
        //towers = new List<Firearm>();
        am_firing = false;
        lets_make_sparkles = false;
    }


    IEnumerator MakeSparkes(HitMe hitme) {
        Vector3 from = hitme.gameObject.transform.position;
        
        for (int i = 0; i < bullets; i++)
        {
            float angle = UnityEngine.Random.Range(0, 360f);
            Vector3 target = from;
            Vector2 dir = Get.GetDirection(angle, 2f);
            target += new Vector3(dir.x, dir.y, 0f);          

            Arrow arrow = Get.MakeArrow("Arrows/sparkle_arrow", from, target, statsum, -1, null, false);
            arrow.myFirearm = my_firearm;

            int c = Mathf.FloorToInt(Random.Range(0, 10));
            if (c >= colors.Length) { Debug.Log("Color too big"); c = colors.Length - 1; }
            arrow.sprite_renderer.color = colors[c];
            //arrow.transform.rotation = turn;
            arrow.gameObject.SetActive(true);
            arrow.sourceID = hitme.gameObject.GetInstanceID();
        }
        yield return null;
	}
	
    void InitColors()
    {
        colors = new Color[10];
        colors[0] = Color.white;
        colors[1] = Color.red;
        colors[2] = Color.blue;
        colors[3] = Color.cyan;
        colors[4] = Color.yellow;
        colors[5] = Color.green;
        colors[6] = Color.magenta;
        colors[7] = Color.red;
        colors[8] = Color.green;
        colors[9] = Color.yellow;
    }

}
