using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine.UI;




#if UNITY_EDITOR

using UnityEditor;
[CustomEditor(typeof(FakePlayer))]

public class FakePlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        FakePlayer myTarget = (FakePlayer)target;

        if (GUILayout.Button("Run Me"))
            ((FakePlayer)target).RunMe();

        if (GUILayout.Button("Continue"))
            ((FakePlayer)target).Continue();

    }
}
#endif

public class FakePlayer : MonoBehaviour
{
    public string description;

    
    public TimeScale ff = TimeScale.Normal;
    public string snapshot_filename = "sg";
    public bool take_snapshots = true;
    public bool force_all_towers = false;  //don't consider resources, only for balancing skills
    public bool force_all_upgrades = false;
    float wave_start_wait = 8f;    

    public List<BuildTower> towers;
    public List<FakeSpecialAttack> special_attacks;
    public List<FakePotion> wishes;

    List<TowerUpgrade> upgrades;
    List<Toy> ghosts = new List<Toy>();
    int current_upgrade;    
    int current_snapshot;
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
        StartCoroutine(BuildTowers());
        StartCoroutine(UpgradeTowers());
        StartCoroutine(CollectWishes());
        StartCoroutine(KeepTheFlow());
        StartCoroutine(MaintainFF());
        StartCoroutine(SimulateSpecialAttacks());
        StartCoroutine(MakeWishes());
//#endif
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
        ghosts = new List<Toy>();
        current_upgrade = 0;
        snapshots = new Dictionary<float, bool>();

        foreach (BuildTower sa in towers) sa.done = false;        
        foreach (FakeSpecialAttack sa in special_attacks) sa.done = false;
        foreach (FakePotion p in wishes) p.done = false;
        InitUpgrades();
    }

    public void InitUpgrades()
    {
        upgrades = new List<TowerUpgrade>();
        for (int i = 0; i < towers.Count; i++)
        {
            if (towers[i].done) continue;
            foreach (TowerUpgrade upg in towers[i].upgrades)
            {
                
                upg.tower_id = i;
                upg.done = false;
                upgrades.Add(upg);
            }
        }

    }

    public void RunMe() //run from beginning via button
    {
//#if UNITY_EDITOR
        Init();
        Continue();
        am_running = true;
//#endif
    }

    public void OnDisable()
    {
        StopAllCoroutines();
    }

//#if UNITY_EDITOR
    public void Update()
    {

        if (am_running && Central.Instance.state != GameState.InGame) Stop();


    }
