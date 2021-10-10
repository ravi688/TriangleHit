using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformAnimCurve : MonoBehaviour
{

    public AnimationCurve YOffset;
    public AnimationCurve XOffset;
    public Vector2 MaxOffset = Vector2.one *10; 

    public bool isLoop = true;
    public float PlayAfterSeconds = 1.0f;
    public float duration = 5.0f;
    public float animationSpeed = 1.0f;  
    public bool Play;

    bool isTrigger;
    bool isPlaying;
    float timing;
    Vector2 initial_position;
    void Awake()
    {
        isTrigger = false;
        isPlaying = false;
       // initial_position = transform.position;
    }

    void Update()
    {
        if (Play && !isTrigger)
        {
            isTrigger = true;
            timing = Time.time;
        }
        if (!isPlaying && isTrigger && Time.time - timing > PlayAfterSeconds)
        {
            isPlaying = true;
            timing = Time.time;
            initial_position = transform.position; 
        }
        if (isLoop && Play && isPlaying)
            transform.position = initial_position + new Vector2(XOffset.Evaluate(Time.time * animationSpeed) * MaxOffset.x,
                YOffset.Evaluate(Time.time * animationSpeed) * MaxOffset.y);
        else if(!isLoop)
        {
            if (isPlaying)
            {

                float _01t = Mathf.Clamp01((Time.time - timing) / duration);
                transform.position = initial_position + new Vector2(XOffset.Evaluate(_01t * animationSpeed) * MaxOffset.x,
                    YOffset.Evaluate(_01t * animationSpeed)* MaxOffset.y);
                if (!Play || _01t == 1.0f)
                {
                    isPlaying = false;
                    Play = false;
                    isTrigger = false; 
                }

            }
        }
    }



}
