using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;


#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(GameSnapshotTester))]


public class GameSnapshotTesterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GameSnapshotTester myTarget = (GameSnapshotTester)target;
        GUILayout.Label("Playing lvl#  Loaded (wave)_(wavelet)\n  Setup (towers/empty/whatever)\n Loaded From File eo#\n And then (restart_at/rewave/tomap)\n When was (wave_when)_(wavelent_when)");
        GUILayout.Label("lvl#_(wave)_(wavelet)_(towers/empty/whatever)_eo#_(restart_at/rewave/tomap)_(wave_when)_(wavelent_when)");

        if (GUILayout.Button("Take snapshot"))
        {
            string hey = ((GameSnapshotTester)target).SaveGame();
            Debug.Log(hey);
        }

        if (GUILayout.Button("Lvl 1 2.0 play new"))
            ((GameSnapshotTester)target).new_snapshot_name = "lvl1_2_0_play_new";

        if (GUILayout.Button("Lvl 1 6.0 play new"))
            ((GameSnapshotTester)target).new_snapshot_name = "lvl1_6_0_play_new";

        if (GUILayout.Button("Lvl 1 6.0 play new then restart wave at 6.0 to 2.0"))
            ((GameSnapshotTester)target).new_snapshot_name = "lvl1_6_0_play_new_restart_wave_2_0";


        if (GUILayout.Button("Lvl 1 0.0 empty EO1"))
            ((GameSnapshotTester)target).new_snapshot_name = "lvl1_0_0_empty_eo1";
    
        if (GUILayout.Button("Lvl 1 0.0 towers EO1"))
            ((GameSnapshotTester)target).new_snapshot_name = "lvl1_0_0_towers_eo1";

        if (GUILayout.Button("Lvl 1 2.0 play EO1"))
            ((GameSnapshotTester)target).new_snapshot_name = "lvl1_2_0_play_eo1";


        if (GUILayout.Button("Lvl 1 0.0 towers EO1 restart at 1.0"))
            ((GameSnapshotTester)target).new_snapshot_name = "lvl1_0_0_towers_eo1_restart_1_0";


    }

}
#endif
public class GameSnapshotTester : MonoBehaviour {

    public static GameSnapshotTester Instance { get; private set; }


    public string new_snapshot_name = "";
    public zzTransparencyCaptureExample screenshot_capture;
    string snapshot_location;

    void Awake()
    {
        
        if (Instance != null && Instance != this)
        {        
            Destroy(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

    }


    public string SaveGame(string name)
    {
        new_snapshot_name = name;
        return SaveGame();
    }

    public string SaveGame()
    {
        snapshot_location = Get.savegame_location;
        if (Central.Instance.getState() != GameState.InGame)
        {
            screenshot_capture.takeScreenshot(snapshot_location, new_snapshot_name);
            return "";
        }
        if (new_snapshot_name.Equals(""))
        {
            Debug.Log("No snapshot name provided!\n");
            return "";
        }

       

        CompleteSaveGame savegame = new CompleteSaveGame();        
        savegame.summary = new SaveGameSummary();
        savegame.summary.name = new_snapshot_name;
       

        CompleteSaveState save_state = new CompleteSaveState();
        savegame.save_state = save_state;


        if (FakeRunner.Instance != null && FakeRunner.Instance.hasFakePlayer() && !FakeRunner.Instance.current_player.fake_player.save_xp_summary_only)
        {
            save_state.SaveBasicMidLevelShit();
            save_state.SaveIslands();
            save_state.SaveWishes();
            save_state.SaveEvents();
            save_state.SaveRewards();
            save_state.SaveSkillInventory();
            save_state.SaveHeroStats();
            save_state.SaveToyStats();
            save_state.SaveScore();
        }
        save_state.SaveTowerStats();

        StringBuilder sb = new StringBuilder();
                
        foreach (tower_stats ts in save_state.tower_stats)
        {
            if (ts.name.Equals("")) continue;

            sb.Append(savegame.summary.name);
            sb.Append(",");        
            sb.Append(ts.name);
            sb.Append(",");
            sb.Append(ts.hits);
            sb.Append(",");
            sb.Append(ts.shots_fired);
            sb.Append(",");
            

            foreach (skill_stat_group ss_group in ts.skill_stats)
            {
                sb.Append(ss_group.effect_type);
                sb.Append(",");
                foreach (skill_stat ss in ss_group.skill_stats)
                {
                    if (ss.xp > 0)
                    {
                        sb.Append(ss.lvl);
                        sb.Append(",");
                        sb.Append(ss.xp);
                        sb.Append(",");
                    }
                }
            }            
            sb.Append("\n");
        }


        savegame.SaveFile();
        screenshot_capture.takeScreenshot(snapshot_location, new_snapshot_name);
        return sb.ToString();
    }

    
}