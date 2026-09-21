using System;
using System.Collections.Generic;
using Game2048.Core;
using NUnit.Framework;

namespace Game2048.Tests.EditMode
{
    public sealed class BoardModelTests
    {
        [Test]
        public void Reset_ClearsEveryCell()
        {
            var model = new BoardModel(new PredictableRandom());
            model.StartNewGame();

            model.Reset();

            for (int row = 0; row < BoardModel.Size; row++)
            {
                for (int column = 0; column < BoardModel.Size; column++)
                {
                    Assert.That(model.GetCell(row, column), Is.EqualTo(0));
                }
            }
        }

        [Test]
        public void StartNewGame_SpawnsTwoTilesInDifferentCells()
        {
            var model = new BoardModel(new PredictableRandom());

            model.StartNewGame();

            List<BoardPosition> occupiedCells = GetOccupiedCells(model);
            Assert.That(occupiedCells, Has.Count.EqualTo(2));
            Assert.That(occupiedCells[0], Is.Not.EqualTo(occupiedCells[1]));
        }

        [Test]
        public void SpawnedTiles_AreAlwaysTwoOrFour()
        {
            var model = new BoardModel(new PredictableRandom());

            for (int spawnCount = 0; spawnCount < BoardModel.Size * BoardModel.Size; spawnCount++)
            {
                Assert.That(model.TrySpawnTile(), Is.True);
            }

            for (int row = 0; row < BoardModel.Size; row++)
            {
                for (int column = 0; column < BoardModel.Size; column++)
                {
                    Assert.That(model.GetCell(row, column), Is.EqualTo(2).Or.EqualTo(4));
                }
            }
        }

        [Test]
        public void TrySpawnTile_DoesNotOverwriteAnOccupiedCell()
        {
            var model = new BoardModel(new PredictableRandom());
            model.TrySpawnTile();
            BoardPosition firstTile = GetOccupiedCells(model)[0];
            int firstValue = model.GetCell(firstTile.Row, firstTile.Column);

            model.TrySpawnTile();

            Assert.That(model.GetCell(firstTile.Row, firstTile.Column), Is.EqualTo(firstValue));
            Assert.That(GetOccupiedCells(model), Has.Count.EqualTo(2));
        }

        [Test]
        public void TrySpawnTile_ReturnsFalseWhenBoardIsFull()
        {
            var model = new BoardModel(new PredictableRandom());

            for (int spawnCount = 0; spawnCount < BoardModel.Size * BoardModel.Size; spawnCount++)
            {
                Assert.That(model.TrySpawnTile(), Is.True);
            }

            Assert.That(model.TrySpawnTile(), Is.False);
        }

        [Test]
        public void GetCell_WithInvalidCoordinates_ThrowsClearException()
        {
            var model = new BoardModel();

            Assert.That(
                () => model.GetCell(-1, 0),
                Throws.TypeOf<ArgumentOutOfRangeException>().With.Property("ParamName").EqualTo("row"));
            Assert.That(
                () => model.GetCell(0, BoardModel.Size),
                Throws.TypeOf<ArgumentOutOfRangeException>().With.Property("ParamName").EqualTo("column"));
        }

        [TestCase(new int[] { 2, 0, 2, 4 }, new int[] { 4, 4, 0, 0 }, 4)]
        [TestCase(new int[] { 2, 2, 2, 2 }, new int[] { 4, 4, 0, 0 }, 8)]
        [TestCase(new int[] { 2, 2, 4, 0 }, new int[] { 4, 4, 0, 0 }, 4)]
        [TestCase(new int[] { 4, 4, 4, 0 }, new int[] { 8, 4, 0, 0 }, 8)]
        [TestCase(new int[] { 4, 4, 8, 8 }, new int[] { 8, 16, 0, 0 }, 24)]
        public void MoveLeft_MergesRowsCorrectly(int[] input, int[] expected, int scoreGained)
        {
            var model = new BoardModel(CreateBoard(input));

            MoveResult result = model.MoveLeft();

            Assert.That(result.Changed, Is.True);
            Assert.That(result.ScoreGained, Is.EqualTo(scoreGained));
            AssertRow(model, 0, expected);
        }

        [Test]
        public void MoveLeft_LeavesUnchangedBoardUntouched()
        {
            var model = new BoardModel(CreateBoard(new[] { 2, 4, 8, 16 }));

            MoveResult result = model.MoveLeft();

            Assert.That(result.Changed, Is.False);
            Assert.That(result.ScoreGained, Is.EqualTo(0));
            AssertRow(model, 0, new[] { 2, 4, 8, 16 });
        }

        [Test]
        public void MoveLeft_ProcessesMultipleRowsAndPreventsDoubleMerge()
        {
            var model = new BoardModel(new[,]
            {
                { 2, 2, 2, 2 },
                { 0, 4, 0, 4 },
                { 8, 8, 16, 16 },
                { 0, 0, 0, 0 }
            });

            MoveResult result = model.MoveLeft();

            Assert.That(result.Changed, Is.True);
            Assert.That(result.ScoreGained, Is.EqualTo(64));
            AssertRow(model, 0, new[] { 4, 4, 0, 0 });
            AssertRow(model, 1, new[] { 8, 0, 0, 0 });
            AssertRow(model, 2, new[] { 16, 32, 0, 0 });
        }

