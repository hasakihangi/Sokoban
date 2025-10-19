using System;
using UnityEngine;

namespace DebugTool
{
    public class DebugPanelLabelItem : DebugGUIPanelItem
    {
        private Func<string> getValue;

        public DebugPanelLabelItem(Func<string> getValue)
        {
            this.getValue = getValue;
        }

        public override void Draw(Rect rect, GUIStyle labelStyle = null, GUIStyle buttonStyle = null)
        {
            if (getValue != null)
            {
                string text = getValue.Invoke();
                if (labelStyle != null)
                {
                    GUI.Label(rect, text, labelStyle);
                }
                else
                {
                    GUI.Label(rect, text);
                }
            }
        }

        public override float GetRequiredHeight()
        {
            // 根据文本长度调整高度，适配更大字体
            string text = getValue?.Invoke() ?? "";
            float baseHeight = text.Length > 50 ? 40f : 30f; // 调大一点
            return DebugPanelManager.GetAdaptiveHeight(baseHeight);
        }
    }

}
