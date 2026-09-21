using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game2048.UI
{
    public sealed class TileView : MonoBehaviour
    {
        private const float TwoDigitFontSize = 72f;
        private const float ThreeDigitFontSize = 60f;
        private const float FourDigitFontSize = 48f;
        private const float LargeNumberFontSize = 38f;
        private const float MinimumAutoSizeFont = 20f;

        [Header("References")]
        [SerializeField] private Image background;
        [SerializeField] private TMP_Text valueText;

        public void SetValue(int value)
        {
            if (!HasRequiredReferences())
            {
                return;
            }

            if (value < 0)
            {
                Debug.LogError($"TileView on '{name}' cannot display a negative value.", this);
                return;
            }

            bool hasValue = value > 0;
            background.color = GetBackgroundColor(value);
            valueText.gameObject.SetActive(hasValue);

            if (!hasValue)
            {
                return;
            }

            valueText.text = value.ToString();
            valueText.color = GetTextColor(value);
            float targetFontSize = GetFontSize(value);
            valueText.enableAutoSizing = true;
            valueText.fontSizeMin = Mathf.Min(MinimumAutoSizeFont, targetFontSize);
            valueText.fontSizeMax = targetFontSize;
            valueText.fontSize = targetFontSize;
        }

        public void SetAnimationScale(float scale)
        {
            transform.localScale = Vector3.one * scale;
        }

        [ContextMenu("Preview/Empty (0)")]
        private void PreviewEmpty()
        {
            SetValue(0);
        }

        [ContextMenu("Preview/2")]
        private void PreviewTwo()
        {
            SetValue(2);
        }

        [ContextMenu("Preview/4")]
        private void PreviewFour()
        {
            SetValue(4);
        }

        [ContextMenu("Preview/8")]
        private void PreviewEight()
        {
            SetValue(8);
        }

        [ContextMenu("Preview/16")]
        private void PreviewSixteen()
        {
            SetValue(16);
        }

        [ContextMenu("Preview/32")]
        private void PreviewThirtyTwo()
        {
            SetValue(32);
        }

        [ContextMenu("Preview/64")]
        private void PreviewSixtyFour()
        {
            SetValue(64);
        }

        [ContextMenu("Preview/128")]
        private void PreviewOneHundredTwentyEight()
        {
            SetValue(128);
        }

        [ContextMenu("Preview/256")]
        private void PreviewTwoHundredFiftySix()
        {
            SetValue(256);
        }

        [ContextMenu("Preview/512")]
        private void PreviewFiveHundredTwelve()
        {
            SetValue(512);
        }

        [ContextMenu("Preview/1024")]
        private void PreviewOneThousandTwentyFour()
        {
            SetValue(1024);
        }

        [ContextMenu("Preview/2048")]
        private void PreviewTwoThousandFortyEight()
        {
            SetValue(2048);
        }

        [ContextMenu("Preview/4096")]
        private void PreviewFourThousandNinetySix()
        {
            SetValue(4096);
        }

        private bool HasRequiredReferences()
        {
            if (background != null && valueText != null)
            {
                return true;
            }

            Debug.LogError(
                $"TileView on '{name}' requires both Image Background and Value Text references in the Inspector.",
                this);
            return false;
        }

        private static Color GetBackgroundColor(int value)
        {
            return value switch
            {
                0 => new Color32(205, 193, 180, 255),
                2 => new Color32(238, 228, 218, 255),
                4 => new Color32(237, 224, 200, 255),
                8 => new Color32(242, 177, 121, 255),
                16 => new Color32(245, 149, 99, 255),
                32 => new Color32(246, 124, 95, 255),
                64 => new Color32(246, 94, 59, 255),
                128 => new Color32(237, 207, 114, 255),
                256 => new Color32(237, 204, 97, 255),
                512 => new Color32(237, 200, 80, 255),
                1024 => new Color32(237, 197, 63, 255),
                2048 => new Color32(237, 194, 46, 255),
                4096 => new Color32(60, 58, 50, 255),
                _ => new Color32(60, 58, 50, 255)
            };
        }

        private static Color GetTextColor(int value)
        {
            return value is 2 or 4
                ? new Color32(119, 110, 101, 255)
                : new Color32(249, 246, 242, 255);
        }

        private static float GetFontSize(int value)
        {
            if (value < 100)
            {
                return TwoDigitFontSize;
            }

            if (value < 1000)
            {
                return ThreeDigitFontSize;
            }

            return value < 10000 ? FourDigitFontSize : LargeNumberFontSize;
        }
    }
}
