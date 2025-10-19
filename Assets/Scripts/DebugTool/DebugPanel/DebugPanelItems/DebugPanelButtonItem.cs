using System;
using UnityEngine;

namespace DebugTool
{
    public class DebugGUIPanelButtonItem : DebugGUIPanelItem
    {
        private Action onClick;
        private string buttonText;

        public DebugGUIPanelButtonItem(string title, Action onClick)
        {
            this.onClick = onClick;
            this.buttonText = title;
        }

        public override void Draw(Rect rect, GUIStyle labelStyle = null, GUIStyle buttonStyle = null)
        {
            // Button使用默认样式但文字设为白色
            Color originalColor = GUI.contentColor;
            GUI.contentColor = Color.white;

            if (GUI.Button(rect, buttonText) && onClick != null)
            {
                onClick();
            }

            GUI.contentColor = originalColor;
        }

        public override float GetRequiredHeight()
        {
            return DebugPanelManager.GetAdaptiveHeight(35f); // 按钮高度，调大一点适配更大字体
        }

        public void SetButtonText(string text)
        {
            buttonText = text;
        }
    }

}
