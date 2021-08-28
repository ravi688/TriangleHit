#if DISABLE_WARNINGS
#pragma warning disable
#endif


using System.IO;
using UnityEngine;

public static class GameConstants
{
    public static class Tags
    {
        public static string SPAWN_POINTS_CONTAINER { get { return spawn_points_container; } }

        private static string spawn_points_container = "SpawnPointsContainer";
    }
    public static class Config
    {
        public static int AGAIN_INSTANTIATE_TIME { get { return again_instantiate_time; } }
        public static float BLACK_FADE_DURATION { get { return black_fade_duration; } }
        public static float MENU_FADE_DURATION {  get { return menu_fade_duration;  } }

        public static int MEDIPACK_DURATION_BASE_COST = 1;
        public static int HURT_DURATION_BASE_COST = 1;
        public static int SHIELD_DURATION_BASE_COST = 1;
        public static int STAR_DURATION_BASE_COST = 1;
        public static int COIN_DURATION_BASE_COST = 1;

        public static int MEDIPACK_FREQUENCY_BASE_COST = 1;
        public static int HURT_FREQUENCY_BASE_COST = 1;
        public static int SHIELD_FREQUENCY_BASE_COST = 1;
        public static int STAR_FREQUENCY_BASE_COST = 1;
        public static int COIN_FREQUENCY_BASE_COST = 1;

        public static int MEDIPACK_HEALTH_CREDIT_BASE_COST = 1;
        public static int SHIELD_EFFECT_DURATION_BASE_COST = 1;
        public static int HURT_LIVE_CREDIT_BASE_COST = 1;
        public static int STAR_CREDIT_BASE_COST = 1;
        public static int COIN_CREDIT_BASE_COST = 1;


        private static float menu_fade_duration = 0.4f;
        private static int again_instantiate_time = 4;
        private static float black_fade_duration = 0.3f;
    }
    public static class DefaultPurchasedIDs
    {
        public static string TRIANGLE_PLAYER { get { return triangle_player; } }
        public static string LEVEL_1 { get { return level_1; } }

        private static string triangle_player = "purchase-10-player-triangle";
        private static string level_1 = "purchase-10-level-Level 1-0";
    }
    public static class PlayerNames
    {
        public static string TRIANGLE { get { return triangle; } }
        public static string SQUARE { get { return square; } }
        public static string PENTAGON { get { return pentagon; } }
        public static string HEXAGON { get { return hexagon; } }

        private static string triangle = "Triangle";
        private static string square = "Square";
        private static string pentagon = "Pentagon";
        private static string hexagon = "Hexagon";
    }
    public static class PowerUpNames
    {
        public static string SHIELD { get { return shield; } }
        public static string COIN { get { return coin; } }
        public static string STAR { get { return star; } }
        public static string MEDIPACK { get { return medipack; } }
        public static string HURT { get { return hurt; } }

        private static string medipack = "Medipack";
        private static string star = "Star";
        private static string coin = "Coin";
        private static string shield = "Shield";
        private static string hurt = "Hurt";
    }
    public static class UpgradeNames
    {
        public static string POWERUP_DURATION { get { return powerup_duration; } }
        public static string POWERUP_FREQUENCY { get { return powerup_frequency; } }
        public static string POWERUP_HEALTH_CREDIT { get { return powerup_health_credit; } }
        public static string POWERUP_SHIELD_DURATION { get { return powerup_shield_duration; } }
        public static string POWERUP_LIVE_CREDIT { get { return powerup_live_credit; } }
        public static string POWERUP_STAR_CREDIT { get { return powerup_star_credit; } }
        public static string POWERUP_COIN_CREDIT { get { return powerup_coin_credit; } }

