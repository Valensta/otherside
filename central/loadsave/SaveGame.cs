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
    public string savegame_version;
    
    public int id;



    

    public string getFileName()
    {
        return Get.savegame_location + name + SaveData.extension;
    }
}

[System.Serializable]
public class SaveGame
{
    public SaveGameSummary summary;
    public SaveState persistent;    
    public SaveState[] midlevels; // 3 for 3 rolling waves
    private string rawText;
    public string description;//break it out into an array or something, contails current level, score
    private bool _isLoaded;
    //int midlevel_id = 1;
    //int persistent_id = 0;

    public string getRawText()
    {
        return rawText;
    }
        
    public bool isLoaded()
    {
        return _isLoaded;
    }
    private void isLoaded(bool set_isloaded)
    {
        _isLoaded = set_isloaded;
    }

    public int getMaxLevel()
    {
        int current_lvl = getCurrentLevel();

        SaveState state = persistent;
        if (state.score_details == null) return Mathf.Max(0, current_lvl);

        return Mathf.Max(state.score_details.Count, current_lvl);
    }


    public SaveState getOldestMidLevel()
    {
        SaveState me = null;
        foreach (SaveState state in midlevels)
        {
            if (me == null || me.current_wave > state.current_wave) me = state;
        }
        return me;
    }

    public SaveState getLatestMidLevel()
    {
        SaveState me = null;
        if (midlevels == null) return null;
        foreach (SaveState state in midlevels)
        {
            if (state.current_wave < 0) continue;
            if (me == null || me.current_wave < state.current_wave) me = state;
        }
        return me;
    }

    public int getCurrentLevel()
    {
        SaveState state = getLatestMidLevel();
        if (state == null || state.current_level < 0) state = persistent;

        return (state == null) ? -1 : state.current_level;
    }

    public int getCurrentLevelDisplay()
    {
        int l = getCurrentLevel();

        return (l == -1) ? l : l + 1;
    }

    public int getCurrentWave()
    {
        SaveState state = getLatestMidLevel();
        
        if (state == null || state.current_level < 0 || !state.isValid()) return -1;

        return state.current_wave;
    }

    public Difficulty getCurrentDifficulty()
    {
        SaveState state = getLatestMidLevel();
        if (state == null || state.current_level < 0) return Difficulty.Normal;

        return state.difficulty;
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
        persistent = save_data.GetValue<SaveState>("persistent");
        midlevels = save_data.GetValue<SaveState[]>("midlevels");
    }


    public void ResetMidLevelState(SaveWhen type)
    {
        if (!(type == SaveWhen.MidLevel || type == SaveWhen.GoBack1 || type == SaveWhen.GoBack2)) return;
        int latest_wave = getLatestMidLevel().current_wave;

        int looking_for_wave = (type == SaveWhen.MidLevel)? latest_wave : (type == SaveWhen.GoBack1) ? latest_wave - 1 : latest_wave - 2;

        if (looking_for_wave <= 0) return;

        for (int i = 0; i < midlevels.Length; i++)
        {
            if (midlevels[i].current_wave == looking_for_wave)
            {
                midlevels[i] = getMidLevelState(type);
                midlevels[i] = new SaveState();
                midlevels[i].current_wave = -1;
            }
        }
        
    }

    public SaveState getMidLevelState(SaveWhen type)
    {
        if (!(type == SaveWhen.MidLevel || type == SaveWhen.GoBack1 || type == SaveWhen.GoBack2)) return null;

        int latest_wave = getLatestMidLevel().current_wave;
        int looking_for_wave = (type == SaveWhen.MidLevel) ? latest_wave : (type == SaveWhen.GoBack1) ? latest_wave - 1 : latest_wave - 2;
        if (looking_for_wave < 0) return null;

        foreach (SaveState state in midlevels)
        {
            if (state.current_wave == looking_for_wave) return state;
        }
        return null;
    }

    public SaveState getSaveState(SaveStateType type)
    {
        if (persistent == null || midlevels == null || midlevels.Length < Get.midlevel_savegames) InitNewGame(summary.id, summary.name);

        if (type == SaveStateType.MidLevel) return getLatestMidLevel();
        else return persistent;
        

    }

    public void setNewMidlevel(SaveState state) //
    {
        if (state.type == SaveStateType.Persistent)
        {
            Debug.LogError("Set new midlevel on persistent??\n");
            return;
        }

        if (persistent == null || midlevels == null || midlevels.Length < Get.midlevel_savegames)
        {
            InitNewGame(summary.id, summary.name);
        }else
        {
            InitMidLevel();
        }

        midlevels[0] = state;
        
    }

