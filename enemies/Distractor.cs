using UnityEngine;
using System.Collections;



public enum DistractorType{ Decoy, Monster, Shield, Null }

public class Distractor : Modifier
{
    public int number;          // make this many at a time
    public int max_number = -1; // up to this many active at one time (for Decoy and Monster)
    public float period;
    public float interval; //interval between each decoy ie, - - -         - - -          - - -
    public float how_far;  //                                 |interval         |  --period--  |
    public float init_velocity;
    float last_time;
    float TIME;
    public string decoy = "Monsters/Helpers/default_decoy";
    public DistractorType type;
    public HitMe my_hitme;
    float how_many_times = 99;
    public float init_how_many_times = 99;
    public Vector3 decoy_position = Vector3.zero;
    public Decoy my_shield;
    public bool upon_death = false;
    public bool one_level = true; //if the spawned has a distractor, disable it. only doing one level
    

    private MyArray<DistractorObject> decoys;
    
    void Start() {
        if (interval * number > period / 2f) {
            interval = period / (2f * number);
        }

    }

    void OnEnable() {
        last_time = 0f;
        TIME = 0f;
        how_many_times = init_how_many_times;
        decoys = new MyArray<DistractorObject>();
    }



    // Update is called once per frame
    protected override void YesUpdate() {
        if (upon_death) return;
        if (how_many_times < 1) { return; }

        if (disabled_timer > 0 && type == DistractorType.Shield && (bool)my_shield?.gameObject.activeSelf )
        {
            my_shield.current_lives = 0;
        }


        TIME += Time.deltaTime;

        if (TIME - last_time > period) {
            last_time = TIME;
            StartCoroutine("MakeDecoys");
        }


    }

    protected override void SafeDisable() {
        StopAllCoroutines();
    }


    IEnumerator MakeDecoys(){
        
        how_many_times --;
		int count = 0;
        int adjusted_number = getAdjustedNumberOfDecoys();
        
		while (count < adjusted_number){
            if (Time.timeScale == 0) yield return new WaitForSeconds(interval);
            RandomLaunchDecoy();
			count++;
			
		}
		yield return null;
	}

    void MakeDecoysRightNow()
    {
        int adjusted_number = getAdjustedNumberOfDecoys();
        
        int count = 0;
        while (count < adjusted_number)
        {
            RandomLaunchDecoy();
            count++;     
        }
    }

    int getAdjustedNumberOfDecoys()
    {
        if (max_number <= 0|| type == DistractorType.Shield) return number;
        int have = decoys.cleanInactiveAndGetCount();
        int ok = Mathf.Min(number, max_number - have);
        
        Debug.Log($"{gameObject.name} has {have} decoys, making {ok}\n");
        return ok;
    }

    public override void DisableAction()
    {
      //  Debug.Log("Stuff\n");
        if (upon_death && disabled_timer <= 0)
            MakeDecoysRightNow();
        
    }

    void RandomLaunchDecoy()
    {

        float angle = UnityEngine.Random.Range(0, 1f);
        float rad = angle * 2 * Mathf.PI;

        Vector3 direction = Get.GetDirection(rad, how_far);
        Vector3 pos = this.transform.position + direction;

        my_hitme.setVisuals(MonsterType.SummonDecoy, 0.5f, true);
        GameObject obj;

        switch (type)
        {

            case DistractorType.Decoy:
                pos.z++;
                obj = Peripheral.Instance.zoo.getObject(decoy, false);
                float degrees = angle * 360f;
                obj.transform.localEulerAngles = new Vector3(0, 0, degrees);
                obj.transform.position = this.transform.position;

                obj.name = obj.name + Peripheral.Instance.IncrementMonsterCount();

                obj.transform.parent = this.transform.parent;
                obj.SetActive(true);
                obj.GetComponent<Rigidbody2D>().velocity = direction * init_velocity * 2f;
                
                if (max_number > 0) decoys.addByID(obj.GetComponent<Decoy>());
                break;
            case DistractorType.Monster:
                Moon.Instance.enemySpawned(my_hitme.my_ai.my_dogtag.wave);
                
                HitMe hey = Peripheral.Instance.makeMonster(decoy, my_hitme.my_ai.path, pos, 0f, 0f);

                if (one_level)
                {
                    foreach (Modifier tech in hey.enemy_tech)
                    {
                        if (tech.GetType() == typeof(Distractor)) tech.setActive(false);
                    }
                }
                hey.my_ai.my_dogtag.wave = my_hitme.my_ai.my_dogtag.wave;
                if (max_number > 0) decoys.addByID(hey);
                break;
            case DistractorType.Shield:
                decoy_position.z = 1f;

                if (my_shield == null)
                {
                    my_shield = Peripheral.Instance.zoo.getObject(decoy, false).GetComponent<Decoy>();
                    my_shield.transform.parent = this.transform;
                    my_shield.transform.localPosition = decoy_position;
                    my_shield.transform.localRotation = Quaternion.identity;
                }

                my_shield.Init(DistractorType.Shield, my_hitme.my_body);
                my_shield.gameObject.SetActive(true);
                break;
        }
    }
}
