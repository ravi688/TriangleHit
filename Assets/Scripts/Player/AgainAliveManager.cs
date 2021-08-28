using UnityEngine;
using UnityEngine.UI;

public class AgainAliveManager : IManualUpdatable
{
    public bool IsRunning { get { return isAgainSpawnTimerSet || timerTextFadeController.IsRunning;  } }
    public Text TimerText;
    public GameObject PlayerPrefab;
    public float AgainInstantiateTime = 4;

   
    private bool isAgainSpawnTimerSet;
    private Timer timer;
    private FadeController timerTextFadeController;
    private GameObject current_object;
    private bool is_loaded = false;
    private GameObject count_text_prefab;
    public void UnloadCounterText()
    {
        if (!is_loaded)
        {
#if DEBUG_MODE
            GameManager.GetLogManager().LogWarning("CountText is not loaded, but you are still trying to unload it");
#endif
            return;
        }
        GameManager.UnloadResource<GameObject>(count_text_prefab);
        count_text_prefab = null;
        is_loaded = false;
    }
    public void LoadCounterText()
    {
        if (is_loaded)
        {
#if DEBUG_MODE
            GameManager.GetLogManager().LogWarning("CountText is already loaded, but you are still trying to load it");
#endif
            return;
        }
        count_text_prefab = GameManager.LoadResource<GameObject>(GameConstants.ResourceFilePaths.COUNT_TIMER_TEXT);
        is_loaded = true;
    }

    public void Initialize()
    {
        Transform count_timer_text_transform = Object.Instantiate(count_text_prefab).transform;
        count_timer_text_transform.SetParent(GameManager.GetCanvas().transform);
        count_timer_text_transform.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        TimerText = count_timer_text_transform.GetComponent<Text>();

        AlphaAdapter adapter = new UITextAlphaAdapter(TimerText);
        adapter.SetAlpha(0);
        timerTextFadeController = new FadeController(adapter);
        timerTextFadeController.OnFadeOut = delegate { TimerText.gameObject.SetActive(false); };
        timer = new Timer(0, AgainInstantiateTime, 1.0f);

        timer.AddListner(ShowTimerText, OnTimer.Start);
        timer.AddListner(UpdateTimerText, OnTimer.Update);
        timer.AddListner(Respawn, OnTimer.End);

        isAgainSpawnTimerSet = false;
        if (TimerText.gameObject.activeSelf)
            TimerText.gameObject.SetActive(false);
    }
    public void Update()
    {
        GameLoop.Update(timer);
        GameLoop.Update(timerTextFadeController);
    }
    public void BeginRespawn()
    {
        timer.Start();
        isAgainSpawnTimerSet = true;
    }
    private void ShowTimerText()
    {
        TimerText.gameObject.SetActive(true);
        TimerText.text = "";
        timerTextFadeController.FadeIn();
    }
    private void UpdateTimerText()
    {
        TimerText.text = ((int)timer.time).ToString();
    }
    public void Spawn(Vector3 position)
    {
        GameObject obj = MonoBehaviour.Instantiate(PlayerPrefab);
        current_object = obj;
        obj.transform.position = position;
    }
    public void SetPrefab(GameObject prefab)
    {
        PlayerPrefab = prefab;
    }
    public GameObject GetCurrent() { return current_object;  }
    private void Respawn()
    {
        timerTextFadeController.FadeOut();
        GameObject obj = MonoBehaviour.Instantiate(PlayerPrefab); 
        obj.transform.position = PlayerCore.KilledPosition;
        current_object = obj;
        isAgainSpawnTimerSet = false;
    }
}
