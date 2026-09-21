namespace Game2048.Core
{
    public readonly struct TileMovement
    {
        public TileMovement(
            int sourceRow,
            int sourceColumn,
            int targetRow,
            int targetColumn,
            int startValue,
            bool merged,
            int resultValue)
        {
            SourceRow = sourceRow;
            SourceColumn = sourceColumn;
            TargetRow = targetRow;
            TargetColumn = targetColumn;
            StartValue = startValue;
            Merged = merged;
            ResultValue = resultValue;
        }

        public int SourceRow { get; }
        public int SourceColumn { get; }
        public int TargetRow { get; }
        public int TargetColumn { get; }
        public int StartValue { get; }
        public bool Merged { get; }
        public int ResultValue { get; }
    }
}
