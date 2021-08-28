using UnityEngine;
using System.Collections.Generic;
using System;


public class TouchManager : IManualUpdatable
{
    public bool IsRunning { get;  set; }
    public Action OnInitialize;

    private Touch[] touches;                         //Stores the Current Raw Touches present on the Screen
    private List<TouchLayer> TouchLayers;            //Touch Layers registered by the Queries
    private List<int> ReservedFingerIDs;             //This is used to store the current reserved touche finger ids based on the conditions
    private List<TouchEvent> RegisteredEvents;       //This stores all the Registered Touch Events of all the layers

    private int num_touches;                         //This stores the no of raw touch counts present on the screen
    private int layer_count;                         //This stores touch layer count which are registerd by TouchEvents
    private bool IsAnyChange;                        //This will be true in the current frame in which any change in touch count has occured
    private bool isCleared;                          //This is temporary variable , holds the information whether Registered Ids are cleared or not after the Touch count == 0
    private bool IsInitialized;                      //This is temporary variable and most important, initially It is false
    //It is assumed that no  script can register TouchEvent During After one Frame Update
    //Since after the Start, Update is Called for First Time , in the update
    //all the layers are sorted and initialized
    //This is so because any script is free to register TouchEvents in Start or Awake
    private List<int> ignoredLayerIDs;

    public void UnregisterEvent(TouchEvent touchEvent)
    {
        if (RegisteredEvents == null)
            return;
        if (TouchLayers == null)
            return;
        for (int i = 0; i < layer_count; i++)
        {
            if (TouchLayers[i].id == touchEvent.LayerID)
            {
                TouchLayers[i].touchEvents.Remove(touchEvent);
            }
        }
    }

    //This setter must be called to register an TouchEvent from any Script
    public void RegisterEvent(TouchEvent touchEvent)
    {
        if (RegisteredEvents == null)
            RegisteredEvents = new List<TouchEvent>();
        if (TouchLayers == null)
            TouchLayers = new List<TouchLayer>();
        if (!IsAlreadyRegisteredLayer(touchEvent.LayerID))
        {
            TouchLayer new_layer = new TouchLayer();
            new_layer.id = touchEvent.LayerID;
            new_layer.touchEvents.Add(touchEvent);
            TouchLayers.Add(new_layer);
        }
        else    //if Already Registered Layer
        {       //Find the Layer and Add the touchEvent to this Event
            GetTouchLayerWithID(touchEvent.LayerID).touchEvents.Add(touchEvent);
        }

        RegisteredEvents.Add(touchEvent);
    }
    //This is to Query whether passed TouchEvent has Touched or not
    public bool IsTouched(TouchEvent touchEvent)
    {
        return IsRunning && isReserved(touchEvent.touch.fingerId);
    }

    public void AddIgnoreLayer(int id)
    {
        if (ignoredLayerIDs == null)
            ignoredLayerIDs = new List<int>();
        if (!ignoredLayerIDs.Contains(id))
            ignoredLayerIDs.Add(id);
    }
    public void RemoveIgnoreLayer(int id)
    {
        if (ignoredLayerIDs == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning("Ignore Layer List is empty, but you still trying to get element from it");
#endif
            return;
        }
        ignoredLayerIDs.Remove(id);
    }

