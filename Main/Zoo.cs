using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class Zoo : MonoBehaviour {
	public int default_add = 3;
	public List<Poolette> init;
	Dictionary<string, Pool> zoo = new Dictionary<string, Pool>();
	int added = 0;
	int max_order = 0;
    private static Zoo instance;
    // Use this for initialization
    public static Zoo Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("zoo got destroyeed\n");
            Destroy(gameObject);
        }
        Instance = this;
    }

    public void InitZoo(){
		// ORDER 0 IS MANDATORY BEFORE LEVEL STARTS
		for (int i = 0; i < init.Count; i++) {			
			Poolette p = init[i];
			if (p.order > max_order) max_order = p.order;
			if (p.order > 0) continue;
			if (zoo.ContainsKey(p.name)){added++; continue;}
					
			Pool pool = new Pool(p.name, p.count);
            pool.non_refundable = p.non_refundable;
            int need = p.count;
			foreach (GameObject obj in p.objects){
		//		Debug.Log("PREADDED " + p.name + "\n");
				SetObject(obj, ref pool, p.name);	
				need--;
			}
			
			
			if (need > 0){
				AddToPool(ref pool, p.name, need);				
			}
			
			if (pool.count > 0){				
		//		Debug.Log("Added " + p.name + " to zoo\n");
				zoo.Add(p.name, pool);			
			}
			added ++;
			
		}
	//	Debug.Log("Done with init zoo\n");
		

	}

	
	IEnumerator SmartInitZoo(){
		int order = 1;
		while (added < init.Count && order <= max_order){
			for (int i = 0; i < init.Count; i++) {			
				Poolette p = init[i];
				if (zoo.ContainsKey(p.name)){added++; continue;}
				if (p.order == order){
					Pool pool = new Pool(p.name, p.count);
                    pool.non_refundable = p.non_refundable;
					int need = p.count;
					
					foreach (GameObject obj in p.objects){
			//			Debug.Log("PREADDED " + p.name + "\n");
						SetObject(obj, ref pool, p.name);	
						need--;
					}
						
					while (need > 0){
				//		Debug.Log("Added " + p.name + "\n");	
						AddToPool(ref pool, p.name, 1);						
						need--;	
						yield return  new WaitForEndOfFrame();
					}
					
			//		Debug.Log("Zoo ADDING " + p.name + "\n");
										
					if (!zoo.ContainsKey(p.name))zoo.Add(p.name, pool);					
					added++;
				}
			
			}
			order++;
		
		}
//		Debug.Log("Done with smarrt init\n");
		yield return null;
	}

	public void Start(){

		InitZoo ();
		StartCoroutine("SmartInitZoo");
		//Debug.Log("Zoo is done with tart\n");
		//InitZoo();
	}


	public GameObject getObject(string name, bool active){
		Pool p = new Pool ();

		zoo.TryGetValue (name, out p);
		GameObject o = null;
		if (p != null) {
			o = GetFromPool (ref p, active);
		} else {
			Debug.Log("Zoo does not contain a " + name + "!! Making a new pool with " + default_add + " items.\n");
			p = new Pool();
            p.non_refundable = false;
			p.name = name;
			AddToPool(ref p, p.name, default_add);
			o = GetFromPool (ref p, active);
			zoo.Add(name,p);
		}

		if (o == null) {
			Debug.Log ("Zoo does contain a " + name + " but does not have one available!! Very strange.\n");
		}
	    return o;
	}


	GameObject GetFromPool(ref Pool p, Vector3 pos, bool active){
		GameObject o = GetFromPool (ref p, false);
		o.transform.position = pos;
		o.SetActive (active);
		return o;
	}

	GameObject GetFromPool(ref Pool p, Vector3 pos, Quaternion orientation, bool active){
		GameObject o = GetFromPool (ref p, false);
		o.transform.position = pos;
		o.transform.localRotation = orientation;
		o.SetActive (active);
		return o;
	}

	GameObject GetFromPool(ref Pool p, bool active){

		GameObject o = null;
		for (int i = 0; i < p.pool.Count; i++) {
            
            if (p.pool[i] != null && !p.pool[i].activeSelf){ o = p.pool[i];}
		}

		if (o == null) {			
			AddToPool(ref p, p.name, default_add);
			o = p.pool[p.pool.Count-1];
		}
		o.SetActive (active);
		return o;
	}
	
	public void returnObject(GameObject o){
//		o.transform.parent = this.transform;
	//	Debug.Log("Returning " + o.name + "\n");
	//	Debug.Log("Returning " + o.name + "\n");

		if (o.activeSelf) o.SetActive (false);
	}

	public void returnAll()
    {
        foreach(Pool p in zoo.Values)
        {
            if (p.non_refundable)
            {
               // Debug.Log("Not returning " + p.name + "\n");
                continue;
            }
          //  Debug.Log("returning all " + p.name + "\n");
            foreach (GameObject obj in p.pool)
            {
                //if (!obj.activeSelf) continue;
                
                returnObject(obj, true);
            }
        }
    }

	public void returnObject(GameObject o, bool return_parent){
   //     Debug.Log("Returning " + o.name + "\n");
        if (return_parent == true){ o.transform.SetParent(this.transform);	}
		o.SetActive (false);

      //  if (o.name.Equals("castle0"))
        //    Debug.Log("What\n");
	}


	//should this be a coroutine?
	void AddToPool(ref Pool p, string name, int count){
    //   Debug.Log("Adding " + name + "\n");
        while (count > 0) {

            Object r = Resources.Load(name, typeof(GameObject));
            if (r == null) { Debug.Log("Did not find object " + name + " to instantiate!\n"); return; }

            GameObject o = Instantiate (r) as GameObject;
			SetObject(o, ref p, name);
			count--;
			
		}
	}
	
	void SetObject(GameObject o, ref Pool p, string name){
		o.name = name;
		o.transform.SetParent(this.transform, false);
		o.SetActive(false);
		p.pool.Add(o);
		p.count++;
	}

}

