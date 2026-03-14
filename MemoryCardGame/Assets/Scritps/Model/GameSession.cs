namespace MemoryCardGame
{
    public static class GameSession
    {
        public static GameConfig CurrentConfig { get; set; }
        public static GameResult? LastResult { get; set; }

        // Index of the level currently being played (matches the index in StartMenu.layoutOptions)
        public static int CurrentLevelIndex { get; set; }

        // Highest unlocked level index for this run (0-based). Start with first level unlocked.
        public static int MaxLevelUnlocked { get; set; } = 0;

        public static bool HasConfig => CurrentConfig.Rows > 0 && CurrentConfig.Columns > 0;
    }
}
