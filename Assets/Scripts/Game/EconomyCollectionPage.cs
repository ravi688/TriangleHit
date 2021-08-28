#if DISABLE_WARNINGS
#pragma warning disable
#endif

using UnityEngine;
using UnityEngine.UI;

public class EconomyCollectionPage : TVScreenPage
{
    [SerializeField]
    private Text economy_count_text;
    [SerializeField]
    private StandardAnimation text_animation;
    [SerializeField]
    private GameObject coin_prefab;
    [SerializeField]
    private float economy_spawn_duration = 1.0f;
    [SerializeField]
    private float next_page_show_trigger_time = 0.5f;

    private Timer timer;
    private Timer next_page_show_timer;
    private StandardAnimationPlayer text_anim_player;
    private float anim_duration;
    private int economy_collected;
    private Vector3 economy_initial_local_scale;

    protected override void Awake()
    {
        base.Awake();
        text_anim_player = new StandardAnimationPlayer();
        text_anim_player.InitializeWith(null, economy_count_text.transform, text_animation);

        anim_duration = 0.9f;
        next_page_show_timer = new Timer(0, next_page_show_trigger_time, 0.1f);
        next_page_show_timer.AddListner(() =>
        {
            Deactivate();
            base.next_page.Activate();
        },
        OnTimer.End);
        economy_initial_local_scale = economy_count_text.transform.localScale;
        economy_count_text.text = "0";
    }
    protected override void Update()
    {
        base.Update();
        if (timer != null)
            GameLoop.Update(timer);
        GameLoop.Update(next_page_show_timer);
        GameLoop.Update(text_anim_player);
    }
    protected override void OnActive()
    {
        base.OnActive();
        CollectionEconomy(10);
    }
    public void IncrementEconomy()
    {
        economy_collected++;
        economy_count_text.text = economy_collected.ToString();
        text_anim_player.PlayAtPosition(Vector3.zero, economy_initial_local_scale, Quaternion.identity);
    }
    public void CollectionEconomy(int num_coins)
    {
        float each_coin_spawn_time = economy_spawn_duration / num_coins;
        timer = null;
        timer = new Timer(0, economy_spawn_duration, each_coin_spawn_time);
        economy_collected = 0;

        timer.AddListner(() =>
        {
            GameObject coin = Instantiate(coin_prefab);
            coin.GetComponent<Economy>().economyCollectionPage = this;
            coin.transform.SetParent(transform.GetChild(0));
            coin.transform.SetSiblingIndex(1);
            /*TODO: 
             * Replace Destroy(_coin, anim_duration) with
             * IncrementEconomy();
             */
            Destroy(coin, anim_duration);
        }, OnTimer.Update);
        timer.AddListner(next_page_show_timer.Start, OnTimer.End);
        timer.Start();
    }
}