        [TestCase(MoveDirection.Right, new int[] { 2, 0, 2, 4 }, new int[] { 0, 0, 4, 4 })]
        [TestCase(MoveDirection.Left, new int[] { 2, 2, 2, 2 }, new int[] { 4, 4, 0, 0 })]
        [TestCase(MoveDirection.Right, new int[] { 2, 2, 2, 2 }, new int[] { 0, 0, 4, 4 })]
        public void Move_ProcessesHorizontalDirections(MoveDirection direction, int[] input, int[] expected)
        {
            var model = new BoardModel(CreateBoard(input));
            MoveResult result = model.Move(direction);
            Assert.That(result.Changed, Is.True);
            AssertRow(model, 0, expected);
        }

        [TestCase(MoveDirection.Up, new int[] { 4, 4, 0, 0 })]
        [TestCase(MoveDirection.Down, new int[] { 0, 0, 4, 4 })]
        public void Move_ProcessesVerticalDirections(MoveDirection direction, int[] expected)
        {
            var model = new BoardModel(new[,] { { 2, 0, 0, 0 }, { 0, 0, 0, 0 }, { 2, 0, 0, 0 }, { 4, 0, 0, 0 } });
            MoveResult result = model.Move(direction);
            Assert.That(result.ScoreGained, Is.EqualTo(4));
            for (int row = 0; row < BoardModel.Size; row++) Assert.That(model.GetCell(row, 0), Is.EqualTo(expected[row]));
        }

