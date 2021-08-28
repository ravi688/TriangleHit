using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MenuManager : IManualUpdatable
{
    public bool IsRunning { get { return true; } }
    private Action on_load;
    public List<Menu> Menus
#if !RELEASE_MODE
    {
        get
        {
            return __menus;
        }
        set { __menus = value; }
    }
    private List<Menu> __menus;
#else
        ;
#endif

    public MenuManager()
    {
        Menus = new List<Menu>();
        SceneManager.sceneUnloaded += (scene) =>
            {
                Menus.Clear();
            };
        SceneManager.activeSceneChanged += (scene, scene2) =>
        {
            if (on_load != null) is_new_scene_active = true;
        };
    }
    private bool is_new_scene_active = false;
    //Must be called in the first call of the Update callback method
    public void Update()
    {
        if (!is_new_scene_active)
            return;
        on_load();
        on_load = null;
        is_new_scene_active = false;
    }
    public void ActivateMenuOnLoad(string str)
    {
        on_load = () => GetMenuByName(str).Activate();
    }
    public Menu GetMenuByName(string str)
    {
        int count = Menus.Count;
        Menu menu = null;
        for (int i = 0; i < count; i++)
            if (Menus[i].name == str)
            {
                menu = Menus[i];
                break;
            }
#if DEBUG_MODE
        if (menu == null)
            GameManager.GetLogManager().LogWarning(string.Format("Menu with name {0} is not found", str));
#endif
        return menu;
    }
}
