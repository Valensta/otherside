using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine.UI;




public class PlayerSimulator : MonoBehaviour
{
   public delegate bool executeEvent(MyPlayerEvent e);

    public List<FakeRunLoader> runs = new List<FakeRunLoader>();

    int current_run = -1;
    int current_event = -1;

    TimeScale ff = TimeScale.Normal;
    public string snapshot_filename = "sg";
    public bool take_snapshots = true;
    public bool force_all_towers = false;  //don't consider resources, only for balancing skills
    public bool force_all_upgrades = false;
    float wave_start_wait = 8f;

    List<Toy> ghosts = new List<Toy>();
    List<Toy> toys = new List<Toy>();

    float timer;
    string skills_and_inventory;
    Dictionary<float, bool> snapshots;
    public bool save_xp_summary_only = true;
    bool am_running = false;
    bool done;
    public string log = "";

   // public static FakePlayer Instance { get; private set; }

    public void Continue()
    {
//#if UNITY_EDITOR
        Debug.Log("Starting Fake Player\n");

        StartCoroutine(KeepTheFlow());
        StartCoroutine(MaintainFF());

//#endif
    }

    public void Update()
    {
        if (!Moon.Instance.WaveInProgress) timer += Time.deltaTime; else timer = 0;
    }

    public void Stop()
    {
        if (!am_running) return;
        StopAllCoroutines();
        am_running = false;
    }

    public bool amRunning() { return am_running; }
    public bool amDone() { return done; }
    public void setDone(bool done) { this.done = done; }

    public void Init()
    {
        done = false;
        am_running = false;
     
        snapshots = new Dictionary<float, bool>();
     
    }

    public void loadHeroSkills(string text, RuneType runetype)
    {
        string[] inventory = text.Split('|');

        string[] skills = inventory[0].Split(':');
        foreach (string s in skills)
        {
            if (s.Equals("R")) continue;
            if (s.Equals("")) continue;

            string[] skill = s.Split('_');
            EffectType type = (EffectType)int.Parse(skill[0]);
            int skill_lvl = int.Parse(skill[1]);

           // Debug.Log("loading " + type + "\n");

            Rune r = Central.Instance.getHeroRune(runetype);
            if (!r.HasUpgrade(type)) continue;
            for (int i = 0; i < skill_lvl; i++) r.Upgrade(type, false, true);

        }



        string[] special_skills = inventory[1].Split(':');
        foreach (string s in special_skills)
        {
            if (s.Equals("S")) continue;
            if (s.Equals("")) continue;
       
            string[] skill = s.Split('_');
            EffectType type = (EffectType)int.Parse(skill[0]);
            int skill_lvl = int.Parse(skill[1]);
            bool in_inv = (skill[2].Equals("True"));
            if (in_inv)
            {
                Debug.Log("loading " + type + "\n");

                Rune r = Central.Instance.getHeroRune(runetype);
                if (!r.HasUpgrade(type)) continue;
                for (int i = 0; i < skill_lvl; i++) r.Upgrade(type, false, true);
                r.LoadSpecialSkills();
                Peripheral.Instance.my_skillmaster.setInventory(type, true);

            }
        }
    }

    public void RunMe(int id) 
    {
        //Tracker.track = false;
        current_run = id;

        MyPlayerEvent e = runs[current_run].findEventType(PlayerEvent.LoadGame);

        string[] settings = e.attribute_1.Split('|');
               
        Difficulty difficulty = EnumUtil.EnumFromString<Difficulty>(settings[0], Difficulty.Normal);

        int level = (int)e.metric_1;
        Central.Instance.game_saver.DeleteSaveGame(2);


               
        Central.Instance.game_saver.SelectSaveGame(2, true);        
        Central.Instance.level_list.test_mode = true;
        Central.Instance.setCurrentLevel(level);
        Central.Instance.level_list.SetDifficulty(difficulty);

        Central.Instance.changeState(GameState.Loading, "NewLevel");
        skills_and_inventory = e.attribute_2;
        getNextEvent();
        am_running = true;
        done = false;
        StartCoroutine(KeepTheFlow());
        StartCoroutine(runEvents());
        StartCoroutine(MaintainFF());
        StartCoroutine(TrackXP());
    }

    public void getNextEvent()
    {
        int current = Mathf.Max(current_event,0);
        int new_current = -1;
        for (int i = current; i < runs[current_run].GetRowList().Count;i++)
        {
            if (runs[current_run].GetRowList()[i].wave_time < 0) continue;
            if (runs[current_run].GetRowList()[i].event_complete) continue;
            new_current = i;
            break;
        }
        if (new_current == -1)
        {
            done = true;
        }
        current_event = new_current;
    }

