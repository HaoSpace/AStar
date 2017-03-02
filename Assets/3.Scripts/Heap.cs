using UnityEngine;
using System.Collections;
using System;

public interface ITreeItem<T> : IComparable<T>
{
	int HeapIndex {get;set;} //紀錄順序
}

//實作排序,降低運算次數
public class Tree<T> where T : ITreeItem<T>
{
	private T[] mItems = null;
	private int mCurrentItemCount = 0;

	public int Count{get{return mCurrentItemCount;}}
	
	public Tree(int vHeapSize)
	{
		mItems = new T[vHeapSize];
	}

	//新增節尾
	public void Add (T item)
	{
		item.HeapIndex = mCurrentItemCount;
		mItems[mCurrentItemCount] = item;
		//向上排序
		SortUp(item);
		mCurrentItemCount++;
	}

	//減去開頭
	public T RemoveFirst ()
	{
		T firstItem = mItems[0];
		mCurrentItemCount--;
		mItems[0] = mItems[mCurrentItemCount];
		mItems[0].HeapIndex = 0;
		//向下排序
		SortDown(mItems[0]);
		return firstItem;
	}

	public void UpdateItem (T item)
	{
		SortUp(item);
	}

	//是否有該項目
	public bool Contains(T item)
	{
		return Equals(mItems[item.HeapIndex], item);
	}

	//往後排列
	private void SortDown (T item)
	{
		while (true)
		{
			int childIndexLeft = item.HeapIndex * 2 + 1;
			int childIndexRight = item.HeapIndex * 2 + 2;
			int swapIndex = 0;

			//檢查是否節尾
			if (childIndexLeft < mCurrentItemCount)
			{
				swapIndex = childIndexLeft;

				//子節點相互比較
				if (childIndexRight < mCurrentItemCount)
				{
					if (mItems[childIndexLeft].CompareTo(mItems[childIndexRight]) < 0)
						swapIndex = childIndexRight;
				}
				//比較子節點
				if (item.CompareTo(mItems[swapIndex]) < 0)
				{
					Swap (item,mItems[swapIndex]);
				}
				else
					return;
			}
			else
				return;
		}
	}

	//往前排列
	private void SortUp (T item)
	{
		int parentIndex = (item.HeapIndex - 1) / 2;

		while (true)
		{
			T parentItem = mItems[parentIndex];
			//比較父節點
			if (item.CompareTo(parentItem) > 0)
				Swap (item,parentItem);
			else
				break;

			parentIndex = (item.HeapIndex - 1) / 2;
		}
	}

	//交換
	private void Swap (T itemA, T itemB)
	{
		mItems[itemA.HeapIndex] = itemB;
		mItems[itemB.HeapIndex] = itemA;
		int itemAIndex = itemA.HeapIndex;
		itemA.HeapIndex = itemB.HeapIndex;
		itemB.HeapIndex = itemAIndex;
	}
}