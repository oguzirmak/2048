using System;
using System.Collections.Generic;

namespace Game2048.Core
{
    public sealed class BoardModel
    {
        public const int Size = 4;

        private readonly int[,] board = new int[Size, Size];
        private readonly Random random;

        public BoardModel(Random random = null)
        {
            this.random = random ?? new Random();
            Reset();
        }

        public BoardModel(int[,] initialBoard, Random random = null)
            : this(random)
        {
            if (initialBoard == null)
            {
                throw new ArgumentNullException(nameof(initialBoard));
            }

            if (initialBoard.GetLength(0) != Size || initialBoard.GetLength(1) != Size)
            {
                throw new ArgumentException($"Initial board must be a {Size}x{Size} array.", nameof(initialBoard));
            }

            Array.Copy(initialBoard, board, board.Length);
        }

        public void Reset()
        {
            Array.Clear(board, 0, board.Length);
        }

        public int GetCell(int row, int column)
        {
            ValidatePosition(row, column);
            return board[row, column];
        }

        public IReadOnlyList<BoardPosition> GetEmptyCells()
        {
            var emptyCells = new List<BoardPosition>();

            for (int row = 0; row < Size; row++)
            {
                for (int column = 0; column < Size; column++)
                {
                    if (board[row, column] == 0)
                    {
                        emptyCells.Add(new BoardPosition(row, column));
                    }
                }
            }

            return emptyCells;
        }

        public bool TrySpawnTile()
        {
            IReadOnlyList<BoardPosition> emptyCells = GetEmptyCells();
            if (emptyCells.Count == 0)
            {
                return false;
            }

            BoardPosition position = emptyCells[random.Next(emptyCells.Count)];
            board[position.Row, position.Column] = random.Next(10) == 0 ? 4 : 2;
            return true;
        }

        public void StartNewGame()
        {
            Reset();
            TrySpawnTile();
            TrySpawnTile();
        }

        public bool HasWon()
        {
            for (int row = 0; row < Size; row++)
            for (int column = 0; column < Size; column++)
                if (board[row, column] >= 2048) return true;
            return false;
        }

        public bool CanMove()
        {
            for (int row = 0; row < Size; row++)
            for (int column = 0; column < Size; column++)
            {
                int value = board[row, column];
                if (value == 0) return true;
                if (row + 1 < Size && value == board[row + 1, column]) return true;
                if (column + 1 < Size && value == board[row, column + 1]) return true;
            }
            return false;
        }

        public bool IsGameOver() => !CanMove();

        public MoveResult MoveLeft()
        {
            return Move(MoveDirection.Left);
        }

        public MoveResult Move(MoveDirection direction)
        {
            if (direction != MoveDirection.Left && direction != MoveDirection.Right &&
                direction != MoveDirection.Up && direction != MoveDirection.Down)
            {
                throw new ArgumentOutOfRangeException(nameof(direction), direction, "Unsupported move direction.");
            }

            bool changed = false;
            int scoreGained = 0;
            var movements = new List<TileMovement>();

            for (int line = 0; line < Size; line++)
            {
                int[] input = ReadLine(line, direction);
                int lineScore;
                int[] result = ProcessLine(input, line, direction, movements, out lineScore);
                scoreGained += lineScore;
                for (int index = 0; index < Size; index++)
                {
                    BoardPosition position = GetPosition(line, index, direction);
                    if (board[position.Row, position.Column] != result[index])
                    {
                        changed = true;
                        board[position.Row, position.Column] = result[index];
                    }
                }
            }

            return new MoveResult(changed, scoreGained, movements);
        }

        private int[] ReadLine(int line, MoveDirection direction)
        {
            var values = new int[Size];
            for (int index = 0; index < Size; index++)
            {
                BoardPosition position = GetPosition(line, index, direction);
                values[index] = board[position.Row, position.Column];
            }
            return values;
        }

        private static int[] ProcessLine(
            int[] input,
            int line,
            MoveDirection direction,
            List<TileMovement> movements,
            out int scoreGained)
        {
            var compressed = new List<int>();
            var sourcePositions = new List<BoardPosition>();
            for (int index = 0; index < input.Length; index++)
            {
                if (input[index] == 0) continue;
                compressed.Add(input[index]);
                sourcePositions.Add(GetPosition(line, index, direction));
            }

            var result = new int[Size];
            scoreGained = 0;
            int write = 0;
            for (int read = 0; read < compressed.Count; read++)
            {
                BoardPosition target = GetPosition(line, write, direction);
                if (read + 1 < compressed.Count && compressed[read] == compressed[read + 1])
                {
                    int mergedValue = compressed[read] * 2;
                    result[write] = mergedValue;
                    scoreGained += mergedValue;
                    movements.Add(CreateMovement(sourcePositions[read], target, compressed[read], true, mergedValue));
                    movements.Add(CreateMovement(sourcePositions[read + 1], target, compressed[read + 1], true, mergedValue));
                    read++;
                }
                else
                {
                    result[write] = compressed[read];
                    BoardPosition source = sourcePositions[read];
                    if (!source.Equals(target))
                    {
                        movements.Add(CreateMovement(source, target, compressed[read], false, compressed[read]));
                    }
                }
                write++;
            }
            return result;
        }

        private static TileMovement CreateMovement(
            BoardPosition source,
            BoardPosition target,
            int startValue,
            bool merged,
            int resultValue)
        {
            return new TileMovement(
                source.Row,
                source.Column,
                target.Row,
                target.Column,
                startValue,
                merged,
                resultValue);
        }

        private static BoardPosition GetPosition(int line, int index, MoveDirection direction)
        {
            return direction switch
            {
                MoveDirection.Left => new BoardPosition(line, index),
                MoveDirection.Right => new BoardPosition(line, Size - 1 - index),
                MoveDirection.Up => new BoardPosition(index, line),
                MoveDirection.Down => new BoardPosition(Size - 1 - index, line),
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Unsupported move direction.")
            };
        }

        private static void ValidatePosition(int row, int column)
        {
            if (row < 0 || row >= Size)
            {
                throw new ArgumentOutOfRangeException(nameof(row), row, $"Row must be between 0 and {Size - 1}.");
            }

            if (column < 0 || column >= Size)
            {
                throw new ArgumentOutOfRangeException(nameof(column), column, $"Column must be between 0 and {Size - 1}.");
            }
        }
    }
}
