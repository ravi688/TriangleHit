#if DISABLE_WARNINGS
#pragma warning disable
#endif

using System;
using UnityEngine;

public class CAnimator : MonoBehaviour
{

     [SerializeField]
    private CAnimation[] Animations;
     [SerializeField]
     private bool PlayOnAwake = false;
     [SerializeField]
     private string EntryAnimation = "Idle";


    private CAnimation CurrentAnimation;
    private SpriteRenderer AnimRenderer;
    private float timing = 0;
    private float eachFrameTime = 0;
    private int currentFrameIndex = 0;
    private bool isPlay = false;
    private bool animationTriggered = false; 

    private void Awake()
    {
        AnimRenderer = GetComponent<SpriteRenderer>();
        if (PlayOnAwake)
            SetCurrentAnimation(EntryAnimation); 
    }
    private void Update()
    {
        if (PlayOnAwake)
            Play(); 
        if (isPlay) Play();
    }
    public void Play()
    {
        if (Time.time - timing >= eachFrameTime)
        {
            currentFrameIndex++;
            if (currentFrameIndex > CurrentAnimation.frames.Length - 1 && !CurrentAnimation.isLoop)
            { isPlay = false; animationTriggered = false;  return; }
            AnimRenderer.sprite = CurrentAnimation.frames[currentFrameIndex % CurrentAnimation.frames.Length];
            timing = Time.time;
        }
    }
    public void ResetAnimator()
    {
        isPlay = false;
        timing = 0;
        currentFrameIndex = 0; 
    }
    public void PlayAnimation(string name)
    {
        if (animationTriggered) return; 
        if (name != (CurrentAnimation == null ?"NULL_ANIMATION" : CurrentAnimation.name))
        {
            ResetAnimator();
            SetCurrentAnimation(name); 
        }
        Play(); 
    }
    public void SetCurrentAnimation(string name)
    {
        CurrentAnimation = SearchAnimation(name);
        eachFrameTime = 1 / (float)CurrentAnimation.fps;
        timing = Time.time;
        currentFrameIndex = 0;
        AnimRenderer.sprite = CurrentAnimation.frames[currentFrameIndex];
    }
    public void TriggerAnimation(string name)
    {
        isPlay = true;
        CurrentAnimation = SearchAnimation(name);
        eachFrameTime = 1 / (float)CurrentAnimation.fps;
        timing = Time.time;
        currentFrameIndex = 0;
        AnimRenderer.sprite = CurrentAnimation.frames[currentFrameIndex];
        animationTriggered = true;
    }
    private CAnimation SearchAnimation(string name)
    {
        foreach (CAnimation anim in Animations)
        {
            if (name == anim.name)
                return anim;
        }
        Debug.LogWarning(string.Format("Animatin named : {0} is not found", name));
        return null;
    }

}
