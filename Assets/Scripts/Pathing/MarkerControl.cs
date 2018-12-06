using UnityEngine;
using System.Collections;

public class MarkerControl : MonoBehaviour
{
	private GameObject ship;

	public void SetOwnerShip (GameObject owner)
	{
		ship = owner;
		transform.parent = Global.currentMarkerContainer.transform;
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject == ship)
			Destroy (this.gameObject);
	}
}
