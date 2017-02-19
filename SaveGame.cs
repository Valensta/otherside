using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

[System.Serializable]
public class SaveGameSummary : System.Object
{
    //public int current_level;
    SaveWhen current_state;
    public string name;
    public string savegame_filename; //filename

    
    public int id;    

    public string getFileName()
    {
        return savegame_filename;
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
        if (save_states.Length < 2) InitNewGame();

        if (type == SaveStateType.MidLevel) return save_states[midlevel_id];
        else return save_states[persistent_id];
        

    }

    public void setSaveState(SaveState state)
    {
        if (save_states.Length < 2) InitNewGame();

        if (state.type == SaveStateType.MidLevel) save_states[midlevel_id] = state;
        else save_states[persistent_id] = state;


    }

    public void LoadFile()
    {
        if (isLoaded()) { return; }
        
        SaveData save_data = SaveData.Load(summary.getFileName());
        save_states = save_data.GetValue<SaveState[]>("save_states");
        summary = save_data.GetValue<SaveGameSummary>("summary");
        isLoaded(true);
    }
    
    public void Unload()
    {

        save_states = null;
        summary = null;
        isLoaded(false);
    }

    public void DeleteFile()
    {
        if (System.IO.File.Exists(summary.getFileName())) System.IO.File.Delete(summary.getFileName());
    }

    public void SaveFile()
    {       
        SaveData save_data = new SaveData(summary.savegame_filename);
        save_data["summary"] = summary;
        save_data["save_states"] = save_states;

        DeleteFile();
        save_data.Save(summary.getFileName());
    }

    public void CheckFile()
    {
        summary.savegame_filename = Application.persistentDataPath + "/" + summary.name + ".uml";
        Debug.Log("Checking file " + summary.savegame_filename + "\n");
        if (System.IO.File.Exists(summary.savegame_filename))
        {
            LoadFile();
            UpdateDescription();
        }
        else
        {
            InitNewGame();
            UpdateDescription();
        }
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
        SaveFile();
        LoadFile();
    }

    public void InitNewGame()
    {
        Debug.Log("Initializing new game\n");
        save_states = new SaveState[2];
        SaveState persistent = new SaveState();
        persistent.type = SaveStateType.Persistent;
        persistent.current_level = -1;
        save_states[persistent_id] = persistent;
        save_states[midlevel_id] = null;
        UpdateDescription();
        SaveFile();
    }


    public void UpdateDescription()
    {

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
