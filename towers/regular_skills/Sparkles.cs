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
    StatSum my_statsum;
    public Firearm my_firearm;
    ArrowName arrow_name;


    public void initStats(StatBit skill, int ID)
    {
        float[] stats = skill.getStats();

        bullets = Mathf.RoundToInt(stats[0]);
        fire_time = stats[1];

        //        if (!am_firing)       {
        my_statsum = new StatSum();
        my_statsum.towerID = ID;
        my_statsum.runetype = RuneType.Sensible;
        StatBit[] statbits;

        float finisher_percent = (stats.Length == StaticStat.StatLength(EffectType.Sparkles, true)) ? stats[3] : 0;
        if (finisher_percent > 0 && UnityEngine.Random.Range(0, 1) < finisher_percent)
        {
            statbits = new StatBit[3];
            statbits[2] = new StatBit();
            statbits[2].effect_type = EffectType.Sparkles;
            statbits[2].Base_stat = stats[4];
            statbits[2].level = skill.level;
        }
        else
        {
            statbits = new StatBit[2];
        }

        statbits[0] = new StatBit();
        statbits[0].effect_type = EffectType.Force;
        statbits[1] = new StatBit();
        statbits[1].effect_type = EffectType.Range;
        statbits[1].Base_stat = 1.5f + skill.level / 2f;
        my_statsum.stats = statbits;
        //      }

        //lets_make_sparkles = false;
        my_statsum.stats[0].Base_stat = stats[2];
        my_statsum.stats[0].dumb = true;
        my_statsum.stats[0].very_dumb = true;
        my_statsum.stats[0].level = skill.level;
        //this works like a lava

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

        float walk_angle = hitme.my_ai.forward_direction_angle/(2*Mathf.PI) - Mathf.PI;
        float collider_size = Get.getColliderSize(hitme.my_collider);
        
        for (int i = 0; i < bullets; i++)
        {
            //float angle = UnityEngine.Random.Range(walk_angle - Mathf.PI*.3f, walk_angle + Mathf.PI*.3f);

            float angle = Get.RandomNormal() * (Mathf.PI *.75f) + walk_angle;
            Vector3 target = from;
            Vector2 dir = Get.GetDirection(angle, 2f);

            Vector3 mod_from = from + (new Vector3(dir.x, dir.y, 0f)) * collider_size * 1.1f;

            target += new Vector3(2f*dir.x, 2f*dir.y, 0f);          

            Arrow arrow = Get.MakeArrow(arrow_name, mod_from, target, my_statsum, -1, null, false);
            arrow.myFirearm = my_firearm;

            int c = Mathf.FloorToInt(Random.Range(0, colors.Length));
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
        arrow_name = new ArrowName(ArrowType.Sparkle);
        float a = 0.66f;
        float b = 0.33f;
        colors = new Color[7];
        int i = 0;
        colors[i++] = Color.white;

        colors[i] = Color.red * a + Color.white * b;  colors[i++].a = 1f;
        colors[i] = Color.blue * a + Color.white * b; colors[i++].a = 1f;
        colors[i] = Color.cyan * a + Color.white * b; colors[i++].a = 1f;
        colors[i] = Color.yellow * a + Color.white * b; colors[i++].a = 1f;
        colors[i] = Color.green * a + Color.white * b; colors[i++].a = 1f;
        colors[i] = Color.magenta * a + Color.white * b; colors[i++].a = 1f;
        
    }

}
