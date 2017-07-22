using System.Collections.Generic;
using UnityEngine;

using System.IO;

public class MultiLevelStateSaver : MonoBehaviour {

    public delegate void onSaveCompleteStartWaveHandler();
    public static event onSaveCompleteStartWaveHandler onSaveCompleteStartWave;
    public string savegame_version;    //THIS IS SET ON THE GAMEOBJECT
    private int current_savegame_id = -1;
    public SaveStateType current_state;
   // public int selected_savegame_id = -1;

    public List<SaveGame> games; // all saved game
    public GameObject savegame_panel;
    public List<string> preloaded_games;

    public MySaveGameButton[] savegame_buttons;

    public void toggleSaveGamePanel()
    {
      //  Debug.Log("Toggle savegame panel to " + !savegame_panel.activeSelf + "\n");
        savegame_panel.SetActive(!savegame_panel.activeSelf);
        if (savegame_panel.activeSelf) Init();
    }

    public void OnEnable()
    {
        //Init();

        Get.savegame_location = Application.persistentDataPath + "/";
        Get.preloadgame_location = Application.persistentDataPath + "/";
        DumpPremadeSaveGames();
    }

    public void DeleteAllSaveGames()
    {

    }

    public void Init()
    {
        for (int i = 0; i < games.Count; i++)
        {
            InitButton(i);
        }
    }

    public bool ForceSaveGame(int id, string raw_text)
    {
        return games[id].ForceGame(raw_text);
    }

    public bool DeleteSaveGame(int id)
    {
        if (games[id].DeleteFile())
        {
            InitButton(id);
            return true;
        }
        else
        {

            Debug.Log("Found no file to delete for id " + id + "\n");
            return false;
        }

        
    }

    public bool DumpPremadeSaveGames()
    {//move premade save games from Preload/ to regular savegame dir

        foreach (string file in preloaded_games)
        {
            try
            {//Preload/lvl_1
                UnityEngine.Object hey = Resources.Load("Preload/" + file);
                string stuff = ((TextAsset)hey).ToString();
                string save_to = Get.savegame_location + file + SaveData.extension;
                if (System.IO.File.Exists(save_to)) System.IO.File.Delete(save_to);

                StreamWriter writer = new System.IO.StreamWriter(save_to);
                writer.Write(stuff);
                writer.Close();               
            }
            catch (System.Exception e)
            {
                Debug.Log("Failed to dump preloaded game (" + file + ") : " + e.ToString());
                Tracker.ThrowNonFatal("Failed to dump preloaded game (" + file + ") : " + e.ToString());
                return false;
            }
        }

        return true;
    }

    public bool CopyPremadeSaveGame(int id, string preload)
    {

        //string copy_from = Get.preloadgame_location + "preload_lvl_" + name + ".uml";
        string copy_from = Get.preloadgame_location + preload + SaveData.extension;

        if (!System.IO.File.Exists(copy_from))
        {
            Debug.Log("Cound not find " + copy_from + "\n");
            return false;
        }        

        games[id].ReloadFile(copy_from);
        savegame_buttons[id].level.text = games[id].getDescription(false);
        savegame_buttons[id].score.text = games[id].getScoreText();
        return true;
    }

    public void InitButton(int id)
    {
        bool check_ok = games[id].CheckFile();

        savegame_buttons[id].level.text = games[id].getDescription(false);
        if (check_ok)
            savegame_buttons[id].score.text = games[id].getScoreText();
        else
        {
            DeleteSaveGame(id);
            savegame_buttons[id].score.text = games[id].getScoreText();
        }
    }

    public SaveGame getCurrentGame()
    {
        return games[current_savegame_id];
    }

    public void SelectSaveGame(int id, bool set)
    {
        if (current_savegame_id > 0) games[current_savegame_id].Unload();
        current_savegame_id = id;

        games[current_savegame_id].LoadFile("");

        savegame_buttons[current_savegame_id].SetSelectedToy(false);
        
          
        savegame_panel.SetActive(false);

        //int lvl = games[current_savegame_id].getCurrentLevel();
        int lvl = games[current_savegame_id].getMaxLevel();
        if (lvl >= 0) Central.Instance.level_list.setMaxLvl(lvl);


        if (games[current_savegame_id].getSaveState(SaveStateType.MidLevel) != null 
            && games[current_savegame_id].getSaveState(SaveStateType.MidLevel).current_level >= 0 && games[current_savegame_id].getSaveState(SaveStateType.MidLevel).isValid())
        {
            Central.Instance.changeState(GameState.Loading, "LoadSnapshot");
        }
        else
        {
            Central.Instance.changeState(GameState.LevelList, "ToMap");
        }
    }

  

