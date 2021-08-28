using UnityEngine;
using System.Collections;

public abstract class PowerUpSettings : ScriptableObject
{
    //Non Serialized Properties
    public float frequency { get { return 1 / time_interval; } set { time_interval = 1 / value; } }


    //Serialized Fields
    public new string name;
    public Sprite glow_sprite;
    public Sprite body_sprite;
    public StandardAnimation pop_up_anim;
    public StandardAnimation missed_anim;
    public StandardAnimation glow_anim; 
    public string sorting_layer_name;
    public int sorting_order = 0;
    public float detect_radius = 1.0f;
    public float duration = 5.0f;
    public float time_interval = 4; //seconds
}