        private static string powerup_duration = "Duration";
        private static string powerup_frequency = "Frequency";
        private static string powerup_health_credit = "Health Credit";
        private static string powerup_shield_duration = "Shield Duration";
        private static string powerup_live_credit = "Live Credit";
        private static string powerup_star_credit = "Star Credit";
        private static string powerup_coin_credit = "Coin Credit";
    }
    public static class MenuNames
    {
        public static string STATS_MENU { get { return stats_menu; } }
        public static string START_MENU { get { return start_menu; } }
        public static string MORE_MENU { get { return more_menu; } }
        public static string SETTINGS_MENU { get { return settings_menu; } }
        public static string PAUSE_MENU { get { return pause_menu; } }
        public static string EXIT_MENU { get { return exit_menu; } }
        public static string WATCH_ADS_MENU { get { return watch_ads_menu; } }
        public static string JOSTICK_CONFIG_MENU { get { return joystick_config_menu; } }
        public static string LEVELS_MENU { get { return levels_menu; } }
        public static string PLAYER_SELECTION_MENU { get { return player_selection_menu; } }
        public static string UPGRADES_MENU {  get { return upgrades_menu;  } }

        private static string stats_menu = "stats_menu";
        private static string start_menu = "start_menu";
        private static string more_menu = "more_menu";
        private static string settings_menu = "settings_menu";
        private static string pause_menu = "pause_menu";
        private static string exit_menu = "exit_menu";
        private static string watch_ads_menu = "watch_ads_menu";
        private static string joystick_config_menu = "joystick_config_menu";
        private static string levels_menu = "levels_menu";
        private static string player_selection_menu = "player_selection_menu";
        private static string upgrades_menu = "upgrades_menu";
    }
    public static class ResourceFilePaths
    {
        public static string PREFABS { get { return prefabs; } }
        public static string SPRITES { get { return sprites; } }
        public static string SCRIPTABLE_OBJECTS { get { return scriptable_objects; } }

        public static string LEVELS { get { return Path.Combine(SCRIPTABLE_OBJECTS, levels); } }
        public static string STD_ANIMS { get { return Path.Combine(SCRIPTABLE_OBJECTS, std_anims); } }
        public static string POWER_UP_SETTINGS { get { return Path.Combine(SCRIPTABLE_OBJECTS, power_up_settings); } }
        public static string UPGRADES_SETTINGS {  get { return Path.Combine(SCRIPTABLE_OBJECTS, upgrades_settings);  } }
        public static string PLAYERS_SETTINGS { get { return Path.Combine(SCRIPTABLE_OBJECTS, players_settings); } }

        public static string PLAYERS { get { return Path.Combine(PREFABS, players); } }
        public static string HUD_CANVAS { get { return Path.Combine(PREFABS, hud_canvas); } }
        public static string MENUS { get { return Path.Combine(PREFABS, menus); } }
        public static string PLAYERS_NATIVE_SPRITES { get { return Path.Combine(SPRITES, players_native_sprites); } }
        public static string PLAYERS_HALF_SPRITES { get { return Path.Combine(SPRITES, players_half_sprites); } }

        public static string TRIANGLE_PLAYER_SETTINGS { get { return Path.Combine(PLAYERS_SETTINGS, triangle_player_settings); } }
        public static string SQUARE_PLAYER_SETTINGS { get { return Path.Combine(PLAYERS_SETTINGS, square_player_settings); } }
        public static string PENTAGON_PLAYER_SETTINGS { get { return Path.Combine(PLAYERS_SETTINGS, pentagon_player_settings); } }
        public static string HEXAGON_PLAYER_SETTINGS { get { return Path.Combine(PLAYERS_SETTINGS, hexagon_player_settings); } }


        public static string CAMERA { get { return Path.Combine(PREFABS, camera); } }
        public static string LEVEL_CARD { get { return Path.Combine(PREFABS, level_card); } }

        public static string GAME_OVER_TEXT_ANIM { get { return Path.Combine(STD_ANIMS, game_over_text_anim); } }
        public static string SCORE_TEXT_ANIM { get { return Path.Combine(STD_ANIMS, score_text_anim); } }

        public static string COIN_POWER_UP_SETTINGS { get { return Path.Combine(POWER_UP_SETTINGS, coin_power_up_settings); } }
        public static string LIVE_POWER_UP_SETTINGS { get { return Path.Combine(POWER_UP_SETTINGS, live_power_up_settings); } }
        public static string MEDIPACK_POWER_UP_SETTINGS { get { return Path.Combine(POWER_UP_SETTINGS, medipack_power_up_settings); } }
        public static string SHIELD_POWER_UP_SETTINGS { get { return Path.Combine(POWER_UP_SETTINGS, shield_power_up_settings); } }
        public static string STAR_POWER_UP_SETTINGS { get { return Path.Combine(POWER_UP_SETTINGS, star_power_up_settings); } }

