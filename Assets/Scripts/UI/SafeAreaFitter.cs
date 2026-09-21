using UnityEngine;

namespace Game2048.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeAreaFitter : MonoBehaviour
    {
        private RectTransform target;
        private Rect lastSafeArea = new Rect(-1f, -1f, -1f, -1f);
        private Vector2Int lastScreenSize = new Vector2Int(-1, -1);

        private void Awake()
        {
            target = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            if (target == null) target = GetComponent<RectTransform>();
            ApplySafeArea(true);
        }

        private void Update()
        {
            ApplySafeArea(false);
        }

        private void ApplySafeArea(bool force)
        {
            int screenWidth = Screen.width;
            int screenHeight = Screen.height;
            if (target == null || screenWidth <= 0 || screenHeight <= 0) return;

            Rect safeArea = Screen.safeArea;
            var screenSize = new Vector2Int(screenWidth, screenHeight);
            if (!force && safeArea == lastSafeArea && screenSize == lastScreenSize) return;

            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;
            anchorMin.x = Mathf.Clamp01(anchorMin.x / screenWidth);
            anchorMin.y = Mathf.Clamp01(anchorMin.y / screenHeight);
            anchorMax.x = Mathf.Clamp01(anchorMax.x / screenWidth);
            anchorMax.y = Mathf.Clamp01(anchorMax.y / screenHeight);

            target.anchorMin = anchorMin;
            target.anchorMax = anchorMax;
            target.offsetMin = Vector2.zero;
            target.offsetMax = Vector2.zero;

            lastSafeArea = safeArea;
            lastScreenSize = screenSize;
        }
    }
}
