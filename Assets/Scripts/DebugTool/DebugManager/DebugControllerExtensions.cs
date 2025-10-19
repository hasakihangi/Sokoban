using System;

namespace DebugTool
{
    public static class DebugControllerExtensions
    {
        public static void Register(this IDebugGUIController controller)
        {
            DebugManager.Instance.guiControllers.Add(controller);
        }

        public static void Register(this IDebugGizmosController controller)
        {
            DebugManager.Instance.gizmosControllers.Add(controller);
        }

        public static void Register(this IDebugGameObjectController controller)
        {
            DebugManager.Instance.AddGameObjectController(controller);
        }
    }
}
