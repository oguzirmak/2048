using UnityEngine;
using UnityEngine.UI;

namespace Game2048.UI
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform), typeof(GridLayoutGroup))]
    public sealed class ResponsiveSquareGrid : MonoBehaviour
    {
        private const int GridSize = 4;

        private RectTransform boardRect;
        private GridLayoutGroup grid;
        private Vector2 lastRectSize = new Vector2(-1f, -1f);
        private Vector2 lastSpacing = new Vector2(-1f, -1f);
        private Vector4 lastPadding = new Vector4(-1f, -1f, -1f, -1f);

        private void OnEnable()
        {
            CacheComponents();
            RefreshCellSize();
        }

        private void OnRectTransformDimensionsChange()
        {
            RefreshCellSize();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            CacheComponents();
            RefreshCellSize();
        }
#endif

        [ContextMenu("Refresh Cell Size")]
        private void RefreshCellSize()
        {
            CacheComponents();
            if (boardRect == null || grid == null) return;

            Vector2 rectSize = boardRect.rect.size;
            if (rectSize.x <= 0f || rectSize.y <= 0f) return;
            Vector4 padding = new Vector4(grid.padding.left, grid.padding.right, grid.padding.top, grid.padding.bottom);
            if (rectSize == lastRectSize && grid.spacing == lastSpacing && padding == lastPadding &&
                grid.constraint == GridLayoutGroup.Constraint.FixedColumnCount && grid.constraintCount == GridSize) return;

            float horizontalSpace = grid.padding.left + grid.padding.right + grid.spacing.x * (GridSize - 1);
            float verticalSpace = grid.padding.top + grid.padding.bottom + grid.spacing.y * (GridSize - 1);
            float cellSize = Mathf.Min(
                (rectSize.x - horizontalSpace) / GridSize,
                (rectSize.y - verticalSpace) / GridSize);
            if (cellSize <= 0f) return;

            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = GridSize;
            Vector2 squareSize = new Vector2(cellSize, cellSize);
            if (grid.cellSize != squareSize) grid.cellSize = squareSize;
            lastRectSize = rectSize;
            lastSpacing = grid.spacing;
            lastPadding = padding;
        }

        private void CacheComponents()
        {
            if (boardRect == null) boardRect = GetComponent<RectTransform>();
            if (grid == null) grid = GetComponent<GridLayoutGroup>();
        }
    }
}
