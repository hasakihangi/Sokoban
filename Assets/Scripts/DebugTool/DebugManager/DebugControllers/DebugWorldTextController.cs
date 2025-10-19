using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DebugTool
{
    public class DebugWorldTextController : IDebugGameObjectController
    {
        public Transform root;
        public List<DebugWorldText> _items = new List<DebugWorldText>();

        public Vector3 direction = Vector3.down;
        public Color color;
        public float fontSize = 2.5f;


        public DebugWorldTextController(Color color)
        {
            this.color = color;
        }

        public DebugWorldText AddText(string content, Vector3 position)
        {
            GameObject obj = new GameObject("DebugText");
            obj.transform.SetParent(root);
            obj.transform.position = position;
            obj.transform.rotation = Quaternion.LookRotation(direction);

            TextMeshPro textComponent = obj.AddComponent<TextMeshPro>();
            textComponent.text = content;
            textComponent.fontSize = fontSize;
            textComponent.color = color;
            textComponent.alignment = TextAlignmentOptions.Center;

            DebugWorldText item = new DebugWorldText(textComponent, position);
            _items.Add(item);
            return item;
        }

        public void RemoveText(DebugWorldText item)
        {
            Object.Destroy(item.textMesh.gameObject);
            _items.Remove(item);
        }

        public void AddTexts(IEnumerable<(string content, Vector3 position)> texts)
        {
            foreach (var text in texts)
            {
                AddText(text.content, text.position);
            }
        }

        public void Clear()
        {
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                var item = _items[i];
                Object.Destroy(item.textMesh.gameObject);
                _items.RemoveAt(i);
            }
        }

        public void OnRegister()
        {
            root = new GameObject(nameof(DebugWorldTextController) + " Root").transform;
            Object.DontDestroyOnLoad(root.gameObject);
        }

        public void OnUnregister()
        {
            Object.Destroy(root.gameObject);
        }

        public void Update()
        {

        }
    }

    public class DebugWorldText
    {
        public TextMeshPro textMesh;
        public Vector3 worldPos;

        public DebugWorldText(TextMeshPro textMesh, Vector3 worldPos)
        {
            this.textMesh = textMesh;
            this.worldPos = worldPos;
        }
    }
}
