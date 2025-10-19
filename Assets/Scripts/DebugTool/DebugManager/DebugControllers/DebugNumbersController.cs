using System;
using System.Collections.Generic;
using UnityEngine;

namespace DebugTool
{
    public class DebugNumbersController: DebugController<(Vector2 screenPos, float number)>, IDebugGUIController
    {
        public Camera camera;
        public GUIStyle style;

        public DebugNumbersController(Camera camera, int fontSize = 30)
        {
            this.camera = camera;
            this.style = new GUIStyle()
            {
                fontSize = fontSize
            };
        }

        public void AddNumbers(IEnumerable<(Vector3 worldPos, float number)> numbers)
        {
            foreach (var number in numbers)
            {
                Vector2 screenPos = camera.WorldToScreenPoint(number.worldPos);
                _items.Add((screenPos, number.number));
            }
        }


        public void DrawGUI()
        {
            foreach (var item in _items)
            {
                string text = item.number.ToString();
                Vector2 screenPos = item.screenPos;

                // 转换Unity屏幕坐标到GUI坐标系 (Y轴翻转)
                screenPos.y = Screen.height - screenPos.y;

                // 计算文本大小
                Vector2 textSize = style.CalcSize(new GUIContent(text));

                // 将文本居中对齐到点的位置
                Rect rect = new Rect(
                    screenPos.x - textSize.x / 2,
                    screenPos.y - textSize.y / 2,
                    textSize.x,
                    textSize.y
                );

                GUI.Label(rect, text, style);
            }
        }
    }
}
