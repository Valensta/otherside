using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
public enum VelocityType{ Velocity, AngularVelocity};

public class AI : MultiPathfinding {
    public Transform player;
    private CharacterController controller;
    private bool newPath = true;
    private bool moving = false;
	public float seewhen;
	public float stopwhen;
	public float keepwalking;
	public float notsure;
	public float speed;
	public bool orient;
	public float howclose = 0.25f;
	public AnimationType animate = AnimationType.None;
	public bool sprite;
    public bool forward_only; //do not go back to previous path, normal mode. false if teleported or feared


    public bool can_be_stunned = true;
	Vector3 last_position;
	Animator animator;
	WaypointNodelet previousPath;
    
    
	bool rethink;
	bool hit;
    private bool stunned; //deactivate movement for this length
    float velocity_lerp;
	public VelocityType velocityType;
	string myname;
	Rigidbody2D my_rigidbody;
	float old_velocity_mag;
	public int path = 0;
	public float current_speed;
	bool init_orientation = false;
	public DogTag my_dogtag;
float orient_timer;
	IEnumerator face_foward_coroutine;
    public float rotation_interval = 0.05f;
	public float rotation_lerp_amount = 0.5f;
	public float rotation_inverse_speed_factor = 4f;
    public Vector3 blah_velocity;
	float rotation_timer = 0f;
	float interval = 0.05f;
	//float lerp_amount = 0.4f;
	float rotation_steps;
	//rotation_lerp_amount
	int current_steps = 0;
	
	Quaternion direction = Quaternion.identity;
    //if (Path.Count > 0) direction = Quaternion.LookRotation(Path[0]);

    bool do_it = false;
	bool orienting = false;

    public bool Stunned
    {
        get
        {
            return stunned;
        }

        set
        {
            if (!can_be_stunned && value) return;
            stunned = value;
        }
    }

    public void getNewPath(bool wherever){
        forward_only = false;
        getNewPath();
        forward_only = true;
	}



    public void getNewPath()
    {
        StartCoroutine(NewPath());
    }



    public void Die(){		
		StopAllCoroutines ();
		CancelInvoke ();
	}

	
	void OnDisable(){
		Die ();
	}

	public void Init () 
    {
        rotation_steps = Mathf.FloorToInt(4f / rotation_lerp_amount);
		init_orientation = false;
		my_dogtag.Init();
	//	if (orient){StartCoroutine("OrientMe");}
		if (my_rigidbody == null){
			my_rigidbody = this.GetComponent<Rigidbody2D>();
		}
		current_speed = speed;
        hit = false;
        forward_only = true;
		myname = this.name;
		InvokeRepeating ("updateLastPosition", 2, 1f);

		if (velocityType == VelocityType.AngularVelocity) {
						velocity_lerp = 1f;
				} else {
						velocity_lerp = 0.1f;
				}
		if (animate != AnimationType.None) {
				 animator = GetComponentInChildren<Animator> ();
				}
		StartCoroutine(NewPath());

		
	}
	void updateLastPosition(){
		if (Vector3.Distance (last_position, this.transform.position) < speed / 2f) {
			getNewPath();
		}
		last_position = this.transform.position;
	}    

