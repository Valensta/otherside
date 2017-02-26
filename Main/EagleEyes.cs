using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System.Text.RegularExpressions;

[System.Serializable]
public class GUIState
{
	public GameState state;
	public Transform bucket;
    public FadeMe fademe;
	[SerializeField]
	public List<MyLabel> labels;
}


public class EagleEyes : MonoBehaviour {
	public Camera cam;
	GameState gameState;
	public bool menu;
	float w_to_h;
    public Canvas canvas;
	public Peripheral peripheral;
	public Central central;
	public GameObject start_game_confirm_panel;
	public GameObject to_map_confirm_panel;
	public Settings_Panel my_settings_panel;
	GameObject infobox; //current infobox, if any
    public Toy_Scroll_Driver toy_scroll_driver;
    public Toy_Scroll_Driver wish_scroll_driver;
    public RewardList_Toy_Button_Driver rewards_scroll_driver;
    public float camera_size = 1;
	public Vector3 camera_scale = new Vector3(1,1,1);
	public Vector3 camera_position;
	public Vector3 camera_rotation;
	public float x_coord;
	public float y_coord;
    public float max_x;
    public float max_y;
	SpriteRenderer health_visual = null;
	GameObject saving_game_visual = null;
	public InGame_Toy_Button_Driver global_rune_panel;
	//string wave_count = "";
	public static EagleEyes Instance { get; private set; }
    // Use this for initialization
    float default_map_x_tiles = 24f;
    float default_map_y_tiles = 15f;
    public GameObject events;
	public GameObject world_space_events;
	public List<GUIState> GUI_states = new List<GUIState> ();
	GUIState current_GUIState = null;
	public Dictionary<string, GameObject> elements = new Dictionary<string, GameObject>();
    public List<FFButton> ff_buttons = new List<FFButton>();
  //  public float tween_duration = 0.3f;
	public delegate void PriceUpdateHandler(string type, float price);
	public static event PriceUpdateHandler onPriceUpdate;
    public Island_Floating_Button_Driver floating_tower_scroll_driver;
    public MyButton pause_button;
    public Text wave_timer;

    public float getMapXSize() { return default_map_x_tiles; }

    public float getMapYSize() { return default_map_y_tiles; }

    public void setMapSize(float x, float y)
    {
        default_map_x_tiles = x;
        default_map_y_tiles = y;
     //   central.my_spyglass.map_x_size = default_map_x_tiles;
    //    central.my_spyglass.map_y_size = default_map_y_tiles;
        float screen_x = Screen.currentResolution.width;
        float screen_y = Screen.currentResolution.height;
        float screen_ratio = screen_x / screen_y;
        float default_ratio = 1920f / 1080f;

      //  float blocks = 10;
      //  float _base = screen_y - 320f;
      //  float ortho_size = 5f + 0.5f * (_base - (_base) % 224f) / 224f;
        float adjusted_y = 100f * default_ratio / screen_ratio;
       float ortho_size = 6.5f;
        Camera.main.orthographicSize = ortho_size;

       // float bg_sprite_scale = Monitor.Instance.background_image.gameObject.transform.localScale.x;
        //1.369641f; //whatever
       // Debug.Log("Screen ratio is " + screen_ratio + "\n");
        
        Camera.main.transform.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 78f);
       // max_y = (default_map_y_tiles / 2f - ortho_size) * bg_sprite_scale;
       // max_x = (default_map_x_tiles/2f - ortho_size * screen_ratio) * bg_sprite_scale; //not quite right
        //Central.Instance.my_spyglass.max_y = (default_map_y_tiles / 2f - ortho_size);
        //Central.Instance.my_spyglass.max_x = (default_map_x_tiles / 2f) - ortho_size * default_ratio; //eh this ain't right
        for (int i = 0; i < GUI_states.Count; i++)
        {
            GUI_states[i].bucket.transform.localScale = new Vector3(100f, adjusted_y, 1f);
        }
    }

	void InitElements(){
	
		for (int i = 0; i < GUI_states.Count; i++){
		//	GUI_states[i].bucket.transform.localScale = new Vector3(100f, adjusted_y, 1f);
            if (GUI_states[i].fademe == null) GUI_states[i].fademe = GUI_states[i].bucket.GetComponent<FadeMe>();
            for (int j = 0; j < GUI_states[i].bucket.childCount; j++){


				GameObject child = GUI_states[i].bucket.GetChild(j).gameObject;
			//	Debug.Log(GUI_states[i].state + " Got child " + child.name);
				elements.Add(child.name, child);
					
			}
		}
	}


	//InGame, Inventory, Won, Lost, MainMenu, Null, WonGame

	public void DisplayConfirmPanel(MenuButton type, bool state){
		if (type == MenuButton.Start){			
			start_game_confirm_panel.SetActive(state);
		}else	
		if (type == MenuButton.ToMap){			
			to_map_confirm_panel.SetActive(state);
		}
	}

    void Start() {

        InitElements();
        //my_settings_panel.SetVolume(5);
        ClearGUI(GameState.Null);
        ScoreKeeper.onScoreUpdate += onScoreUpdate;
        //	peripheral = (GameObject.Find ("Peripheral")).gameObject.GetComponentInParent<Peripheral>();
        peripheral = Peripheral.Instance;

        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        }

        Instance = this;
        central = Central.Instance;
        //	cameras.Add(GameObject.Find("MAINCAMERA").transform);

        x_coord = Screen.width;
        y_coord = Screen.height;


        UICamera.genericEventHandler = this.gameObject;
        PlaceState(GameState.MainMenu);
    }
     
