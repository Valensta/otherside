using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine.UI;


#if UNITY_EDITOR

using UnityEditor;
[CustomEditor(typeof(FakeRunner))]

public class FakeRunnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        FakeRunner myTarget = (FakeRunner)target;

        if (GUILayout.Button("Run Me"))
            ((FakeRunner)target).RunMe();

        if (GUILayout.Button("Select ALL"))
            ((FakeRunner)target).SelectAll();

        if (GUILayout.Button("Select NONE"))
            ((FakeRunner)target).SelectNone();

    }
}
#endif
[System.Serializable]
public class FakePlayerWrapper
{
    public bool run_me = true;
    public FakePlayer fake_player;
}

public class FakeRunner : MonoBehaviour
{
    public string description;

    public List<FakePlayerWrapper> fake_players;
    int current_player_id;
    public FakePlayerWrapper current_player;
    bool am_running;
    public static FakeRunner Instance { get; private set; }
    public TimeScale timescale_override = TimeScale.Null;

    public bool auto_run = false;

    public void RunMe()
    {
        current_player_id = 0;
        current_player = fake_players[current_player_id];
        am_running = true;
        if (fake_players[current_player_id].run_me)
        {
            setTimeOverride();
            current_player.fake_player.RunMe();

        }
        else
        {
            current_player.fake_player.GetComponent<FakePlayer>().setDone(true);
        }
    }

    public bool hasFakePlayer()
    {
        return (current_player != null && current_player.fake_player != null);
    }

    public void SelectAll()
    {
        foreach (FakePlayerWrapper w in fake_players) w.run_me = true;
    }

    public void SelectNone()
    {
        foreach (FakePlayerWrapper w in fake_players) w.run_me = false;
    }

    public void Update()
    {
#if !UNITY_EDITOR
        if (!am_running && auto_run && Central.Instance.state == GameState.InGame)
        {
            RunMe();
            auto_run = false;
        }
#endif

        if (!am_running) return;
        if (Central.Instance.state != GameState.InGame)
        {
            if (current_player.fake_player != null) current_player.fake_player.Stop();
            return;
        }

        if (current_player_id >= fake_players.Count)
        {
            DumpLogs();
            am_running = false;
        }

        if (!am_running) return;

        if (current_player.fake_player.amDone())
        {
            current_player.fake_player.Stop();
            if (!incrementCurrentPlayerID()) return;
            current_player = fake_players[current_player_id];
            Central.Instance.changeState(GameState.Loading, "LoadStartLevelSnapshot");
            return;
        }

        if (current_player.run_me && !current_player.fake_player.amRunning() && !current_player.fake_player.amDone())
        {
            setTimeOverride();
            current_player.fake_player.RunMe();
        }


    }

    bool incrementCurrentPlayerID()
    {

        while (current_player_id < fake_players.Count)
        {
            current_player_id++;
            if (current_player_id <= fake_players.Count && fake_players[current_player_id].run_me) return true;
        }
        am_running = false;
        return false;

    }

    void setTimeOverride()
    {
        if (timescale_override != TimeScale.Null) current_player.fake_player.ff = timescale_override;

    }

    public void DumpLogs()
    {
        foreach (FakePlayerWrapper wrapper in fake_players)
        {
            if (wrapper.fake_player.log.Equals("")) continue;
            Tracker.LogDump(wrapper.fake_player.log);
        }

        Tracker.EndLog();
    }



    void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

    }


    
}