using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

public class Seeker : MonoBehaviour
{
	private float mSpeed = 20;
	private Vector3[] mPath = null;
	private int mMoveIndex = 0;

	public NodeMgr mNodeMgr;

	public void SetTarget(Vector3 vTargetPos)
	{
		StartFindPath(transform.position,vTargetPos);
	}

	//10:鄰近權值 14:斜邊權值
	private int GetDistance (Node vNodeA, Node vNodeB)
	{
		int vDistX = Mathf.Abs(vNodeA.GridX - vNodeB.GridX);
		int vDistY = Mathf.Abs(vNodeA.GridY - vNodeB.GridY);
		
		if (vDistX > vDistY)
			return 14 * vDistY + 10 * (vDistX-vDistY);
		return 14 * vDistX + 10 * (vDistY-vDistX);
	}

	//開始尋找路線
	public void StartFindPath (Vector3 vStartPos, Vector3 vTargetPos)
	{
		Stopwatch sw = new Stopwatch();
		sw.Start();
		
		Vector3[] vWaypoints = new Vector3[0];
		bool vIsSuccess = false;
		
		Node vStartNode = mNodeMgr.GetNodeFromWorldPoint(vStartPos);
		Node vTargetNode = mNodeMgr.GetNodeFromWorldPoint(vTargetPos);
		
		if (vStartNode.IsWalkable && vTargetNode.IsWalkable)
		{
			Tree<Node> vOpenSet = new Tree<Node>(mNodeMgr.MaxSize);
			HashSet<Node> vClosedSet = new HashSet<Node>();
			vOpenSet.Add(vStartNode);
			
			while (vOpenSet.Count > 0) 
			{
				Node vCurrentNode = vOpenSet.RemoveFirst();
				vClosedSet.Add(vCurrentNode);
				
				if (vCurrentNode == vTargetNode) 
				{
					sw.Stop();
					print ("Path found: " + sw.ElapsedMilliseconds + " ms");
					vIsSuccess = true;
					break;
				}
				
				foreach (Node vNeighbor in mNodeMgr.GetNeighbors(vCurrentNode))
				{
					if (!vNeighbor.IsWalkable || vClosedSet.Contains(vNeighbor))
					{
						continue;
					}
					
					int vToNeighborCost = vCurrentNode.GCost + GetDistance(vCurrentNode, vNeighbor);
					if (vToNeighborCost < vNeighbor.GCost || !vOpenSet.Contains(vNeighbor))
					{
						vNeighbor.GCost = vToNeighborCost;
						vNeighbor.HCost = GetDistance(vNeighbor, vTargetNode);
						vNeighbor.Parent = vCurrentNode;
						
						if (!vOpenSet.Contains(vNeighbor))
							vOpenSet.Add(vNeighbor);
					}
				}
			}
		}
		
		if (vIsSuccess)
			vWaypoints = RetracePath(vStartNode,vTargetNode);

		OnPathFound(vWaypoints,vIsSuccess);
	}
	
	//依據關聯串連起來
	private Vector3[] RetracePath (Node vStartNode, Node vEndNode)
	{
		List<Node> path = new List<Node>();
		Node vCurrentNode = vEndNode;
		
		while (vCurrentNode != vStartNode)
		{
			path.Add(vCurrentNode);
			vCurrentNode = vCurrentNode.Parent;
		}
		
		Vector3[] vWaypoints = SimplifyPath(path);
		Array.Reverse(vWaypoints);
		return vWaypoints;
	}
	
	//刪除多餘節點
	private Vector3[] SimplifyPath (List<Node> vPath)
	{
		List<Vector3> vWaypoints = new List<Vector3>();
		Vector2 vDirectOld = Vector2.zero;
		
		for (int i = 1; i < vPath.Count; i ++)
		{
			Vector2 vDirectNew = new Vector2(vPath[i-1].GridX - vPath[i].GridX,vPath[i-1].GridY - vPath[i].GridY);
			
			if (vDirectNew != vDirectOld)
			{
				vWaypoints.Add(vPath[i].WorldPosition);
			}
			vDirectOld = vDirectNew;
		}
		return vWaypoints.ToArray();
	}


	//路線確定
	public void OnPathFound (Vector3[] vNewPath, bool vIsSuccess)
	{
		if (vIsSuccess)
		{
			mPath = vNewPath;
			mMoveIndex = 0;
			StopCoroutine("IFollowPath");
			StartCoroutine("IFollowPath");
		}
	}

	IEnumerator IFollowPath ()
	{
		Vector3 vCurrentPos = mPath[0];

		while (true)
		{
			if (transform.position == vCurrentPos)
			{
				mMoveIndex++;
				if (mMoveIndex >= mPath.Length)
				{
					yield break;
				}
				vCurrentPos = mPath[mMoveIndex];
			}

			transform.position = Vector3.MoveTowards(transform.position,vCurrentPos,mSpeed * Time.deltaTime);
			yield return null;
		}
	}
	//劃出路線
	public void OnDrawGizmos()
	{
		if (mPath != null) 
		{
			for (int i = mMoveIndex; i < mPath.Length; i ++)
			{
				Gizmos.color = Color.black;
				Gizmos.DrawCube(mPath[i], Vector3.one);

				if (i == mMoveIndex)
				{
					Gizmos.DrawLine(transform.position, mPath[i]);
				}
				else
				{
					Gizmos.DrawLine(mPath[i-1],mPath[i]);
				}
			}
		}
	}
}