// Update is called once per frame


    public void SetFFButtons(bool set)
    {        
        foreach (FFButton b in ff_buttons)
            b.SetActiveState(set);

        if (!set) Peripheral.Instance.ResumeNormalSpeed();
    }

    Vector3 endpoint()
    {
        Vector3 endpoint = Vector3.forward * 1000;
        return endpoint;
    }



    void PlaceButtons(string state, bool bg, List<MenuButton> buttons)
    {
        int i = buttons.Count;

        if (bg)
        {
            PlaceElement("mainmenu_bg");//, state, 0, 0, 0f, 1, 1,true);
        }

        PlaceElement("mainmenu_button_bg");//, state, 0, 0, 0f, 1, 1,true);
        foreach (MenuButton b in buttons)
        {
            switch (b)
            {
                case MenuButton.Start:
                    PlaceElement("mainmenu_play");//, state, 0, start_pos + i *segment_height, 1f, button_height*4, button_height, false);
                                                  //PlaceElement ("mainmenu_confirm_start_game");
                                                  //DisplayConfirmPanel(MenuButton.Start, false);
                    i--;
                    break;
                case MenuButton.Continue:
                    PlaceElement("mainmenu_continue");
                    i--;
                    break;
                case MenuButton.LoadSnapshot:
                    switch (Central.Instance.state)
                    {
                        case GameState.InGame:
                            PlaceElement("mainmenu_restartwave");
                            break;
                        case GameState.MainMenu:
                            PlaceElement("mainmenu_play");
                            break;
                        default:
                            Debug.Log("Trying to place MenuButton LoadSnapshot during an invalid GameState " + Central.Instance.state + "\n");
                            break;
                    }
                    i--;
                    break;
                case MenuButton.LoadStartLevelSnapshot:
                    PlaceElement("mainmenu_restartlevel");
                    i--;
                    break;

                case MenuButton.Quit:
                    PlaceElement("mainmenu_quit");//, state, 0, start_pos + i *segment_height, 1f, button_height*4, button_height, false);
                    i--;
                    break;
                case MenuButton.Settings:
                    PlaceElement("mainmenu_settings");
                    PlaceElement("mainmenu_settings_panel");
                    my_settings_panel.DisablePanel();
                    i--;
                    break;
                case MenuButton.Rewards:
                    PlaceElement("mainmenu_rewards");
                    PlaceElement("mainmenu_rewards_panel");
                    rewards_scroll_driver.DisableMe();
                    i--;
                    break;
                case MenuButton.ToMap:
                    PlaceElement("mainmenu_to_map");//, state, 0, start_pos + i *segment_height, 1f, button_height*4, button_height, false);
                    PlaceElement("mainmenu_confirm_to_map");
                    DisplayConfirmPanel(MenuButton.ToMap, false);
                    i--;
                    break;
                case MenuButton.ToMainMenu:
                    //PlaceElement ("GUI/mainmenu_to_mainmenu", state, 0, start_pos + i *segment_height, 1f, button_height*4, button_height, false);
                    PlaceElement("mainmenu_to_mainmenu");
                    i--;
                    break;
            }

        }

    }


	void PlaceInGameButtons(){
        wish_scroll_driver.Init();
        toy_scroll_driver.Init();

        UpdateToyButtons ("blah",ToyType.Null, true);
	}



    IEnumerator UpdateInGame() { 
        while (true)
        {
            _UpdateInGame();
            yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(0.25f));
        }
    }


    void onScoreUpdate(float score)
    {
    //    Debug.Log("Onscoreupdate\n");
        LabelUpdate("score_status", Get.Round(score,1));
    }

    void onWishChanged(Wish a, bool added, bool visible, float delta){
   //     Debug.Log("Got onwishchanged\n");
		WishUpdate(a, added, visible);
        UpdateToyButtons("Blah", ToyType.Temporary, false);
	}
	
	//void onWaveEnd(int i){
		//WaveCountUpdate();
	//}
	
	void onHealthChanged(float i){
        //LabelUpdate("points_status", i);
		HealthUpdate(i, true);
	}
	
	
	void onDreamsChanged(float i, bool visual, Vector3 pos){
        
       // LabelUpdate("dreams_status", Mathf.Round(Peripheral.Instance.getDreams() * 10f) / 10f);
        DreamUpdate(Peripheral.Instance.getDreams(), true, pos);
	}

    void LabelUpdate(string label, float i)
    {
    //    Debug.Log("Label update " + label + " " + i + "\n");
        foreach (MyLabel l in current_GUIState.labels)
        {

            if (l == null) continue;
          //  if (l.type != "ingame") continue;
            if (l.text == null) continue;
       //     Debug.Log(l.content + " " + label + "\n");
            if (l.content == label)
            { 
                //Debug.Log("!\n");
                l.text.text = i.ToString();
                return;
            }   
        }

    }

    void DreamUpdate(float i, bool visual, Vector3 pos)
    {
        foreach (MyLabel l in current_GUIState.labels)
        {

            if (l == null) continue;
            //	if (l.type != "ingame") continue;
            if (l.text == null) continue;
            
            switch (l.content)
            {
                case "dreams_status":
                    //Debug.Log("!\n");
                    l.text.text = (Mathf.Round(i * 10f) / 10f).ToString();
                    if (visual)
                    {

                        
                       // l.tweener.duration = tween_duration;
                        l.tweener.Init();
                        if (l.number != -1)
                        {
                         //   MakeFloaty(pos, i - l.number, "GUI/plus_dreams_floaty");
                        }
                        
                    }
                    l.number = i;
                    break;
                default:
                    break;
            }
        }
    }

    public void MakeFloaty(Vector3 pos, float number, string floaty_name)
    {

        return;
        if (Mathf.Abs(number) < 0.1) return;

        Stat_Change_Floaty floaty = Zoo.Instance.getObject(floaty_name, false).GetComponent<Stat_Change_Floaty>();
        floaty.Init(pos, number);

        floaty.gameObject.SetActive(true);
    }

    public void WaveCountUpdate(){
		float dreams = peripheral.getDreams ();
		// Debug.Log("Update in game " + current_GUIState.state + "\n");
		foreach (MyLabel l in current_GUIState.labels){
			
			if (l == null) continue;
			if (l.type != "ingame") continue;
			if (l.text == null) continue;
            switch (l.content)
            {

                case "wave_count":
                    l.text.text = Moon.Instance.getWaveText();
                    break;

                default:
                    break;
            }
		}	
	}




    public void HealthUpdate(float i, bool visual)
    {
        float dreams = peripheral.getDreams();
        // Debug.Log("Update in game " + current_GUIState.state + "\n");
        foreach (MyLabel l in current_GUIState.labels)
        {

            if (l == null) continue;
            if (l.type != "ingame") continue;
            if (l.text == null) continue;
            switch (l.content)
            {

                case "points_status":
                    //	Debug.Log("!\n");
                    l.text.text = i.ToString();
                    if (visual)
                    {
                  //      l.tweener.duration = tween_duration;
                  
                        l.tweener.Init();
                    }
                    break;
                default:
                    break;
            }
        }
    }
	
	
	public void onSelected(SelectedType type, string n){

        if (type == SelectedType.Toy) SelectedToyUpdate(n);
	}
	
	void SelectedToyUpdate(string name){
    //    Debug.Log("Selected toy update " + name + "\n");
        
		string asdf = name;
		asdf = Regex.Replace (asdf, "_", " ");
        if (asdf == "") asdf = "-o-";
	//	Debug.Log("Selected toy update " + name + "\n");
		foreach (MyLabel l in current_GUIState.labels){			
			if (l == null) continue;
			if (l.type != "ingame") continue;
			if (l.text == null) continue;
			if (l.content == "ingame_selected_toy"){	
		//		Debug.Log("!\n");		
				l.text.text = asdf;
			}			
		}	
	}
	
	public void WishUpdate (Wish w,bool added, bool visual){
        if (w.type == WishType.Sensible)
        {
            foreach (MyLabel l in current_GUIState.labels)
            {
                if (l == null) continue;
                if (l.type != "ingame") continue;
                if (l.text == null) continue;
                if (l.content == "sensible_wish_status")
                {
                    //	Debug.Log("!\n");
                    l.text.text = (w.Strength).ToString();
                    if (visual)
                    {
                      //  l.tweener.duration = tween_duration;
                        l.tweener.Init();
                    }
                }
            }
            return;
        }
        

		

	}

    void _UpdateInGame(){ 
		foreach (MyLabel l in current_GUIState.labels){
			
			if (l == null) continue;
			if (l.type != "ingame") continue;
            if (l.text == null) continue;
			switch (l.content)
			{
			case "time_status":			
				if (Time.timeScale > 0){
				//	Debug.Log("!\n");
					l.text.text = (peripheral.TIME).ToString();
				}else{
					Debug.Log("!\n");
				//	l.text.text = "...";
				}
				break;							
			case "fastforward_status":
				if (Time.timeScale > 1f){
					l.text.text = Time.timeScale.ToString() + "x";
				}else if (Time.timeScale < 1f){
					l.text.text = "ZZZ";
				}else{					
					l.text.text = "...";//Debug.Log("!\n");
					
				}
				break;
			default:
				break;
			}
		}
	}

    public ErrorType canBuildToy(string name, Cost toy_cost, unitStats toy)
    {
        

        bool vocal = false;
        if (!Central.Instance.getToy(name).isUnlocked)
        {
            if (vocal) Debug.Log("Peripeheral does not have toy " + name + "\n");
            return ErrorType.No;
        }
        else if (!Peripheral.Instance.canBuildToy(toy))        
        {

            if (vocal) Debug.Log(name + "toy is not active\n");
            return ErrorType.No;
        }
        else if (!peripheral.HaveResource(toy_cost))
        {
            if (vocal) Debug.Log("Can't afford " + name + "\n");
            return ErrorType.NoResources;

        }
        return ErrorType.Yes;
    }

    public void UpdateToyButtons(string mytoy, ToyType kind, bool reset)
    {
        bool vocal = false;
        string selected_toy = peripheral.getSelectedToy();

        if (current_GUIState.state != GameState.InGame) {
            //Debug.Log("UpdateToyButtons is in " + current_GUIState.state + ", not in game state!\n");
            return;
        }
        if (vocal) Debug.Log("UpdateToyButtons running " + kind + "\n");

       // Debug.Log("Beep\n");
        foreach (MyLabel l in current_GUIState.labels)
        {
            if (l == null || l.toytype == ToyType.Null) continue;
            if (l.toytype != kind && kind != ToyType.Null) continue;
            if (reset) l.ResetButton();
            string name = l.content;

            float cost = 9999;
            unitStats toy = central.getToy(name);
            Cost toy_cost = null;
            if (toy != null) toy_cost = toy.cost_type;

            if (toy_cost != null) cost = toy_cost.Amount;

            switch (canBuildToy(name, toy_cost, toy))
            {
                case ErrorType.No:
                    l.SetActive(false);
                    break;
                case ErrorType.NoResources:
                    l.SetActive(true);
                    ClearInfo();
                    SetRowButton(l, false, false);
                    if (mytoy.Equals(name)) Central.Instance.button_selected = null;// is this used??!!
                    break;
                case ErrorType.Yes:                    
                    if (vocal) Debug.Log("Turning on toy " + l.content + "\n");
                    l.SetActive(true);
                    if (vocal) Debug.Log("Turning on thing " + selected_toy + " == " + name + "\n");
                    if (selected_toy == name)
                    {
                        if (l != null) l.SetSelected(true, true);
                        if (l != null) l.SetInteractable(true);
                    }
                    else
                    {
                        //if (l != null) l.SetSelected(true, false);
                        if (l != null) l.SetInteractable(true);
                    }
                    break;
            }
            string[] vars = new string[2];
            vars[0] = name;
            vars[1] = cost.ToString();
            StartCoroutine("DoOnPriceUpdate", vars);
        }

    }



	IEnumerator DoOnPriceUpdate(string[] vars){
		yield return new WaitForEndOfFrame();
		
		if (onPriceUpdate != null) { onPriceUpdate(vars[0], int.Parse(vars[1])); }
		yield return null;
	}


	public GUIState getState(GameState s){
		GUIState r = null;
		for (int i = 0; i < GUI_states.Count; i++) {
			if (GUI_states[i].state == s){ r = GUI_states[i];}
		}
		return r;
	}


	public void SetRowButton(MyLabel l, bool active, bool selected){

		if (l == null) return;
      
        if (l != null) l.SetSelected(selected, active);
        if (l != null) l.SetInteractable(active);
		//l.button.interactable = active;
		
	}


	//clear gui for all but the given state, duh
	public void ClearGUI(GameState state, bool menu){

        string string_state = state.ToString();
		foreach (Transform gamestate in canvas.transform) {
			if (!gamestate.name.Equals(string_state) && !gamestate.name.Equals(menu)){          
				Transform bucket = gamestate.transform;
				int c = bucket.transform.childCount;
				for (int i = 0; i < c; i++){			
					Transform child = bucket.GetChild(i);
					for (int j = 0; j < child.transform.childCount; j++){
						child.transform.GetChild(j).gameObject.SetActive(false);			
					}					
				}
			}
		}

	}
	


	public void ClearGUI(GameState s)	{
		if (s.Equals(GameState.Null)) {
			s = gameState;
		}

		ClearGUI(s, menu);
	}
    public void PlaceState(GameState s)
    {
        //	Debug.Log ("Eyes placing state " + s + "\n");
        current_GUIState = getState(s);
        if (s != GameState.InGame && gameState == GameState.InGame) StopCoroutine("UpdateInGame");
        gameState = s;
        switch (gameState)
        {
            case GameState.InGame:
                ClearGUI(GameState.InGame);
                PlaceInGameGUI();
                Inventory.onWishChanged += onWishChanged;
             //   Moon.onWaveEnd += onWaveEnd;
                Peripheral.onHealthChanged += onHealthChanged;
                Peripheral.onDreamsChanged += onDreamsChanged;
                Peripheral.onSelected += onSelected;
                
                WaveCountUpdate();
                HealthUpdate(Peripheral.Instance.GetHealth(), false);
                DreamUpdate(Peripheral.Instance.getDreams(), false, Vector3.zero);
                WishUpdate(Peripheral.Instance.my_inventory.wishes[0].my_wish, false, false);
                //WishUpdate(RuneType.Airy, Peripheral.Instance.GetWish(RuneType.Airy));
                //WishUpdate(RuneType.Vexing, Peripheral.Instance.GetWish(RuneType.Vexing), false);
                LabelUpdate("score_status", Get.Round(ScoreKeeper.Instance.getTotalScore(), 1));
                StartCoroutine("UpdateInGame");
                break;
            case GameState.Lost:
                ClearGUI(GameState.Lost);
                PlaceLostScreen();
                break;
            case GameState.MainMenu:
                ClearGUI(GameState.MainMenu);
                PlaceMenu(true);
                break;
            case GameState.Won:
                ClearGUI(GameState.Won);
                PlaceWonScreen();
                break;
            case GameState.Loading:
                ClearGUI(GameState.Loading);
                PlaceLoadingScreen();
                break;
            case GameState.LoadingWave:
                ClearGUI(GameState.LoadingWave);
                PlaceLoadingScreen();
                break;
            case GameState.LevelList:
                ClearGUI(GameState.LevelList);
                PlaceLevelList();
                break;
            default:
                break;
        }
    }
