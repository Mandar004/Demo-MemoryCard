namespace MemoryCardGame
{
    public static class GameSession
    {
        public static GameConfig CurrentConfig { get; set; }
        public static GameResult? LastResult { get; set; }

        public static bool HasConfig => CurrentConfig.Rows > 0 && CurrentConfig.Columns > 0;
    }
}
