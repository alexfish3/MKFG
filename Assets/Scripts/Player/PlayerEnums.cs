///
/// Created by Alex Fischer | May 2024
/// This is where I create enums to be used across the project
/// 

public enum InputType
{
    DLLKeyboard,
    UnityKeyboard,
    UnityController
}

// The control profile of the players, are they controlling a ui, driving the player, or nothing
public enum ControlProfile
{
    None = 0,
    UI = 1,
    Driving = 2,
}

// Used to keep track of where the game is
public enum GameStates
{
    Default,
    MainMenu,
    GameModeSelect,
    Options,
    CharacterSelect,
    MapSelect,
    RuleSelect,
    Pause,
    Results,

    Loading,
    LoadMatch,
    MainLoop,
    LoadMainMenu,
}

// Used to identify types of menus in code
public enum UITypes
{
    Default,
    MainMenu,
    GameModeSelect,
    Options,
    CharacterSelect,
    MapSelect,
    RuleSelect,
    Pause,
    Results,
}

// Used for menus to identify input directions
public enum Direction
{
    Left,
    Right,
    Up,
    Down
}

// Used for when players pause the game locally, determine if the player is a host pause or sub pause
public enum PauseType
{
    Host,
    Sub
}

// Used for the character select menu, allowing players to scroll down to their own nametag and swap settings
public enum CharacterSelectionPosition
{
    Characters,
    Teams,
    PlayerTagMoveset
}

// Used for defining different types of maps, allowing for straight maps to bypass the lap counter set in ruleset
public enum MapType
{
    Loop,
    Straight
};

