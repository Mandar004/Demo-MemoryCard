namespace MemoryCardGame
{
    public struct GameResult
    {
        public int Score { get; }
        public int Turns { get; }
        public int TotalPairs { get; }

        public GameResult(int score, int turns, int totalPairs)
        {
            Score = score;
            Turns = turns;
            TotalPairs = totalPairs;
        }
    }
}
