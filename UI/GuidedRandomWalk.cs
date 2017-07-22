using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GuidedRandomWalk : MonoBehaviour {
    
    Vector2 finish;
    
    Vector2 direction;
    float init_velocity = 3.5f;
    float velocity;
    bool moving_out = true;
    float life;
    float time;
    float turn_time = 0.2f;


    public void StartMe(Vector2 f)
    {
       // Debug.Log(gameObject.name + " started, going to " + f + "\n");
        finish = f;
        PickDirection();
        velocity = init_velocity;
        
       life = Random.Range(0.25f, 0.35f);
    }


    public void UpdateMe(Vector2 f)
    {
     //   Debug.Log(gameObject.name + " updated to " + f + "\n");
        finish = f;
    }
    
    void Update () {


        this.transform.position = new Vector2(this.transform.position.x +  Time.deltaTime * direction.x * velocity,
                                             this.transform.position.y + Time.deltaTime * direction.y * velocity);

       // velocity = (life - time*1/3f) * init_velocity;

        time -= Time.deltaTime;

        if (time <= 0)
        {
            velocity = init_velocity;
            
            PickDirection();
         //   Debug.Log("Picking direction " + Duration.realtimeSinceStartup + " " + velocity + " " + direction + "\n");
            time = life;
        }
    }

    void PickDirection()
    {
        Vector2 old_pos = new Vector2(this.transform.position.x, this.transform.position.y);
        Vector2 new_pos = old_pos;
        Vector2 new_dir = new Vector2(Random.RandomRange(-0.99f, .99f), Random.RandomRange(-.99f, .99f));
        new_pos = new Vector2(this.transform.position.x + Time.deltaTime * new_dir.x,
                                     this.transform.position.y + Time.deltaTime * new_dir.y);

        int max = 10;
        while (Vector2.Distance(new_pos, finish) > Vector2.Distance(old_pos, finish) && max > 0)
        {
            new_dir = new Vector2(Random.RandomRange(-0.99f, .99f), Random.RandomRange(-.99f, .99f));
            
            new_pos = new Vector2(this.transform.position.x + Time.deltaTime * new_dir.x,
                                         this.transform.position.y + Time.deltaTime * new_dir.y);
            max--;
        }
        new_dir = new_dir.normalized;
        Vector2 fin = finish.normalized;
        Vector3 perfect_dir = fin - old_pos.normalized;
        new_dir.x = new_dir.x * 0.2f + perfect_dir.x * 0.8f;
        new_dir.y = new_dir.y * 0.2f + perfect_dir.y * 0.8f;
        new_dir = new_dir.normalized;
        StartCoroutine(TurnMe(new_dir));
        //   Debug.Log("picked a direction " + direction + "\n");
    }

    IEnumerator TurnMe(Vector2 new_dir)
    {
        float timer = turn_time;        
        while (timer > 0)
        {                       
            direction = Vector2.Lerp(direction, new_dir, 0.4f);
            yield return new WaitForSeconds(0.02f);
            timer -= Time.deltaTime;
        }
        yield return null;

    }
}
