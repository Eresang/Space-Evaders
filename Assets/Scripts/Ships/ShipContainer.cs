using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipContainer : TouchInput
{
	public bool touchStartSelect = true;
	public GameObject markerPrefab;
	public float tapTime;
	
	private float tapTimer = -1f;

	private GameObject selection;
	private GameObject marker;
	private int layerMask;
	private Movement movement;
	private Vector3 lastPosition;

	public static GameObject lastContainer;
	private static LinkedList<GameObject> spaceships = new LinkedList<GameObject> ();
	private bool resetOnNext = false;

	public void ResetSelection ()
	{
		selection = null;
		movement = null;
	}

	/// <summary>
	/// Adds the space ship.
	/// </summary>
	/// <param name="newSpaceship">New spaceship.</param>
	public static void AddSpaceShip (GameObject newSpaceship)
	{
		spaceships.AddFirst (newSpaceship);
	}

	/// <summary>
	/// Gets the space ship.
	/// </summary>
	/// <returns>The space ship.</returns>
	/// <param name="index">Index.</param>
	public static GameObject GetSpaceShip (int index)
	{
		if (spaceships.Count > 0 && spaceships.Count > index) {
			LinkedListNode<GameObject> ship = spaceships.Last;
			for (int i = 0; i < index; i++) {
				ship = ship.Previous;
			}
			return ship.Value;
		} else {
			return null;
		}
	}

	public static GameObject GetMainShip ()
	{
		LinkedListNode<GameObject> ship = spaceships.Last;
		while ((ship != null) && (!ship.Value.name.Contains("Mothership"))) {
			ship = ship.Previous;
		}
		if (ship == null) {
			return null;
		} else {
			return ship.Value;
		}
	}

	/// <summary>
	/// Gets the spaceship count.
	/// </summary>
	/// <returns>The spaceship count.tapTimerurns>
	public static int GetSpaceshipCount ()
	{
		return spaceships.Count;
	}

	/// <summary>
	/// Removes the space shitapTimer// </summary>
	/// <returns>The space ship.</returns>
	/// <param name="spatapTimerp">SpaceshtapTimerparam>
	public static int RemoveSpaceShip (GameObject spaceship)
	{
		spaceships.Remove (spaceship);
		return spaceships.Count;
	}

	// -----------------------------------------------------------------------------------------------------------------
	// -----------------------------------------------------------------------------------------------------------------
	// -----------------------------------------------------------------------------------------------------------------

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start ()
	{
		lastContainer = this.gameObject;
		layerMask = (1 << LayerMask.NameToLayer ("UI"));
	}

	public override bool AskDelayTouch (Vector3 position)
	{
		RaycastHit2D hit = Physics2D.Raycast (position, Vector3.forward, Mathf.Infinity, layerMask);
		return !(hit.collider && ((hit.collider.gameObject.tag == "Spaceship") || (hit.collider.gameObject.tag == "Satellite")) && (!Global.paused));
	}

	public override void OnDoubleTap (Vector3 position)
	{
		// Find the first object under the touch position
		RaycastHit2D hit = Physics2D.Raycast (position, Vector3.forward, Mathf.Infinity, layerMask);

		if (hit.collider && (!Global.paused)) {
			if (hit.collider.gameObject.tag == "Spaceship") {
				Movement m = hit.collider.gameObject.GetComponent<Movement> ();
				if (m) {
					m.StartSpeedBoost ();
				}
			} else if (hit.collider.gameObject.tag == "Satellite") {
				ScrapControl c = hit.collider.gameObject.GetComponent<ScrapControl> ();
				if (c) {
					c.Instruct ();
				}
			}
		}
	}

	void Select (GameObject toSelect)
	{
		if (toSelect) {
			movement = toSelect.GetComponent<Movement> ();
			if (movement) {
				if (movement.selectableType == SelectableType.MultiTarget) {
					resetOnNext = true;
					MotionPath path = movement.gameObject.GetComponentInChildren<MotionPath> ();
					if (path) {
						path.RemoveMarkers ();
						//path.UpdateTarget ();
					}
				}
				selection = toSelect.gameObject;
			}
		}
	}

	void OnDelayedTap ()
	{
		RaycastHit2D hit = Physics2D.Raycast (lastPosition, Vector3.forward, Mathf.Infinity, layerMask);
		if (hit.collider && ((hit.collider.gameObject.tag == "Spaceship") || (hit.collider.gameObject.tag == "Satellite")) && (!Global.paused)) {
			Select (selection);
		}
	}

	/// <summary>
	/// Raises the touch event.
	/// </summary>
	/// <param name="position">Position.</param>
	public override void OnTap (Vector3 position)
	{
		lastPosition = position;

		// Find the first object under the touch position
		RaycastHit2D hit = Physics2D.Raycast (position, Vector3.forward, Mathf.Infinity, layerMask);
			
		if (hit.collider && ((hit.collider.gameObject.tag == "Spaceship") || (hit.collider.gameObject.tag == "Satellite")) && (!Global.paused)) {
			if (tapTimer < 0f) {
				if (hit.collider.gameObject != selection) {
					Select (hit.collider.gameObject);
				}
				tapTimer = tapTime;
			} else if (hit.collider.gameObject == selection) {
				OnDoubleTap (position);
				tapTimer = -1f;
			}
		} else if ((movement) && (movement.gameObject.tag == "Spaceship") && (!Global.paused)) {
			// movement variable still set from a previous touch
			if (resetOnNext) {
				movement.ClearTargets ();
				resetOnNext = false;
			}
			movement.AddTarget (new Vector3 (position.x, position.y, 0f));
			MotionPath path = movement.gameObject.GetComponentInChildren<MotionPath> ();
			if (path)
				path.UpdateTarget ();
		}
	}

	/// <summary>
	/// Raises the touch start event.
	/// </summary>
	/// <param name="start">Start.</param>
	/*public override void OnTouchStart (Vector3 start)
	{
		if ((touchStartSelect) && (!Global.paused)) {
			// Find the first object under the touch position
			RaycastHit2D hit = Physics2D.Raycast (start, Vector3.forward, Mathf.Infinity, layerMask);
		
			if (hit.collider && hit.collider.gameObject.tag == "Spaceship") {
				selection = hit.collider.gameObject;
				movement = selection.GetComponent<Movement> ();
			}
		}
	}

	/// <summary>
	/// Raises the touch end event.
	/// </summary>
	/// <param name="start">Start.</param>
	/// <param name="end">End.</param>
	public override void OnTouchEnd (Vector3 start, Vector3 end)
	{
		if ((!touchStartSelect) && (!Global.paused)) {
			OnTouchStart (start);
		}
		if ((selection) && !Global.paused) {
			Vector3 target = new Vector3 (end.x, end.y, 0f);
			movement = selection.GetComponent<Movement> ();
			if (movement)
				movement.SetTarget (target);
			MotionPath path = movement.gameObject.GetComponentInChildren<MotionPath> ();
			if (path)
				path.UpdateTarget ();
		}
		movement = null;
		selection = null;
	}*/

	void FixedUpdate ()
	{
		if (tapTimer >= 0f && tapTimer - Time.fixedDeltaTime < 0f) {
			OnDelayedTap ();
		}
		tapTimer -= Time.fixedDeltaTime;
	}

	void LateUpdate ()
	{
		if ((movement) && (selection) && (selection.tag == "Spaceship")) {
			if (!(marker) && (markerPrefab)) {
				marker = Instantiate (markerPrefab) as GameObject;
			}
			if (marker)
				marker.transform.position = new Vector3 (selection.transform.position.x, selection.transform.position.y, marker.transform.position.z);
		} else {
			Destroy (marker);
		}
	}
}
