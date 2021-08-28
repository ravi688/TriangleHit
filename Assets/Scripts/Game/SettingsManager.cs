#if DISABLE_WARNINGS
#pragma warning disable
#endif


using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;
    public static bool IsSettingsModified { get { return PlayerPrefs.HasKey("GameSettingsModified"); } }
    public static GameSettings CurrentSettings
    {
        get
        {
            GameSettings settings;
            if (IsSettingsModified)
                LoadGameSettings(out settings);
            else
                settings = GameSettings.GetDefaults();
            return settings;
        }
    }
    public bool IsConfiguringJoystick
    {
        set
        {
            if (value == true)
            {
                LoadCurrentJoyStickSettingsToUI();
            }
            else
            {
                SaveJoystickSettings();
            }
        }
    }
    [SerializeField]
    private Slider SfxSlider;
    [SerializeField]
    private Slider MusicSlider;
    [SerializeField]
    private GameObject JoyStickDemo;

    private bool IsRecordingCurrentSettings;
    private GameSettings RecordedSettings;
    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
            PlayerPrefs.DeleteAll();
    }

    public void MakeDafaultSettings()
    {
        PlayerPrefs.DeleteKey("GameSettingsModified");
        LoadCurrentSetttingsToUI();
        SaveGameSettings(CurrentSettings);
    }
    //Must be called when the apply button of the Joystick configur menu is pressed
    public void SaveJoystickSettings()
    {
        RecordedSettings.JoyStickPosition = JoyStickDemo.transform.localPosition;
    }
    public void EnterSettingsMode()
    {
        if (IsRecordingCurrentSettings) return;
        LoadCurrentSetttingsToUI();
        IsRecordingCurrentSettings = true;
        RecordedSettings = new GameSettings();
    }
    //Ext the Setting Mode By calling the back Button 
    public void ExitSettingsMode()
    {
        IsRecordingCurrentSettings = false;
    }

    //Must be called when the apply button of the Setings menu is pressed
    public void SaveRecordedSettings()
    {
        RecordedSettings.SfxSound = SfxSlider.value;
        RecordedSettings.MusicSound = MusicSlider.value;
        SaveGameSettings(RecordedSettings);
    }
    private static void SaveGameSettings(GameSettings settings)
    {
        if (!IsSettingsModified)
            PlayerPrefs.SetInt("GameSettingsModified", 1);

        PlayerPrefs.SetFloat("JoyStickPosX", settings.JoyStickPosition.x);
        PlayerPrefs.SetFloat("JoyStickPosY", settings.JoyStickPosition.y);
        PlayerPrefs.SetFloat("SfxVolume", settings.SfxSound);
        PlayerPrefs.SetFloat("MusicVolume", settings.MusicSound);
        // PlayerPrefs.Save();
    }

    private void LoadCurrentSetttingsToUI()
    {
        SfxSlider.value = CurrentSettings.SfxSound;
        MusicSlider.value = CurrentSettings.MusicSound;

    }
    private void LoadCurrentJoyStickSettingsToUI()
    {
        JoyStickDemo.transform.localPosition = CurrentSettings.JoyStickPosition;
    }

    private static void LoadGameSettings(out GameSettings out_settings)
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {

            out_settings = new GameSettings();
            out_settings.JoyStickPosition.x = PlayerPrefs.GetFloat("JoyStickPosX");
            out_settings.JoyStickPosition.y = PlayerPrefs.GetFloat("JoyStickPosY");
            out_settings.SfxSound = PlayerPrefs.GetFloat("SfxVolume");
            out_settings.MusicSound = PlayerPrefs.GetFloat("MusicVolume");
        }
        else
            out_settings = GameSettings.GetDefaults();
    }


    private static void ApplyGameSettings(GameSettings settings)
    {
        Debug.Log("Game Settings are applied");
    }
}
