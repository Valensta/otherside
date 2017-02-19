using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RandomWalk : MonoBehaviour {

    public RectTransform image;
    Vector2 direction;
    float init_velocity = .1f;
    float velocity;
    bool moving_out = true;
    float life = 3f;
    float time;
    void Start()
    {
        direction = new Vector2(Random.RandomRange(-0.99f, .99f), Random.RandomRange(-.99f, .99f));
        velocity = init_velocity;
        float thing = Random.RandomRange(1f, 2f);
        thing = Mathf.FloorToInt(thing)/2f;
        life = 1f * thing + 0.2f;
        init_velocity = Random.RandomRange(0.04f, 0.06f);
        image.RotateAroundLocal(Vector3.forward, Random.RandomRange(0f, 360f));
    }

	
    void OnDisable()
    {

    }

   

    void Update () {
        
        
        image.anchoredPosition = new Vector2(image.anchoredPosition.x +  Time.deltaTime * direction.x * velocity,
                                             image.anchoredPosition.y + Time.deltaTime * direction.y * velocity);

        velocity = (life - time  + 0.3f) * init_velocity;

        time -= Time.fixedDeltaTime;

        if (time <= 0)
        {
            velocity = init_velocity;            
            direction = new Vector2(Random.RandomRange(-0.99f, .99f), Random.RandomRange(-.99f, .99f));
            time = life;
        }
    }
}
