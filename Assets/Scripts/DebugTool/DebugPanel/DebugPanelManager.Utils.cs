using System;
using UnityEngine;

namespace DebugTool
{
    public partial class DebugPanelManager
    {
        private float GetScaleFactor()
        {
            float scaleFactor = Screen.height / baseScreenHeight;
            return Mathf.Clamp(scaleFactor, 0.5f, 2.0f);
        }

        // 静态方法：应用分辨率自适应到高度值
        public static float GetAdaptiveHeight(float baseHeight)
        {
            float scaleFactor = Screen.height / 1080f;
            scaleFactor = Mathf.Clamp(scaleFactor, 0.5f, 2.0f);
            return baseHeight * scaleFactor;
        }
    }
}
