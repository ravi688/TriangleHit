using UnityEngine;
using System;

public enum ScrollAxis
{
    Vertical,
    Horizontal
}

public class ScrollingSystem : IManualUpdatable
{
    public bool IsAutoCullCards = false;

    //Data Holders
    private int Sensitivity;
    private int Damping;
    private int CardCount;
    private int DeactivateOffset;
    private float ClampStiffness;
    private Transform[] cards;


    //Runtime Variables
    private Action OnDrawerGrab = delegate { };
    private Action OnDrawerRelease = delegate { };
    private Action OnDrawerDragged = delegate { };

    private Vector2 restMousePos;
    private Vector2 prev_dir = Vector2.right;
    private Vector2 curr_dir;
    private Vector2 prevMousePos;
    private Vector2 DeltaMouseDisplacement;

    private float speed;
    private float restTime;
    private bool isDrawerGrabbed = false;
    private ScrollAxis scroll_axis;
    private RectTransform scroll_rect;

    private bool isRightClamping;
    private bool isLeftClamping;
    private bool isInputWorking;
    private bool isRunning;
    public bool IsRunning
    {
        get { return isRunning; }
        set
        {
            if (value == false)
            {
                Deactivate();
                isRunning = false;
            }
            else
            {
                HandleCullingOfCards();
                isRunning = true;
            }
        }
    }

    public ScrollingSystem(RectTransform rect, Transform transform,
        int cardPixelSize = 240,
        int offset = 0,
        int sensitivity = 1,
        int damping = 10,
        int clampStiffness = 10,
        int deactivateOffset = 100,
        ScrollAxis axis = ScrollAxis.Horizontal)
    {
        this.Sensitivity = sensitivity;
        this.Damping = damping;
        this.ClampStiffness = clampStiffness;
        this.DeactivateOffset = deactivateOffset;
        this.scroll_axis = axis;
        this.scroll_rect = rect;
        this.isRunning = false;

        CardCount = transform.childCount;
        cards = new Transform[CardCount];
        Vector3 dir = scroll_axis == ScrollAxis.Vertical ? Vector3.down : Vector3.right;
        for (int i = 0; i < CardCount; i++)
        {
            cards[i] = transform.GetChild(i);
            cards[i].localPosition = (cardPixelSize + offset) * i * dir;
        }
        OnDrawerGrab += InitializeDrawer;

        OnDrawerRelease += CalculateSpeed;

        OnDrawerDragged += MoveDrawer;
        OnDrawerDragged += RefreshRestMousePos;
    }
    public void SetDrawerToCardIndex(int index)
    {
        MoveCardsTo(-cards[index].localPosition);
    }
    private void Deactivate()
    {
        for (int i = 0; i < CardCount; i++)
            cards[i].gameObject.SetActive(false);
    }

    public void Update()
    {
        //This is for Detecting Whether the Touch or mouse Pointer is Inside the Game Screen or Not !
        if (!isInputWorking && InputManager.isInputPresent)
        {
            prevMousePos = InputManager.GetInputPosition();
            isInputWorking = true;
        }
        if (isInputWorking && !InputManager.isInputPresent)
        {
            isInputWorking = false;
        }
        //////////////////////////////////////////////////////////////////////////////////////////////

        //Must be before Handle Input since in handleInput actually events such as moving drawer are called which depend on this
        //Here Order is Important in following 3 lines of Code
        //First Calculate the Mouse Delta Position Based on the previous position
        DeltaMouseDisplacement = InputManager.GetInputPosition() - prevMousePos;
        //Fires all the Events such as when the DrawerisGrabbed , DrawerisBeginDragged, DrawerisReleased
        HandleInput();
        prevMousePos = InputManager.GetInputPosition();

        //This is for Handling OverFlow of Drawer and Interia Simulation
        isRightClamping = ((scroll_axis == ScrollAxis.Vertical) ? -cards[0].transform.localPosition.y : cards[0].transform.localPosition.x) > 0;
        isLeftClamping = ((scroll_axis == ScrollAxis.Vertical) ? -cards[CardCount - 1].transform.localPosition.y : cards[CardCount - 1].transform.localPosition.x) < 0;
        if (!isDrawerGrabbed)
        {

            if (speed != 0)
                //Just like inertia
                AutoScrollBasedOnSpeed();
            //If The Drawer is Clamped to be Left or Right
            if (isRightClamping || isLeftClamping)
            {
                ProcessSpringClamping();
                if (speed != 0) speed = 0;
            }
        }
        /////////////////////////////////////////////////////////////////
        HandleCullingOfCards();

    }
    private void HandleCullingOfCards()
    {
        bool isActiveCard;
        bool isOutSideOfScreen = false;
        for (int i = 0; i < CardCount; i++)
        {
            isActiveCard = cards[i].gameObject.activeSelf;
            switch (scroll_axis)
            {
                case ScrollAxis.Horizontal:
                    isOutSideOfScreen = Mathf.Abs(cards[i].localPosition.x) > (scroll_rect.rect.width * 0.5f + DeactivateOffset);
                    break;
                case ScrollAxis.Vertical:
                    isOutSideOfScreen = Mathf.Abs(cards[i].localPosition.y) > (scroll_rect.rect.height * 0.5f + DeactivateOffset);
                    break;
            }
            if (isOutSideOfScreen && isActiveCard)
            {
                cards[i].gameObject.SetActive(false);
            }
            else if (!(isOutSideOfScreen || isActiveCard))
                cards[i].gameObject.SetActive(true);
        }
    }
    private void ProcessSpringClamping()
    {
        if (isRightClamping)
            MoveCardsTo(-cards[0].localPosition * Time.deltaTime * ClampStiffness);
        if (isLeftClamping)
            MoveCardsTo(-cards[CardCount - 1].localPosition * Time.deltaTime * ClampStiffness);
    }

