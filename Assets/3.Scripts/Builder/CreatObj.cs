using UnityEngine;
using System.Collections;

public class CreatObj : MonoBehaviour
{
	public Vector2 mSize = Vector2.one;

//	void Reset ()
//	{
//		CreateBlock();
//	}

	public void CreateBlock ()
	{
		for (int x = -50; x < 50; x += (int)(100/mSize.x))
		{
			for (int y = -50; y < 50; y += (int)(100/mSize.y))
			{
				GameObject vObj = new GameObject("Block");
				vObj.transform.parent = transform;
				vObj.transform.localPosition = new Vector3(x, 0, y);
				BoxCollider vCollider = vObj.AddComponent<BoxCollider>();
				vCollider.size = new Vector3(100/mSize.x, 1, 100/mSize.y);
			}
		}
	}

	public void ClearAll ()
	{
		GameObject vObj = new GameObject("BlockGroup");
		vObj.AddComponent<CreatObj>();
		vObj.AddComponent<GetObject>();
		GameObject.DestroyImmediate(transform.gameObject);
	}
}