    public int getCurrentLevel()
    {
        if (current_savegame_id >= 0)
        {
            return games[Current_savegame_id].getCurrentLevel();
            
        }else
        {
            return -1;
        }
    }
  
    


    public bool LoadGame(SaveWhen type)
    {
        //if (!(type == SaveWhen.MidLevel || type == SaveWhen.BetweenLevels|| type == SaveWhen.BeginningOfLevel))
        if (type == SaveWhen.Null || type == SaveWhen.EndOfLevel)
        {
            Debug.Log("Cannot load game, unsupported SaveWhen type " + type + "\n");
            return false;
        }

        
        if (FakeRunner.Instance != null && FakeRunner.Instance.hasFakePlayer()) FakeRunner.Instance.current_player.fake_player.Stop();
        RewardOverseer.RewardInstance.StopOverseer();

        games[Current_savegame_id].LoadFile("");

        //bool ok = true;

        SaveState midlevel = games[current_savegame_id].getMidLevelState(type);
        SaveState persistent = games[current_savegame_id].getSaveState(SaveStateType.Persistent);

        Debug.Log("!!!!!!! Loading game " + type + "\n");
        switch (type)
        {
            case SaveWhen.GoBack1:        
            case SaveWhen.GoBack2:                
            case SaveWhen.MidLevel:
                //load

                Tracker.Log(PlayerEvent.LoadGame,false,
                    customAttributes: new Dictionary<string, string>() {
                        { "attribute_2", Peripheral.Instance.difficulty.ToString()},                        
                        { "attribute_1", type.ToString() } },
                    customMetrics: new Dictionary<string, double> { { "metric_1", midlevel.current_level } });

                if (type == SaveWhen.GoBack1) games[current_savegame_id].ResetMidLevelState(SaveWhen.MidLevel);
                if (type == SaveWhen.GoBack2)
                {
                    games[current_savegame_id].ResetMidLevelState(SaveWhen.MidLevel);
                    games[current_savegame_id].ResetMidLevelState(SaveWhen.GoBack1);
                }

                midlevel.LoadRewards();
                midlevel.LoadSkillsInventory(false);
                Peripheral.Instance.LoadBasicMidLevelShit(midlevel);
               
                midlevel.LoadWishes();
                //persistent.LoadScore();                
                midlevel.LoadScore(false);
                midlevel.LoadHeroStats();                                
                midlevel.LoadEvents();                
                midlevel.LoadToyStats();

                //if (FakePlayer.Instance != null) midlevel.LoadFakePlayer();
                if (FakeRunner.Instance != null && FakeRunner.Instance.hasFakePlayer()) midlevel.LoadFakePlayer();

                Central.Instance.updateCost(Peripheral.Instance.getToys());
                EagleEyes.Instance.UpdateToyButtons("blah", ToyType.Null, false); // UpdateToyButtons does not happen by default on changeState
                Peripheral.Instance.my_skillmaster.my_panel.UpdatePanel();

                    
                Central.Instance.changeState(GameState.InGame);
                Peripheral.Instance.Pause(false);
                if (type == SaveWhen.GoBack1 || type == SaveWhen.GoBack2) games[current_savegame_id].SaveFile();

                break;

            case SaveWhen.BetweenLevels:                    //load

                
                persistent.LoadScore(true);
                persistent.LoadHeroStats();
                persistent.LoadRewards();
                persistent.LoadWishes(); //marketplace
                //persistent.LoadToyStats(); ???!!
                persistent.LoadSkillsInventory(true);

                if (midlevel != null)
                {
                    midlevel.resetMidLevelStuff(); //lose all progress. yes, forever.
                    games[current_savegame_id].SaveFile();
                    games[current_savegame_id].LoadFile("");
                }
                break;


            case SaveWhen.BeginningOfLevel:                    //load
       Debug.Log("Loading Beginning Of Level\n");

                
                
                Monitor.Instance.ResetIslands();

                midlevel = new SaveState();
                midlevel.type = SaveStateType.MidLevel;
                midlevel.current_level = persistent.current_level;
                midlevel.hero_stats = persistent.hero_stats;
                midlevel.score_details = persistent.score_details;
                midlevel.actor_stats = persistent.actor_stats;
                midlevel.wishes = persistent.wishes;
                midlevel.rewards = persistent.rewards;
                midlevel.skills_in_inventory = persistent.skills_in_inventory;

                games[current_savegame_id].setNewMidlevel(midlevel);

                games[current_savegame_id].SaveAndReload(); // deep copy

                persistent.LoadRewards();
                persistent.LoadToyStats();
                persistent.LoadHeroStats();                
                persistent.LoadSkillsInventory(true);                
                persistent.LoadWishes();
                persistent.LoadScore(true);
                persistent.LoadEvents(); //still need to reset this                               

                Tracker.Log(PlayerEvent.LoadGame, false,
                    customAttributes: new Dictionary<string, string>() {                        
                        {"attribute_2", Get.getCarryOverInventory() },
                        { "attribute_1", Peripheral.Instance.difficulty.ToString() + "|" + type.ToString() } },
                    customMetrics: new Dictionary<string, double> { { "metric_1", persistent.current_level } });

                Central.Instance.updateCost(0);
                Central.Instance.changeState(GameState.InGame);

                break;
        }
        RewardOverseer.RewardInstance.StartOverseer() ;
        return true;

    }