    //This is to Query whether the passed Finger id is current running for other Events or not 
    private bool isReserved(int id)
    {
        int count = ReservedFingerIDs.Count;
        for (int i = 0; i < count; i++)
            if (ReservedFingerIDs[i] == id)
                return true;
        return false;
    }
    //This is used to Get Touch with fingerId, used to assign appropriate touches to the TouchEvents for Continuos tracking
    private Touch GetTouchWithID(int id)
    {
        Touch out_touch = new Touch();
        for (int i = 0; i < num_touches; i++)
            if (id == touches[i].fingerId)
            {
                out_touch = touches[i];
                break;
            }
            else
                continue;
        return out_touch;
    }
    //This is used to Reserve a fingerId so that any other TouchEvent can't access the touch
    //until this reserved fingerId is UnReserved by calling and passing the fingerId to UnReserve(int id) method
    private void ReserveID(int id)
    {
        if (ReservedFingerIDs == null)
            ReservedFingerIDs = new List<int>();
        ReservedFingerIDs.Add(id);
    }
    private void UnReserveID(int id)
    {
        ReservedFingerIDs.Remove(id);
    }
    //This is used to Get the TouchLayer with the layerId
    private TouchLayer GetTouchLayerWithID(int id)
    {
        TouchLayer out_layer = new TouchLayer();
        for (int i = 0; i < TouchLayers.Count; i++)
            if (TouchLayers[i].id == id)
                out_layer = TouchLayers[i];
        return out_layer;
    }
    //This is used to Query Whether the Current Layerid is Already Exists or Not
    //If this Exists the new TouchEvent with the same Id will be add to the Layer
    private bool IsAlreadyRegisteredLayer(int id)
    {
        for (int i = 0; i < TouchLayers.Count; i++)
        {
            if (TouchLayers[i].id == id)
                return true;
        }
        return false;
    }
    //This is used to Initialize the layers during the First Update Call
    private void InitializeRegisteredEvents()
    {
        if (OnInitialize != null)
            OnInitialize();

        if (ReservedFingerIDs == null)
            ReservedFingerIDs = new List<int>();
        isCleared = true;
        IsInitialized = true;
        layer_count = TouchLayers.Count;
        SortTouchLayer();
#if UNITY_EDITOR
        if (RegisteredEvents.Count != TouchEvent.EventCount)
            Debug.LogWarning("NOTE: You have created some Touch Events but some are left to register");
#endif

    }
    private void SortTouchLayer()
    {
        //TODO: 
        //Replace this Sorting Algorith, i.e. Bubble Sort, with Faster Sorting Algorithm.
        for (int i = 1; i < layer_count; i++)
            for (int j = 0; j < (layer_count - i); j++)
                if (TouchLayers[j].id < TouchLayers[j + 1].id)
                {
                    TouchLayer layer = TouchLayers[j];
                    TouchLayers[j] = TouchLayers[j + 1];
                    TouchLayers[j + 1] = layer;
                }
    }
    private static bool is_continue = false;
    //This is Very Important , the Core of the TouchManager
    private void HandleTouchAssignments()
    {
        if (IsAnyChange)
        {
            for (int i = 0; i < num_touches; i++)
            {
                Touch touch = touches[i];
                for (int j = 0; j < layer_count; j++)
                {
                    is_continue = false;
                    for (int k = 0; ignoredLayerIDs != null && k < ignoredLayerIDs.Count; k++)
                        if (TouchLayers[j].id == ignoredLayerIDs[k])
                        {
                            is_continue = true;
                            break;
                        }
                    if (is_continue)
                        continue;
                    int eventCount = TouchLayers[j].touchEvents.Count;
                    List<TouchEvent> touchEvents = TouchLayers[j].touchEvents;
                    for (int k = 0; k < eventCount; k++)
                    {
                        if (!IsTouched(touchEvents[k]) && !isReserved(touch.fingerId) && touchEvents[k].Condition(touch.position - DeviceUtility.screen_size * 0.5f) == true)
                        {
                            touchEvents[k].touch = touch;
                            touchEvents[k].OnBegan();
                            ReserveID(touch.fingerId);
                        }

                    }

                }
            }
        }
        for (int j = 0; j < layer_count; j++)
        {
            is_continue = false;
            for (int i = 0; ignoredLayerIDs != null && i < ignoredLayerIDs.Count; i++)
                if (TouchLayers[j].id == ignoredLayerIDs[i])
                {
                    is_continue = true;
                    break;
                }
            if (is_continue)
                continue;
            int eventCount = TouchLayers[j].touchEvents.Count;
            List<TouchEvent> touchEvents = TouchLayers[j].touchEvents;
            for (int k = 0; k < eventCount; k++)
            {
                TouchEvent touchEvent = touchEvents[k];
                if (touchEvent.touch.fingerId != -1)
                {
                    touchEvent.touch = GetTouchWithID(touchEvent.touch.fingerId);
                    if (touchEvent.OnMoved != null && touchEvent.touch.phase == TouchPhase.Moved)
                        touchEvent.OnMoved();
                    else if (touchEvent.OnStationary != null && touchEvent.touch.phase == TouchPhase.Stationary)
                        touchEvent.OnStationary();
                    else if (touchEvent.touch.phase == TouchPhase.Ended || touchEvent.touch.phase == TouchPhase.Canceled)
                    {
                        if (touchEvent.OnEnded != null && !touchEvent.IsCallOnEndedAfterTouchDataLost)
                            touchEvent.OnEnded();
                        UnReserveID(touchEvent.touch.fingerId);
                        touchEvent.touch = new Touch();
                        touchEvent.touch.fingerId = -1;
                        if (touchEvent.OnEnded != null && touchEvent.IsCallOnEndedAfterTouchDataLost)
                            touchEvent.OnEnded();
                    }
                }
            }
        }
    }
    //It Takes the Input from the Device
    private void ManageRawTouchInput()
    {
        touches = Input.touches;
        if (isCleared && Input.touchCount != 0)
            isCleared = false;
        if (!isCleared && Input.touchCount == 0)
        {
            ReservedFingerIDs.Clear();
            isCleared = true;
        }
        if (num_touches != Input.touchCount)
        {
            num_touches = Input.touchCount;
            IsAnyChange = true;
        }
        else
            IsAnyChange = false;
    }


    public TouchManager()
    {
        if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
        {
 
            IsRunning = false;
#if DEBUG_MODE
            GameManager.GetLogManager().LogWarning("Touch System is Disabled, Since the current platform isn't a mobile platform");
#endif
            return;
        }
        IsInitialized = false;
        IsRunning = true;
    }
    public void Update()
    {
        if (!IsInitialized)
            InitializeRegisteredEvents();
        ManageRawTouchInput();
        HandleTouchAssignments();
    }
}
