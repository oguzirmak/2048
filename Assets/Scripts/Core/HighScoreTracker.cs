using System;

namespace Game2048.Core
{
    public sealed class HighScoreTracker
    {
        public HighScoreTracker(int storedHighScore = 0)
        {
            HighScore = Math.Max(0, storedHighScore);
        }

        public int HighScore { get; private set; }

        public bool TryUpdate(int score)
        {
            if (score < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(score), score, "Score cannot be negative.");
            }

            if (score <= HighScore)
            {
                return false;
            }

            HighScore = score;
            return true;
        }
    }
}
