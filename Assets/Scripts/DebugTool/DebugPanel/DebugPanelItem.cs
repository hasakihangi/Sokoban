using System;
using UnityEngine;

namespace DebugTool
{
    public abstract class DebugGUIPanelItem
    {
        public abstract void Draw(Rect rect, GUIStyle labelStyle = null, GUIStyle buttonStyle = null);
        public abstract float GetRequiredHeight();
    }

    public static class DebugGUIItemExtension
    {
        public static void Register(this DebugGUIPanelItem panelItem)
        {
            DebugPanelManager.Instance.RegisterDebugItem(panelItem);
        }

        public static void Unregister(this DebugGUIPanelItem panelItem)
        {
            DebugPanelManager.Instance.RemoveDebugItem(panelItem);
        }
    }
}
