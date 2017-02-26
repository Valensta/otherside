using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;


[System.Serializable]
public class GameAction  : MonoBehaviour {
	public delegate void PanelHandler(string name);
//	public static event PanelHandler onPanelRequest; 
	public string _name;
    public string _text;
    public float _number;
    public bool _bool = true;
    public ActionType _type;
    public GameObject[] _target;
    public Vector3 _vector;

    public delegate void onMakeWishHandler(List<Wish> inventory, Vector3 pos);
    public static event onMakeWishHandler onMakeWish;

    public void setActionType(ActionType type){
		_type = type;
	}
	
	public void setNumber(float number){
		_number = number;
	}
	
	public void setText(string text){
		_text = text;
	}
	
	public void setBool(bool b){
		_bool = b;
	}
	
	public void setName(string name){
		_name = name;
	}

	public void setTargets(GameObject[] target){
		_target = target;
	}
	
	public void setTarget(GameObject target){
		_target = new GameObject[1];
		_target [0] = target;
	}
	
	public GameAction(ActionType action, string text, string name, float number, bool clickable){
		_number = number;
		_type = action;
		_name = name;		
		_bool = clickable;	
	}
	
	public GameAction(ActionType action){			
		_type = action;
	}

    /*
	public void StopTween(GameObject obj){
		Tweener[] stopme = obj.GetComponentsInChildren<Tweener> ();
		for (int i = 0; i < stopme.Length; i++) 
			{
				stopme[i].StopMe();
			}
	}
	*/
    public void Do()
    {
     //   Debug.Log("Doing gameaction " + this._type + " " + this.name + " " + _text + "\n");
        switch (_type)
        {
            case ActionType.Panel:

                GameObject panel = Peripheral.Instance.zoo.getObject(_name, true);
                if (panel == null) { Debug.Log("GameAction could not find object " + _name + "\n"); }
                if (!_bool)
                {
                    panel.transform.SetParent(EagleEyes.Instance.events.transform);
                }
                else {
                    panel.transform.SetParent(EagleEyes.Instance.world_space_events.transform);
                }
                panel.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                panel.transform.localPosition = Vector3.zero;
                panel.transform.localScale = Vector3.one;
                panel.transform.localRotation = Quaternion.identity;
                break;
            case ActionType.MakeFloaty:
                GameObject floaty = Peripheral.Instance.zoo.getObject(_name, true);
                if (floaty == null) { Debug.Log("GameAction could not find object " + _name + "\n"); }
                    floaty.GetComponent<Floaty>().Init(_vector);
                break;
            case ActionType.AddWish:

                WishType w = Get.WishTypeFromString(_text);
                Debug.Log("We should add a wish " + _text + " " + w + "\n");
                Peripheral.Instance.my_inventory.AddWish(w, _number, 1);
                break;
            case ActionType.GiveSpecialSkill:
                EffectType effect_type = EnumUtil.EnumFromString<EffectType>(_name, EffectType.Null);
                RuneType rune_type = EnumUtil.EnumFromString<RuneType>(_text, RuneType.Null);
                Rune r = Central.Instance.getHeroRune(rune_type);
                
                if (r == null) { Debug.LogError("Cannot find a rune for hero of type " + rune_type + ", cannot give skill " + effect_type + "\n"); }
                r.GiveSpecialSkill(effect_type);
                break;
            case ActionType.MakeWish:
                List<Wish> inv = new List<Wish>();
                inv.Add(new Wish(WishType.Sensible, 0.2f * _number));
                inv.Add(new Wish(WishType.MoreDamage, 0.1f * _number));
                inv.Add(new Wish(WishType.MoreXP, 015f * _number));
                inv.Add(new Wish(WishType.MoreDreams, 0.1f * _number));
                inv.Add(new Wish(WishType.MoreHealth, 0.1f * _number));
                if (onMakeWish != null) onMakeWish(inv, _vector);
                
                Debug.Log("We should make a wish\n");                
                break;
            case ActionType.Pause:
                Peripheral.Instance.Pause(true);
                break;
            case ActionType.Resume:
                Debug.Log("Resuming\n");
                Peripheral.Instance.Pause(false);
                break;
            case ActionType.DisableMonitor:
                Monitor.Instance.is_active = _bool;
                break;
            case ActionType.HideUIElement:
                //name is include, //text is exclude
                //	Debug.Log("Setting " + _name + " to " + _bool + "\n");
                EagleEyes.Instance.PlaceElement(_name, _bool);
                break;
            case ActionType.DisableUIElement:
                //name is include, //text is exclude
                //BUTTON HAS TO BE in GUI STATES WITH A MYLABEL

                EagleEyes.Instance.DisableElement(_name, _bool);
                //EagleEyes.Instance.SetEnableButtons(_name, _text, _bool);
                break;
            case ActionType.RemoveEventObjects:
                RemoveEventObjects();
                break;
            case ActionType.UnlockToy:
                Debug.Log("Unlocking toy " + _text + "\n");
                //Peripheral.Instance.ActivateToy(_text);
                unitStats toy = Central.Instance.getToy(_text);
                toy.isUnlocked = true;
                EagleEyes.Instance.UpdateToyButtons("blah", toy.toy_type, false);
                break;
            case ActionType.PointSpyGlass:
                if (Monitor.Instance != null) Monitor.Instance.my_spyglass.PointSpyglass(_vector);
                break;
            case ActionType.EnableSpyGlass:
                if (Monitor.Instance != null) Monitor.Instance.my_spyglass.DisableByEvent(!_bool);
                break;
            case ActionType.RemoveObject:
                if (_target != null)
                    foreach (GameObject t in _target)
                        Peripheral.Instance.zoo.returnObject(t);
                if (_name != null)
                    Debug.Log("GameAction removing object with name, TERRIBLE\n");
                Peripheral.Instance.zoo.returnObject(GameObject.Find(_name));
                break;
            case ActionType.EnableReward:
                RewardType rt = EnumUtil.EnumFromString<RewardType>(_text, RewardType.Null);
                if (rt != RewardType.Null)
                {
                    RewardOverseer.RewardInstance.EnableReward(rt);
                }
                else
                {
                    Debug.LogError(this.gameObject.name + " RewardType gameAction has an invalid rewardType( " + _text + ")\n");
                }
                break;
            default:
                Debug.LogError(this.gameObject.name + " " + _type + " gameAction has an unsupported ActionType\n");
                break;
        }
    }
	

	
	void RemoveEventObjects(){
		Dictionary<string,bool> remove_us = new Dictionary<string, bool>();
		if (_name !=  null){
			List<string> meh = new List<string>(_name.Split(';'));
			for (int i = 0; i  < meh.Count; i++){
				remove_us.Add(meh[i], true);
		//		Debug.Log("Going to remove " + meh[i] + "\n");
			}
		}
		
		if (!_bool){
			foreach(Transform t in EagleEyes.Instance.events.transform){
				
				if (remove_us.Count == 0 || remove_us.ContainsKey(t.name)){
					Peripheral.Instance.zoo.returnObject(t.gameObject, true);
				}
			}
		}
		
		if (_bool){
			foreach(Transform t in EagleEyes.Instance.world_space_events.transform){
				
				if (remove_us.Count == 0 || remove_us.ContainsKey(t.name)){
					Peripheral.Instance.zoo.returnObject(t.gameObject, true);
				}
			}
		}
	}
	
}




	