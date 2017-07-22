using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;


//basically same as telepoort cloud, should make it generic
public class SentientCloud : Interactable, IPointerClickHandler
{

    public BoxCollider collider;
    int how_many;
    float range = 0f;
    public List<Cluster> clusters = new List<Cluster>();
    List<object_distance> distances = new List<object_distance>();    
    public EffectType skill_effect_type; // 
    public EffectType lava_effect_type; // 
    public EffectCloudType cloud_type;
    public RuneType rune_type;
    public string attack_lava;
    float lava_life;
    StatSum stats;
    
    bool am_running;//is currently running
    float initial_delay = 0.05f;
    Vector3 start_from = Vector3.zero;
    public string my_hero;
    List<Lava> active_lavas = new List<Lava>();
    float TIMER = 0f;
    float UPDATE_LAVA_TIMER = 0f;
    float update_lava_time = 0.5f;
    int level;

    void Start()
    {
        Deactivate();
    }
    
    public override void Deactivate()
    {
        am_active = false;
        collider.enabled = false;
    }

    public void TurnMeOff()
    {
        am_active = false;
        am_running = false;
        Debug.Log("Deactivating sentient cloud\n");
        clusters = new List<Cluster>();
        UPDATE_LAVA_TIMER = 0f;
        active_lavas = new List<Lava>();
        start_from = Vector3.zero;
    }

