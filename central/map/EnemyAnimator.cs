using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;

public class EnemyAnimator : MonoBehaviour
{
    public AnimationType type;
    public Animator animator;
    public AI my_ai;
    public Rigidbody2D my_rigidbody;
    public float input_x;
        public float input_y;
    public bool isWalking;

    void Update()
    {
        if (my_ai.animate == AnimationType.None) return;
    
        isWalking = (my_ai.current_speed > 0.1f);
        Vector3 velocity = my_rigidbody.velocity;
       

        switch (type)
        {
            case AnimationType.Animate_Sprite_4Dir:
                set4Dir();
                break;
            case AnimationType.Animate_Sprite_8Dir:
                set8Dir();
                break;
        }
        
     

        
        animator.SetBool("isWalking", isWalking);
        if (isWalking)
        {
            animator.SetFloat("x", input_x);
            animator.SetFloat("y", input_y);
         //   Debug.Log(my_ai.forward_direction_angle + " -> " + " X " + input_x + " Y " + input_y + "\n");
        }
    }

    void set4Dir()
    {
        Vector3 velocity = my_rigidbody.velocity;
        if (Mathf.Abs(velocity.x) > Mathf.Abs(velocity.y))
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
        else
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
        }

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