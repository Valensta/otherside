using System.Collections.Generic;
using UnityEngine;


public class MultiLevelStateSaver : MonoBehaviour {

    public delegate void onSaveCompleteStartWaveHandler();
    public static event onSaveCompleteStartWaveHandler onSaveCompleteStartWave;

    private int current_savegame_id = -1;
    public SaveStateType current_state;
   // public int selected_savegame_id = -1;

    public List<SaveGame> games; // all saved game
    public GameObject savegame_panel;

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
    }

    public void Init()
    {
        for (int i = 0; i < games.Count; i++)
        {
            InitButton(i);
        }
    }

    public void DeleteSaveGame(int id)
    {
        games[id].DeleteFile();
        InitButton(id);
    }

    public void InitButton(int id)
    {
        
        games[id].CheckFile();
        savegame_buttons[id].level.text = games[id].getDescription();
        savegame_buttons[id].score.text = games[id].getScoreText();
    }

    public SaveGame getCurrentGame()
    {
        return games[current_savegame_id];
    }

    public void SelectSaveGame(int id, bool set)
    {
        if (current_savegame_id > 0) games[current_savegame_id].Unload();
        current_savegame_id = id;

        games[current_savegame_id].LoadFile();

        savegame_buttons[current_savegame_id].SetSelectedToy(false);
        
          
        savegame_panel.SetActive(false);

        if (games[current_savegame_id].getSaveState(SaveStateType.MidLevel) != null && games[current_savegame_id].getSaveState(SaveStateType.MidLevel).current_level > 0)
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

    private bool validateCurrentSaveType(SaveStateType type)
    {
        if (Current_savegame_id < 0)
        {
            Debug.Log("No current game id defined, cannot load save game type " + type + "\n");
            return false;
        }

        if (games.Count <= current_savegame_id)
        {
            Debug.Log("Could not find game id " + current_savegame_id + "\n");
            return false;
        }

        if (games[current_savegame_id] == null)
        {
            Debug.Log("Game id " + current_savegame_id + " is NULL\n");
            return false;
        }

        if (games[current_savegame_id].getSaveState(type) == null)
        {
            Debug.Log("Game id " + current_savegame_id + " has no valid SaveGameState " + type + "\n");
            return false;
        }

        return true;
    }




    public bool LoadGame(SaveWhen type)
    {
        if (!(type == SaveWhen.MidLevel || type == SaveWhen.BetweenLevels|| type == SaveWhen.BeginningOfLevel))
        {
            Debug.Log("Cannot load game, unsupported SaveWhen type " + type + "\n");
            return false;
        }

        Debug.Log("Loading game " + type + "\n");

        games[Current_savegame_id].LoadFile();
        
        bool ok = true;

        SaveState midlevel = games[current_savegame_id].getSaveState(SaveStateType.MidLevel);
        SaveState persistent = games[current_savegame_id].getSaveState(SaveStateType.Persistent);

        switch (type)
        {
            case SaveWhen.MidLevel:

                Peripheral.Instance.LoadBasicMidLevelShit(midlevel);

                foreach (actorStats a in midlevel.actor_stats) { Central.Instance.setToy(a, a.toy_type == ToyType.Hero); }
                midlevel.LoadWishes();
                persistent.LoadScore();                
                midlevel.LoadHeroStats();                
                midlevel.LoadSkillsInventory(false);
                midlevel.LoadEvents();
                midlevel.LoadRewards();
                midlevel.LoadToyStats();

                Central.Instance.updateCost(Peripheral.Instance.getToys());
                EagleEyes.Instance.UpdateToyButtons("blah", ToyType.Null, false); // UpdateToyButtons does not happen by default on changeState
                Peripheral.Instance.my_skillmaster.my_panel.UpdatePanel();
                
                

                Peripheral.Instance.Pause(false);
                Central.Instance.changeState(GameState.InGame);

                break;

            case SaveWhen.BetweenLevels:

                
                persistent.LoadScore();
                persistent.LoadHeroStats();
                persistent.LoadSkillsInventory(true);                
                break;
                
            case SaveWhen.BeginningOfLevel:               
                Debug.Log("Loading Beginning Of Level\n");
                if (midlevel == null) midlevel = new SaveState();
                midlevel.type = SaveStateType.MidLevel;
                midlevel.current_level = persistent.current_level;
                midlevel.hero_stats = persistent.hero_stats;
                midlevel.score_details = persistent.score_details;
                midlevel.actor_stats = persistent.actor_stats;
                midlevel.wishes = persistent.wishes;
                midlevel.rewards = persistent.rewards;

                games[current_savegame_id].setSaveState(midlevel);
                


                games[current_savegame_id].SaveFile();
                games[current_savegame_id].LoadFile();

                persistent.LoadToyStats();
                persistent.LoadHeroStats();
                persistent.LoadSkillsInventory(true);
                persistent.LoadRewards();
                persistent.LoadWishes();
                persistent.LoadScore();

                Central.Instance.changeState(GameState.InGame);

                break;
        }

        return true;

    }

    public void MidLevelTestSaveGame()
    {
        SaveGame(SaveWhen.MidLevel);
    }

    public void SaveGame(SaveWhen type)
    {
        if (!(type == SaveWhen.MidLevel || type == SaveWhen.EndOfLevel || type == SaveWhen.BetweenLevels))
        {
            Debug.Log("Cannot save game, unsupported SaveWhen type " + type + "\n");
            return;
        }
        EagleEyes.Instance.RunSavingGameVisual();

        Debug.Log("Saving game " + type + "\n");

        SaveState midlevel = games[current_savegame_id].getSaveState(SaveStateType.MidLevel);
        SaveState persistent = games[current_savegame_id].getSaveState(SaveStateType.Persistent);

        switch (type)
        {
            case SaveWhen.MidLevel:

                
                midlevel.SaveBasicMidLevelShit();
                midlevel.type = SaveStateType.MidLevel;
                midlevel.SaveIslands();
                midlevel.SaveWishes();
                midlevel.SaveEvents();
                midlevel.SaveRewards();
                midlevel.SaveSkillInventory();
                midlevel.SaveHeroStats();
                midlevel.SaveToyStats(); //why is this necessary - think to save status of available toys
                //saver.hero_stats = Central.Instance.getAllHeroStats();

                break;
            case SaveWhen.EndOfLevel:

                persistent.SaveWishes();
                persistent.SaveToyStats();
                persistent.SaveHeroStats();
                persistent.SaveRewards();
                persistent.SaveEvents();
                persistent.SaveSkillInventory();
                persistent.SaveScore();
                persistent.current_level = Central.Instance.getCurrentLevel();
              //  Debug.Log("hey current level is " + saver.current_level + "\n");
                persistent.current_level++;
               //games[current_savegame_id].SaveMidLevelToPersistent();
                midlevel.resetMidLevelStuff();

                //INCREMENT LEVEL SOMEWHERE
                break;
            case SaveWhen.BetweenLevels:

             
                persistent.SaveScore();
                persistent.SaveHeroStats();
                persistent.SaveSkillInventory();
                break;

        }

        games[current_savegame_id].SaveFile();
        games[current_savegame_id].LoadFile();
        if (onSaveCompleteStartWave != null) onSaveCompleteStartWave();
    }



    /*
    public void getHeroStats(ToySaver saveme)
    {
        //do I need this?
        List<ToySaver> hero_toy_stats = Central.Instance.hero_toy_stats;
        for (int i = 0; i < hero_toy_stats.Count; i++)
        {
            if (hero_toy_stats[i].rune.runetype == saveme.rune.runetype)
            {
                hero_toy_stats[i] = saveme;
                return;
            }
        }
        hero_toy_stats.Add(saveme);
    }*/

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