    void _OrientMe(Quaternion direction, float amount)
    {        
		transform.rotation = Quaternion.Lerp(transform.rotation, direction, amount);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, direction, amount); 
	
	}
	
    

    
    public Quaternion GetForwardDirection(bool start){
		Quaternion direction = Quaternion.identity;
						
		if (Path.Count > 0 || start) {
			Vector3 to_dir = Vector3.one;
			if (start) {
				to_dir = WaypointMultiPathfinder.Instance.paths[path].start.position;
			}else{
				to_dir = Path[0].position;
			}
			 
			to_dir.z = 0;
			Vector3 from_dir = transform.position;
			from_dir.z = 0;
			Vector3 dir = to_dir - from_dir;
			float angle = Mathf.Atan2(dir.y, dir.x)*Mathf.Rad2Deg;
			direction = Quaternion.AngleAxis(angle, Vector3.forward);
		}
		return direction;		
    }





    IEnumerator OrientMe()
    {
        Debug.Log("False\n");
        float rotation_timer = 0f;
        float rotation_interval = 0.05f;
        //float lerp_amount = 0.4f;
        float steps = 4f / rotation_lerp_amount;
        //rotation_lerp_amount
        int current_steps = 0;

        Quaternion direction = Quaternion.identity;
        //if (Path.Count > 0) direction = Quaternion.LookRotation(Path[0]);


        bool orienting = false;
        while (true)
        {

            if (!orienting && hit) { rotation_timer = 0f; current_steps = 0; orienting = true; my_rigidbody.angularVelocity = 0f; }
            if (orienting && current_steps == steps) { orienting = false; current_speed = speed; hit = false; }
            if (orienting) rotation_timer += Time.deltaTime;

            if (orienting && rotation_timer > rotation_interval && current_steps < steps)
            {
                direction = GetForwardDirection(false);


                //if (Regex.Match(name, "plane0").Success) Debug.Log("Orienting " + current_steps + "\n");
                _OrientMe(direction, rotation_lerp_amount);
                if (!Stunned) current_speed = speed + speed * (steps - current_steps) / (rotation_inverse_speed_factor * steps);
                current_steps++;
            }

            yield return new WaitForSeconds(1 / 30f);
        }
    }




    void FixedUpdate () {
        
		if (orient)
		{
			if (orient_timer > 0){orient_timer -= Time.deltaTime;}
			else
			{
				if (!orienting && hit) { rotation_timer = 0f; current_steps = 0;orienting = true;my_rigidbody.angularVelocity = 0f;}
				if (orienting && current_steps == rotation_steps) {orienting = false; current_speed = speed; hit = false;}
				if (orienting) rotation_timer += Time.deltaTime;
				
				if (orienting && rotation_timer > interval && current_steps < rotation_steps)
				{
					direction = GetForwardDirection(false);
		
					_OrientMe(direction,rotation_lerp_amount);
					if (!Stunned) current_speed = speed + speed*(rotation_steps - current_steps)/(rotation_inverse_speed_factor* rotation_steps);       
					current_steps++;
				}
			
			}
			
		}
		
		
		if (!hit && Mathf.Abs(my_rigidbody.angularVelocity) < 50f &&  Mathf.Abs(Quaternion.Angle(transform.rotation, GetForwardDirection(false))) > 10f)
			{hit = true;
			//Debug.Log(my_rigidbody.angularVelocity);
			}


		//else{Debug.Log(Mathf.Abs(Quaternion.Angle(transform.localRotation, GetForwardDirection())));}

		if (player == null) {
			player = this.transform;
		}

        if (Vector3.Distance(player.position, transform.position) < seewhen && !moving)
		{
            if (newPath)
            {
                StartCoroutine(NewPath());
            }
            moving = true;
        }
        else if (Vector3.Distance(player.position, transform.position) < stopwhen)
        {//
		//	Debug.Log("what is this " + player.position + " " + transform.position);
        }
        else if (Vector3.Distance(player.position, transform.position) < keepwalking && moving)
        {
            if (Path.Count > 0)
			{
                if (Vector3.Distance(player.position, Path[Path.Count - 1].position) > notsure)
                {
                    StartCoroutine(NewPath());
                }
            }
            else
			{
                if (newPath)
                {
                    StartCoroutine(NewPath());
                }
            }
            MoveMethod();
        }
        else
		{
            moving = false;
        }
        blah_velocity = my_rigidbody.velocity;
	}

    IEnumerator NewPath(){
        newPath = false;
        //      Debug.Log("New Path\n");
        if (transform == null)
        {
            Debug.Log(this.name + " Eh AI newpath traksform is null\n"); yield return null;
        }
        FindPath(path, transform.position, player.position, getPreviousPath());
        yield return new WaitForSeconds(.1F);
        newPath = true;
        if (!init_orientation){
		//	Debug.Log(GetForwardDirection(false));
			transform.rotation = GetForwardDirection(false);
			init_orientation = true;
        }
    }


    private WaypointNodelet getPreviousPath()
    {
        if (forward_only) return previousPath;
        
        return new WaypointNodelet( WaypointMultiPathfinder.Instance.paths[path].start);
    }

    private void MoveMethod()
	{
		if (!do_it) { do_it = true; return; }
		if (Path.Count > 0)
        {
            Vector2 transform_position = transform.position;
			Vector2 direction = ((Vector2)Path[0].position - transform_position).normalized;			
			Vector2 velocityVector;

            Vector2 rb_velocity = my_rigidbody.velocity;
            if ((rb_velocity - direction * current_speed).magnitude > current_speed / 5)
            {
                velocityVector = Vector2.Lerp(rb_velocity, direction * current_speed, velocity_lerp);             
                my_rigidbody.velocity = velocityVector;
                rb_velocity = velocityVector;
            }			
			
            float velocity_magnitude = rb_velocity.magnitude;


            if (Vector2.Distance(transform_position, Path[0].position) < howclose){
				previousPath = Path[0];
				Path.RemoveAt(0);
			}
			if (rethink) rethink = false;
			
			old_velocity_mag = velocity_magnitude;
		}
    }


	
/*
	void GetClosestPoint(){
		if (Path.Count == 0){ 
			moving = false;
			return;
		}
		Vector3 a = (transform.position - previousPath);
		Vector3 b = (Path[0].vector - previousPath);
		float dot = Vector3.Dot(a,b);
		float factor = dot/b.magnitude;

		float distance = a.magnitude * a.magnitude - factor * factor;

		float factor2 = 0;
		float distance2 = Mathf.Infinity;
		float dot2 = 0;
		Vector3 b2 = Vector3.one;
		
		if (Path.Count > 1) {
			Vector3 a2 = (transform.position - Path[0].vector);
			b2 = (Path[1].vector - Path[0].vector);
			dot2 = Vector3.Dot(a2,b2);
			factor2 = dot2/b2.magnitude;
			
			distance2 = a2.magnitude * a2.magnitude - factor2 * factor2;
		}
	
		if (distance2 < distance) 
			Path[0] = new PathPoint(Vector3.MoveTowards(Path[0],Path[1],dot2/b2.magnitude), Path[1].id);
		else 		
			Path.Insert(0,new PathPoint(Vector3.MoveTowards(previousPath,Path[0],dot/b.magnitude), Path[0].id));

	}*/
}