    private void InitializeDrawer()
    {
        speed = 0;
        //The position of the mouse from where the Drawer is Grabbed
        restMousePos = InputManager.GetInputPosition();
        //The time when the Drawer is grabbed
        //This is used to Calculate the Average Velocity of the Mouse Pointer 
        //So that the Drawer Can be Auto Scrolled based on the Velocity
        restTime = Time.time;
    }

    //This Method is devoted to Refresh the Rest Mouse position
    //Since the user can change the Dragging Diraction Any time , So we have to keep track of when the 
    //direction is changed and reset the RestMousePos and RestTime
    private void RefreshRestMousePos()
    {
        curr_dir = InputManager.GetInputPosition() - prevMousePos;
        //Whether the direction is Changed or not
        if (Vector2.Dot(prev_dir, curr_dir) < 0)
        {
            restMousePos = InputManager.GetInputPosition();
            restTime = Time.time;
            prev_dir = curr_dir;
        }
    }
    private void MoveDrawer()
    {
        MoveCardsTo(scroll_axis == ScrollAxis.Horizontal ? Vector2.right * DeltaMouseDisplacement.x : Vector2.up * DeltaMouseDisplacement.y);
    }
    private void HandleInput()
    {
        if (!isDrawerGrabbed && InputManager.IsInputDown() && IsInDrawerBounds(InputManager.GetInputPosition()))
        { isDrawerGrabbed = true; OnDrawerGrab(); }
        if (isDrawerGrabbed && InputManager.IsInputUp())
        { isDrawerGrabbed = false; OnDrawerRelease(); }
        if (isDrawerGrabbed)
            OnDrawerDragged();
    }
    private void CalculateSpeed()
    {
        switch (scroll_axis)
        {
            case ScrollAxis.Horizontal:
                speed = (float)(InputManager.GetInputPosition().x - restMousePos.x) / (Time.time - restTime);
                break;
            case ScrollAxis.Vertical:
                speed = (float)(InputManager.GetInputPosition().y - restMousePos.y) / (Time.time - restTime);
                break;
        }
    }
    private void AutoScrollBasedOnSpeed()
    {
        if (Double.IsNaN(speed))
            return;
        MoveCardsTo(Sensitivity * speed  * Time.deltaTime * (scroll_axis == ScrollAxis.Horizontal ?  Vector3.right : Vector3.up));
        speed = Mathf.Lerp(speed, 0, Time.deltaTime * Damping);
    }
    private bool IsInDrawerBounds(Vector2 inputPos)
    {
        bool result =  RectTransformUtility.RectangleContainsScreenPoint(scroll_rect, inputPos);
#if DEBUG_MODE
        if (!result)
            Debug.Log("Not in bounds");
#endif
        return result;
        //return inputPos.y < Screen.height * 0.5f + DrawerHeight * 0.5f && inputPos.y > Screen.height * 0.5f - DrawerHeight * 0.5f;
    }
    private void MoveCardsTo(Vector2 Displacement)
    {
        for (int i = 0; i < CardCount; i++)
            cards[i].localPosition += (Vector3)Displacement;

    }

}