    public void MidLevelTestSaveGame()
    {
        SaveGame(SaveWhen.MidLevel);
    }

    public void SaveGame(SaveWhen type)
    {
        if (LevelBalancer.Instance.am_enabled) return;
       // Debug.Log("maybe saving game " + type + "\n");
        if (!(type == SaveWhen.MidLevel || type == SaveWhen.EndOfLevel || type == SaveWhen.BetweenLevels))
        {
            Debug.Log("Cannot save game, unsupported SaveWhen type " + type + "\n");
            return;
        }

        
        EagleEyes.Instance.RunSavingGameVisual();

       Debug.Log("!!!SAVING GAME " + type + "\n");

        SaveState midlevel = games[current_savegame_id].getOldestMidLevel();
        SaveState persistent = games[current_savegame_id].getSaveState(SaveStateType.Persistent);

        switch (type)
        {
            case SaveWhen.MidLevel: //save

                
                midlevel.SaveBasicMidLevelShit();
                midlevel.type = SaveStateType.MidLevel;
                midlevel.SaveIslands();  //ToySaver and other stuff ok
                midlevel.SaveWishes();//deepclone
                midlevel.SaveEvents();  //meh int and a list of strings that never change
                midlevel.SaveRewards();//deepclone
                midlevel.SaveSkillInventory();    //deepclone
                midlevel.SaveHeroStats();  //list of RuneSavers getSnapshot
                midlevel.SaveToyStats(); // list of unitStatSavers getSnapshot
                midlevel.SaveScore(); // need to save possible wishes etc
                midlevel.SaveFakePlayer();

                break;
            case SaveWhen.EndOfLevel:   //save

                persistent.SaveWishes(); 
                persistent.SaveToyStats();
                persistent.SaveHeroStats();
                persistent.SaveRewards();               
                persistent.SaveSkillInventory();
                persistent.SaveScore();
                persistent.current_level = Central.Instance.getCurrentLevel();              
                persistent.current_level++;                
                games[current_savegame_id].InitMidLevel();                 

                //INCREMENT LEVEL SOMEWHERE
                break;
            case SaveWhen.BetweenLevels:   //save
                
                persistent.SaveScore();
                persistent.SaveHeroStats();
                persistent.SaveSkillInventory();
                persistent.SaveWishes(); //marketplace
                persistent.SaveRewards(); //for intralevel events
                break;

        }

        games[current_savegame_id].SaveFile();
        games[current_savegame_id].LoadFile("");
        if (onSaveCompleteStartWave != null) onSaveCompleteStartWave();
    }

    public int Current_savegame_id
    {
        get
        {
            return current_savegame_id;
        }

        set
        {
            current_savegame_id = value;
        }
    }
}