    public void OnDisable()
    {
        StopAllCoroutines();
    }


    IEnumerator runEvents()
    {
        yield return new WaitForSeconds(0.5f);


        while(am_running && !done)
        {
            MyPlayerEvent e = runs[current_run].GetRowList()[current_event];

            if (e.event_complete)
            {
                getNextEvent();
                e = runs[current_run].GetRowList()[current_event];
            }



            if (!e.event_complete && timeToExecuteEvent(e))
            {
                executeEvent ee = null;

                if (e.eventtype == PlayerEvent.TowerBuilt) ee += new executeEvent(TowerBuild); //force
                if (e.eventtype == PlayerEvent.UsedInventory) ee += new executeEvent(UsedInventory); //force
                if (e.eventtype == PlayerEvent.SkillUpgrade) ee += new executeEvent(SkillUpgrade);  
                if (e.eventtype == PlayerEvent.AmmoRecharge) ee += new executeEvent(AmmoRecharge); //force
                if (e.eventtype == PlayerEvent.SpecialSkillUsed) ee += new executeEvent(SpecialSkillUsed); //force
                if (e.eventtype == PlayerEvent.StartWaveEarly) ee += new executeEvent(StartWaveEarly); 
                if (e.eventtype == PlayerEvent.TimeScaleChange) ee += new executeEvent(TimeScaleChange);
                if (e.eventtype == PlayerEvent.SellTower) ee += new executeEvent(SellTower);

                if (ee != null)
                {
                    Debug.Log("Executing event " + e.toString() + "\n");
                    bool success = ee(e);
                    if (!success) { Debug.LogError("Event " + e.toString() + " FAILED to execute\n"); }
                }

                e.event_complete = true;
            }
            yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(0.3f));
        }

    }

    bool timeToExecuteEvent(MyPlayerEvent e)
    {
        switch (e.eventtype)
        {
            case PlayerEvent.StartWaveEarly:
                float hey = (e.wave_time + e.metric_1 - e.metric_2);
                float comp = Sun.Instance.current_time_of_day + timer;
            //    Debug.Log("wavetime " + e.wave_time + " wave interval - " + e.metric_1 + " time saved " + e.metric_2 + " -> " + hey + " " + comp + "\n");
                return (e.wave_time + e.metric_1 - e.metric_2 <= comp);
            default:

                return (e.wave_time <= Sun.Instance.current_time_of_day);

        }         
    }

    bool TimeScaleChange(MyPlayerEvent e)
    {
        TimeScale timescale = EnumUtil.EnumFromString<TimeScale>(e.attribute_1, TimeScale.Null);
        if (timescale == TimeScale.Null)
        {
            Debug.LogError("Invalid TimeScaleChange to " + e.attribute_1 + "\n");
            return false;
        }

        ff = timescale;
        return true;
    }




    bool SpecialSkillUsed(MyPlayerEvent e)
    {
        List<Vector2> positions = new List<Vector2>();
        string[] list = e.attribute_2.Split('|');
        foreach (string s in list)
        {
            if (s.Equals("")) continue;
            string[] coords = s.Split('_');
            positions.Add(new Vector2(float.Parse(coords[0]), float.Parse(coords[1])));
        }

        EffectType effectType = EnumUtil.EnumFromString<EffectType>(e.attribute_1, EffectType.Null);
        if (effectType == EffectType.Null || positions.Count == 0)
        {
            Debug.LogError("Cannot do special attack with an invalid effecttype or positions " + e.attribute_1 + " " + e.attribute_2 + "\n");
            return false;
        }
        Peripheral.Instance.my_skillmaster.SimulateSkill(positions, effectType);
        return true;

    }

    bool SkillUpgrade(MyPlayerEvent e)
    {
        int order = (int)e.metric_2;
        Toy upgrade_me = null;
        EffectType effect_type = EnumUtil.EnumFromString<EffectType>(e.attribute_1, EffectType.Null);

        if (effect_type == EffectType.Null)
        {
            Debug.LogError("Cannot upgrade toy of order " + order + " to invalid effectType " + e.attribute_1 + "\n");
            return false;
        }

        foreach (Toy toy in toys) if (toy.rune.order == order) upgrade_me = toy;            
        


        if (upgrade_me == null)
        {
            Debug.LogError("Could not find toy of order " + order + " to upgrade\n");
            return false;
        }


        if (!upgrade_me.island.blocked || upgrade_me.island.my_toy.building.construction_in_progress)
        {
            Debug.LogError("Toy of order " + order + " does not exist or is still building\n");
            return false;
        }

        // StateType canUpgrade = upgrade_me.island.my_toy.rune.CanUpgrade(effect_type, upgrade_me.runetype);
        //if (canUpgrade == StateType.Yes)

        //{                       
        float success = upgrade_me.island.my_toy.rune.Upgrade(effect_type, true, true);
            if (success == 0)
            {
                Debug.LogError("Failed to upgrade toy " + order + " " + effect_type + "\n");
                return false;
            }
        //}
        //else
        //{
            //Debug.LogError("Cannot upgrade toy " + order + " " + effect_type + ": " + canUpgrade + "\n");
            //return false;
        //}
        
        return true;
    }


    bool UsedInventory(MyPlayerEvent e)
    {
        Wish w = new Wish();
        w.type = EnumUtil.EnumFromString(e.attribute_1, WishType.Null);
        if (w.type == WishType.Null)
        {
            Debug.LogError("Invalid wishtype " + e.attribute_1 + "\n");
            return false;
        }
        w.strength = 1;

        Peripheral.Instance.my_inventory.UseWish(w, true);

        return true;
    }

    public bool StartWaveEarly(MyPlayerEvent e)
    {
        Peripheral.Instance.StartWave();
        return true;
    }

    public bool AmmoRecharge(MyPlayerEvent e)
    {
        int toy_order = (int)e.metric_2;
        Toy recharge_me = null;
        foreach (Toy ghost in ghosts)
        {
            if (ghost.rune.order == toy_order) recharge_me = ghost;
        }
        if (recharge_me == null)
        {
            Debug.LogError("Invalid toy order to recharge " + e.metric_2+ "\n");
            return false;
        }

        recharge_me.firearm.AddAmmo(1, true);


        return true;
    }

    public bool ResetSkills(MyPlayerEvent e)
    {
        RuneType runetype = EnumUtil.EnumFromString<RuneType>(e.attribute_1, RuneType.Null);
        if (runetype == RuneType.Null)
        {
            Debug.LogError("Cannot reset skill for invalid runetype " + e.attribute_1 + "\n");
            return false;
        }
        Rune rune = Central.Instance.getHeroRune(runetype);
        if (rune == null)
        {
            Debug.LogError("Cannot locate hero rune to reset: " + e.attribute_1 + "\n");
            return false;
        }

        bool special = e.wave_time == -1;
        rune.resetSkills(special);

        return true;

    }

    public bool SellTower(MyPlayerEvent e)
    {
        if (e.event_complete) return true;

        foreach (Toy t in toys)
        {
            if (t.rune.order == e.metric_1)
            {
                Peripheral.Instance.sellToy(t, 0);
                return true;
            }
        }
        Debug.LogError("Could not find toy for event " + e.toString() + "\n");
        return false;
    }

    bool canBuildTower(unitStats toy, bool force)
    {
        Cost toy_cost = null;
        float cost = 9999;
        if (toy != null) toy_cost = toy.cost_type;

        if (toy_cost != null) cost = toy_cost.Amount;

        StateType answer = EagleEyes.Instance.canBuildToy(toy.name, toy_cost, toy);
        Debug.Log("Can build tower " + toy.name + " " + answer + " force " + force + "\n");
        return (!force && answer == StateType.Yes) || (force && answer != StateType.No);
    }

    public bool TowerBuild(MyPlayerEvent e)
    {
        if (e.event_complete) return true;


        //unitStats stats = Central.Instance.getToy(tower.toy_id.rune_type, tower.toy_id.toy_type);
        unitStats stats = Central.Instance.getToy(e.attribute_1);
        string toy_name = stats.name;
        bool force = true;
        
        if (force || canBuildTower(stats, force))
        {
            Toy f = null;
            Island_Button island = Monitor.Instance.getIslandByID(e.attribute_2);
            if (island == null)
            {
                Debug.LogError("Could not locate island " + e.attribute_2 + " for tower " + e.attribute_1 + "\n");
                return false;
            }

            GameObject o = Peripheral.Instance.makeToy(toy_name, island, true, ref f, true);
            Toy toy = o.GetComponent<Toy>();
            toy.my_tower_stats.name = e.metric_1.ToString();

            if (toy.toy_type == ToyType.Temporary) ghosts.Add(toy); else toys.Add(toy);

            if (toy.toy_type == ToyType.Hero) loadHeroSkills(skills_and_inventory, toy.runetype);

            e.event_complete = true;
            return true;
        }else
        {
            Debug.LogError("Could not build tower " + e.attribute_2 + " for tower " + e.attribute_1 + "\n");
            return false;
        }

        return false;

    }


  

    IEnumerator MaintainFF()
    {
        //if (ff != TimeScale.Fast) yield return null;
        Debug.Log("Maintaining FF " + ff + "\n");
        while (true)

        {

            if (ff == TimeScale.SuperFastPress && Peripheral.Instance.getCurrentTimeScale() != TimeScale.SuperFastPress && Peripheral.Instance.getCurrentTimeScale() != TimeScale.Pause)
            {
                Peripheral.Instance.ChangeTime(TimeScale.SuperFastPress);
            }

            if (ff == TimeScale.Normal && Peripheral.Instance.getCurrentTimeScale() != TimeScale.Normal && Peripheral.Instance.getCurrentTimeScale() != TimeScale.Pause)
            {
                Peripheral.Instance.ChangeTime(TimeScale.Normal);
            }

            if (ff == TimeScale.Fast && Peripheral.Instance.getCurrentTimeScale() != TimeScale.Fast && Peripheral.Instance.getCurrentTimeScale() != TimeScale.Pause)
            {
                Peripheral.Instance.ChangeTime(TimeScale.Fast);
            }
            
            yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(0.5f));
        }
    }

    IEnumerator TrackXP()
    {
        while (true)
        {
            yield return new WaitForSeconds(20f);

            if (Central.Instance.state != GameState.InGame) continue;

            foreach (Toy t in toys)
            {
                string xp = t.my_tower_stats.getConciseString();
                if (xp.Equals("")) continue;

                Tracker.Log(PlayerEvent.XP, true,
                       customAttributes: new Dictionary<string, string>() {
                        { "attribute_1", xp } },
                       customMetrics: null);
            }
        }
    }

 
    void SimulateSpecialAttacks()
    {
        /*
        int done = 0;
        while (done < special_attacks.Count)
        {
            yield return new WaitForSeconds(.5f);

            FakeSpecialAttack attack = special_attacks[done];
            if (attack.done) { done++; continue; }
            if (attack.wave_start_time < Sun.Instance.current_time_of_day)
            {
                Peripheral.Instance.my_skillmaster.SimulateSkill(attack.positions, attack.type);
                yield return new WaitForSeconds(1f);
                attack.done = true;
                done++;
            }            
        }*/
    }

    IEnumerator KeepTheFlow() //start waves and event pauses
    {
        int start_child = 0;
        Debug.Log("KEEPING THE FLOW\n");
        while (true)
        {
            if (Peripheral.Instance.WaveCountdownOngoing())
            {
             //   if (take_snapshots) TakeSnapshot(filename);
                if (Moon.Instance.current_wave == 0)
                    yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(1f));
                else
                    yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(wave_start_wait));

                Peripheral.Instance.StartWave();
            }


            if (Central.Instance.state == GameState.InGame && EagleEyes.Instance.events.transform.childCount > 0)
            {
                int max = EagleEyes.Instance.events.transform.childCount;
                for (int i = start_child; i < max; i++)
                {

                    Transform child = EagleEyes.Instance.events.transform.GetChild(i).GetChild(0);
                    start_child = i;
                    if (!child.gameObject.activeSelf) continue;
                    if (EagleEyes.Instance.events.transform.GetChild(i).gameObject.name.Contains("hint")) continue;
                    if (EagleEyes.Instance.events.transform.GetChild(i).gameObject.name.Contains("enemy_description")) continue;
                    Button button = child.gameObject.GetComponent<Button>();
                    if (!button) continue;
                    button.onClick.Invoke();

                }

            }
            /*
            if (Central.Instance.state == GameState.InGame && Moon.Instance.current_wave == Moon.Instance.waves.Count)
            {
                if (take_snapshots) TakeSnapshot(filename);
            }
            */
            yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(0.1f));

        }
    }




    void Awake()
    {

        foreach (FakeRunLoader r in runs)
        {
            r.setSimulator(this);
        }
        DontDestroyOnLoad(gameObject);

#if UNITY_EDITOR
        foreach (FakeRunLoader r in runs)
        {
            r.showButton(true);
            r.setSimulator(this);
            r.Load();
        }        
#else
           foreach (FakeRunLoader r in runs)
        {
            r.showButton(false);
        }
#endif
    }

}
