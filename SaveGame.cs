using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using System.Xml.Serialization;

[System.Serializable]
public class SaveGameSummary : System.Object
{
    //public int current_level;
    SaveWhen current_state;
    public string name;
    public string savegame_filename; //filename
    public string savegame_version;
    
    public int id;

    public string Savegame_filename
    {
        get
        {
            return savegame_filename;
        }

        set
        {
        //    Debug.Log("Setting savegame filename from " + savegame_filename + " to " + value + "\n");
            savegame_filename = value;
        }
    }

    public string getFileName()
    {
        return Savegame_filename;
    }
}

[System.Serializable]
public class SaveGame
{
    public SaveGameSummary summary;
    public SaveState[] save_states; // 1 for persistent, 1 for midlevel
    public string description;//break it out into an array or something, contails current level, score
    private bool _isLoaded;
    int midlevel_id = 1;
    int persistent_id = 0;

    public bool isLoaded()
    {
        return _isLoaded;
    }
    private void isLoaded(bool set_isloaded)
    {
        _isLoaded = set_isloaded;
    }

    public int getCurrentLevel()
    {
        SaveState state = save_states[midlevel_id];
        if (state == null || state.current_level < 0) state = save_states[persistent_id];

        return (state == null) ? -1 : state.current_level;
    }

    public void SaveMidLevelToPersistent()
    {
        SaveState midlevel = getSaveState(SaveStateType.MidLevel);
        SaveState persistent = getSaveState(SaveStateType.Persistent);
        
        midlevel.resetMidLevelStuff();
        persistent.hero_stats = midlevel.hero_stats;
        persistent.score_details = midlevel.score_details;
        persistent.wishes = midlevel.wishes;
        persistent.rewards = midlevel.rewards;
        persistent.skills_in_inventory = midlevel.skills_in_inventory;
        persistent.total_score = midlevel.total_score;
        
        SaveFile();

        //force reload so that we're not using stuff by ref
        SaveData save_data = SaveData.Load(summary.getFileName());
        save_states = save_data.GetValue<SaveState[]>("save_states");
    }

   

    public SaveState getSaveState(SaveStateType type)
    {
        if (save_states.Length < 2) InitNewGame(summary.id, summary.name);

        if (type == SaveStateType.MidLevel) return save_states[midlevel_id];
        else return save_states[persistent_id];
        

    }

    public void setSaveState(SaveState state)
    {
        if (save_states.Length < 2) InitNewGame(summary.id, summary.name);

        if (state.type == SaveStateType.MidLevel) save_states[midlevel_id] = state;
        else save_states[persistent_id] = state;


    }

    public bool LoadFile(string alt_filename)
    {
        if (alt_filename.Equals("") && isLoaded()) { return true; }
        string filename = (alt_filename.Equals("")) ? summary.getFileName() : alt_filename;

        try
        {
            SaveData save_data = SaveData.Load(filename);
            summary = save_data.GetValue<SaveGameSummary>("summary");
            if (!summary.savegame_version.Equals(Central.Instance.game_saver.savegame_version))
            {
                // DeleteFile();
                description = "file savegame version " + summary.savegame_version + " does not match game version " + Central.Instance.game_saver.savegame_version + ". Start over:(";
                Debug.Log("Found Savegame version mismatch " + summary.savegame_version + " does not match game version " + Central.Instance.game_saver.savegame_version + "\n");
                return false;
            }

            save_states = save_data.GetValue<SaveState[]>("save_states");
        }catch(Exception e)
        {
            description = "Error: " + e.Message + " Start Over:(";
            Debug.LogError("Could not load savegame " + filename + ": "  + e.Message);
            return false;
        }


        isLoaded(true);
        return true;
    }
    
    public void Unload()
    {

        save_states = null;
       // summary = null;
        isLoaded(false);
    }

    public bool DeleteFile()
    {
        Debug.Log("Gonna delete " + summary.getFileName() + "\n");
        if (System.IO.File.Exists(summary.getFileName()))
        {
            System.IO.File.Delete(summary.getFileName());
            description = "New Game";
            return true;
        }
        else
        {
            return false;
        }

        
    }

    public void SaveAndReload()// the only point of this is to avoid doing a deep copy manually
    {
        string junk_file = Get.savegame_location + "junk.uml";

        if (System.IO.File.Exists(junk_file)) System.IO.File.Delete(junk_file);
    
        SaveData save_data = new SaveData(junk_file);
        save_data["summary"] = summary;
        save_data["save_states"] = save_states;        
        save_data.Save(junk_file);
        LoadFile(junk_file);

        if (System.IO.File.Exists(junk_file)) System.IO.File.Delete(junk_file);
    }

    public void SaveFile()
    {
        Debug.Log("SAVING FILE!!!!\n");
        SaveData save_data = new SaveData(summary.Savegame_filename);
        save_data["summary"] = summary;
        save_data["save_states"] = save_states;

        DeleteFile();
        save_data.Save(summary.getFileName());

    }

    public void ReloadFile(string copy_from)
    {
        Debug.Log("Copying from " + copy_from + " to " + summary.Savegame_filename + "\n");

        
        SaveData save_data = SaveData.Load(copy_from);        

        save_states = save_data.GetValue<SaveState[]>("save_states");

        SaveFile();
        UpdateDescription();
    }


    public bool CheckFile()
    {
        summary.Savegame_filename = Get.savegame_location + summary.name + ".uml";
    //    Debug.Log("Checking file " + summary.Savegame_filename + "\n");
        if (System.IO.File.Exists(summary.Savegame_filename))
        {
            if (LoadFile("")) UpdateDescription();
            else return false;
        }
        else
        {
            InitNewGame(summary.id, summary.name);
            UpdateDescription();
        }
        return true;
    }

    public void InitMidLevel()
    {
        Debug.Log("Initializing midlevel\n");
        SaveState midlevel = save_states[midlevel_id];
        SaveState persistent = save_states[persistent_id];
        midlevel = new SaveState();
        midlevel.type = SaveStateType.MidLevel;
        midlevel.current_level = persistent.current_level;
        midlevel.hero_stats = persistent.hero_stats;
        midlevel.score_details = persistent.score_details;
        midlevel.wishes = persistent.wishes;
        midlevel.rewards = persistent.rewards;

        save_states[midlevel_id] = midlevel;
        SaveAndReload();
    }

    public void InitNewGame(int id, string name)
    {
        Debug.Log("Initializing new game\n");
        save_states = new SaveState[2];
        SaveState persistent = new SaveState();
        persistent.type = SaveStateType.Persistent;
        persistent.current_level = -1;
        save_states[persistent_id] = persistent;
        save_states[midlevel_id] = null;
        summary.savegame_version = Central.Instance.game_saver.savegame_version;
        summary.name = name;
        summary.id = id;
        UpdateDescription();
        SaveFile();
    }


    public void UpdateDescription()
    {

        if (!_isLoaded) return;
        if (save_states == null || save_states.Length == 0 || getCurrentLevel() < 0)
        {
            description = "New Game";
        } else
        {
            description = "Level: " + getCurrentLevel().ToString();
        }
    }

    public string getScoreText()
    {
        return "Score: " + Get.Round(save_states[0].total_score, 0).ToString();
    }
    public string getDescription()
    {
        UpdateDescription();
        return description;
    }
}
