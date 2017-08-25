using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public class EnemyAnimator : MonoBehaviour
{
    public AnimationType type;
    public Animator animator;
    public AI my_ai;
    public Rigidbody2D my_rigidbody;
    public float input_x;
    public float input_y;
    
    float desired_input_x;
    float desired_input_y;
    
    public bool isWalking;
    public bool vehicle;

    private float min_direction_time = 0.2f;
    private float direction_time = 0f;

    public void Start()
    {
        min_direction_time = (type == AnimationType.Animate_Vehicle_4Dir) ? 0.2f : 0.2f;
    }
    
    void Update()
    {
        if (my_ai.animate == AnimationType.None) return;
                
        //isWalking = (my_ai.current_speed > 0.1f);
        isWalking = (my_rigidbody.velocity.magnitude > 0.02f);
        
        bool changed_direction = false;

        switch (type)
        {
            case AnimationType.Animate_Sprite_4Dir:
                changed_direction = set4Dir();
                break;
            case AnimationType.Animate_Vehicle_4Dir:
                changed_direction = setVehicle4Dir();
                break;
            case AnimationType.Animate_Sprite_8Dir:
                set8Dir();
                break;
        }
        
     

        
        animator.SetBool("isWalking", isWalking);
        if (isWalking && changed_direction)
        {
       //    Debug.Log($"Velocity {my_rigidbody.velocity} changed direction {changed_direction}\n");
            animator.SetFloat("x", input_x);
            animator.SetFloat("y", input_y);
         //   Debug.Log(my_ai.forward_direction_angle + " -> " + " X " + input_x + " Y " + input_y + "\n");
        }
    }

    bool changeDirection(float new_x, float new_y)
    {
        /*
        if (desired_input_x == 0 && desired_input_y == 0 && (new_x != 0 || new_y != 0))
        {
            desired_input_x = new_x;
            desired_input_y = new_y;
            direction_time = min_direction_time;
        }
        else(*/ if (new_x == desired_input_x && new_y == desired_input_y)
            direction_time += Time.deltaTime;
        else
        {
            direction_time = 0f;
            desired_input_x = new_x;
            desired_input_y = new_y;
        }


        if (direction_time < min_direction_time) return false;
   
            
        input_x = desired_input_x;
        input_y = desired_input_y;
        return true;
    }
    
    bool set4Dir()
    {
        float new_x, new_y;
        
        Vector3 velocity = my_rigidbody.velocity;
        if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y))
        {
            if (velocity.x > 0)
            {                               
                new_x = 1f;
                new_y = 0f;
            }
            else
            {
                new_x = -1f;
                new_y = 0f;
            }

        }
        else
        {
            if (velocity.y > 0)
            {
                new_y = 1f;
                new_x = 0f;
            }
            else
            {
                new_y = -1f;
                new_x = 0f;
            }
        }
        return changeDirection(new_x, new_y);
    }


    bool setVehicle4Dir()
    {
        float new_x, new_y;
        Vector3 velocity = my_rigidbody.velocity;
        if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y)*0.4f)
        {
            if (velocity.x > 0)
            {
                new_x = 1f;
                new_y = 0f;
            }
            else
            {
                new_x = -1f;
                new_y = 0f;
            }

        }
        else
        {
            if (velocity.y > 0)
            {
                new_y = 1f;
                new_x = 0f;
            }
            else
            {
                new_y = -1f;
                new_x = 0f;
            }
        }
        
        return changeDirection(new_x, new_y);
    }

    void set8Dir()
    {
        Vector3 velocity = my_rigidbody.velocity.normalized;
       // Debug.Log("velociy " + velocity + "\n");

        float max = 0.9f;

        if (Mathf.Abs(velocity.x) > max)
        {
            if (velocity.x > 0)
            {
                input_x = 1f;
                input_y = 0f;
            }
            else
            {
                input_x = -1f;
                input_y = 0f;
            }

        }
        else if (Mathf.Abs(velocity.y) > max)
        {
            if (velocity.y > 0)
            {
                input_y = 1f;
                input_x = 0f;
            }
            else
            {
                input_y = -1f;
                input_x = 0f;
            }
        }else if (Mathf.Abs(velocity.x) > 0)
        {
            if (velocity.y > 0)
            {
                input_x = 0.5f;
                input_y = 0.5f;                
            }
            else
            {
                input_x = 0.5f;
                input_y = -0.5f;                
            }
        }else
        {
            if (velocity.y > 0)
            {
                input_x = -0.5f;
                input_y = 0.5f;
            }
            else
            {
                input_x = -0.5f;
                input_y = -0.5f;
            }
        }

    }
}