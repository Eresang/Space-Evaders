using UnityEngine;
using System.Collections;

public class BlackHoleEventHorizonControl : MonoBehaviour
{
	private float rangeSqr;
	public float strength = 2f;

	void Start ()
	{
		rangeSqr = GetComponent<CircleCollider2D> ().radius;
		rangeSqr *= rangeSqr;
	}

	void OnTriggerStay2D (Collider2D other)
	{
		if (other.gameObject.tag == "Spaceship" || other.gameObject.tag == "SpaceObject") {
			GameObject p = other.gameObject.transform.parent.gameObject;

			Vector3 d = transform.position - p.transform.position;
			// Blegh Pow eww
			float a = Mathf.Pow (d.sqrMagnitude / rangeSqr, 0.25f);
			d = (p.transform.position - Vector3.Lerp (transform.position, p.transform.position, a)) * Time.fixedDeltaTime * strength;

			p.transform.position -= d;

			ShipControl s = p.GetComponent<ShipControl> ();
			if ((s) && (s.path)) {
				s.path.StopPathing ();
			}
		}
	}

	void OnTriggerEnter2d (Collider2D other)
	{
		OnTriggerStay2D (other);
	}
}
