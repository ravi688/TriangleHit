using UnityEngine;
using System;


public class FadeController : IManualUpdatable
{
    public bool IsRunning { get; private set; }
    public Action OnFadeIn;
    public Action OnFadeOut;
    public bool IsUnscaledTime = false;

    private float inverse_fade_duration;
    bool isActivated = true;
    float timing = 0;
    AlphaAdapter alphaChangeable;
    public float Alpha
    {
        get { return alphaChangeable.GetAlpha(); }
        set
        {
            if (value == 0)
            {
                isActivated = false;
                IsRunning = false;
                alphaChangeable.SetAlpha(0);
            }
            else if (value == 1)
            {
                isActivated = true;
                IsRunning = true;
                alphaChangeable.SetAlpha(1);
            }
            alphaChangeable.SetAlpha(value);
        }
    }

    public FadeController(AlphaAdapter AlphaChangeable, float duration = 0.5f)
    {
        IsRunning = false;
        inverse_fade_duration = 1 / duration;
        alphaChangeable = AlphaChangeable;
        isActivated = alphaChangeable.GetAlpha() == 0 ? false : true;
    }
    public void Reset()
    {
        IsRunning = false;
        isActivated = alphaChangeable.GetAlpha() == 0 ? false : true;
    }
    public void InstantFadeIn()
    {
        alphaChangeable.SetAlpha(1);
        isActivated = true;
        if (OnFadeIn != null)
            OnFadeIn();
    }
    public void InstantFadeOut()
    {

        isActivated = false;
        if (OnFadeOut != null)
            OnFadeOut();
    }
    public void Update()
    {
        float _01t = (Time.time - timing) *  inverse_fade_duration;
        float alpha_value;
        if (!isActivated)
            alpha_value = Mathf.SmoothStep(1.0f, 0.0f, _01t);
        else
            alpha_value = Mathf.SmoothStep(0.0f, 1.0f, _01t);
        if (_01t >= 1.0f)
        {
            IsRunning = false;
            if (OnFadeIn != null && isActivated)
                OnFadeIn();
            if (OnFadeOut != null && !isActivated)
                OnFadeOut();
        }
        alphaChangeable.SetAlpha(alpha_value);
    }
    public void FadeOut()
    {
        if (!isActivated) return;
        isActivated = false;
        IsRunning = true;
        timing = Time.time;

    }
    public void FadeIn()
    {
        if (isActivated) return;
        isActivated = true;
        IsRunning = true;
        timing = Time.time;
    }

}