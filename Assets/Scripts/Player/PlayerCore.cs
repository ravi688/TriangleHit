
using System;
using UnityEngine;

public enum PlayerState
{
    Thrusting,
    Moving
}
#if DISABLE_WARNINGS
#pragma warning disable
#endif


public abstract class PlayerCore : MonoBehaviour
{
    private BarMeter lives_meter;
    private BarMeter health_meter;
    private BarMeter thrust_meter;
    public static BarMeter HealthMeter { get { return instance.health_meter; } protected set { instance.health_meter = value; } }
    public static BarMeter ThrustMeter { get { return instance.thrust_meter; } protected set { instance.thrust_meter = value; } }
    public static int MaxLives
    {
        get
        {
            if (instance == null)
            {
#if DEBUG_MODE
                GameManager.GetLogManager().LogWarning("PlayerCore::instance == null, returing MaxLives zero");
#endif
                return 0;
            }
            else return instance.player_settings.max_lives;
        }
    }

    public static int LiveCount
    {
        get
        {
            return (int)StaticMemory.LivesMeter.Value;
        }
        protected set
        {
            StaticMemory.LivesMeter.Value = value;
            GameLogic.OnUpdatePlayerLiveCount((int)StaticMemory.LivesMeter.Value);
            if (StaticMemory.LivesMeter.Value == 0)
                GameLogic.OnGameOver(StaticMemory.EnemyCount, StaticMemory.DisposedCount);
            GameLogic.OnPlayerLivesFull();
        }
    }
    public static int HealthAmount
    {
        get
        {
            return (int)HealthMeter.Value;
        }
        protected set
        {
            HealthMeter.Value = value;
            GameLogic.OnUpdatePlayerHealth(HealthMeter.Value01);
            if (HealthMeter.Value == 0)
            {
                if (instance != null) instance.OnHealthZero();
                GameLogic.OnPlayerKilled(LiveCount);
            }
            else if (HealthMeter.Value == HealthMeter.MaxValue)
                GameLogic.OnPlayerHealthFull();
        }
    }
    public static float ThrustAmount
    {
        get
        {
            return ThrustMeter.Value;
        }
        protected set
        {
            ThrustMeter.Value = value;
            GameLogic.OnUpdateThrust(ThrustMeter.Value01);
            if (ThrustMeter.Value == 0)
                GameLogic.OnThrustExhausted();
            else if (ThrustMeter.Value == ThrustMeter.MaxValue)
                GameLogic.OnThrustFull();
        }
    }

    protected abstract void OnHealthZero();


    public static Vector2 KilledPosition
#if !RELEASE_MODE
    { get; protected set; }
#else
        ;
#endif

    public static PlayerCore instance;
    public static float BindableRegionDistance
#if !RELEASE_MODE
    { get { return 5.0f; } }
#else
        = 5.0f;
#endif

    private int sorting_layer_id;
    private int player_sorting_order;
    private int dash_sorting_order;
    private bool is_first_player = false;

    private float killTime = 1.0f;

    protected PlayerCoreSettings player_settings;

    private PlayerState STATE;
    private Rigidbody2D body;

    private float currentSpeed;
    private float turnSpeed = 10.0f;                    //Don't modify it since it is harcoded

    protected FadeController thrustMarkController;
    protected FadeController playerFadeController;

    private float moveSpeed;
    private float thrustSpeed;
    private float thrustBarFillUpRate;
    private float thrustBarUseUpRate;

    protected abstract void OnLoadPlayerSettings();

    private void LoadSettings()
    {
        OnLoadPlayerSettings();
        moveSpeed = player_settings.move_speed;
        thrustSpeed = player_settings.thrust_speed;
        thrustBarFillUpRate = player_settings.thrust_bar_fillup_rate;
        thrustBarUseUpRate = player_settings.thrust_bar_useup_rate;
    }
    protected virtual void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
        //Order Matters
        LoadSettings();
        AllocateMemoryForStaticObjects();
        AllocateMemoryForInstanceObjects();
        InitializeVariables();
    }
    protected virtual void Start()
    {
        HealthMeter.Reset();
        ThrustMeter.Reset();
        thrustMarkController.InstantFadeOut();
        playerFadeController.InstantFadeOut();
        thrustMarkController.FadeIn();
        playerFadeController.FadeIn();
        SetCorrectSortingLayer();
    }
    private void AllocateMemoryForStaticObjects()
    {
        HealthMeter = new BarMeter(0, 100);
        ThrustMeter = new BarMeter(0, 100);

        if (StaticMemory.LivesMeter == null)
            StaticMemory.LivesMeter = new BarMeter(0, MaxLives, MaxLives);
    }

    private void AllocateMemoryForInstanceObjects()
    {
        thrustMarkController = new FadeController(new SpriteRendererAlphaAdapter(transform.GetChild(0).GetComponent<SpriteRenderer>()), 0.2f);
        playerFadeController = new FadeController(new SpriteRendererAlphaAdapter(GetComponent<SpriteRenderer>()), killTime);
    }

    private void InitializeVariables()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void SetCorrectSortingLayer()
    {
        if (!is_first_player)
        {
            is_first_player = true;
            sorting_layer_id = GetComponent<SpriteRenderer>().sortingLayerID;
            player_sorting_order = GetComponent<SpriteRenderer>().sortingOrder;
            dash_sorting_order = transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder;
        }
        else
        {
            GetComponent<SpriteRenderer>().sortingLayerID = sorting_layer_id;
            transform.GetChild(0).GetComponent<SpriteRenderer>().sortingLayerID = sorting_layer_id;
            GetComponent<SpriteRenderer>().sortingOrder = player_sorting_order;
            transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = dash_sorting_order;
        }
    }
    protected void TakeInput()
    {
        if (GameManager.GetGameControlSystem().DashButton)
        {
            STATE = PlayerState.Thrusting;
        }
        else
        {
            STATE = PlayerState.Moving;
        }
    }
    private void FixedUpdate()
    {
        switch (STATE)
        {
            case PlayerState.Thrusting:
                if (ThrustMeter.Value > 0)
                {
                    currentSpeed = thrustSpeed;
                    ThrustAmount -= thrustBarUseUpRate * Time.fixedDeltaTime;
                    thrustMarkController.FadeIn();
                }
                else
                {
                    currentSpeed = moveSpeed;
                    thrustMarkController.FadeOut();
                }
                Thrust(GameManager.GetGameControlSystem().AxisController.Axis, currentSpeed);
                break;
            case PlayerState.Moving:
                Move(GameManager.GetGameControlSystem().AxisController.Axis, currentSpeed);
                ThrustAmount += thrustBarFillUpRate * Time.fixedDeltaTime;
                thrustMarkController.FadeOut();
                currentSpeed = moveSpeed;
                break;
        }
    }
    private void Thrust(Vector2 direction, float speed)
    {
        if (body.angularVelocity != 0)
            body.angularVelocity = 0;
        if (direction != VectorUtility.null_vector2)
        {
            body.MoveRotation(VectorUtility.GetSignedAngle(transform.up, direction) * Time.deltaTime * turnSpeed + body.rotation);
            Move(direction, speed);
        }
        else
        {
            Move(transform.up, moveSpeed);
        }
    }
    private void Move(Vector2 direction, float speed)
    {
        if (direction != VectorUtility.null_vector2 && body.velocity != VectorUtility.null_vector2)
            body.velocity = VectorUtility.null_vector2;
        body.velocity = speed * direction;
    }
}