    public override void Activate(StatBit skill)
    {
        float[] _stats = skill.getStats();
        level = skill.level;

        collider.enabled = true;
        my_firearm = Peripheral.Instance.getHeroFirearm(RuneType.Vexing); //only used for bees right now
        if (start_from == Vector3.zero)
        {
            Firearm f = Peripheral.Instance.getHero(rune_type);
            if (f == null) {

                Peripheral.Instance.my_skillmaster.CancelSkill(skill_effect_type);
                Noisemaker.Instance.Click(ClickType.Error);
                Debug.Log("Cannot use Sentient Cloud skill " + skill_effect_type + " " + rune_type + ", no appropriate hero found\n");
                return;
            }
            start_from = f.transform.position;
        }

        am_active = true;             
        StatBit[] sb = new StatBit[1];

        how_many = Mathf.FloorToInt(_stats[0]);
        
        sb[0] = new StatBit();
        sb[0].effect_type = lava_effect_type;
        sb[0].updateStat(_stats[1]);
        sb[0].dumb = true;
        sb[0].very_dumb = true;
        //range = -s.stat * 2;
        range = _stats[2];
        lava_life = _stats[3];
        //bullets = 0, damage = 1, range = 2, lava kife = 3
        
        stats = new StatSum(1, 0, sb, rune_type);

        Tracker.Log(PlayerEvent.SpecialSkillUsed, true,
          customAttributes: new Dictionary<string, string>() { { "attribute_1", EffectType.Bees.ToString() }},
          customMetrics: new Dictionary<string, double>() { { "metric_1", level } });
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!am_active) return;
        if (cloud_type == EffectCloudType.SpecialSkill) Peripheral.Instance.my_skillmaster.UseSkill(skill_effect_type);
        Debug.Log("Activated sentientcloud, need " + how_many + " clusters " + "\n");
        SummonCloud();
        am_running = true;
        Deactivate();
    }

    public void SummonCloud()
    {
        if (!am_active) return;


        PickClusters();
        


        Fire();
        TIMER = lava_life;
        UPDATE_LAVA_TIMER = update_lava_time;
        Deactivate();

    }


    void PickClusters()
    {
        Transform t = Peripheral.Instance.monsters_transform;
        clusters = new List<Cluster>();


        if (t.childCount == 0) { Debug.Log("sentient cloud found no enemies, will move in random directions\n"); }
        if (t.childCount <= clusters.Count) { Debug.Log("sentient cloud found few enemies, doing simple distribution of clouds");
            int i = 0;
            foreach (Cluster c in clusters)
            {
                c.addObject(t.GetChild(i));
                i++;
                if (i <= t.childCount) i = 0;
            }

            return;
        }


        List<Transform> active_objects = new List<Transform>();
        foreach (Transform a in t)
        {
            if (a.gameObject.activeSelf) active_objects.Add(a);
        }

        int total_clusters = how_many;
        //pick a random dude
        int centerID;
        Transform first = active_objects[Mathf.FloorToInt(UnityEngine.Random.Range(0, active_objects.Count))];
        centerID = first.gameObject.GetInstanceID();
      //  centers.Add(first.position);
        int current_cluster = 0;
        
        distances = getDistances(active_objects, centerID);
        distances.Sort(delegate (object_distance a, object_distance b){return a.CompareTo(b);});
        float range = (distances[distances.Count - 1].distance) / 2f;
        bool done = false;


        while (!done)
        {

            Cluster new_cluster = new Cluster();
            
            foreach (object_distance od in distances)
            {
         
                if (od.distance <= range)
                {
                    Debug.Log("Adding object " + od.to.gameObject.name + " to cluster " + current_cluster + "\n");
                    new_cluster.addObject(od.to);
                }
            }
            clusters.Add(new_cluster);
            current_cluster++;
            Transform center_object = distances[distances.Count - 1].to;
            centerID = center_object.gameObject.GetInstanceID();
           // centers.Add(center_object.position);

            active_objects = new List<Transform>();

            foreach (Transform a in t)
            {
                if (a.gameObject.activeSelf && getCluster(t.gameObject.GetInstanceID()) == -1) active_objects.Add(a);
            }
            if (active_objects.Count == 0)
            {
                Debug.Log("Ran out of objects to cluster at cluster " + current_cluster + " out of " + total_clusters + "\n");
                done = true;
            }
            done = current_cluster >= total_clusters;

            if (done) continue;
            distances = getDistances(active_objects, centerID);
            if (distances.Count == 0) { done = true; continue; }
            distances.Sort(delegate (object_distance a, object_distance b) { return a.CompareTo(b); });

            range = (distances[distances.Count - 1].distance) / 2f;
            
        }

        Debug.Log("Picked " + clusters.Count + " clusters\n");
    }

    Transform getTransform(int ID, List<Transform> active_objects)
    {
        foreach (Transform t in active_objects)
        {
            if (t.gameObject.GetInstanceID() == ID) return t;
        }
        return null;
    }

    List<object_distance> getDistances(List<Transform> active_objects, int centerID)
    {
        List<object_distance> distances = new List<object_distance>();
        //calculate distance
        Transform from = null;
        from = getTransform(centerID, active_objects);

        foreach (Transform to in active_objects)
        {
            if (getCluster(to.gameObject.GetInstanceID()) != -1) continue; //already clustered, skip
            float distance = Vector2.Distance(from.position, to.position);
            distances.Add(new object_distance(from, to, distance));
        }

        return distances;
    }

    int getCluster(int ID)
    {
        for (int i = 0; i < clusters.Count; i++)
        {
            if (clusters[i].contains(ID)) return i;
        }
        return -1;
    }

    float getDistance(int from, int to)
    {
        foreach (object_distance od in distances)
        {
            if (od.Equals(from, to)) return od.distance;
        }
        return 99999f;

    }

    void Fire()
    {
        
        foreach (Cluster target in clusters)
        {            

            Lava lava = Peripheral.Instance.zoo.getObject(attack_lava, false).GetComponent<Lava>();
            
            lava.SetLocation(this.transform, start_from, range, Quaternion.identity);
            lava.Init(skill_effect_type, level, stats, lava_life, true, null);            
            target.my_lava = lava;
            
            lava.gameObject.SetActive(true);
            target.startLava();
            active_lavas.Add(lava);
        }
    }

    void Update()
    {
        if (!am_running) return;
        if (Time.timeScale == 0) return;

        if (TIMER <= 0) Deactivate();
        if (UPDATE_LAVA_TIMER <= 0)
        {
            UpdateLavas();
            UPDATE_LAVA_TIMER = update_lava_time;
        }

        TIMER -= Time.fixedDeltaTime;
        UPDATE_LAVA_TIMER -= Time.fixedDeltaTime; 
        
               
        
    }
    public override void Reset()
    {
        TurnMeOff();
    }

    public override void Simulate(List<Vector2> positions)
    {
        Debug.LogError("Don't know how to simulate SentientCloud yet:(");
    }

    void UpdateLavas()
    {
        //   Debug.Log("Updating lavas\n");
        for (int i = 0; i < active_lavas.Count; i++)
        {
            if (!active_lavas[i].gameObject.activeSelf)
                active_lavas.RemoveAt(i);
        }

        for (int i = 0; i < clusters.Count; i++)
        {
            if (!clusters[i].has_stuff)
                clusters.RemoveAt(i);
        }


        foreach (Cluster c in clusters) c.updateLava();
        if (active_lavas.Count == 0) TurnMeOff();
    }

    class object_distance
    {
        public Transform from;
        public Transform to;
        public float distance;

        public object_distance(Transform f, Transform t, float dist)
        {
            from = f;
            to = t;
            distance = dist;
        }

        public bool Equals(int f, int t)
        {
            return ((from.gameObject.GetInstanceID() == f && to.gameObject.GetInstanceID() == t)
                 || (from.gameObject.GetInstanceID() == t && to.gameObject.GetInstanceID() == f));
        }

        public int CompareTo(object_distance to)
        {
            return (distance < to.distance) ? -1 :
              ((distance > to.distance) ? 1 : 0);
        }
    }


}
