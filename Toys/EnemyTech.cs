using UnityEngine;

public class EnemyTech : MonoBehaviour
{
    
    public float disabled_timer;
    //float my_time = 0f;

    public void Disrupt(float timer)
    {
        disabled_timer = timer;
    
    }

    void Update()
    {
        if (disabled_timer > 0)
        {
            disabled_timer -= Time.deltaTime;
            return;
        }
        YesUpdate();

    }

    protected virtual void YesUpdate() { }
}