
using UnityEngine; 


public class GameSettings 
{
    public Vector2 JoyStickPosition;
    public float SfxSound;
    public float MusicSound;


    public static GameSettings GetDefaults()
    {
        GameSettings settings = new GameSettings();
        settings.JoyStickPosition = new Vector2(400, -200);
        settings.SfxSound = 0.6f;
        settings.MusicSound = 0.7f;
        return settings; 
    }
}
