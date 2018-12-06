using UnityEngine;
using System.Collections;

public class SpaceObjectControl : MonoBehaviour
{
	public Movement movement;
	public MotionPath path;
	public bool persistent = false;

	/// <summary>
	/// Raises the collision enter2d event.
	/// </summary>
	/// <param name="coll">Coll.</param>
	void OnCollisionEnter2D (Collision2D coll)
	{
		if (path) {
			path.StopPathing ();
		}
		if ((Global.defaultCrunch) && !persistent) {
			// Don't spawn things in OnDestroy()
			Instantiate (Global.defaultCrunch, transform.position, transform.rotation);
			Destroy (this.gameObject);
		}
	}

	/// <summary>
	/// Raises the collision stay2d event.
	/// </summary>
	/// <param name="coll">Coll.</param>
	/*void OnCollisionStay2D (Collision2D coll)
	{
		if (path) {
			path.StopPathing ();
		}
	}*/

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start ()
	{
		SpaceObjectContainer.AddSpaceObject (transform.gameObject);
	}
	
	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	void OnDestroy ()
	{
		SpaceObjectContainer.RemoveSpaceObject (transform.gameObject);
	}
}
