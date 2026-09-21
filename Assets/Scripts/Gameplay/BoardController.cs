using System.Collections;
using System.Collections.Generic;
using Game2048.Audio;
using Game2048.Core;
using Game2048.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game2048.Gameplay
{
    public sealed class BoardController : MonoBehaviour
    {
        private const string HighScorePlayerPrefsKey = "Game2048.HighScore";

        [SerializeField] private List<TileView> tileViews = new List<TileView>();
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text highScoreText;
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject gameOverPanel;

        [Header("Audio")]
        [SerializeField] private GameAudioController audioController;

        [Header("Animation")]
        [SerializeField] private bool animationsEnabled = true;
        [SerializeField, Min(0f)] private float moveAnimationDuration = 0.10f;
        [SerializeField, Min(0f)] private float additionalCellSlideDuration = 0.03f;
        [SerializeField, Min(0f)] private float maximumSlideDuration = 0.19f;
        [SerializeField, Min(0f)] private float mergeAnimationDuration = 0.10f;
        [SerializeField, Min(0f)] private float spawnAnimationDuration = 0.12f;

        private BoardModel boardModel;
        private bool gameFinished;
        private bool animationInProgress;
        private Coroutine animationRoutine;
        private RectTransform animationLayer;
        private HighScoreTracker highScoreTracker;
        private bool warnedAboutAudioController;

        public int Score { get; private set; }
        public int HighScore => highScoreTracker?.HighScore ?? 0;

        private void Reset()
        {
            tileViews = new List<TileView>(GetComponentsInChildren<TileView>(true));
        }

        private void Start()
        {
            if (!HasValidReferences()) return;
            EnsureHighScoreLoaded();
            StartNewGame();
        }

        public void StartNewGame()
        {
            if (!HasValidReferences()) return;

            StopActiveAnimation();
            EnsureHighScoreLoaded();
            boardModel = new BoardModel();
            boardModel.StartNewGame();
            Score = 0;
            gameFinished = false;
            winPanel.SetActive(false);
            gameOverPanel.SetActive(false);
            RefreshScoreText();
            RefreshHighScoreText();
            RefreshView();
            audioController.PlaySpawn();

            var spawnedIndices = GetOccupiedIndices(CaptureBoardValues());
            if (CanAnimate(spawnAnimationDuration))
            {
                BeginAnimation(RunSpawnAnimation(spawnedIndices, true));
            }
        }

        public bool TryMove(MoveDirection direction)
        {
            if (!HasValidReferences() || gameFinished || animationInProgress) return false;
            EnsureHighScoreLoaded();
            if (boardModel == null)
            {
                StartNewGame();
                return false;
            }

            int[,] beforeMove = CaptureBoardValues();
            MoveResult result = boardModel.Move(direction);
            if (!result.Changed)
            {
                RefreshView();
                UpdateGameState();
                return false;
            }

            Score += result.ScoreGained;
            RefreshScoreText();
            if (highScoreTracker.TryUpdate(Score))
            {
                SaveHighScore();
                RefreshHighScoreText();
            }

            int[,] afterMove = CaptureBoardValues();
            boardModel.TrySpawnTile();
            int[,] finalBoard = CaptureBoardValues();

            List<int> mergedIndices = GetMergedTargetIndices(result.Movements);
            int spawnedIndex = FindSpawnedIndex(afterMove, finalBoard);
            bool hasMerge = mergedIndices.Count > 0;
            bool hasTerminalState = boardModel.HasWon() || boardModel.IsGameOver();
            bool shouldPlaySpawn = spawnedIndex >= 0 && !hasTerminalState;
            if (hasMerge) audioController.PlayMerge();
            else audioController.PlayMove();

            bool hasAnimation =
                result.Movements.Count > 0 && moveAnimationDuration > 0f && maximumSlideDuration > 0f ||
                mergedIndices.Count > 0 && mergeAnimationDuration > 0f ||
                spawnedIndex >= 0 && spawnAnimationDuration > 0f;
            if (!animationsEnabled || !hasAnimation)
            {
                RefreshView();
                if (shouldPlaySpawn) audioController.PlaySpawn();
                UpdateGameState();
                return true;
            }

            BeginAnimation(RunMoveAnimation(
                beforeMove,
                afterMove,
                finalBoard,
                result.Movements,
                mergedIndices,
                spawnedIndex,
                shouldPlaySpawn));
            return true;
        }

        [ContextMenu("Preview/New Game")]
        private void PreviewNewGame() => StartNewGame();
        [ContextMenu("Preview/Move Left")]
        private void PreviewLeft() => TryMove(MoveDirection.Left);
        [ContextMenu("Preview/Move Right")]
        private void PreviewRight() => TryMove(MoveDirection.Right);
        [ContextMenu("Preview/Move Up")]
        private void PreviewUp() => TryMove(MoveDirection.Up);
        [ContextMenu("Preview/Move Down")]
        private void PreviewDown() => TryMove(MoveDirection.Down);

#if UNITY_EDITOR
        [ContextMenu("Preview/Test Win State")]
        private void PreviewWinState()
        {
            if (!HasValidReferences()) return;
            StopActiveAnimation();
            boardModel = new BoardModel(new[,]
            {
                { 2048, 2, 4, 8 }, { 16, 32, 64, 128 },
                { 256, 512, 1024, 2 }, { 4, 8, 16, 32 }
            });
            gameFinished = false;
            winPanel.SetActive(false);
            gameOverPanel.SetActive(false);
            RefreshView();
            UpdateGameState();
        }

        [ContextMenu("Preview/Test Game Over State")]
        private void PreviewGameOverState()
        {
            if (!HasValidReferences()) return;
            StopActiveAnimation();
            boardModel = new BoardModel(new[,]
            {
                { 2, 4, 8, 16 }, { 32, 64, 128, 256 },
                { 512, 1024, 2, 4 }, { 8, 16, 32, 64 }
            });
            gameFinished = false;
            winPanel.SetActive(false);
            gameOverPanel.SetActive(false);
            RefreshView();
            UpdateGameState();
        }
#endif

        private void OnDisable()
        {
            StopActiveAnimation();
        }

        private bool HasValidReferences()
        {
            if (scoreText == null)
            {
                Debug.LogError($"BoardController on '{name}' requires a Score Text reference in the Inspector.", this);
                return false;
            }
            if (highScoreText == null)
            {
                Debug.LogError($"BoardController on '{name}' requires a High Score Text reference in the Inspector.", this);
                return false;
            }
            if (winPanel == null || gameOverPanel == null)
            {
                Debug.LogError($"BoardController on '{name}' requires Win Panel and Game Over Panel references in the Inspector.", this);
                return false;
            }
            if (audioController == null)
            {
                if (!warnedAboutAudioController)
                {
                    warnedAboutAudioController = true;
                    Debug.LogError($"BoardController on '{name}' requires an Audio Controller reference in the Inspector.", this);
                }
                return false;
            }
            warnedAboutAudioController = false;
            if (tileViews == null || tileViews.Count != BoardModel.Size * BoardModel.Size)
            {
                Debug.LogError($"BoardController on '{name}' requires exactly 16 TileView references.", this);
                return false;
            }
            for (int index = 0; index < tileViews.Count; index++)
            {
                if (tileViews[index] == null)
                {
                    Debug.LogError($"BoardController on '{name}' has a missing TileView at index {index}.", this);
                    return false;
                }
            }
            return true;
        }

        private void RefreshScoreText()
        {
            scoreText.text = Score.ToString();
        }

        private void RefreshHighScoreText()
        {
            highScoreText.text = HighScore.ToString();
        }

        private void EnsureHighScoreLoaded()
        {
            if (highScoreTracker != null) return;
            int storedHighScore = PlayerPrefs.GetInt(HighScorePlayerPrefsKey, 0);
            highScoreTracker = new HighScoreTracker(storedHighScore);
        }

        private void SaveHighScore()
        {
            PlayerPrefs.SetInt(HighScorePlayerPrefsKey, HighScore);
            PlayerPrefs.Save();
        }

        private void RefreshView()
        {
            for (int row = 0; row < BoardModel.Size; row++)
            for (int column = 0; column < BoardModel.Size; column++)
                tileViews[row * BoardModel.Size + column].SetValue(boardModel.GetCell(row, column));
        }

        private void RefreshView(int[,] values)
        {
            for (int row = 0; row < BoardModel.Size; row++)
            for (int column = 0; column < BoardModel.Size; column++)
                tileViews[row * BoardModel.Size + column].SetValue(values[row, column]);
        }

        private int[,] CaptureBoardValues()
        {
            var values = new int[BoardModel.Size, BoardModel.Size];
            for (int row = 0; row < BoardModel.Size; row++)
            for (int column = 0; column < BoardModel.Size; column++)
                values[row, column] = boardModel.GetCell(row, column);
            return values;
        }

        private void BeginAnimation(IEnumerator routine)
        {
            animationInProgress = true;
            animationRoutine = StartCoroutine(routine);
        }

        private IEnumerator RunMoveAnimation(
            int[,] beforeMove,
            int[,] afterMove,
            int[,] finalBoard,
            IReadOnlyList<TileMovement> movements,
            List<int> mergedIndices,
            int spawnedIndex,
            bool playSpawnSound)
        {
            if (movements.Count > 0 && CanAnimate(moveAnimationDuration) && maximumSlideDuration > 0f)
            {
                yield return AnimateSlide(beforeMove, afterMove, movements);
            }
            else RefreshView(afterMove);

            if (mergedIndices.Count > 0 && CanAnimate(mergeAnimationDuration))
            {
                yield return AnimateScale(mergedIndices, 1f, 1.12f, mergeAnimationDuration);
            }

            RefreshView(finalBoard);
            if (playSpawnSound) audioController.PlaySpawn();
            if (spawnedIndex >= 0 && CanAnimate(spawnAnimationDuration))
            {
                var spawnedIndices = new List<int> { spawnedIndex };
                SetTileScales(spawnedIndices, 0f);
                yield return AnimateScale(
                    spawnedIndices,
                    0f,
                    1f,
                    spawnAnimationDuration,
                    false);
            }

            FinishAnimation(true);
        }

        private IEnumerator RunSpawnAnimation(List<int> spawnedIndices, bool updateGameStateAtEnd)
        {
            SetTileScales(spawnedIndices, 0f);
            yield return AnimateScale(spawnedIndices, 0f, 1f, spawnAnimationDuration, false);
            FinishAnimation(updateGameStateAtEnd);
        }

        private IEnumerator AnimateScale(
            List<int> indices,
            float startScale,
            float peakScale,
            float duration,
            bool returnToNormal = true)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float progress = Mathf.Clamp01(elapsed / duration);
                float scale;
                if (returnToNormal)
                {
                    float pulse = 1f - Mathf.Abs(progress * 2f - 1f);
                    scale = Mathf.LerpUnclamped(startScale, peakScale, SmoothStep(pulse));
                }
                else
                {
                    scale = Mathf.LerpUnclamped(startScale, peakScale, SmoothStep(progress));
                }

                foreach (int index in indices)
                {
                    tileViews[index].SetAnimationScale(scale);
                }
                yield return null;
            }

            foreach (int index in indices)
            {
                tileViews[index].SetAnimationScale(1f);
            }
        }

        private void FinishAnimation(bool updateGameState)
        {
            ResetTileScales();
            animationRoutine = null;
            animationInProgress = false;
            if (updateGameState)
            {
                UpdateGameState();
            }
        }

        private void StopActiveAnimation()
        {
            if (animationRoutine != null)
            {
                StopCoroutine(animationRoutine);
                animationRoutine = null;
            }

            CleanupAnimationLayer();
            ResetTileScales();
            animationInProgress = false;
        }

        private void ResetTileScales()
        {
            if (tileViews == null) return;
            for (int index = 0; index < tileViews.Count; index++)
            {
                if (tileViews[index] != null) tileViews[index].SetAnimationScale(1f);
            }
        }

        private void SetTileScales(List<int> indices, float scale)
        {
            foreach (int index in indices)
            {
                tileViews[index].SetAnimationScale(scale);
            }
        }

        private bool CanAnimate(float duration)
        {
            return animationsEnabled && duration > 0f;
        }

        private static float SmoothStep(float value)
        {
            return value * value * (3f - 2f * value);
        }

        private static List<int> GetOccupiedIndices(int[,] values)
        {
            var indices = new List<int>();
            for (int row = 0; row < BoardModel.Size; row++)
            for (int column = 0; column < BoardModel.Size; column++)
                if (values[row, column] != 0) indices.Add(row * BoardModel.Size + column);
            return indices;
        }

        private static int FindSpawnedIndex(int[,] beforeSpawn, int[,] afterSpawn)
        {
            for (int row = 0; row < BoardModel.Size; row++)
            for (int column = 0; column < BoardModel.Size; column++)
                if (beforeSpawn[row, column] == 0 && afterSpawn[row, column] != 0)
                    return row * BoardModel.Size + column;
            return -1;
        }

        private IEnumerator AnimateSlide(
            int[,] beforeMove,
            int[,] afterMove,
            IReadOnlyList<TileMovement> movements)
        {
            CreateAnimationLayer();
            var animatedTiles = new List<AnimatedTile>(movements.Count);
            float longestDuration = 0f;

            foreach (TileMovement movement in movements)
            {
                AnimatedTile animatedTile = CreateAnimatedTile(movement);
                if (animatedTile == null) continue;
                animatedTiles.Add(animatedTile);
                longestDuration = Mathf.Max(longestDuration, animatedTile.Duration);
            }

            RefreshView(CreateSlideBackground(beforeMove, movements));

            float elapsed = 0f;
            while (elapsed < longestDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                foreach (AnimatedTile animatedTile in animatedTiles)
                {
                    float progress = animatedTile.Duration <= 0f
                        ? 1f
                        : Mathf.Clamp01(elapsed / animatedTile.Duration);
                    animatedTile.RectTransform.position = Vector3.LerpUnclamped(
                        animatedTile.StartPosition,
                        animatedTile.TargetPosition,
                        SmoothStep(progress));
                }
                yield return null;
            }

            foreach (AnimatedTile animatedTile in animatedTiles)
            {
                animatedTile.RectTransform.position = animatedTile.TargetPosition;
            }

            CleanupAnimationLayer();
            RefreshView(afterMove);
        }

        private void CreateAnimationLayer()
        {
            CleanupAnimationLayer();

            var layerObject = new GameObject(
                "AnimationLayer",
                typeof(RectTransform),
                typeof(LayoutElement),
                typeof(Canvas),
                typeof(CanvasGroup));
            animationLayer = layerObject.GetComponent<RectTransform>();
            animationLayer.SetParent(transform, false);
            animationLayer.anchorMin = Vector2.zero;
            animationLayer.anchorMax = Vector2.one;
            animationLayer.offsetMin = Vector2.zero;
            animationLayer.offsetMax = Vector2.zero;
            animationLayer.SetAsLastSibling();

            layerObject.GetComponent<LayoutElement>().ignoreLayout = true;
            Canvas layerCanvas = layerObject.GetComponent<Canvas>();
            layerCanvas.overrideSorting = true;
            layerCanvas.sortingOrder = 100;
            CanvasGroup canvasGroup = layerObject.GetComponent<CanvasGroup>();
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        private AnimatedTile CreateAnimatedTile(TileMovement movement)
        {
            int sourceIndex = movement.SourceRow * BoardModel.Size + movement.SourceColumn;
            int targetIndex = movement.TargetRow * BoardModel.Size + movement.TargetColumn;
            RectTransform sourceRect = tileViews[sourceIndex].transform as RectTransform;
            RectTransform targetRect = tileViews[targetIndex].transform as RectTransform;
            if (sourceRect == null || targetRect == null || animationLayer == null)
            {
                Debug.LogError($"BoardController on '{name}' could not create a slide visual for a TileView.", this);
                return null;
            }

            GameObject clone = Instantiate(tileViews[sourceIndex].gameObject, animationLayer, false);
            clone.name = $"AnimatedTile_{movement.SourceRow}_{movement.SourceColumn}";
            TileView cloneView = clone.GetComponent<TileView>();
            if (cloneView != null) cloneView.SetValue(movement.StartValue);

            LayoutElement layoutElement = clone.GetComponent<LayoutElement>();
            if (layoutElement == null) layoutElement = clone.AddComponent<LayoutElement>();
            layoutElement.ignoreLayout = true;
            foreach (Graphic graphic in clone.GetComponentsInChildren<Graphic>(true))
            {
                graphic.raycastTarget = false;
            }

            RectTransform cloneRect = clone.transform as RectTransform;
            cloneRect.anchorMin = new Vector2(0.5f, 0.5f);
            cloneRect.anchorMax = new Vector2(0.5f, 0.5f);
            cloneRect.pivot = sourceRect.pivot;
            cloneRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sourceRect.rect.width);
            cloneRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sourceRect.rect.height);
            cloneRect.localRotation = Quaternion.identity;
            cloneRect.localScale = Vector3.one;
            cloneRect.position = sourceRect.position;

            int distance = Mathf.Abs(movement.TargetRow - movement.SourceRow) +
                           Mathf.Abs(movement.TargetColumn - movement.SourceColumn);
            float duration = CalculateSlideDuration(distance);
            return new AnimatedTile(cloneRect, sourceRect.position, targetRect.position, duration);
        }

        private float CalculateSlideDuration(int cellDistance)
        {
            if (cellDistance <= 0) return 0f;
            float duration = moveAnimationDuration +
                             Mathf.Max(0, cellDistance - 1) * additionalCellSlideDuration;
            return Mathf.Min(duration, maximumSlideDuration);
        }

        private static int[,] CreateSlideBackground(
            int[,] beforeMove,
            IReadOnlyList<TileMovement> movements)
        {
            var values = new int[BoardModel.Size, BoardModel.Size];
            System.Array.Copy(beforeMove, values, beforeMove.Length);
            foreach (TileMovement movement in movements)
            {
                values[movement.SourceRow, movement.SourceColumn] = 0;
            }
            return values;
        }

        private static List<int> GetMergedTargetIndices(IReadOnlyList<TileMovement> movements)
        {
            var indices = new List<int>();
            foreach (TileMovement movement in movements)
            {
                if (!movement.Merged) continue;
                int index = movement.TargetRow * BoardModel.Size + movement.TargetColumn;
                if (!indices.Contains(index)) indices.Add(index);
            }
            return indices;
        }

        private void CleanupAnimationLayer()
        {
            if (animationLayer != null)
            {
                animationLayer.gameObject.SetActive(false);
                if (Application.isPlaying) Destroy(animationLayer.gameObject);
                else DestroyImmediate(animationLayer.gameObject);
                animationLayer = null;
            }
        }

        private void UpdateGameState()
        {
            if (gameFinished) return;

            if (boardModel.HasWon())
            {
                gameFinished = true;
                winPanel.SetActive(true);
                audioController.PlayWin();
            }
            else if (boardModel.IsGameOver())
            {
                gameFinished = true;
                gameOverPanel.SetActive(true);
                audioController.PlayGameOver();
            }
        }

        private sealed class AnimatedTile
        {
            public AnimatedTile(
                RectTransform rectTransform,
                Vector3 startPosition,
                Vector3 targetPosition,
                float duration)
            {
                RectTransform = rectTransform;
                StartPosition = startPosition;
                TargetPosition = targetPosition;
                Duration = duration;
            }

            public RectTransform RectTransform { get; }
            public Vector3 StartPosition { get; }
            public Vector3 TargetPosition { get; }
            public float Duration { get; }
        }

    }
}
