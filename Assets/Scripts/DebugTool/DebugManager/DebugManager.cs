using System;
using System.Collections.Generic;

namespace DebugTool
{
    public class DebugManager : SingletonBehaviour<DebugManager>
    {
        public List<IDebugGizmosController> gizmosControllers = new List<IDebugGizmosController>();
        public List<IDebugGUIController> guiControllers = new List<IDebugGUIController>();
        // public List ?
        private List<IDebugGameObjectController> gameObjectControllers = new List<IDebugGameObjectController>();

        public void AddGameObjectController(IDebugGameObjectController controller)
        {
            controller.OnRegister();
            gameObjectControllers.Add(controller);
        }

        public void RemoveGameObjectController(IDebugGameObjectController controller)
        {
            controller.OnUnregister();
            gameObjectControllers.Remove(controller);
        }

        private void Update()
        {
            foreach (var controller in gameObjectControllers)
            {
                controller.Update();
            }
        }

        private void OnGUI()
        {
            foreach (var controller in guiControllers)
            {
                controller.DrawGUI();
            }
        }

        private void OnDrawGizmos()
        {
            foreach (var controller in gizmosControllers)
            {
                controller.DrawGizmos();
            }
        }
    }
}
