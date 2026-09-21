using Game2048.Core;
using NUnit.Framework;

namespace Game2048.Tests.EditMode
{
    public sealed class HighScoreTrackerTests
    {
        [Test]
        public void Constructor_RestoresStoredHighScore()
        {
            var tracker = new HighScoreTracker(512);

            Assert.That(tracker.HighScore, Is.EqualTo(512));
        }

        [TestCase(0)]
        [TestCase(999)]
        [TestCase(1000)]
        public void TryUpdate_DoesNotReplaceEqualOrLowerHighScore(int score)
        {
            var tracker = new HighScoreTracker(1000);

            bool changed = tracker.TryUpdate(score);

            Assert.That(changed, Is.False);
            Assert.That(tracker.HighScore, Is.EqualTo(1000));
        }

        [Test]
        public void TryUpdate_ReplacesHighScoreWhenScoreIsHigher()
        {
            var tracker = new HighScoreTracker(256);

            bool changed = tracker.TryUpdate(512);

            Assert.That(changed, Is.True);
            Assert.That(tracker.HighScore, Is.EqualTo(512));
        }
    }
}
