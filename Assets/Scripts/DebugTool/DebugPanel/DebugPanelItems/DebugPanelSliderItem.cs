using System;
using UnityEngine;

namespace DebugTool
{
       public class DebugPanelSliderItem : DebugGUIPanelItem
    {
        public string Title { get; private set; }
        private float minValue;
        private float maxValue;
        private Action<float> setValue;
        private Func<float> getValue;

        public DebugPanelSliderItem(string title, float min, float max, Action<float> setValue, Func<float> getValue)
        {
            this.Title = title;
            this.minValue = min;
            this.maxValue = max;
            this.setValue = setValue;
            this.getValue = getValue;
        }

        public override void Draw(Rect rect, GUIStyle labelStyle = null, GUIStyle buttonStyle = null)
        {
            if (getValue != null && setValue != null)
            {
                float currentY = rect.y;
                float currentValue = getValue();

                // 绘制标题（如果有标题）
                if (!string.IsNullOrEmpty(Title))
                {
                    float titleHeight = DebugPanelManager.GetAdaptiveHeight(25f);
                    Rect titleRect = new Rect(rect.x, currentY, rect.width, titleHeight);

                    if (labelStyle != null)
                    {
                        GUI.Label(titleRect, Title, labelStyle);
                    }
                    else
                    {
                        Color originalColor = GUI.contentColor;
                        GUI.contentColor = Color.white;
                        GUI.Label(titleRect, Title);
                        GUI.contentColor = originalColor;
                    }
                    currentY += titleHeight;
                }

                // 绘制滑动条（使用自适应高度）
                float sliderHeight = DebugPanelManager.GetAdaptiveHeight(20f);
                Rect sliderRect = new Rect(rect.x, currentY, rect.width, sliderHeight);
                float newValue = GUI.HorizontalSlider(sliderRect, currentValue, minValue, maxValue);

                if (!Mathf.Approximately(newValue, currentValue))
                {
                    setValue(newValue);
                }

                // 绘制数值显示（使用更小的字体）
                float labelHeight = DebugPanelManager.GetAdaptiveHeight(18f);
                float spacing = DebugPanelManager.GetAdaptiveHeight(2f);
                Rect valueRect = new Rect(rect.x, currentY + sliderHeight + spacing, rect.width, labelHeight);
                string valueText = $"{newValue:F2} ({minValue:F1} - {maxValue:F1})";

                // 创建小一些的字体样式
                GUIStyle smallLabelStyle;
                if (labelStyle != null)
                {
                    smallLabelStyle = new GUIStyle(labelStyle);
                    smallLabelStyle.fontSize = Mathf.RoundToInt(labelStyle.fontSize * 0.85f); // 缩小15%
                    GUI.Label(valueRect, valueText, smallLabelStyle);
                }
                else
                {
                    // 使用更小的默认字体
                    Color originalColor = GUI.contentColor;
                    GUI.contentColor = Color.white;

                    smallLabelStyle = new GUIStyle(GUI.skin.label);
                    float scaleFactor = Screen.height / 1080f;
                    scaleFactor = Mathf.Clamp(scaleFactor, 0.5f, 2.0f);
                    smallLabelStyle.fontSize = Mathf.RoundToInt(12 * scaleFactor * 0.85f); // 基础字体12，缩放后再缩小15%
                    smallLabelStyle.normal.textColor = Color.white;

                    GUI.Label(valueRect, valueText, smallLabelStyle);
                    GUI.contentColor = originalColor;
                }
            }
        }

        public override float GetRequiredHeight()
        {
            float baseHeight = 45f; // 滑动条 + 数值显示，适配更大字体

            // 如果有标题，增加标题高度
            if (!string.IsNullOrEmpty(Title))
            {
                baseHeight += 25f; // 标题高度
            }

            return DebugPanelManager.GetAdaptiveHeight(baseHeight);
        }

        public float GetValue()
        {
            return getValue?.Invoke() ?? 0f;
        }
    }

}
