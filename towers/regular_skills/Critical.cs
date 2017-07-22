using UnityEngine;


public class Critical: MonoBehaviour {

    float min_multiplier;
    float max_multiplier;
    float percent_hit;
    float base_mass;
    float finisher_percent;
	
	public void Init(float[] stats, float mass){
        min_multiplier = stats[0];
        max_multiplier = stats[1];
        percent_hit = stats[2];
        base_mass = mass;

        finisher_percent = (stats.Length == StaticStat.StatLength(EffectType.Critical, true)) ? stats[3] : 0;
        

    }
		
	public float getCriticalForce()
    {
        float random = UnityEngine.Random.Range(0, 1f);
      //  Debug.Log("Random " + random + " < " + percent_hit + "?");
        if (random > percent_hit) return 0;


        float mult_random = UnityEngine.Random.Range(min_multiplier, max_multiplier);
        //   Debug.Log("Critical damage mult is " + mult_random + "\n");
        if (finisher_percent > 0 && UnityEngine.Random.RandomRange(0, 1) < finisher_percent)
        {
            mult_random = 999f;
        }
        Debug.Log("Critical mult_random " + mult_random + " min " + min_multiplier + " max " + max_multiplier + " percent " + percent_hit + "\n");
        return mult_random;
    }
		

}
