using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ParallaxLayer
{
    public Transform layer;
    public float distance;
}


public class ParallaxAnimation : MonoBehaviour
{

    public ParallaxLayer[] layers;
    public float max_forground_offset;
    public float duration = 5.0f;
    public float animation_speed = 1.0f;
    public bool isLoop = false;
    public AnimationCurve XDirectionOffsetOverTime;
    public AnimationCurve YDirectionOffsetOverTime;
    public bool Play = false;

    short num_layers;
    float timing;
    bool is_animating;
    bool _isInitialized;
    Vector2[] initial_layers_positions;
    private float inverse_duration;

    void Awake()
    {
        num_layers = (short)layers.Length;
        initial_layers_positions = new Vector2[num_layers];
        is_animating = false;
        _isInitialized = false;
        for (int i = 0; i < num_layers; i++)
        {
            initial_layers_positions[i] = layers[i].layer.position;
            layers[i].distance = 1 / layers[i].distance;
        }
        inverse_duration = (float)1 / duration;
    }


    void LateUpdate()
    {
        if (Play && !_isInitialized)
        {
            timing = Time.time;
            is_animating = true;
            _isInitialized = true;
            ResetToInitialPosition();
        }
        if (is_animating)
        {
            Vector2 offset;
            if (!isLoop)
            {
                float _01t = Mathf.Clamp01((Time.time - timing) * inverse_duration);
                offset = max_forground_offset * new Vector2(XDirectionOffsetOverTime.Evaluate(_01t), YDirectionOffsetOverTime.Evaluate(_01t));
                if (_01t == 1) { is_animating = false; _isInitialized = false; Play = false; }
            }
            else
                offset = max_forground_offset * new Vector2(XDirectionOffsetOverTime.Evaluate(Time.time * animation_speed), 
                    YDirectionOffsetOverTime.Evaluate(Time.time * animation_speed));
            MoveLayers(offset);
            if (!Play)
                is_animating = false;
        }

    }

    void ResetToInitialPosition()
    {
        for (int i = 0; i < num_layers; i++)
            layers[i].layer.position = initial_layers_positions[i];
    }

    void MoveLayers(Vector3 max_for_ground_offset)
    {
        for (int i = 0; i < num_layers; i++)
            layers[i].layer.position = (Vector3)initial_layers_positions[i] + max_for_ground_offset * layers[i].distance;
    }

}
