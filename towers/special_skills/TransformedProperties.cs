using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System;

[System.Serializable]
public class TransformedProperties
{
    public string name;
    public float speed;
    public List<Defense> defenses;
    public Vector3 sprite_size;
    public Sprite sprite;
    public Vector2 collider_size;
    public float rotation_lerp_amount;
    public float rotation_inverse_speed_factor;
    public float rotation_interval;
    public string physics_material;
    public float linear_drag;
    public float angular_drag;
}
