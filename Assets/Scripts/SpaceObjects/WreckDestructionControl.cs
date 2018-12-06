using UnityEngine;
using System.Collections;

public class WreckDestructionControl : MonoBehaviour
{
	public float distanceToCenter;
	// Update is called once per frame
	void Update ()
	{
		if (transform.position.sqrMagnitude < distanceToCenter) {
			SelfDestruct s = GetComponent<SelfDestruct> ();
			if (s)
				s.DestroyAttached ();
		}
	}
}