    public bool LoadFile(string alt_filename)
    {


        if (alt_filename.Equals("") && isLoaded()) { return true; }
        string filename = (alt_filename.Equals("")) ? summary.getFileName() : alt_filename;

        try
        {
            SaveData save_data = SaveData.Load(filename);                       
            SaveGameSummary newSummary = save_data.GetValue<SaveGameSummary>("summary");

                if (!newSummary.savegame_version.Equals(Central.Instance.game_saver.savegame_version))
            {                
                DeleteFile();
                Debug.Log("Found Savegame version mismatch " + newSummary.savegame_version + " does not match game version " + Central.Instance.game_saver.savegame_version + "\n");
                
                return false;
            }

            persistent = save_data.GetValue<SaveState>("persistent");
            midlevels = save_data.GetValue<SaveState[]>("midlevels");

            if (getCurrentWave() == -1) rawText = save_data.raw_text;

            summary = newSummary;
        }
        catch(Exception e)
        {
            DeleteFile();
            Debug.LogError($"Deleting file {filename}\n");
            Debug.LogError($"{e.Message}\n");
           // Tracker.ThrowNonFatal(e.ToString());
            return false;
        }


        isLoaded(true);
        return true;
    }
    
    public void Unload()
    {

        persistent = null;
        midlevels = null;
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
        string junk_file = Get.savegame_location + "junk" + SaveData.extension;

        if (System.IO.File.Exists(junk_file)) System.IO.File.Delete(junk_file);
            
        SaveData save_data = setSaveData(junk_file);

        save_data.Save(junk_file);
        LoadFile(junk_file);

        if (System.IO.File.Exists(junk_file)) System.IO.File.Delete(junk_file);
    }


    public bool ForceGame(string raw_text)
    {
        this.rawText = raw_text;
        try
        {
            SaveData.SaveRaw(summary.getFileName(), raw_text);            
        }catch (Exception e)
        {
            Debug.LogError("Could not save raw file into " + summary.getFileName() + ": " + e.Message + "\n");
            return false;
        }
        return true;
    }
    public void SaveFile()
    {
     //   Debug.Log("SAVING FILE!!!!\n");
        SaveData save_data = setSaveData(summary.getFileName());        

        DeleteFile();
        save_data.Save(summary.getFileName());
        
    }

    public SaveData setSaveData(string filename)
    {
        SaveData save_data = new SaveData(filename);
        save_data["summary"] = summary;
        save_data["persistent"] = persistent;
        save_data["midlevels"] = midlevels;

        return save_data;
    }

    public void ReloadFile(string copy_from)
    {
     //   Debug.Log("Copying from " + copy_from + " to " + summary.Savegame_filename + "\n");        
        SaveData save_data = SaveData.Load(copy_from);        

        midlevels = save_data.GetValue<SaveState[]>("midlevels");
        persistent = save_data.GetValue<SaveState>("persistent");

        SaveFile();
        UpdateDescription();
    }


    public bool CheckFile()
    {
        //summary.Savegame_filename = summary.getFileName() + SaveData.extension;
    //    Debug.Log("Checking file " + summary.Savegame_filename + "\n");
        if (System.IO.File.Exists(summary.getFileName()) && LoadFile(""))
        {
            UpdateDescription();            
        }
        else
        {
            InitNewGame(summary.id, summary.name);
            UpdateDescription();
        }
        return true;
    }

    public void InitNewGame(int id, string name)
    {
        //Debug.Log("Initializing new game\n");
        InitMidLevel();
        persistent = new SaveState();
        persistent.type = SaveStateType.Persistent;
        persistent.current_level = -1;
                        
        summary.savegame_version = Central.Instance.game_saver.savegame_version;
        summary.name = name;
        summary.id = id;
        UpdateDescription();
        SaveFile();
    }

    public void InitMidLevel()
    {
        midlevels = new SaveState[Get.midlevel_savegames];
        for (int i = 0; i < Get.midlevel_savegames; i++)
        {
            SaveState hey = new SaveState();
            hey.type = SaveStateType.MidLevel;
            hey.current_wave = -1;
            midlevels[i] = hey;
        }
    }

    string _updateDescription()
    {    
        if (!_isLoaded) return "";
        if (midlevels == null || midlevels.Length == 0 || persistent == null || getCurrentLevel() < 0)
        {
            return "New Game";
        }
        else
        {
            return $"LVL: {getCurrentLevelDisplay()}";            
        }
    }


    public void UpdateDescription()
    {

        if (!_isLoaded) return;

        description = _updateDescription();
        if (midlevels == null || midlevels.Length == 0 || persistent == null || getCurrentLevel() < 0)
        {
            return;
        } else
        {            
            int current_wave = getCurrentWave();
            if (current_wave > 0) description += " Wave: " + (current_wave + 1);
            string diff = Get.getDifficultyText(getCurrentDifficulty());             
            if (!diff.Equals("")) description += "    (" + diff + ")";
        }
    }

    public string getScoreText()
    {
        return "Score: " + Get.Round(persistent.total_score, 0).ToString();
    }
    public string getDescription(bool brief)
    {
        if (brief) return _updateDescription();
        UpdateDescription();
        return description;
    }
}
