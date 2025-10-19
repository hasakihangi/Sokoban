using System;
using System.Collections.Generic;

public class Heap<T> where T : struct, IComparable<T>
{
	private List<T> heap;
	private Func<T, T, bool> compare;

	public int Count => heap.Count;

	// 建立空堆
	public Heap(bool isMinHeap = true)
	{
		heap = new List<T>();
		DetermineCompare(isMinHeap);
	}

	// 通过已有的List建立堆
	public Heap(List<T> list, bool isMinHeap = true)
	{
		heap = list;
		DetermineCompare(isMinHeap);
		BuildHeap();
	}

	private void BuildHeap()
	{
		for (int i = heap.Count / 2 - 1; i >= 0; i--)
		{
			HeapifyDown(i);
		}
	}

	// 通过确定比较函数, 定义最小堆或最大堆
	private void DetermineCompare(bool isMaxHeap)
	{
		if (!isMaxHeap)
		{
			compare = (a, b) => a.CompareTo(b) < 0;
		}
		else
		{
			compare = (a, b) => a.CompareTo(b) > 0;
		}
	}

	public T PullRoot()
	{
		if (heap.Count == 0)
			return default(T);

		T rootValue = heap[0];
		heap[0] = heap[heap.Count - 1];
		// heap[0] = heap[^1];
		heap.RemoveAt(heap.Count - 1);
		HeapifyDown(0);

		return rootValue;
	}

	public bool PullRoot(out T value)
	{
		value = default(T);
		if (heap.Count == 0)
			return false;

		T rootValue = heap[0];
		heap[0] = heap[heap.Count - 1];
		// heap[0] = heap[^1];
		heap.RemoveAt(heap.Count - 1);
		HeapifyDown(0);

		return true;
	}

	public T PeekRoot()
	{
		if (heap.Count == 0)
			return default(T);

		return heap[0];
	}

	public bool PeekRoot(out T value)
	{
		value = default(T);
		if (heap.Count == 0)
			return false;
		value = heap[0];
		return true;
	}

	public void Insert(T value)
	{
		heap.Add(value);
		HeapifyUp(heap.Count - 1);
	}

	// 将索引为 index 的元素往上边送, 因为插入元素在最后, 所以一般 index=heap.Count - 1
	void HeapifyUp(int index)
	{
		while (index > 0)
		{
			int parentIndex = (index - 1) / 2;

			// 当父节点的值 < 子节点的值, 表明该子节点位置正确
			if (compare(heap[parentIndex], heap[index]))
				break;

			Swap(index, parentIndex);
			index = parentIndex;
		}
	}

	// 将索引为 index 的元素往下边送
	// 取出最大值后, 将末尾元素放到根位置后, 末尾元素, 这种情况下 index=0
	void HeapifyDown(int index)
	{
		while (index < heap.Count)
		{
			int leftChildIndex = 2 * index + 1;
			int rightChildIndex = 2 * index + 2;
			int targetIndex = index;

			if (leftChildIndex < heap.Count && compare(heap[leftChildIndex], heap[targetIndex]))
				targetIndex = leftChildIndex;

			if (rightChildIndex < heap.Count && compare(heap[rightChildIndex], heap[targetIndex]))
				targetIndex = rightChildIndex;

			if (targetIndex == index)
				break;

			Swap(index, targetIndex);
			index = targetIndex;
		}
	}

	private void Swap(int index_1, int index_2)
	{
		(heap[index_1], heap[index_2]) = (heap[index_2], heap[index_1]);
	}

	public void Clear()
	{
		heap.Clear();
	}
}
