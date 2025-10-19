using System;
using UnityEngine;

namespace DebugTool
{
    public class DebugPanelSpaceItem : DebugGUIPanelItem
    {
        private float spaceHeight;

        public DebugPanelSpaceItem(float height = 20f)
        {
            this.spaceHeight = height;
        }

        public override void Draw(Rect rect, GUIStyle labelStyle = null, GUIStyle buttonStyle = null)
        {
            // 不绘制任何内容，只是占用空间
        }

        public override float GetRequiredHeight()
        {
            return DebugPanelManager.GetAdaptiveHeight(spaceHeight);
        }

        public void SetHeight(float height)
        {
            spaceHeight = height;
        }
    }
}
