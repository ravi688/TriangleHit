#if DISABLE_WARNINGS
#pragma warning disable
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickDemoController : MonoBehaviour
{


    [SerializeField]
    private StandardAnimation AnimationData;
    [SerializeField]
    private float Radius;

    private Vector2 InputPos;
    private Vector2 offset;
    private bool IsGrabbed;
    private StandardAnimationPlayer AnimationEngine;

    private void Awake()
    {
        AlphaAdapter adapter = new CanvasGroupAlphaAdapter(GetComponent<CanvasGroup>()) ; 
       AnimationEngine = new StandardAnimationPlayer();
       AnimationEngine.InitializeWith(adapter, transform, AnimationData); 
    }
    void OnDisable()
    {
        AnimationEngine.Stop();
    }
    void OnEnable()
    {
        AnimationEngine.PlayAtPosition(transform.localPosition, Vector2.zero, Quaternion.identity);
    }

    void Update()
    {
        GameLoop.Update(AnimationEngine);
        if (!IsGrabbed && InputManager.IsInputDown() && IsInBounds())
        {
            IsGrabbed = true;
            offset = (Vector2)transform.localPosition - InputPos;
        }
        if (IsGrabbed)
        {
            InputPos = InputManager.GetInputPosition() - DeviceUtility.screen_size * 0.5f;
            transform.localPosition = offset + InputPos;
        }
        if (IsGrabbed && InputManager.IsInputUp())
        {
            IsGrabbed = false;
        }
    }

    bool IsInBounds()
    {
        return ((InputPos = InputManager.GetInputPosition() - DeviceUtility.screen_size * 0.5f) - (Vector2)transform.localPosition).sqrMagnitude < Radius * Radius;
    }

}
