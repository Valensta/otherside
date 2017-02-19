using UnityEngine;

public class Modifier : MonoBehaviour
{
    
    public float disabled_timer;
    public bool is_active;
    //float my_time = 0f;



    public void setActive(bool _active)
    {
        if (is_active == true && _active == false)
        {
            SafeDisable();
        }

        is_active = _active;        
    }

    

    public bool isActive()
    {
        return is_active;
    }

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

        if (is_active)
            YesUpdate();

    }

 
    protected virtual void YesUpdate() { }

    public virtual void DisableAction() { }

    protected virtual void SafeDisable() { }
}