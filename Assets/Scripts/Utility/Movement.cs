using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MovementType
{
	InstantRotation,
	RotateBeforeMoving,
	MoveWhileRotating,
	RotateWhileMoving
}

public enum SelectableType
{
	Swipe,
	MultiTarget
}

public class Movement : MonoBehaviour
{
	// Editor settings
	public MovementType movementType;
	public bool stopOnTarget = false;
	public SelectableType selectableType;

	[Range(1, 8)]
	public int
		maxTargets = 1;
	public float speed;
	public float rotationSpeed; // radian angular speed

	[Range(1, 20)]
	public int
		speedBoostMultiplier = 1;
	public float speedBoostDuration;

	private float speedBoostTime;

	// Hidden variables
	private LinkedList<Vector3> targets = new LinkedList<Vector3> ();
	private Vector3 target;
	private bool targetReached = true;

	// Only used by motion path, copying everything isn't necessairy
	public void CopyFrom (Movement m)
	{
		movementType = m.movementType;
		stopOnTarget = m.stopOnTarget;
		selectableType = m.selectableType;
		maxTargets = m.maxTargets;
		speed = m.speed;
		rotationSpeed = m.rotationSpeed;
		speedBoostMultiplier = m.speedBoostMultiplier;
		speedBoostDuration = m.speedBoostDuration;

		speedBoostTime = m.SpeedBoostTime ();
		targets.Clear ();
		LinkedListNode<Vector3> t = m.Targets ().First;
		while (t != null) {
			targets.AddLast (t.Value);
			t = t.Next;
		}
		target = m.Target ();
		targetReached = m.TargetReached ();
	}

	public float SpeedBoostTime ()
	{
		return speedBoostTime;
	}

	public LinkedList<Vector3> Targets ()
	{
		return targets;
	}

	public Vector3 Target ()
	{
		return target;
	}
	

	void Start ()
	{
		if (!stopOnTarget) {
			target = transform.position + (transform.up * Screen.GetDiameter ());
		} else {
			target = transform.position;
		}
		targetReached = true;
	}

	void OnDestroy ()
	{
		ClearTargets ();
	}

	// Set a new target to fly to
	public void SetTarget (Vector3 newTarget)
	{
		ClearTargets ();
		targets.AddLast (newTarget);
		target = newTarget;

		targetReached = false;
	}

	public void ClearTargets ()
	{
		if (!stopOnTarget) {
			target = transform.position + (transform.up * Screen.GetDiameter ());
		} else {
			target = transform.position;
		}
		targets.Clear ();
		targetReached = stopOnTarget;
	}

	public Vector3 LastTarget ()
	{
		return targets.Last != null ? targets.Last.Value : this.gameObject.transform.position;
	}
	
	public void AddTarget (Vector3 newTarget)
	{
		if (targets.Count >= maxTargets) {
			ClearTargets ();
		}

		// Retrieve last position to compare to
		LinkedListNode<Vector3> last = targets.Last;
		Vector3 lastP = Vector3.zero;
		if (last == null) {
			lastP = transform.position;
		} else {
			last = last.Previous;
			if (last == null) {
				lastP = targets.Last.Value;
			} else {
				lastP = last.Value;
			}
		}
		// Don't do anything with zero magnitude in the difference
		if (lastP != newTarget) {
			// If magnitude in difference < a certain value, move newTarget to minimum distance from last position
			if ((lastP - newTarget).magnitude < Screen.screenDragThresholdSqr) {
				lastP = lastP - ((lastP - newTarget).normalized * Screen.screenDragThresholdSqr);
			} else {
				lastP = newTarget;
			}

			// Add target
			if (selectableType == SelectableType.MultiTarget) {
				targets.AddLast (lastP);
				if (targets.Count == 1) {
					SetTarget (lastP);
				}
			} else {
				SetTarget (lastP);
			}
		}
	}

	/// <summary>
	/// Starts the speed boost.
	/// </summary>
	public void StartSpeedBoost ()
	{
		speedBoostTime = Time.time + speedBoostDuration;
	}

	/// <summary>
	/// Determines whether this instance is speed boost active.
	/// </summary>
	/// <returns><c>true</c> if this instance is speed boost active; otherwise, <c>false</c>.</returns>
	public bool IsSpeedBoostActive ()
	{
		return Time.time < speedBoostTime;
	}

