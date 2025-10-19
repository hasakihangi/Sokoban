using System.Collections.Generic;
using UnityEngine;

namespace DebugTool
{
    // circle?
    public class DebugCirclesController: DebugController<DebugCircle>, IDebugGizmosController
    {
        public Color defaultColor = Color.red;
        public float plusHeight = 0.05f;
        public int segments = 40;

        public DebugCirclesController(Color? defaultColor)
        {
            if (defaultColor != null)
            {
                this.defaultColor = defaultColor.Value;
            }
        }

        // public int AddCircle(Vector3 center, float radius)
        // {
        //     _circles.Add(new DebugCircle(center, radius, color));
        //     // 但是.. 一旦移除, 其余的index就会变化, 不能作为id
        //     // 需要是一个无限大的数组空间? 并不是, 应该是一个足够动态的, 每个格子有使用标记的, 找到第一个没有使用的进行占用, 如果空间不够就进行移动扩充
        //     // 在大多数情况这样做都是没有意义的
        // }
        //
        // public int AddCircle(DebugCircle circle)
        // {
        //
        // }

        public DebugCircle AddCircle(Vector3 center, float radius)
        {
            DebugCircle circle = new DebugCircle(center, radius, defaultColor);
            _items.Add(circle);
            return circle;
        }


        public void Debug(IEnumerable<(Vector3, float)> circles)
        {
            Clear();
            AddCircles(circles);
        }

        public void AddCircles(IEnumerable<(Vector3, float)> circles)
        {
            foreach (var circle in circles)
            {
                _items.Add(new DebugCircle(circle.Item1, circle.Item2, defaultColor));
            }
        }

        public void DrawGizmos()
        {
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                var circle = _items[i];
                circle.Draw(plusHeight, segments);
            }
        }

        // private void DrawCircle(Vector3 center, float radius)
        // {
        //     float angelStep = 360f / segments;
        //     Vector3 prevPoint = default;
        //
        //     for (int i = 0; i <= segments; i++)
        //     {
        //         float angle = i * angelStep * Mathf.Deg2Rad;
        //         Vector3 point = center + new Vector3(
        //             Mathf.Cos(angle) * radius,
        //             plusHeight,
        //             Mathf.Sin(angle) * radius
        //         );
        //
        //         if (i > 0)
        //         {
        //             Gizmos.DrawLine(prevPoint, point);
        //         }
        //
        //         prevPoint = point;
        //     }
        // }
    }

    // id ?
    // 如果里面不写方法, 就算是struct, 里面也可以写方法
    public class DebugCircle
    {
        public Vector3 center;
        public float radius;
        public Color color;

        public DebugCircle(Vector3 center, float radius, Color color)
        {
            this.center = center;
            this.radius = radius;
            this.color = color;
        }

        public void Draw(float plusHeight, int segments)
        {
            float angelStep = 360f / segments;
            Vector3 prevPoint = default;

            for (int i = 0; i <= segments; i++)
            {
                float angle = i * angelStep * Mathf.Deg2Rad;
                Vector3 point = center + new Vector3(
                    Mathf.Cos(angle) * radius,
                    plusHeight,
                    Mathf.Sin(angle) * radius
                );

                if (i > 0)
                {
                    Gizmos.DrawLine(prevPoint, point);
                }

                prevPoint = point;
            }
        }
    }
}
