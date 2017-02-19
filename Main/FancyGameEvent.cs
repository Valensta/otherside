using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

/*

public class FancyGameEvent : MonoBehaviour {
	public float time_delay = 0;
	List<Trigger> to_start = new List<Trigger> ();
	List<Trigger> to_finish = new List<Trigger> ();
	List<GameAction> actions = new List<GameAction>();
	List<GameAction> finishactions = new List<GameAction>();
	public delegate bool run_event();
	float check_interval = 0.25f;
	run_event myRunEvent;
	string name;
	public delegate void EventFinishedHandler(string name);
	public static event EventFinishedHandler onEventComplete; 


	public void setName(string n){name = n;}

	public void setTimeDelay(float t){time_delay = t;}

	public void AddStartCondition(Condition c, string t, float n){to_start.Add (new Trigger (c, t, n));}

	public void AddFinishCondition(Condition c, string t, float n){to_finish.Add (new Trigger(c, t, n));}

	public void AddAction(GameAction action)
	{actions.Add (action);}

	public void AddFinishAction(GameAction action)
	{finishactions.Add (action);}

	public void AddAction(ActionType action, string text, string name, float number, GameObject target,bool boolean)
	{actions.Add (new GameAction (action));}

	IEnumerator WaitForStart(){
		int togo = to_start.Count;
		while (togo > 0) {
			for (int i = 0; i < to_start.Count; i++) {
			//	Debug.Log(i + " " + (to_start[i] == null));
				if(to_start[i] != null){			
					bool ok = to_start [i].Check ();
					if (ok) {
						togo--;
					}
				}
			}
			yield return new WaitForSeconds (check_interval);
		}
	//	Debug.Log ("event start conditions met");
		foreach (GameAction a in actions) {
	//		Debug.Log ("going to run action");
			yield return new WaitForSeconds (time_delay);
			a.Do ();
		}
		yield return null;
		StartCoroutine (WaitForFinish ());
	}

	IEnumerator WaitForFinish(){
		int togo = to_finish.Count;
		while (togo > 0) {
			for (int i = 0; i < to_finish.Count; i++) {
				//	Debug.Log(i + " " + (to_start[i] == null));
				if(to_finish[i] != null){			
					bool ok = to_finish [i].Check ();
					if (ok) {
						togo--;
					}
				}
			}
			yield return new WaitForSeconds (check_interval);
		}
	//	Debug.Log ("event finished");
		foreach (GameAction a in finishactions) {
	//		Debug.Log ("going to run finish action");
			a.Do ();
		}
		onEventComplete (name);
		yield return null;
	}


	public void RunEvent(){
		StartCoroutine (this.WaitForStart());	
	}


}
	
/*
	case "highlight_first_toy_button":
			e = gameObject.AddComponent (typeof(GameEvent)) as GameEvent;
			Transform toy = GameObject.Find("ingame_toy_grid").transform.GetChild(0);
			string selected = toy.GetComponentInChildren<MyButton>().content;
			e.setTimeDelay(0.3f);

			GameAction first_toy = new GameAction(ActionType.Panel);
			
			first_toy.setText("Select toy and place it on an island...");
			first_toy.setName("select_toy");			
			e.AddAction (first_toy);

			GameAction tween_toy = new GameAction(ActionType.TweenObject);
			tween_toy.setTarget(toy.gameObject);
			tween_toy.setText("ColorLoop");
			tween_toy.setName("Hint");
			e.AddAction(tween_toy);

			List<GameObject> spots = new List<GameObject>();
		    Island_Button[] islands = GameObject.Find("Props").GetComponentsInChildren<Island_Button>();
			foreach (Island_Button t in islands){
				if (t.size > 1) spots.Add(t.parent);
			}
			GameAction tween_islands = new GameAction(ActionType.TweenObject);
			tween_islands.setTargets(spots.ToArray());
			tween_islands.setText("ColorLoop");
			tween_islands.setName("Hint");
			e.AddAction(tween_islands);

			GameAction hideus = new GameAction(ActionType.HideUIElement);
			hideus.setText(selected);
			hideus.setClickable(false);
			e.AddAction(hideus);

			//e.AddFinishCondition(Condition.Selected,selected,0);
			e.AddFinishCondition(Condition.PlacedToy,selected,0);

			GameAction killme = new GameAction(ActionType.RemoveObject);
			killme.setName("select_toy");
			e.AddFinishAction(killme);

			GameAction showus = new GameAction(ActionType.HideUIElement);
			showus.setText(selected);
			showus.setClickable(true);
			e.AddFinishAction(showus);

			GameAction stop_tween_islands = new GameAction(ActionType.StopTween);
			stop_tween_islands.setTargets(spots.ToArray());

			e.AddFinishAction(stop_tween_islands);

			return e;
		case "press_wave_start":
			e = gameObject.AddComponent (typeof(GameEvent)) as GameEvent;
			e.setName (name);
			GameAction wave_box = new GameAction(ActionType.Panel);
			wave_box.setText("Start the wave");
			wave_box.setName("start_wave");
			e.AddAction(wave_box);

			GameAction wave_start = new GameAction(ActionType.TweenObject);
			wave_start.setTarget(GameObject.Find("ingame_wave_start"));
			wave_start.setText("ColorLoop");
			wave_start.setName("Hint");
			e.AddAction(wave_start);

			e.AddFinishCondition(Condition.WaveStarted,"",0);

			GameAction killwavebox = new GameAction(ActionType.RemoveObject);
			killwavebox.setName("start_wave");
			e.AddFinishAction(killwavebox);
			return e;
		case "get_runes":
			e = gameObject.AddComponent (typeof(GameEvent)) as GameEvent;
			e.setName (name);
			GameAction get_aura = new GameAction(ActionType.Panel);
			get_aura.setText("collect some auras of one color...");
			get_aura.setName("get_runes");
			e.AddAction(get_aura);

			e.AddStartCondition(Condition.WaveStarted,"",1);
			e.setTimeDelay(1f);

			e.AddFinishCondition(Condition.GotWish,"",0.5f);
			
			GameAction killaurabox = new GameAction(ActionType.RemoveObject);
			killaurabox.setName("get_runes");
			e.AddFinishAction(killaurabox);
			return e;
		case "get_charge":
			e = gameObject.AddComponent (typeof(GameEvent)) as GameEvent;
			e.setName (name);
			get_aura = new GameAction(ActionType.Panel);			
			get_aura.setName("get_charge");
			e.AddAction(get_aura);

			e.setTimeDelay(0.2f);
			
			e.AddFinishCondition(Condition.GotCharge,"",0.4f);
			
			killaurabox = new GameAction(ActionType.RemoveObject);
			killaurabox.setName("get_charge");
			e.AddFinishAction(killaurabox);
			return e;
		case "place_effect_toy":
			e = gameObject.AddComponent (typeof(GameEvent)) as GameEvent;
			e.setName (name);
			get_aura = new GameAction(ActionType.Panel);			
			get_aura.setName("get_charge");			
			e.AddAction(get_aura);

			spots = new List<GameObject>();
			islands = GameObject.Find("Props").GetComponentsInChildren<Island_Button>();
			foreach (Island_Button t in islands){
				if (t.blocked == false)spots.Add(t.parent);
			}
			tween_islands = new GameAction(ActionType.TweenObject);
			tween_islands.setTargets(spots.ToArray());
			tween_islands.setText("ColorLoop");
			tween_islands.setName("Hint");
			e.AddAction(tween_islands);

			
			e.setTimeDelay(0.2f);

			e.AddFinishCondition(Condition.PlacedToy,"ghost",0);

			killaurabox = new GameAction(ActionType.RemoveObject);
			killaurabox.setName("get_charge");
			e.AddFinishAction(killaurabox);

			stop_tween_islands = new GameAction(ActionType.StopTween);
			stop_tween_islands.setTargets(spots.ToArray());

			return e;
			*/


	