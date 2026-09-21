using System;
using System.Collections.Generic;

namespace Game2048.Core
{
    public readonly struct MoveResult
    {
        private readonly IReadOnlyList<TileMovement> movements;

        public MoveResult(bool changed, int scoreGained)
            : this(changed, scoreGained, null)
        {
        }

        public MoveResult(bool changed, int scoreGained, IEnumerable<TileMovement> movements)
        {
            Changed = changed;
            ScoreGained = scoreGained;
            this.movements = movements == null
                ? Array.Empty<TileMovement>()
                : new List<TileMovement>(movements).AsReadOnly();
        }

        public bool Changed { get; }

        public int ScoreGained { get; }

        public IReadOnlyList<TileMovement> Movements => movements ?? Array.Empty<TileMovement>();
    }
}
