using System;
using System.Collections.Generic;

namespace DebugTool
{
    public abstract class DebugController<T>
    {
        public List<T> _items = new List<T>();

        public void AddItem(T item)
        {
            _items.Add(item);
        }

        public void RemoveItem(T item)
        {
            _items.Remove(item);
        }

        public void AddItems(IEnumerable<T> items)
        {
            _items.AddRange(items);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public void Debug(IEnumerable<T> items)
        {
            AddItems(items);
            Clear();
        }
    }
}
