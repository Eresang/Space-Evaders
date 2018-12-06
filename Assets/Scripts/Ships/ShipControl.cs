using UnityEngine;
using System.Collections;

public enum ShipType: int
{
	Mothership = 1,
	Pigeon = 2,
	UFO = 3
}

public class ShipControl : MonoBehaviour
{
	public Movement movement;
	public MotionPath path;
	public bool criticalShip;
	public ShipType shipType;
	public ShipShopOptions shipShopOptions;

	public void Reset ()
	{
		if (movement) {
			movement.ClearTargets ();
		}
		if (path) {
			path.UpdateTarget ();
		}
		DroneContainer d = GetComponentInChildren<DroneContainer> ();
		if (d) {
			d.ReReposition ();
		}
	}

	/// <summary>
	/// Raises the collision enter2d event.
	/// </summary>
	/// <param name="coll">Coll.</param>
	void OnCollisionEnter2D (Collision2D coll)
	{
		if (path) {
			path.StopPathing ();
		}
		if (movement) {
			movement.ClearTargets ();
		}
		if (Global.defaultExplosion) {
			// Lesson learned, don't spawn things in OnDestroy()
			Instantiate (Global.defaultExplosion, transform.position, transform.rotation);
		}
		Destroy (this.gameObject);
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
		if (movement) {
			movement.ClearTargets ();
		}
	}*/

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Awake ()
	{
		ShipContainer.AddSpaceShip (transform.gameObject);
		if (ShipContainer.lastContainer) {
			transform.parent = ShipContainer.lastContainer.transform;
		}
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	void OnDestroy ()
	{
		ShipContainer.RemoveSpaceShip (this.gameObject);
		if (criticalShip) {
			Global.currentGlobalControl.GameOver (0.5f);
		}
	}
}
