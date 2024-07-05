///
/// Created by Alex Fischer | May 2024
/// This is where I create enums to be used across projects
/// 
public enum ControlProfile
{
    None = 0,
    UI = 1,
    Driving = 2,
    Custom = 3,
}

public enum GameStates
{
    MainMenu,
    GameModeSelect,
    Options,
    PlayerSelect,
    Loading,

    Begin,
    MainLoop,

    Paused,
    Default
}

// Used to identify types of menus in code
public enum UITypes
{
    MainMenu,
    GameModeSelect,
    CharacterSelect,
    PauseMenu,
}

public enum Direction
{
    Left,
    Right,
    Up,
    Down
}