        public static string COIN_UPGRADES_SETTINGS { get { return Path.Combine(UPGRADES_SETTINGS, coin_upgrades_settings); } }
        public static string LIVE_UPGRADES_SETTINGS { get { return Path.Combine(UPGRADES_SETTINGS, live_upgrades_settings); } }
        public static string MEDIPACK_UPGRADES_SETTINGS { get { return Path.Combine(UPGRADES_SETTINGS, medipack_upgrades_settings); } }
        public static string SHIELD_UPGRADES_SETTINGS { get { return Path.Combine(UPGRADES_SETTINGS, shield_upgrades_settings); } }
        public static string STAR_UPGRADES_SETTINGS { get { return Path.Combine(UPGRADES_SETTINGS, star_upgrades_settings); } }


        public static string JOYSTICK_SETTINGS { get { return Path.Combine(SCRIPTABLE_OBJECTS, joystick_settings); } }
        public static string ROPE_SETTINGS { get { return Path.Combine(SCRIPTABLE_OBJECTS, rope_settings); } }

        public static string ENEMY_DISPOSE_METER_SPRITE { get { return Path.Combine(SPRITES, enemy_dispose_meter_sprite); } }
        public static string LIVE_METER_SPRITE { get { return Path.Combine(SPRITES, live_meter_sprite); } }
        public static string POINT_SPRITE { get { return Path.Combine(SPRITES, point_sprite); } }

        public static string PAUSE_MENU { get { return Path.Combine(MENUS, pause_menu); } }
        public static string EXIT_MENU { get { return Path.Combine(MENUS, exit_menu); } }
        public static string DASH_BAR { get { return Path.Combine(HUD_CANVAS, dash_bar); } }
        public static string HEALTH_BAR { get { return Path.Combine(HUD_CANVAS, health_bar); } }
        public static string BLACK_IMAGE { get { return Path.Combine(HUD_CANVAS, black_image); } }
        public static string COUNT_TIMER_TEXT { get { return Path.Combine(HUD_CANVAS, count_timer_text); } }
        public static string SCORE_TEXT { get { return Path.Combine(HUD_CANVAS, score_text); } }
        public static string YOU_LOOSE { get { return Path.Combine(HUD_CANVAS, you_loose); } }
        public static string YOU_WIN { get { return Path.Combine(HUD_CANVAS, you_win); } }


        public static string TRIANGLE_PLAYER { get { return Path.Combine(PLAYERS, triangle_player); } }
        public static string SQUARE_PLAYER { get { return Path.Combine(PLAYERS, square_player); } }
        public static string PENTAGON_PLAYER { get { return Path.Combine(PLAYERS, pentagon_player); } }
        public static string HEXAGON_PLAYER { get { return Path.Combine(PLAYERS, hexagon_player); } }

        //Dirs
        //Resources/Prefabs
        private static string prefabs = "Prefabs";
        //Resources/ScriptableObjects
        private static string scriptable_objects = "ScriptableObjects";
        //Resources/Sprites
        private static string sprites = "Sprites";

        //Resources/Sprites/Players/Native
        private static string players_native_sprites = "Players/Native";
        //Resources/Sprites/Players/Half
        private static string players_half_sprites = "Players/Half";


        //Resources/Prefabs/HUDCanvas
        private static string hud_canvas = "HUDCanvas";
        //Resources/Prefabs/Players
        private static string players = "Players";
        //Resources/Prefabs/Menus
        private static string menus = "Menus";

        //Resources/ScriptableObjects/Levels
        private static string levels = "Levels";
        //Resources/ScriptableObjects/PowerUpSettings
        private static string power_up_settings = "PowerUpSettings";
        //Resources/ScriptableObjects/UpgradesSettings
        private static string upgrades_settings = "UpgradesSettings";

        //Resources/ScriptableObjects/StandardAnimations
        private static string std_anims = "StandardAnimations";
        //Resources/ScriptableObjects/PlayerSettings
        private static string players_settings = "PlayersSettings";


