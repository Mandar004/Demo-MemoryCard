namespace MemoryCardGame
{
    public struct GameConfig
    {
        public int Rows { get; }
        public int Columns { get; }

        public GameConfig(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
        }
    }
}
