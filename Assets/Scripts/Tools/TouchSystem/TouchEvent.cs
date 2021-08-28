using UnityEngine;
using System;
public class TouchEvent
{
    public static int EventCount { get; private set; }
    public delegate bool TouchCondition(Vector2 inputPos);   //inputPos for touchManager to pass every touch position if they satisfy

    public TouchCondition Condition; //This is  Actually the Condition Based on Which TouchManager will Assign Touch Ids
    public bool isStatic;
    public bool IsCheckConditionEveryFrame;             //Note  : It may take some performance cost
    public bool IsCallOnEndedAfterTouchDataLost;
    public int LayerID;             //Layer Id for this Touch Event
    public Touch touch;             //This is the updated touch assigned every frame 
    public Action OnBegan;        //This is called when the touch just satifies the Condition
    public Action OnMoved;        //This is called when the touch moving
    public Action OnStationary;   //This is called when the touch stationary
    public Action OnEnded;        //This is called when the touch is ended or canceled

    public TouchEvent()
    {
        isStatic = false;
        touch.fingerId = -1;       //Must be -ve initially
        IsCallOnEndedAfterTouchDataLost = true;
        IsCheckConditionEveryFrame = false;
        OnBegan = delegate () { };
        OnMoved = delegate () { };
        OnStationary = delegate () { };
        OnEnded = delegate () { };
        Condition = delegate (Vector2 input_pos) { return false; };
        EventCount++;
    }

}

public class TouchEvent<S> : TouchEvent
{
    public S args;
    public delegate bool TouchCondition<T>(Vector2 inputPos, T args);   //inputPos for touchManager to pass every touch position if they satisfy

    public TouchCondition<S> condition; //This is  Actually the Condition Based on Which TouchManager will Assign Touch Ids

    public Action<S> on_began;        //This is called when the touch just satifies the Condition
    public Action<S> on_moved;        //This is called when the touch moving
    public Action<S> on_stationary;   //This is called when the touch stationary
    public Action<S> on_ended;        //This is called when the touch is ended or canceled

    public TouchEvent(S __args) : base()
    {
        args = __args;
        on_began = delegate (S args) { };
        on_moved = delegate (S args) { };
        on_stationary = delegate (S args) { };
        on_ended = delegate (S args) { };

        OnBegan = delegate () { on_began(args); };
        OnMoved = delegate () { on_moved(args); };
        OnStationary = delegate () { on_stationary(args); };
        OnEnded = delegate () { on_ended(args); };
        Condition = delegate (Vector2 input_pos) { return condition(input_pos, args); };

    }

}