        //Files

        //Resources/Prefabs/HUDcanvas/*
        private static string dash_bar = "dash_bar";
        private static string health_bar = "health_bar";
        private static string black_image = "black_image";
        private static string count_timer_text = "count_timer_text";
        private static string score_text = "score_text";
        private static string you_loose = "you_loose";
        private static string you_win = "you_win";


        //Resources/Prefabs/*
        private static string camera = "camera";
        private static string level_card = "level_card";

        //Resources/Prefabs/Players/*
        private static string triangle_player = PlayerNames.TRIANGLE;
        private static string square_player = PlayerNames.SQUARE;
        private static string pentagon_player = PlayerNames.PENTAGON;
        private static string hexagon_player = PlayerNames.HEXAGON;

        //Resources/Prefabs/Menus/*
        private static string pause_menu = "pause_menu";
        private static string exit_menu = "exit_menu";

        //Resources/Sprites/*
        private static string enemy_dispose_meter_sprite = "enemy_dispose_meter_sprite";
        private static string live_meter_sprite = "live_meter_sprite";
        private static string point_sprite = "point_sprite";

        //Resources/ScriptableObjects/*
        private static string joystick_settings = "joystick_settings";
        private static string rope_settings = "rope_settings";

        //Resources/ScriptableObjects/StandardAnimations/*
        private static string game_over_text_anim = "game_over_text_anim";
        private static string score_text_anim = "score_text_anim";

        //Resources/ScriptableObjects/PowerUpSettings/*
        private static string coin_power_up_settings = "coin_power_up_settings";
        private static string live_power_up_settings = "live_power_up_settings";
        private static string medipack_power_up_settings = "medipack_power_up_settings";
        private static string shield_power_up_settings = "shield_power_up_settings";
        private static string star_power_up_settings = "star_power_up_settings";

        //Resources/ScriptableObjects/PowerUpSettings/*
        private static string coin_upgrades_settings = "CoinUpgrades";
        private static string live_upgrades_settings = "HurtUpgrades";
        private static string medipack_upgrades_settings = "MedipackUpgrades";
        private static string shield_upgrades_settings = "ShieldUpgrades";
        private static string star_upgrades_settings = "StarUpgrades";

        //Resources/ScriptableObjects/PlayerSettings/*
        private static string triangle_player_settings = PlayerNames.TRIANGLE;
        private static string square_player_settings = PlayerNames.SQUARE;
        private static string pentagon_player_settings = PlayerNames.PENTAGON;
        private static string hexagon_player_settings = PlayerNames.HEXAGON;
    }
    public static class DiskFilePaths
    {
        public static string PURCHASE_MANAGER { get { return Path.Combine(Application.persistentDataPath, purchase_manager); } }
        public static string ECONOMY_MANAGER { get { return Path.Combine(Application.persistentDataPath, economy_manager); } }
        public static string PLAYER_MANAGER { get { return Path.Combine(Application.persistentDataPath, player_manager); } }
        public static string UPGRADE_MANAGER { get { return Path.Combine(Application.persistentDataPath, upgrade_manager); } }


        private static string purchase_manager = "bin.purchase.data";
        private static string economy_manager = "bin.economy.data";
        private static string player_manager = "bin.player.data";
        private static string upgrade_manager = "bin.upgrade.data";
    }
    public static class LevelBuildIndices
    {
        public static int START_SCREEN { get { return start_screen; } }
        public static int STATS_SCREEN { get { return stats_screen; } }
        public static int LEVEL0 { get { return level0; } }
        public static int LEVEL1 { get { return level1; } }

        private static int start_screen = 0;
        private static int stats_screen = 1;
        private static int level0 = 2;
        private static int level1 = 3;
    }
    public static class TouchIDs
    {
        public static int JOYSTICK { get { return joystick; } }
        public static int ENEMY { get { return enemy; } }
        public static int DUSTBIN { get { return dustbin; } }
        public static int THRUST { get { return thrust; } }

        private static int joystick = 3;
        private static int enemy = 1;
        private static int dustbin = 2;
        private static int thrust = 0;
    }
}