//#endif


    public void loadSnapshot(FakePlayerSaver saver)
    {
//#if UNITY_EDITOR
        if (saver.towers.Count == 0) return;
        Init();        
        current_snapshot = saver.current_snapshot;
        snapshot_filename = saver.snapshot_filename;
        ff = saver.ff;
        force_all_towers = saver.force_all_towers;
        force_all_upgrades = saver.force_all_upgrades;

        upgrades = CloneUtil.copyList<TowerUpgrade>(saver.upgrades);
        for(int i = 0; i < towers.Count; i++)
        {
            towers[i].loadSnapshot(saver.towers[i]);
        }
               
        foreach (Firearm f in Peripheral.Instance.firearms)
        {
            if (f.toy.toy_type == ToyType.Temporary)
            {
                ghosts.Add(f.toy);
            }
        }

//#endif
    }



    public FakePlayerSaver getSnapshot()
    {
        FakePlayerSaver saver = new FakePlayerSaver();
//#if UNITY_EDITOR
        saver.current_snapshot = current_snapshot;
        saver.snapshot_filename = snapshot_filename;
        saver.ff = ff;
        saver.upgrades = CloneUtil.copyList<TowerUpgrade>(upgrades);
        saver.force_all_upgrades = force_all_upgrades;
        saver.force_all_towers = force_all_towers;

        List<BuildTowerSaver> towers_saver = new List<BuildTowerSaver>();
        for (int i = 0; i < towers.Count; i++)
        {
            towers_saver.Add(towers[i].getSnapshot());
        }
        saver.towers = towers_saver;

//#endif
        return saver;
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


    void TakeSnapshot(string filename)
    {
//#if UNITY_EDITOR
        int wave = Moon.Instance.current_wave;
        int wavelet = Moon.Instance.current_wavelet;
        bool save = false;
        if (snapshots.ContainsKey(wave + (float)wavelet / 10f)) return;

        //if (wave == 0 && wavelet == 0) save = true;
        
        if (wave == Moon.Instance.Waves.Count - 1 && wavelet == 0) save = true;
        //if (wave == Moon.Instance.waves.Count - 2 && wavelet == 0) save = true;
        //if (wavelet == 0 && Get.SaveGameNow(wave)) save = true;

        if (save)
        {
            snapshots.Add(wave + (float)wavelet / 10f, true);
            log += GameSnapshotTester.Instance.SaveGame(filename + "_" + wave + "_" + wavelet);
            
            am_running = false;
            done = true;
        }
//#endif
    }


    IEnumerator MakeWishes()
    {
        int done = 0;
        while (done < wishes.Count)
        {
            yield return new WaitForSeconds(.2f);

            FakePotion wish = wishes[done];
            if (wish.done) { done++; continue; }
            if (wish.wave_start_time > Moon.Instance.TIME) continue; //not sure this will work, used to read time off of Sun
            if (wish.wave_start_time < Moon.Instance.TIME - 2f) { wish.done = true; done++; continue; }


            Wish w = new Wish();
            w.type = WishType.MoreXP;
            w.strength = 1;
            bool yes = false;
            if (Peripheral.Instance.my_inventory.UseWish(w))
            {
                yes = true;                
                yield return new WaitForSeconds(.3f);
            }
            w = new Wish();
            w.type = WishType.MoreDreams;
            w.strength = 1;
            if (Peripheral.Instance.my_inventory.UseWish(w))
           {
                yes = true;
                yield return new WaitForSeconds(.3f);
            }
            if (yes)
            {
                wish.done = true;
                done++;
            }

        }
    }
    IEnumerator SimulateSpecialAttacks()
    {
        int done = 0;
        while (done < special_attacks.Count)
        {
            yield return new WaitForSeconds(.5f);

            FakeSpecialAttack attack = special_attacks[done];
            if (attack.done) { done++; continue; }
            if (attack.wave_start_time < Moon.Instance.TIME)
            {
                Peripheral.Instance.my_skillmaster.SimulateSkill(attack.positions, attack.type);
                yield return new WaitForSeconds(1f);
                attack.done = true;
                done++;
            }            
        }
    }

    IEnumerator KeepTheFlow() //start waves and event pauses
    {

        string filename = snapshot_filename + "_lvl" + Central.Instance.current_lvl + "_ff" + ff.ToString() + "_" + Peripheral.Instance.getLevelMod().ToString();


        int start_child = 0;
        while (true)
        {
            if (Peripheral.Instance.level_state == LState.WaitingToStartNextWave)
            {
                if (take_snapshots) TakeSnapshot(filename);
                if (Moon.Instance.current_wave == 0)
                    yield return new WaitForSeconds(1f);
                else
                    yield return new WaitForSeconds(wave_start_wait);

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

            if (Central.Instance.state == GameState.InGame && Moon.Instance.current_wave == Moon.Instance.Waves.Count)
            {
                if (take_snapshots) TakeSnapshot(filename);
            }

            yield return new WaitForSeconds(0.1f);

        }
    }

    IEnumerator CollectWishes()
    {
        int toy_count = 0;
        while (true)
        {
            if (Wishes.Instance.transform.childCount > 0)
            {
                Wishes.Instance.transform.GetChild(0).GetComponent<Effect_Button>().OnPointerClick(null);
                yield return new WaitForSeconds(1f);
            }

            if (Peripheral.Instance.my_inventory.GetWish(WishType.Sensible) > 0 && ghosts.Count > 0)
            {
                Toy toy = ghosts[toy_count];

                if (toy.gameObject.activeSelf && toy.Active && toy.firearm != null && toy.firearm.CanAddAmmo(1)) toy.firearm.AddAmmo(1);
                toy_count++;
                if (toy_count >= ghosts.Count) toy_count = 0;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator UpgradeTowers()
    {
        yield return new WaitForSeconds(1f);// wait for initial towers to finish

        while (current_upgrade < upgrades.Count)
        {
            foreach (TowerUpgrade upgrade in upgrades)
            {
                if (upgrade.done == true) continue;
                if (!towers[upgrade.tower_id].island.isBlocked() || towers[upgrade.tower_id].island.my_toy.building.construction_in_progress) continue;

                //force = just upgrade, don't bother checking if you can afford it. for balancing skills.
                bool force = upgrade.force || force_all_upgrades;
                if (force || towers[upgrade.tower_id].island.my_toy.rune.CanUpgrade(upgrade.effect_type, towers[upgrade.tower_id].toy_id.rune_type) == StateType.Yes)

                {
                    upgrade.done = true;                    
                    bool use_resource = !force;
                    towers[upgrade.tower_id].island.my_toy.rune.Upgrade(upgrade.effect_type, use_resource, force);
                    current_upgrade++;
                }
            }

            yield return new WaitForSeconds(1f);
        }

        yield return null;
    }
    IEnumerator BuildTowers()
    {
        int built = 0;
        foreach (BuildTower tower in towers) {
            if (tower.done) continue;
            if (!force_all_towers && !tower.force) continue;

            if (AttemptBuilt(tower)) built++;           
            //yield return new WaitForSeconds(0.1f);
        }
        if (force_all_towers) yield return null;

        int order = 0;

        while (built < towers.Count)
        {

            BuildTower tower = towers[order];
            if (tower.done) { built++; order++; continue; }

            if (AttemptBuilt(tower))
            {
                built++;
                order++;
                yield return new WaitForSeconds(1f);
            }
            yield return new WaitForSeconds(.1f);

        }

        yield return null;
    }


    bool AttemptBuilt(BuildTower tower)
    {
        if (tower.done) return false;
        unitStats stats = Central.Instance.getToy(tower.toy_id.rune_type, tower.toy_id.toy_type);
        string toy_name = stats.name;
        bool force = (force_all_towers || tower.force);
        if (canBuildTower(stats, force))
        {
            Toy f = null;
            GameObject o = Peripheral.Instance.makeToy(toy_name, tower.island, !force, ref f, false);
            o.GetComponent<Toy>().my_tower_stats.name = tower.label;
            
            if (tower.toy_id.toy_type == ToyType.Temporary) ghosts.Add(o.GetComponent<Toy>());
            if (tower.disable_tower) StartCoroutine(DisableTower(o));            

            tower.done = true;
            return true;
        }

        return false;
    }

    IEnumerator DisableTower(GameObject o)
    {
        yield return new WaitForSeconds(o.GetComponent<Building>().init_construction_time + 0.5f);
        o.GetComponent<Toy>().DisableMe();
        yield return null;
    }

    bool canBuildTower(unitStats toy, bool force)
    {
        Cost toy_cost = null;
        float cost = 9999;
        if (toy != null) toy_cost = toy.cost_type;

        if (toy_cost != null) cost = toy_cost.Amount;

        StateType answer = EagleEyes.Instance.canBuildToy(toy.name, toy_cost, toy);

        return (!force && answer == StateType.Yes) || (force && answer != StateType.No);
    }

    void Awake()
    {
/*
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
        */
        DontDestroyOnLoad(gameObject);

    }

}

[System.Serializable]
public class BuildTowerSaver
{
    public int tower_id;
    public bool done;
    public BuildTowerSaver()
    {

    }
}

[System.Serializable]
public class BuildTower
{
    public int tower_id;
    public bool force = false; //don't check resources
    public ToyID toy_id;
    public Island_Button island;
    public TowerUpgrade[] upgrades;
    public bool disable_tower = false;
    public string label;
    public bool done;



    public BuildTowerSaver getSnapshot()
    {
        BuildTowerSaver saver = new BuildTowerSaver();
        saver.tower_id = this.tower_id;
        saver.done = this.done;
        return saver;
    }
    public void loadSnapshot(BuildTowerSaver saver)
    {
        if (this.tower_id != saver.tower_id) return;
        this.done = saver.done;
    }
}

[System.Serializable]
public class TowerUpgrade : IDeepCloneable<TowerUpgrade>
{
    public int tower_id;
    public bool force = false; //don't check resources
    public EffectType effect_type;
    public bool done = false;

    public TowerUpgrade() { }

    object IDeepCloneable.DeepClone()
    {
        return this.DeepClone();
    }

    public TowerUpgrade DeepClone()
    {
        TowerUpgrade my_clone = new TowerUpgrade();
        my_clone.tower_id = this.tower_id;
        my_clone.force = this.force;
        my_clone.effect_type = this.effect_type;
        my_clone.done = this.done;
        return my_clone;
    }
}
public class FakePlayerSaver
{
    public List<TowerUpgrade> upgrades;
    public List<BuildTowerSaver> towers;
    public int current_tower;
    public int current_snapshot;
    public string snapshot_filename;
    public TimeScale ff;
    public bool force_all_upgrades;
    public bool force_all_towers;

    public FakePlayerSaver() { }
}
[System.Serializable]
public class FakeSpecialAttack
{
    public EffectType type;
    public float wave_start_time;
    public List<Vector2> positions;
    public bool force;
    public bool done;
}

[System.Serializable]
public class FakePotion
{    
    public float wave_start_time;
    public bool done;
}