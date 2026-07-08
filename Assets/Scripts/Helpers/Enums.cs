namespace Magicat.Helpers
{
    // Tile sets representing each of the various levels
    public enum TileSets : byte
    {
        Forest = 0,
    }

    public enum DoorState : byte
    {
        Closed = 0,
        Open = 1
    }

    public enum Directions : byte
    {
        North = 0,
        East = 1,
        South = 2,
        West = 3
    }

    // Data types specific to the player character
    public enum PlayerDataTypes : byte
    {
        // Stub
    }

    /// <summary>
    /// Data types shared by any enemies
    /// </summary>
    public enum BaseEnemyDataTypes : byte
    {
        // Stub
    }

    public enum ActionMaps : byte
    {
        Gameplay = 0,
        Menu = 1
    }

    /// <summary>
    /// Descriptive state for how an enemy AI should be acting/reacting. Different enemies will implement
    /// these states to handle different actions but the general rules persists for all.
    /// </summary>
    public enum EnemyStates : byte
    {
        // Stub
    }

    public enum BuffTypes : byte
    {
        Base = 0,
        Timer = 1,
        Conditional = 2
    }
}