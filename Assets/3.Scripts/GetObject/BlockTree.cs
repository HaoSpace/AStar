using UnityEngine;
using System.Collections;
using System;

public interface IBlockItem<T> : IComparable<T>
{
	int HeapIndex {get;set;} //紀錄順序
}

public class BlockTree<T> where T : IBlockItem<T>
{
	private T[] mItems = null;
	private int mCurrentItemCount = 0;
	private int mDeviation = 0;                //索引偏差直
	
	public int Count{get{return mCurrentItemCount;}}
	
	public BlockTree(int vHeapSize)
	{
		mItems = new T[vHeapSize];
	}
	
	//新增節尾
	public void Add (T item)
	{
		item.HeapIndex = mCurrentItemCount;
		mItems[mCurrentItemCount] = item;
		mCurrentItemCount++;
	}
	
	//減去開頭
	public T Remove ()
	{
		T vFirstItem;

		if (mDeviation > mItems.Length - 1)
		{
			Debug.Log("ArrayIndexOutOfRange => Size: " + mItems.Length);
			vFirstItem = mItems[0 + mItems.Length - 1];
			return vFirstItem;
		}

		vFirstItem = mItems[0 + mDeviation];
		mDeviation++;

		return vFirstItem;
	}

	public T GetItem (int vIndex)
	{
		if(vIndex + mDeviation > mItems.Length - 1)
			return mItems[0];

		return mItems[vIndex + mDeviation];
	}
	
	//是否有該項目
	public bool Contains (T item)
	{
		return Equals(mItems[item.HeapIndex + mDeviation], item);
	}

	public void Sort ()
	{
		quickSort(0, mCurrentItemCount - 1);
	}

	private void quickSort(int left, int right) 
	{
		if (left < right)
		{
			int pivot = partition(left, right);
			quickSort(left, pivot - 1);
			quickSort(pivot + 1, right);
		}
	}

	private int partition (int left, int right) {
		int i = left - 1;
		int j = right;
		T pivot = mItems[right];       
		
		while (true)
		{
			while (mItems[++i].CompareTo(pivot) > 0)   
				if (i == right)
					break;
			while (mItems[--j].CompareTo(pivot) < 0)    
				if (j == left)
					break;
			if (i >= j)        		
				break;
			Swap(mItems[i], mItems[j]);        
		}
		Swap(mItems[i], mItems[right]);       
		return i;
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