using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node : ITreeItem<Node>
{
	public Node Parent = null;                   //節點來源
	public bool IsWalkable = false;              //可行走標記
	public Vector3 WorldPosition = Vector3.zero; //對應位置
	public int POP = 0;                          //間隔數
	public int GridX = 0;                        //長
	public int GridY = 0;                        //寬
	public int IndexX = 0;
	public int indexY = 0;
	public int GCost = 0;                        //與起點距離
	public int HCost = 0;                        //與終點距離
	public int FCost {get{return GCost + HCost;}}//G+H
	public int HeapIndex {get;set;}              //對應索引
	
	public Node (bool vIsWalkable, Vector3 vWorldPos, int vIndexX, int vIndexY, int vPop)
	{
		IsWalkable = vIsWalkable;
		WorldPosition = vWorldPos;
		POP = vPop; 
		IndexX = vIndexX;
		indexY = vIndexY;
	}

	public Node (bool vIsWalkable, Block vBlock, int vGridSizeX, int vGridSizeY)
	{
		IsWalkable = vIsWalkable;
		WorldPosition = vBlock.WorldPosition;
		POP = vBlock.POP;
		IndexX = vBlock.IndexX;
		indexY = vBlock.IndexY;
		GridX = vBlock.GridX + (vGridSizeX / 2);
		GridY = vBlock.GridY + (vGridSizeY / 2);
	}
	
	//比較f(n) ,若相同比較公式h(n)結果
	public int CompareTo (Node vNodeToCompare)
	{
		int compare = FCost.CompareTo(vNodeToCompare.FCost);
		
		if (compare == 0)
		{
			compare = HCost.CompareTo(vNodeToCompare.HCost);
		}
		return -compare;
	}
}

public class NodeMgr : MonoBehaviour
{
	private float mNodeDiameter = 0.0f; //節點直徑
	private float mNodeRadius = 0.0f;   //節點半徑
	private int mGridSizeX = 0;         //X軸節點數量
	private int mGridSizeY = 0;         //Y軸節點數量
	private Node[][] mGrid = null;       //節點陣列

	public bool displayGridGizmos;     //是否繪製
	public LayerMask unwalkableMask;   //不可移動範圍
	public Vector2 gridWorldSize;      //建立範圍
	public GetObject mMapScript;

	public int MaxSize{get{return mGridSizeX * mGridSizeY;}}

	//生成
	void Awake ()
	{
//		mNodeDiameter = mNodeRadius * 2;
//		mGridSizeX = Mathf.RoundToInt(gridWorldSize.x / mNodeDiameter);
//		mGridSizeY = Mathf.RoundToInt(gridWorldSize.y / mNodeDiameter);

		mMapScript.InstatiateData();
	}

