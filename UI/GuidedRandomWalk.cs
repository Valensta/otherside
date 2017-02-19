using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GuidedRandomWalk : MonoBehaviour {
    
    Vector2 finish;
    
    Vector2 direction;
    float init_velocity = 5f;
    float velocity;
    bool moving_out = true;
    float life = 0.3f;
    float time;
    float turn_time = 0.1f;


    public void StartMe(Vector2 f)
    {
        Debug.Log(gameObject.name + " started, going to " + f + "\n");
        finish = f;
        PickDirection();
        velocity = init_velocity;
        float thing = Random.RandomRange(1f, 2f);
        thing = Mathf.FloorToInt(thing)/2f;
        life = 1f * thing;
        
        
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

        time -= Time.fixedDeltaTime;

        if (time <= 0)
        {
            velocity = init_velocity;

            PickDirection();

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
            timer -= Time.fixedDeltaTime;
        }
        yield return null;

    }
}