	// Get and set might be nicer
	public bool TargetReached ()
	{
		return targetReached;
	}

	// Move towards target angle by the acute angle
	float MoveAngleTowards (float current, float target, float step)
	{
		float temporary = current - target;
		if (Mathf.Abs (temporary) < step) {
			return target;
		} else {
			float signCheckedStep = step;
		
			signCheckedStep *= Mathf.Sign (temporary);
			if (Mathf.Abs (temporary) < 180f) {
				signCheckedStep *= -1f;
			}
			return (current + signCheckedStep + 360f) % 360f;
		}
	}

	// Move one Time.fixedDeltaTime forward
	// Returns true if it has reached either its target or its target orientation
	// This function is public so it can be called by path generating code
	public bool MoveFixedStep (Transform toMove)
	{
		// Target direction
		Vector3 targetDirection = (target - toMove.position);
		// Most acute difference in current and target direction
		float angle = Vector3.Angle (toMove.up, targetDirection);
		float speedDelta = speed * Time.fixedDeltaTime;

		// Should we move and if so, how?
		if (!targetReached && rotationSpeed != 0f) {

			// Turn speed alteration to prevent target circling
			float turnTime = Mathf.Max ((speed * 2f / rotationSpeed) / targetDirection.magnitude, 1f);
			// Normalize targetDirection for direct movement usage
			targetDirection.Normalize ();

			float targetRotation = (Mathf.Atan2 (targetDirection.y, targetDirection.x) * Mathf.Rad2Deg + 630f) % 360f;

			// Movement type code selection
			switch (movementType) {
			case MovementType.InstantRotation:
				// Instant angle correction
				toMove.eulerAngles = new Vector3 (0f, 0f, targetRotation);
				// Move towards position
				toMove.position += toMove.up * speedDelta;	
				break;
				
			case MovementType.MoveWhileRotating:
				// Move towards angle
				toMove.eulerAngles = new Vector3 (0f, 0f, MoveAngleTowards (toMove.rotation.eulerAngles.z, targetRotation, rotationSpeed * Mathf.Rad2Deg * turnTime * Time.fixedDeltaTime));
				// Move while ignoring current rotation
				toMove.position += targetDirection * speedDelta;
				break;
				
			case MovementType.RotateBeforeMoving:
				// Move towards angle
				toMove.eulerAngles = new Vector3 (0f, 0f, MoveAngleTowards (toMove.rotation.eulerAngles.z, targetRotation, rotationSpeed * Mathf.Rad2Deg * turnTime * Time.fixedDeltaTime));
				// Move towards position (conditional)
				if (angle < 0.1f) {
					toMove.position += toMove.up * speedDelta;
				}
				break;
				
			case MovementType.RotateWhileMoving:
				// Move towards angle
				toMove.eulerAngles = new Vector3 (0f, 0f, MoveAngleTowards (toMove.rotation.eulerAngles.z, targetRotation, rotationSpeed * Mathf.Rad2Deg * turnTime * Time.fixedDeltaTime));
				// Move towards position
				toMove.position += toMove.up * speedDelta;
				break;
			}

			// Return wether target has been reached
			speedDelta *= speedDelta;

			if ((target - toMove.position).sqrMagnitude < speedDelta && targets.Count > 0) {
				targets.RemoveFirst ();
				if (targets.First != null) {
					target = targets.First.Value;
				}
			}

			return ((target - toMove.position).sqrMagnitude < speedDelta) || (targets.Count == 0);

		} else {
			if (!stopOnTarget) {
				// Target was reached but the motion should not stop
				toMove.position += toMove.up * speedDelta;
			}
			return true;
		}
	}
	
	// FixedUpdate is called once per physics frame
	void FixedUpdate ()
	{
		if (!Global.paused) {
			if (IsSpeedBoostActive ()) {
				for (int i = 0; i < speedBoostMultiplier; i++) {
					targetReached = MoveFixedStep (transform);
				}
			} else {
				targetReached = MoveFixedStep (transform);
			}

			if (targetReached && stopOnTarget) {
				transform.position = target;
				speedBoostTime = Time.time;
			}
		}
	}
}