	//繪製
	void OnDrawGizmos () 
	{
		Gizmos.DrawWireCube(transform.position,new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
		if (mGrid != null && displayGridGizmos)            
		{
			for (int i = 0; i < mGrid.Length; i++)
			{
				foreach (Node n in mGrid[i])
				{
					Gizmos.color = (n.IsWalkable) ? Color.white : Color.red;
					//Gizmos.DrawCube(n.WorldPosition, Vector3.one * (mNodeDiameter - 0.1f));
					//Gizmos.DrawWireSphere(n.WorldPosition, mNodeDiameter - 0.1f);
					Gizmos.DrawSphere(n.WorldPosition, mNodeDiameter);
				}
			}
		}
	}

	//取得垂直鄰近點
	private Node GetVerticleNode (Node vNode, int vRelativeY)
	{
		int vNodeWeight = vNode.IndexX + vNode.POP;
		Node vNeighbor = null;

		if (vNode.indexY + vRelativeY < mGrid.Length && vNode.indexY + vRelativeY > 0)
		{
			foreach (Node n in mGrid[vNode.indexY + vRelativeY])
			{
				if (n.IndexX + n.POP == vNodeWeight)
					vNeighbor = n;
			}
		}
		
		return vNeighbor;
	}

	//取得水平鄰近點
	private Node GetHorizontalNode (Node vNode, int vRelativeX)
	{
		Node vNeighbor = null;

		if (vNode.IndexX + vRelativeX < mGrid[vNode.indexY].Length && vNode.IndexX + vRelativeX > 0)
		{
			Node n = mGrid[vNode.indexY][vNode.IndexX + vRelativeX];

			if (vNode.POP == n.POP)
				vNeighbor = n;
		}

		return vNeighbor;
	}

	//取得斜邊鄰近點
	private Node GetHypotenuseNode (Node vNode, int vRelativeX, int vRelativeY)
	{
		Node vNeighbor = null;
		int vNodeWeight = vNode.IndexX + vNode.POP + vRelativeX;

		if (vNode.indexY + vRelativeY < mGrid.Length && vNode.indexY + vRelativeY > 0)
		{
			foreach (Node n in mGrid[vNode.indexY + vRelativeY])
			{
				if (n.IndexX + n.POP == vNodeWeight)
					vNeighbor = n;
			}
		}

		return vNeighbor;
	}

	//取得鄰近節點
	public List<Node> GetNeighbors (Node vNode)
	{
		List<Node> vNeighborList = new List<Node>();
		Node vNeighborX = null;
		Node vNeighborY = null;
		for (int y = -1; y <= 1; y++)
		{
			if (y == 0)
				vNeighborY = vNode;
			else
			{
				vNeighborY = GetVerticleNode(vNode, y);
				if (vNeighborY != null)
					vNeighborList.Add(vNeighborY);
			}

			for (int x = -1; x <= 1; x++)
			{
				if (x == 0)
					continue;

				if (vNeighborY != null)
					vNeighborX = GetHorizontalNode(vNeighborY, x);
				else
					vNeighborX = GetHypotenuseNode(vNode, x, y);

				if (vNeighborX != null)
					vNeighborList.Add(vNeighborX);
			}
		}
		
//		for (int x = -1; x <= 1; x++)
//		{
//			for (int y = -1; y <= 1; y++)
//			{
//				if (x == 0 && y == 0)
//					continue;
//
//				int vCheckX = vNode.GridX + x;
//				int vCheckY = vNode.GridY + y;
//				vNeighborList.Add(mGrid[vCheckY][vCheckX]);
//				if (vCheckX >= 0 && vCheckX < mGridSizeX && vCheckY >= 0 && vCheckY < mGridSizeY) 
//				{
//					if (vCheckX < mGrid[vCheckY].Length)
//					{
//						if(vCheckX == mGrid[vCheckY][vCheckX].GridX && vCheckY == mGrid[vCheckY][vCheckX].GridY)
//							vNeighborList.Add(mGrid[vCheckY][vCheckX]);
//					}
//				}
//			}
//		}

		return vNeighborList;
	}

	//由位置取得對應節點
	public Node GetNodeFromWorldPoint (Vector3 vWorldPos)
	{
		//避免出界導致無法計算
		float vPercentX = (vWorldPos.x + gridWorldSize.x / 2) / gridWorldSize.x;
		float vPercentY = (vWorldPos.z + gridWorldSize.y / 2) / gridWorldSize.y;
		vPercentX = Mathf.Clamp01(vPercentX);
		vPercentY = Mathf.Clamp01(vPercentY);

		int x = Mathf.RoundToInt((mGridSizeX - 1) * vPercentX);
		int y = Mathf.RoundToInt((mGridSizeY - 1) * vPercentY);

//		int vGridX = Mathf.RoundToInt(vWorldPos.x);
//		int vGridY = Mathf.RoundToInt(vWorldPos.z);
//
//		vGridX += (mGridSizeX / 2);
//		vGridY += (mGridSizeY / 2);

		return mGrid[y][x];
	}

	public void Init_Grid (ref List<float> vXList, ref List<float> vYList)
	{
		int vWorldX = Mathf.RoundToInt(vXList[vXList.Count - 1] - vXList[0]);
		int vWorldY = Mathf.RoundToInt(vYList[vYList.Count - 1] - vYList[0]);
		gridWorldSize = new Vector2(vWorldX, vWorldY);

		mGridSizeX = vXList.Count;
		mGridSizeY = vYList.Count;
		mNodeRadius = vWorldX / mGridSizeX;
		mNodeDiameter = mNodeRadius * 0.1f;
	}

	//生成節點
	public void Init_Node (ref Block[][] vBlockAry)
	{
		mGrid = new Node[vBlockAry.Length][];

		List<Node> vNodeList = new List<Node>();
		for (int i = 0; i < vBlockAry.Length; i++)
		{
			for(int j = 0; j < vBlockAry[i].Length; j++)
			{
				bool vIsWalkable = !(Physics.CheckSphere(vBlockAry[i][j].WorldPosition,mNodeRadius,unwalkableMask));

				vNodeList.Add(new Node(vIsWalkable, vBlockAry[i][j], mGridSizeX, mGridSizeY));
			}

			mGrid[i] = vNodeList.ToArray();
			vNodeList.Clear();
		}


//		mGrid = new Node[mGridSizeX,mGridSizeY];
//		Vector3 vBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y / 2;
//		
//		for (int x = 0; x < mGridSizeX; x++)
//		{
//			for (int y = 0; y < mGridSizeY; y++)
//			{
//				Vector3 vWorldPoint = vBottomLeft + Vector3.right * (x * mNodeDiameter + mNodeRadius) + Vector3.forward * (y * mNodeDiameter + mNodeRadius);
//				bool vIsWalkable = !(Physics.CheckSphere(vWorldPoint,mNodeRadius,unwalkableMask));
//				mGrid[x,y] = new Node(vIsWalkable,vWorldPoint, x,y);
//			}
//		}
	}
}