using System;
using System.Collections.Generic;
using UnityEngine;

namespace DebugTool
{
    public class DebugPointsController: DebugController<Vector3>, IDebugGizmosController
    {
        public Color color;
        public float radius = 0.2f;

        public DebugPointsController(Color color, float? radius)
        {
            this.color = color;
            if (radius != null)
            {
                this.radius = radius.Value;
            }
        }

        public void DrawGizmos()
        {
            Gizmos.color = color;
            foreach (var point in _items)
            {
                Gizmos.DrawWireSphere(point, radius);
            }
        }
    }
}
