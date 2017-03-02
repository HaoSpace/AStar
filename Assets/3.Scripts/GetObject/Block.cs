using UnityEngine;
using System.Collections;

public class Block : IBlockItem<Block>
{
	public Vector3 WorldPosition = Vector3.zero; //對應位置
	public int GridX = 0;                        //長
	public int GridY = 0;                        //寬
	public int IndexX = 0;                       //陣列索引
	public int IndexY = 0;                       //陣列索引
	public int POP = 0;
	public int HeapIndex {get;set;}              //對應索引
	
	public Block(Vector3 vWorldPos)
	{
		WorldPosition = vWorldPos;
		GridX = Mathf.RoundToInt(vWorldPos.x);
		GridY = Mathf.RoundToInt(vWorldPos.z);
	}

	public void SetIndex (int vIndexX, int vIndexY)
	{
		IndexX = vIndexX - POP - 1;
		IndexY = vIndexY;

	}
	
	//比較f(n) ,若相同比較公式h(n)結果
	public int CompareTo(Block vNodeToCompare)
	{
		int compare = GridY.CompareTo(vNodeToCompare.GridY);
		
		if (compare == 0)
		{
			compare = GridX.CompareTo(vNodeToCompare.GridX);
		}
		return -compare;
	}
}
