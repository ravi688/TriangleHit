using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System; 


public class StandardAnimationPlayer : IManualUpdatable
{
    public bool IsRunning { get; set; }
    public bool IsUnscaledTime { get; set; } 
    public static List<StandardAnimationPlayer> Players;
    public Action OnEnd; 
    AlphaAdapter alphaChangable;
    StandardAnimation animationData;
    Transform transformable;
    Vector3 original_scale;
    Vector3 original_position;
    Quaternion original_rotation;
    float timing = 0.0f;

    public StandardAnimationPlayer()
    {
        if (Players == null)
            Players = new List<StandardAnimationPlayer>();
        Players.Add(this);
        IsUnscaledTime = false;  
    }
    public void Update()
    {
        float _timinig_seed;
        if (!animationData.IsLoop)
            _timinig_seed = ((IsUnscaledTime ? Time.unscaledTime : Time.time) - timing) / animationData.AnimationDuration;
        else
            _timinig_seed = Time.time;
        Vector3 _position = Vector3.zero;
        if (animationData.IsAnimatePosition)
            _position = original_position + new Vector3(
            animationData.MaxPositionOffset.x * animationData.XPositionOffsetOverTime.Evaluate(_timinig_seed),
            animationData.MaxPositionOffset.y * animationData.YPositionOffsetOverTime.Evaluate(_timinig_seed),
            animationData.MaxPositionOffset.z * animationData.ZPositionOffsetOverTime.Evaluate(_timinig_seed)
            );
        Vector3 _localScale = Vector3.zero;
        if (animationData.IsAnimateScale)
            _localScale = original_scale + new Vector3(
            animationData.MaxScaleOffset.x * animationData.XScaleOffsetOverTime.Evaluate(_timinig_seed),
            animationData.MaxScaleOffset.y * animationData.YScaleOffsetOverTime.Evaluate(_timinig_seed),
            animationData.MaxScaleOffset.z * animationData.ZScaleOffsetOverTime.Evaluate(_timinig_seed)
            );

        Quaternion _rotation = Quaternion.identity;
        if (animationData.IsAnimateRotation)
            _rotation = original_rotation * Quaternion.Euler(new Vector3(
            animationData.MaxRotationOffset.x * animationData.XRotationOffsetOverTime.Evaluate(_timinig_seed),
            animationData.MaxRotationOffset.y * animationData.YRotationOffsetOverTime.Evaluate(_timinig_seed),
            animationData.MaxRotationOffset.z * animationData.ZRotationOffsetOverTime.Evaluate(_timinig_seed)
            ));
        float _alpha = animationData.AlphaOverTime.Evaluate(_timinig_seed);
        if (animationData.IsLocalSpace)
        {
            if (animationData.IsAnimatePosition)
                transformable.localPosition = _position;
            if (animationData.IsAnimateRotation)
                transformable.localRotation = _rotation;
        }
        else
        {
            if (animationData.IsAnimatePosition)
                transformable.position = _position;
            if (animationData.IsAnimateRotation)
                transformable.rotation = _rotation;
        }
        if (animationData.IsAnimateScale)
            transformable.localScale = _localScale;
        if (animationData.IsAnimationAlpha)
        {
            alphaChangable.SetAlpha(_alpha);
        }
        if (!animationData.IsLoop && _timinig_seed >= 1.0f)
        {
            IsRunning = false;
            if (OnEnd != null)
            {
                OnEnd(); 
            }
        }
    }
    public void ResetToDefaultConfiguration()
    {
        if (animationData.IsLocalSpace)
        {
            if (animationData.IsAnimatePosition)
                transformable.localPosition = original_position;
            if (animationData.IsAnimateRotation)
                transformable.localRotation = original_rotation;
        }
        else
        {
            if (animationData.IsAnimatePosition)
                transformable.position = original_position;
            if (animationData.IsAnimateRotation)
                transformable.rotation = original_rotation;
        }
        if (animationData.IsAnimateScale)
            transformable.localScale = original_scale;
        if (animationData.IsAnimationAlpha)
            alphaChangable.SetAlpha(animationData.AlphaOverTime.Evaluate(1));
    }

    public void Stop()
    {
        IsRunning = false;
        ResetToDefaultConfiguration();
    }
    public void InitializeWith(AlphaAdapter alphaChangable, Transform transformable, StandardAnimation animationData)
    {
        this.animationData = animationData;
        this.alphaChangable = alphaChangable;
        this.transformable = transformable;
        if (animationData.IsLocalSpace)
        {
            if (animationData.IsAnimatePosition)
                original_position = transformable.localPosition;
            if (animationData.IsAnimateRotation)
                original_rotation = transformable.localRotation;
        }
        else
        {
            if (animationData.IsAnimatePosition)
                original_position = transformable.position;
            if (animationData.IsAnimateRotation)
                original_rotation = transformable.rotation;
        }
        if (animationData.IsAnimateScale)
            original_scale = transformable.localScale;
    }
    public void PlayAtPosition(Vector2 position, Vector3 scale, Quaternion rotation)
    {
        original_position = position;
        original_rotation = rotation;
        original_scale = scale;
        //if (animationData.IsAnimationAlpha)
        //    alphaChangable.SetAlpha(alpha);
        IsRunning = true; timing = IsUnscaledTime ? Time.unscaledTime : Time.time;
    }

}
