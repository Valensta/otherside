using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class End : MonoBehaviour {
//	public GameObject actor;
	void Start()
	{
        Body.onCheckCastleDistance += onCheckCastleDistance;
	}

	void OnTriggerEnter2D(Collider2D c){
	//	Debug.Log ("END REACHED\n");
		Collider2D other = c;
		if (other.tag.Equals("Enemy") ){//&& other.GetComponentInChildren<Body> ()){ //why was this added??
			//actor.GetComponent<Peripheral>().max_dreams -= (int)Mathf.Ceil(other.GetComponent<Actor> ()./3);
			other.tag.Equals("EnemyWon");
			
			
			HitMe my_hitme = other.attachedRigidbody.gameObject.GetComponent<HitMe>();
            string label = my_hitme.my_ai.my_dogtag.getLabel();
          //  Debug.Log(my_hitme.gameObject.name + " " + my_hitme.GetInstanceID() + " " + Duration.time + " DIED!\n");

            my_hitme.DieSpecial();
            if (!LevelBalancer.Instance.am_enabled)
            {
                Peripheral.Instance.AdjustHealth(-1);
                Tracker.Log(PlayerEvent.CastleReached, true,
                    customAttributes: new Dictionary<string, string>() { { "attribute_1", label } },
                                        customMetrics: new Dictionary<string, double>() { { "metric_2", Peripheral.Instance.my_inventory.GetWishCount(WishType.Sensible) } });


            }
            Debug.Log("** Castle reached " + label + "\n");
            GameStatCollector.Instance.CastleInvaded(my_hitme.gameObject.name);
		}else{
			
		}
	}
    void onCheckCastleDistance(Vector3 pos , string label) {
        Vector2 check_me = new Vector2(pos.x, pos.y);
        float distance = Vector2.Distance(check_me, this.transform.position);
        if (distance > 7f)
        {
    //        Debug.Log("Night Tower Hit too far " + distance + "\n");
            return;
        }

        Debug.Log("** " + label + "\n");

        Tracker.Log(PlayerEvent.NightTowerHit,true,
            customAttributes: new Dictionary<string, string>() { { "attribute_1", label } },
            customMetrics: new Dictionary<string, double>() { { "metric_2", Peripheral.Instance.my_inventory.GetWishCount(WishType.Sensible) } });

        
    }

    private void OnDisable()
    {
        Body.onCheckCastleDistance -= onCheckCastleDistance;
    }
}