//	bool bg, bool start, bool b_continue, bool settings, bool restart, bool inventory){
	public void PlaceLevelList(){
		getState(GameState.LevelList).fademe.FadeIn();
		Central.Instance.setLevelInfo(-1);
		PlaceElement ("levellist_bg");
	//	GameObject button = PlaceElement ("levellist_continue");
		PlaceElement ("levellist_mainmenu");
		PlaceElement ("levellist_panel");
		PlaceElement ("levellist_info");
        PlaceElement("special_skill_panel_button");
        if (Central.Instance.level_list.test_mode) PlaceElement("givemestuff_button");
        //PlaceElement("special_skill_panel");
        Central.Instance.level_list.SetStuff();
	}


    public void UpdateWaveTimer(float time)
    {
     //   Debug.Log("want to update wave timer, can I? " + (wave_timer != null) + " \n");
        if (wave_timer == null) return;

        if (time <= 0)
            wave_timer.text = "";
        else
            wave_timer.text = Mathf.FloorToInt(time).ToString();
    }
	
	

	public void PlaceWonScreen(){
		getState(GameState.Won).fademe.FadeIn();

        LabelUpdate("current_level_score_status", Get.Round(ScoreKeeper.Instance.getCurrentLevelScore(),1));
        LabelUpdate("total_score_status", Get.Round(ScoreKeeper.Instance.getTotalScore(),1));
        PlaceElement ("won_bg");
		PlaceElement ("won_image");
		PlaceElement ("won_continue");
	}

	public void PlaceLostScreen(){
		getState(GameState.Lost).fademe.FadeIn();
		PlaceElement ("lost_bg");
		PlaceElement ("lost_image");
		PlaceElement ("lost_quit");
		PlaceElement ("lost_restartlevel");
		PlaceElement ("lost_restartwave");
        
    }


	public void PlaceLoadingScreen(){
		getState(GameState.Loading).fademe.FadeIn();
		PlaceElement ("loading_bg");
		PlaceElement ("loading_image");
	}


	public void PlaceMenu(bool m){
		menu = m;
	//	Debug.Log("Placing menu\n");
		switch (menu) {
		case true:
			getState(GameState.MainMenu).fademe.FadeIn();
			List<MenuButton> list = new List<MenuButton>();
			
			if (gameState == GameState.InGame){
				
				list.Add(MenuButton.Continue);
				//list.Add(MenuButton.ToMainMenu);
				list.Add(MenuButton.Quit);
				list.Add(MenuButton.ToMap);
				list.Add(MenuButton.LoadStartLevelSnapshot);
				list.Add(MenuButton.Settings);
                list.Add(MenuButton.Rewards);
                    if (Central.Instance.saved_level != -1) list.Add(MenuButton.LoadSnapshot); 
                
                                                
				PlaceButtons("MainMenu",false, list);
				Peripheral.Instance.Pause(true);
			}else if (gameState == GameState.Lost){
				Debug.Log("Placing LOST Main menu\n");
				list.Add(MenuButton.LoadSnapshot);
				list.Add(MenuButton.Quit);
				list.Add(MenuButton.ToMap);
				PlaceButtons("MainMenu", true, list);
			}else{
                    //very very intro screen before game starts
           
				if (Central.Instance.saved_level != -1) list.Add(MenuButton.LoadSnapshot);
                list.Add(MenuButton.Start); 
                list.Add(MenuButton.Quit);
				list.Add(MenuButton.Settings);
                    PlaceButtons("MainMenu", true, list);
			}
			break;
		case false:
			ClearGUI(GameState.Null);
		
			if (gameState.Equals("InGame")){
				
				Peripheral.Instance.Pause(false);
			}
			break;
		default:
			break;
		}
	}

	public void WaveButton(bool on){;
        if (on)
        {
            PlaceElement("ingame_wave_start");
            wave_timer = PlaceElement("ingame_wave_timer").GetComponentInChildren<Text>();
            DisableElement("ingame_wave_start", true);
            DisableElement("ingame_wave_timer", true);
        }
        else
        {
            UpdateWaveTimer(0);
       //     Debug.Log("Disablign wave button\n");
            DisableElement("ingame_wave_start", false);
            DisableElement("ingame_wave_timer", false);
            wave_timer = null;
        }
	}
	
	public void StopSignal(){
		}
	
	    Vector2 Coord(Transform parent){

		if (parent == null)return new Vector2 (x_coord, y_coord);
		//Debug.Log ("coord for " + parent.name);
		Vector3 parentScale = parent.localScale;


		if (parentScale.Equals (Vector3.one)) 
		{//	Debug.Log("one (" + x_coord + " " + y_coord + " FOR " + parent.name + "\n");
			return new Vector2 (x_coord, y_coord);
		} else 
		{
			Vector3 scale = GUIScale (null, 1,1,false);
			scale.Set(parentScale.x/scale.x, parentScale.y/scale.y,scale.z);
			scale.Set(scale.x*x_coord,scale.y*y_coord,scale.z);
			return new Vector2(scale.x, scale.y);
		}

					
	}

        Vector3 GUIPosition(Transform parent, float x, float y, float z, Vector3 mysize)
    {
        return GUIPosition(parent, x, y, z, mysize, true);
    }

	Vector3 GUIPosition(Transform parent, float x, float y, float z, Vector3 mysize, bool fit){
		Vector2 coord = Coord (parent);
		Vector2 def = Coord (null);

		if (coord != def) 
		{ 
			coord.x = Camera.main.orthographicSize;//*coord.x/def.x;
			coord.y = coord.x * Screen.height;//*coord.y/def.y;
		}
		float x_scale = 50f;
		float y_scale = 80f;
		Vector3 position = new Vector3 (x*x_scale, y*y_scale, -z);
        if (fit){

		    if (Mathf.Abs(position.x) + Mathf.Abs(mysize.x)  > x_scale && x!= 0) 
				position.x = (x_scale - mysize.x) * x / Mathf.Abs (x);		
		    if (Mathf.Abs(position.y) + Mathf.Abs(mysize.y) > y_scale && y != 0 )
		        position.y = (y_scale - mysize.y) * y / Mathf.Abs (y);
        }

		return position;
	}

	//TRANSFORM.LOCALSCALE of gui element
	Vector3 GUIScale(Transform parent, float sizex, float sizey, bool stretch){
		Vector2 coord = Coord (parent);
		float min = Mathf.Min (Screen.width, Screen.height);
		float default_x = min;
		float default_y = min;
		Vector3 scale;
	//	Debug.Log ("Min is " + min);
		if (coord != Coord (null)) {
			//return Vector3.one;
			return new Vector3(sizex, sizey,0);
		}
		
		if(stretch)
			scale = new Vector3 (sizex*(coord.x)/(default_x), sizey*(coord.y)/(default_y), 0);
		else
			scale = new Vector3 (sizex*(coord.y)/(default_x), sizey*(coord.y)/(default_y), 0);
	//	Debug.Log ("Parent " + parent.name + " coord is " + coord + " Gui scale from " + sizex + " " + sizey + " to " + scale + "\n");
		return scale;


	}


	//THIS IS ONLY USED FOR GUIPOSITION (ALMOST)
	Vector3 GUISize(Vector3 scale){
		float default_x = 100;
		float default_y = 100;
		
		return new Vector3 (scale.x*default_x/2f, scale.y*default_y/2f, 0);//this is half the size of the object

	}


		
	public bool DisableElement ( string what, bool active){
	
		foreach (MyLabel l in current_GUIState.labels){
			if (l == null || l.name != what) continue;
            //	if (l.button == null){Debug.Log("Missing button on " + name + "\n! Cannot set interactable " + active + "\n"); return false;}
            //	l.button.interactable = active;
            l.SetActive(active);
			return true;
		}
		return false;
	}
	


	public GameObject PlaceElement ( string what, bool active){
		
		GameObject element;
		GameObject returnme = null;
		//Debug.Log ("PlaceElement wants to place " + what + "\n");
		elements.TryGetValue (what, out element);
		if (element != null) { 
		//Debug.Log("got a thing\n");
			foreach (Transform child in element.transform){
				child.gameObject.SetActive (active);
			//	Debug.Log("turniing on child " + child.name + "\n");
				returnme = child.gameObject;				
			}
		} else {
			Debug.Log ("PlaceElement failed to find " + what + "\n");
		}
		return returnme;
		
	}

	GameObject PlaceElement( string what){
		return PlaceElement(what, true);
	}





	public void ClearInfo(){
		if (infobox != null) {
			DestroyObject (infobox);
			infobox = null;
		}
	}


	public void DisplayInfo(string type, string name){
		if (infobox != null && infobox.name.Equals("infobox_" + name)) return;
		ClearInfo ();


		switch(type){
		case "toy":
			//infobox = DisplayToyInfo(name);
			break;
		default:
			break;
		}

	}




	//string what, string parent,float x, float y,float z, float sizex, float sizey){
	void PlaceInGameGUI(){
	
		PlaceElement ("ingame_top_bg");//, "InGame", 0, 1, -4f, 1f, 1f, true);	
		PlaceElement ("ingame_fastforward_status");//, "InGame", 0, 1, -4f, 1f, 1f, true);

		//PlaceElement ("ingame_points_text");//,"health", "InGame",-1f,1f, button_height*2f, button_height/2f, false, text_size);
		PlaceElement ("ingame_points_status");//, Central.Instance.points.ToString(), "InGame",-1f+button_height,1f-button_height, 
		PlaceElement ("ingame_dreams_text");
		PlaceElement ("ingame_wave_count");
       // WavePipController pips = PlaceElement ("ingame_wave_pips").GetComponent<WavePipController>();
        PlaceElement("ingame_score_count");
      //  PlaceElement ("ingame_dreams_status");
		PlaceElement ("ingame_sensible_wish_status");
        //PlaceElement("mobile_tower_scroll");
        //	PlaceElement ("ingame_sensible_wish_text");

        health_visual = PlaceElement ("ingame_health_hurt").GetComponent<SpriteRenderer>();
		saving_game_visual = PlaceElement ("ingame_saving_game");
        //		PlaceElement ("ingame_quit");
        pause_button = PlaceElement("ingame_pause").GetComponent<MyButton>();

        PlaceElement("ingame_savetest");
       // PlaceElement("ingame_share_screenshot");
        PlaceElement ("ingame_mainmenu");       

		PlaceElement ("ingame_fastforward");
		//PlaceElement ("ingame_fastforward_superfast");
		PlaceElement ("ingame_fastforward_press");
		PlaceElement ("ingame_selected_toy");
        PlaceElement("ingame_variable_stat_display");
        if (Moon.Instance.one_day > 1) {Sun.Instance.indicator = PlaceElement ("ingame_time_of_day_indicator").GetComponent<TimeOfDayIndicator>();}
	//	PlaceElement ("ingame_monster_count");
		PlaceInGameButtons ();
       // pips.Init();
        Peripheral.Instance.my_skillmaster.my_panel.UpdatePanel();
        
    }

	public void RunSavingGameVisual(){
//	Debug.Log("Running saving game visual\n");
		//LeanTween.cancel(saving_game_visual);
		if (Central.Instance.state == GameState.InGame) LeanTween.alpha(saving_game_visual, 1, .5f).setLoopCount(2).setLoopType(LeanTweenType.pingPong).setLoopPingPong();
		//LeanTween.alpha (saving_game_visual, 1,1f);
	}
	
    public void RunHealthVisual()
    {
        StopCoroutine(RunHealthVisualCoroutine());
        StartCoroutine(RunHealthVisualCoroutine());
    }
    		
	IEnumerator RunHealthVisualCoroutine(){
        //LeanTween.cancel(health_visual);
        //LeanTween.cancel(health_visual.gameObject);
		LeanTween.alpha(health_visual.gameObject, 1f, 0.35f);
		LeanTween.alpha(health_visual.gameObject, 0, 0.35f).setDelay(0.35f);
        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(0.7f));
        Show.SetAlpha(health_visual, 0f);

	}
}