        [Test]
        public void Move_WithInvalidDirection_ThrowsArgumentOutOfRangeException()
        {
            Assert.That(() => new BoardModel().Move((MoveDirection)99), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [TestCase(MoveDirection.Left, 1, 3, 1, 0)]
        [TestCase(MoveDirection.Right, 1, 0, 1, 3)]
        [TestCase(MoveDirection.Up, 3, 2, 0, 2)]
        [TestCase(MoveDirection.Down, 0, 2, 3, 2)]
        public void Move_ReportsSingleTileMovementForEveryDirection(
            MoveDirection direction,
            int sourceRow,
            int sourceColumn,
            int targetRow,
            int targetColumn)
        {
            var board = new int[BoardModel.Size, BoardModel.Size];
            board[sourceRow, sourceColumn] = 2;
            var model = new BoardModel(board);

            MoveResult result = model.Move(direction);

            Assert.That(result.Changed, Is.True);
            Assert.That(result.ScoreGained, Is.EqualTo(0));
            Assert.That(result.Movements, Has.Count.EqualTo(1));
            AssertMovement(result.Movements[0], sourceRow, sourceColumn, targetRow, targetColumn, 2, false, 2);
        }

        [TestCase(MoveDirection.Left, 1, 1, 1, 3, 1, 0)]
        [TestCase(MoveDirection.Right, 1, 0, 1, 2, 1, 3)]
        [TestCase(MoveDirection.Up, 1, 2, 3, 2, 0, 2)]
        [TestCase(MoveDirection.Down, 0, 2, 2, 2, 3, 2)]
        public void Move_ReportsBothMergeSourcesForEveryDirection(
            MoveDirection direction,
            int firstSourceRow,
            int firstSourceColumn,
            int secondSourceRow,
            int secondSourceColumn,
            int targetRow,
            int targetColumn)
        {
            var board = new int[BoardModel.Size, BoardModel.Size];
            board[firstSourceRow, firstSourceColumn] = 2;
            board[secondSourceRow, secondSourceColumn] = 2;
            var model = new BoardModel(board);

            MoveResult result = model.Move(direction);

            Assert.That(result.ScoreGained, Is.EqualTo(4));
            Assert.That(result.Movements, Has.Count.EqualTo(2));
            Assert.That(
                ContainsMovement(result.Movements, firstSourceRow, firstSourceColumn, targetRow, targetColumn),
                Is.True);
            Assert.That(
                ContainsMovement(result.Movements, secondSourceRow, secondSourceColumn, targetRow, targetColumn),
                Is.True);
            foreach (TileMovement movement in result.Movements)
            {
                Assert.That(movement.Merged, Is.True);
                Assert.That(movement.StartValue, Is.EqualTo(2));
                Assert.That(movement.ResultValue, Is.EqualTo(4));
            }
        }

        [Test]
        public void Move_ReportsBothSourcesForMerge()
        {
            var model = new BoardModel(CreateBoard(new[] { 2, 0, 2, 4 }));

            MoveResult result = model.Move(MoveDirection.Left);

            Assert.That(result.Movements, Has.Count.EqualTo(3));
            AssertMovement(result.Movements[0], 0, 0, 0, 0, 2, true, 4);
            AssertMovement(result.Movements[1], 0, 2, 0, 0, 2, true, 4);
            AssertMovement(result.Movements[2], 0, 3, 0, 1, 4, false, 4);
        }

        [Test]
        public void Move_ReportsSeparateTargetsForTwoMergePairs()
        {
            var model = new BoardModel(CreateBoard(new[] { 2, 2, 2, 2 }));

            MoveResult result = model.Move(MoveDirection.Left);

            Assert.That(result.ScoreGained, Is.EqualTo(8));
            Assert.That(result.Movements, Has.Count.EqualTo(4));
            AssertMovement(result.Movements[0], 0, 0, 0, 0, 2, true, 4);
            AssertMovement(result.Movements[1], 0, 1, 0, 0, 2, true, 4);
            AssertMovement(result.Movements[2], 0, 2, 0, 1, 2, true, 4);
            AssertMovement(result.Movements[3], 0, 3, 0, 1, 2, true, 4);
        }

        [Test]
        public void Move_UnchangedBoardHasNoMovementRecords()
        {
            var model = new BoardModel(CreateBoard(new[] { 2, 4, 8, 16 }));

            MoveResult result = model.Move(MoveDirection.Left);

            Assert.That(result.Changed, Is.False);
            Assert.That(result.Movements, Is.Empty);
        }

        [Test]
        public void HasWon_ReturnsTrueWhenBoardContains2048()
        {
            Assert.That(new BoardModel(CreateBoard(new[] { 2048, 0, 0, 0 })).HasWon(), Is.True);
        }

        [Test]
        public void HasWon_ReturnsTrueWhenBoardContains4096()
        {
            Assert.That(new BoardModel(CreateBoard(new[] { 4096, 0, 0, 0 })).HasWon(), Is.True);
        }

        [Test]
        public void HasWon_ReturnsFalseBelow2048()
        {
            Assert.That(new BoardModel(CreateBoard(new[] { 1024, 0, 0, 0 })).HasWon(), Is.False);
        }

        [Test]
        public void CanMove_ReturnsTrueWhenBoardHasEmptyCell()
        {
            Assert.That(new BoardModel(CreateBoard(new[] { 2, 4, 8, 0 })).CanMove(), Is.True);
        }

        [Test]
        public void CanMove_ReturnsTrueWhenFullBoardHasMatchingNeighbors()
        {
            var model = new BoardModel(new[,] { { 2, 2, 4, 8 }, { 16, 32, 64, 128 }, { 256, 512, 1024, 2 }, { 4, 8, 16, 32 } });
            Assert.That(model.CanMove(), Is.True);
            Assert.That(model.IsGameOver(), Is.False);
        }

        [Test]
        public void IsGameOver_ReturnsTrueWhenFullBoardHasNoMatchingNeighbors()
        {
            var model = new BoardModel(new[,] { { 2, 4, 8, 16 }, { 32, 64, 128, 256 }, { 512, 1024, 2, 4 }, { 8, 16, 32, 64 } });
            Assert.That(model.CanMove(), Is.False);
            Assert.That(model.IsGameOver(), Is.True);
        }

        private static int[,] CreateBoard(int[] firstRow)
        {
            return new[,]
            {
                { firstRow[0], firstRow[1], firstRow[2], firstRow[3] },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 }
            };
        }

        private static void AssertRow(BoardModel model, int row, int[] expected)
        {
            for (int column = 0; column < BoardModel.Size; column++)
            {
                Assert.That(model.GetCell(row, column), Is.EqualTo(expected[column]));
            }
        }

        private static void AssertMovement(
            TileMovement movement,
            int sourceRow,
            int sourceColumn,
            int targetRow,
            int targetColumn,
            int startValue,
            bool merged,
            int resultValue)
        {
            Assert.That(movement.SourceRow, Is.EqualTo(sourceRow));
            Assert.That(movement.SourceColumn, Is.EqualTo(sourceColumn));
            Assert.That(movement.TargetRow, Is.EqualTo(targetRow));
            Assert.That(movement.TargetColumn, Is.EqualTo(targetColumn));
            Assert.That(movement.StartValue, Is.EqualTo(startValue));
            Assert.That(movement.Merged, Is.EqualTo(merged));
            Assert.That(movement.ResultValue, Is.EqualTo(resultValue));
        }

        private static bool ContainsMovement(
            IReadOnlyList<TileMovement> movements,
            int sourceRow,
            int sourceColumn,
            int targetRow,
            int targetColumn)
        {
            foreach (TileMovement movement in movements)
            {
                if (movement.SourceRow == sourceRow && movement.SourceColumn == sourceColumn &&
                    movement.TargetRow == targetRow && movement.TargetColumn == targetColumn)
                {
                    return true;
                }
            }
            return false;
        }

        private static List<BoardPosition> GetOccupiedCells(BoardModel model)
        {
            var occupiedCells = new List<BoardPosition>();

            for (int row = 0; row < BoardModel.Size; row++)
            {
                for (int column = 0; column < BoardModel.Size; column++)
                {
                    if (model.GetCell(row, column) != 0)
                    {
                        occupiedCells.Add(new BoardPosition(row, column));
                    }
                }
            }

            return occupiedCells;
        }

        private sealed class PredictableRandom : Random
        {
            public override int Next(int maxValue)
            {
                return maxValue > 1 ? 1 : 0;
            }
        }
    }
}
