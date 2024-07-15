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


// Used to keep track of where the game is
public enum GameStates
{
    MainMenu,
    GameModeSelect,
    Options,
    PlayerSelect,
    MapSelect,
    RuleSelect,
    Loading,

    Begin,
    MainLoop,

    Paused,
    Default
}

public enum ControlProfile
{
    None = 0,
    UI = 1,
    Driving = 2,
    Custom = 3,
}

// Used to identify types of menus in code
public enum UITypes
{
    MainMenu,
    GameModeSelect,
    Settings,
    CharacterSelect,
    MapSelect,
    PauseMenu,
    RuleSelect
}

// Used for menus to identify input directions
public enum Direction
{
    Left,
    Right,
    Up,
    Down
}

// Used for the character select menu, allowing players to scroll down to their own nametag and swap settings
public enum CharacterSelectionPosition
{
    Characters,
    Teams,
    PlayerTagMoveset
}
