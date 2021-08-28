using UnityEngine;
using System.Collections.Generic;

public class PowerUpManager : IManualUpdatable
{
    public bool IsRunning { get; private set; }
    public MedipackPowerUpSettings MedipackSettings
#if !RELEASE_MODE
    { get; set; }
#else 
        ;
#endif
    public ShieldPowerUpSettings ShieldSettings
#if !RELEASE_MODE
    { get; set; }
#else
        ;
#endif
    public StarPowerUpSettings StarSettings
#if !RELEASE_MODE
    { get; set; }
#else
        ;
#endif
    public LivePowerUpSettings LiveSettings
#if !RELEASE_MODE
    { get; set; }
#else
        ;
#endif
    public CoinPowerUpSettings CoinSettings
#if !RELEASE_MODE
    { get; set; }
#else
        ;
#endif
    public Vector3[] Points
#if !RELEASE_MODE
    { get; set; }
#else
        ;
#endif


    private bool[] reserved_bitstring;
    private List<PowerUpJob> automatic_jobs;
    private List<PowerUpJob> manual_jobs;
    private bool is_loaded = false;

    private PowerUpManualJob coin_job;
    private PowerUpManualJob star_job;
    private int automatic_job_count;
    private int manual_job_count;

    public void UnloadPowerUpSettings()
    {

        if (!is_loaded)
        {
#if DEBUG_MODE
            GameManager.GetLogManager().LogWarning("PowerUpSettings is not loaded but you are trying to unload it");
#endif
            return;
        }

        GameManager.UnloadResource<MedipackPowerUpSettings>(MedipackSettings);
        GameManager.UnloadResource<ShieldPowerUpSettings>(ShieldSettings);
        GameManager.UnloadResource<LivePowerUpSettings>(LiveSettings);
        GameManager.UnloadResource<StarPowerUpSettings>(StarSettings);
        GameManager.UnloadResource<CoinPowerUpSettings>(CoinSettings);

        MedipackSettings = null;//zero the reference count
        ShieldSettings = null;//zero the reference count
        LiveSettings = null;//zero the reference count
        StarSettings = null;//zero the reference count
        CoinSettings = null;//zero the reference count
        is_loaded = false;
    }
    public void LoadAllPowerUpSettings()
    {

        if (is_loaded)
        {
#if DEBUG_MODE
            GameManager.GetLogManager().LogWarning("PowerUpSettings is already loaded but you are still trying to load it");
#endif
            return;
        }

        MedipackSettings = GameManager.LoadResource<MedipackPowerUpSettings>(GameConstants.ResourceFilePaths.MEDIPACK_POWER_UP_SETTINGS);
        ShieldSettings = GameManager.LoadResource<ShieldPowerUpSettings>(GameConstants.ResourceFilePaths.SHIELD_POWER_UP_SETTINGS);
        LiveSettings = GameManager.LoadResource<LivePowerUpSettings>(GameConstants.ResourceFilePaths.LIVE_POWER_UP_SETTINGS);
        StarSettings = GameManager.LoadResource<StarPowerUpSettings>(GameConstants.ResourceFilePaths.STAR_POWER_UP_SETTINGS);
        CoinSettings = GameManager.LoadResource<CoinPowerUpSettings>(GameConstants.ResourceFilePaths.COIN_POWER_UP_SETTINGS);
        is_loaded = true;
    }

    //Must be called in the Awake method, after player had called OnAlive in the Start method
    public void Initialize()
    {

        if (!is_loaded)
        {
#if DEBUG_MODE
            GameManager.GetLogManager().LogError("PowerUpSettings Data is not loaded, first load it before calling Initialize method");
#endif
            return;
        }

        automatic_jobs = new List<PowerUpJob>();
        automatic_jobs.Add(new PowerUpAutomaticJob<int>(() => new Medipack(MedipackSettings), SpawnPositionAllocator, SpawnPositionDeallocator, MedipackSettings.time_interval));
        automatic_jobs.Add(new PowerUpAutomaticJob<int>(() => new Live(LiveSettings), SpawnPositionAllocator, SpawnPositionDeallocator, LiveSettings.time_interval));
        automatic_jobs.Add(new PowerUpAutomaticJob<int>(() => new Shield(ShieldSettings), SpawnPositionAllocator, SpawnPositionDeallocator, ShieldSettings.time_interval));
        manual_jobs = new List<PowerUpJob>();
        manual_jobs.Add(star_job = new PowerUpManualJob(() => new Star(StarSettings)));

        reserved_bitstring = new bool[Points.Length];
        automatic_job_count = automatic_jobs.Count;
        manual_job_count = manual_jobs.Count;
    }
    public void Start()
    {
        IsRunning = true;
        StartExecutionOfAutomaticJobs();
    }
    public void Stop()
    {
        IsRunning = false;
    }
    public void Update()
    {
        UpdateAutomaticJobs();
        UpdateManualJobs();
#if DEBUG_MODE
        if (Input.GetKeyDown(KeyCode.O))
        {
            Vector2 pos = GameManager.GetCamera().ScreenToWorldPoint(Input.mousePosition);
            star_job.SetSpawnPosition(pos);
            star_job.Execute();
        }
#endif
    }
    private void UpdateManualJobs()
    {
        for (int i = 0; i < manual_job_count; i++)
            GameLoop.Update(manual_jobs[i]);
    }
    private void UpdateAutomaticJobs()
    {
        for (int i = 0; i < automatic_job_count; i++)
            GameLoop.Update(automatic_jobs[i]);
    }
    private void StartExecutionOfAutomaticJobs()
    {
        for (int i = 0; i < automatic_job_count; i++)
            automatic_jobs[i].Execute();
    }
    private void SpawnPositionDeallocator(int reserved_index)
    {
        Random.InitState(reserved_index);
        reserved_bitstring[reserved_index] = false;     //mark this as unreserved slot, i.e. free for next allocation
    }
    private bool SpawnPositionAllocator(out Vector3 position, out int reserved_index)
    {
        int max_index = reserved_bitstring.Length - 1;
        int available_index = Random.Range(0, max_index);
        bool is_found = false;
        while ((available_index <= max_index) && !(is_found = !reserved_bitstring[available_index]))
            available_index++;
        if (available_index > max_index) available_index = max_index;
        reserved_bitstring[available_index] = is_found;     //mark this as reserved slot
        position = Points[available_index];
        reserved_index = available_index;
        return is_found;
    }
}