using UnityEngine;
using System.Collections;

public class DroneControl : MonoBehaviour
{
	public CircleCollider2D radar;
	public DroneContainer container;
	private Movement movement;
	private GameObject target;
	private GameObject attachment;
	private bool targeted = false;

	void Start ()
	{
		movement = GetComponent<Movement> ();
		Vector3 p = attachment.transform.position - container.transform.position;
		transform.rotation = Quaternion.AngleAxis (Mathf.Atan2 (p.y, p.x) * Mathf.Rad2Deg, Vector3.forward);
		transform.position = attachment.transform.position;
		movement.SetTarget (attachment.transform.position);
	}

	bool MovingAwayFromContainer (GameObject other)
	{
		return Vector3.Dot (container.transform.position - other.transform.position, other.transform.up) <= 0f;
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if ((other.gameObject.tag == "SpaceObject") && !targeted) {
			if ((!MovingAwayFromContainer (other.gameObject)) && 
				((transform.position - other.gameObject.transform.position).sqrMagnitude < (container.transform.position - other.gameObject.transform.position).sqrMagnitude)) {
				Destroy (radar);
				Target (other.gameObject);
				container.RemoveDrone (this.gameObject);
			}
		}
	}

	void OnTriggerStay2D (Collider2D other)
	{
		OnTriggerEnter2D (other);
	}

	public void Target (GameObject newTarget)
	{
		target = newTarget;
		targeted = true;
	}

	public void Spawn (GameObject newAttachment)
	{
		attachment = newAttachment;
	}

	public void Attach (GameObject newAttachment)
	{
		if (!targeted) {
			attachment = newAttachment;
		}
	}

	void FixedUpdate ()
	{
		if (!Global.paused) {
			if (targeted) {
				if ((target) && (movement)) {
					movement.SetTarget (target.transform.position);
				} else {
					Destroy (this.gameObject);
				}
			} else {
				movement.SetTarget (attachment.transform.position);
			}
		}
	}

	void OnCollisionEnter2D (Collision2D coll)
	{
		if (movement) {
			movement.ClearTargets ();
		}
		Destroy (this.gameObject);
	}
}
