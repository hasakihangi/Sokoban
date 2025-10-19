using System;
using System.Collections.Generic;
using UnityEngine;

namespace DebugTool
{
    public class DebugGetPointsController: DebugController<DebugGetPoint>, IDebugGizmosController
    {
        public Color defaultColor = Color.cyan;
        public float defaultRadius = 0.2f;

        public DebugGetPointsController(Color? defaultColor, float? defaultRadius)
        {
            if (defaultColor != null)
            {
                this.defaultColor = defaultColor.Value;
            }

            if (defaultRadius != null)
            {
                this.defaultRadius = defaultRadius.Value;
            }
        }

        public DebugGetPoint AddPoint(Func<Vector3> getPos, Color? color, float? radius)
        {
            Color c = color ?? defaultColor;
            float r = radius ?? defaultRadius;
            DebugGetPoint getPoint = new DebugGetPoint(getPos, c, r);
            _items.Add(getPoint);
            return getPoint;
        }


        public void DrawGizmos()
        {
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                var point = _items[i];
                point.Draw();
            }
        }
    }

    // 如果是class, 将地址作为id
    // 如果是struct, id是数组的index
    public class DebugGetPoint
    {
        public Func<Vector3> getPos;
        public Color color;
        public float radius;

        public DebugGetPoint(Func<Vector3> getPos, Color color, float radius)
        {
            this.getPos = getPos;
            this.color = color;
            this.radius = radius;
        }

        public void Draw()
        {
            if (getPos != null)
            {
                var pos = getPos.Invoke();
                Gizmos.color = color;
                Gizmos.DrawWireSphere(pos, radius);
            }
        }
    }
}
