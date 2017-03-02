using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GetObject : MonoBehaviour 
{
	private List<float> mPointX;
	private List<float> mPointY;

	private Block[][] mGroundArray = null;
	public TextAsset csv; 
	public NodeMgr NodeMgr;

	public void Read ()
	{
		CSVReader.DebugOutputGrid(CSVReader.SplitCsvGrid(csv.text) ); 
	}

	public void InstatiateData ()
	{
		mPointX = new List<float>(GetXValue());
		mPointY = new List<float>(GetYValue());

		NormalizeData(ref mPointX);
		NormalizeData(ref mPointY);

		mGroundArray = GetGroundAry();
		Debug.Log("GridLength:" + mGroundArray[0][0].GridX);

		int vGridSizeX = Mathf.RoundToInt((mPointX[mPointX.Count - 1] - mPointX[0]) / mPointX.Count);
		int vGridSizeY =  Mathf.RoundToInt((mPointY[mPointY.Count - 1] - mPointY[0]) / mPointY.Count);


		NodeMgr.Init_Grid(ref mPointX, ref mPointY);
		NodeMgr.Init_Node(ref mGroundArray);
	}

	private HashSet<float> GetXValue ()
	{
		HashSet<float> vXSet = new HashSet<float>();

		for (int i = 0; i < transform.childCount; i++)
		{
			vXSet.Add(transform.GetChild(i).localPosition.x);
		}

		Debug.Log("集合數X: " + vXSet.Count);

		return vXSet;
	}

	private HashSet<float> GetYValue ()
	{
		HashSet<float> vYSet = new HashSet<float>();
		
		for (int i = 0; i < transform.childCount; i++)
		{
			vYSet.Add(transform.GetChild(i).localPosition.z);
		}
		
		Debug.Log("集合數Y: " + vYSet.Count);

		return vYSet;
	}

	//取得公差
	private float GetTolerance (ref List<float> vPointAry)
	{
		float vTolerance = 100.0f;
		float vLength = 0;

		for (int i = 0; i < vPointAry.Count - 1; i++)
		{
			vLength = vPointAry[i + 1] - vPointAry[i];

			if (vTolerance > vLength)
				vTolerance = vLength;
		}

		return vTolerance;
	}

	//取得正確長度(等差公式)
	private int GetLength (ref List<float> vPointAry)
	{
		float vTolerance = GetTolerance(ref vPointAry);

		float vCount = (vPointAry[vPointAry.Count - 1] - vPointAry[0]) / vTolerance;
		
		return (int)vCount + 1;
	}

	private Block[][] GetGroundAry ()
	{
		int vBlockCount = transform.childCount;

		BlockTree<Block> mBlockTree = new BlockTree<Block>(vBlockCount);

		for (int i = 0; i < vBlockCount; i++)
		{
			mBlockTree.Add(new Block(transform.GetChild(i).localPosition));
		}
	
		mBlockTree.Sort();

		int vPOP = 0;
		int vXCount = 0;
		int vYCount = 0;
		int vSafeCount = 0;
		List<Block> vBlockX = new List<Block>();
		List<Block[]> vGrid = new List<Block[]>();
		
		while (vSafeCount < mPointY.Count)
		{
			Block vBlock = mBlockTree.GetItem(0);

			if (vBlock.GridY == (int)mPointY[vYCount])
			{
				while (vXCount < mPointX.Count)
				{
					vBlock = mBlockTree.GetItem(0);

					if (vBlock.GridX == (int)mPointX[vXCount])
					{
						vXCount++;
						vBlock.POP = vPOP;
						vBlock.SetIndex(vXCount, vYCount);
						vBlockX.Add(mBlockTree.Remove());
					}
					else if (vBlock.GridX > (int)mPointX[vXCount])
					{
						vXCount++;
						vPOP++;
						continue;
					}
					else
					{
						break;
					}
				}
			}
			else
			{
				vPOP = 0;
				vXCount = 0;
				vYCount++;
				vSafeCount++;
				Block[] ArrayX = vBlockX.ToArray();
				vGrid.Add(ArrayX);
				vBlockX.Clear();
			}
		}
		return vGrid.ToArray();
	}

	private void NormalizeData (ref List<float> vList)
	{
		//公差
		float vTolerance = GetTolerance(ref vList);

		//實際數量
		int vLength = GetLength(ref vList);
		
		if (vLength <= vList.Count)
		{
			vList.Sort();
			return;
		}

		List<float> vLack = new List<float>();
		for (int i = 1; i < vLength; i++)
		{
			if (!vList.Contains(vList[0] + i * vTolerance))
				vLack.Add(vList[0] + i * vTolerance);
		}

		vList.AddRange(vLack);
		vList.Sort();
	}
}
