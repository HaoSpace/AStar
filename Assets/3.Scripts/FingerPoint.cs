using UnityEngine;
using System.Collections;

public class FingerPoint : MonoBehaviour
{
	public Seeker PathScript;

	void Awake ()
	{
		BoxCollider vCollider = gameObject.AddComponent<BoxCollider> ();

		vCollider.size = 10 * Vector3.one;
	}

	void OnMouseDown ()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		Physics.Raycast(ray, out hit);
		
		if(hit.collider.gameObject == gameObject)
		{
			Vector3 newTarget = hit.point + new Vector3(0, 0.5f, 0);
			PathScript.SetTarget(newTarget);
		}
	}
}