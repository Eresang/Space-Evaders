using UnityEngine;
using System.Collections;

/// <summary>
/// Rotator type.
/// </summary>
public enum RotatorType
{
	Predefined,
	AttachedA,
	AttachedB
}

/// <summary>
/// Rotator.
/// </summary>
public class Rotator : MonoBehaviour
{
	public RotatorType rotationType;
	public Transform attachedTo;
	public float maxRotation;
	public MinMaxOffsets rotationSpeed;

	private float oldZ = 0f;
	private Vector3 rotateSpeed;

	void Start ()
	{
		rotateSpeed = new Vector3 (SeededRandom.Range (rotationSpeed.minOffset.x, rotationSpeed.maxOffset.x),
		                           SeededRandom.Range (rotationSpeed.minOffset.y, rotationSpeed.maxOffset.y),
		                           SeededRandom.Range (rotationSpeed.minOffset.z, rotationSpeed.maxOffset.z));
	}

	/// <summary>
	/// Gets the new Z rotation.
	/// </summary>
	/// <returns>The new Z rotation.</returns>
	/// <param name="newZ">New z.</param>
	/// <param name="angleYChange">Angle Y change. To allow for change in base model orientation.</param>
	float GetNewZRotation (float newZ, float angleYChange)
	{
		float temporary = oldZ - newZ;
		float sign = Mathf.Sign (temporary);
		temporary = Mathf.Abs (temporary);
		if (temporary < 180f) {
			sign *= -1f;
		} else {
			temporary = -180f + temporary;
		}

		oldZ = newZ;
		return Mathf.LerpAngle (transform.localEulerAngles.y, sign * maxRotation * Mathf.Min (1f, temporary / (45f * Time.fixedDeltaTime)) + angleYChange, 2f * Time.fixedDeltaTime);
	}

	/// <summary>
	/// Update at fixed interval.
	/// </summary>
	void FixedUpdate ()
	{
		if (!Global.paused) {
			switch (rotationType) {
			case RotatorType.Predefined:
				transform.Rotate (rotateSpeed * Time.fixedDeltaTime);
				break;

			case RotatorType.AttachedA:
				if (attachedTo) {
					transform.localEulerAngles = new Vector3 (0f, GetNewZRotation (attachedTo.eulerAngles.z, 270f), 90f);
				}
				break;

			case RotatorType.AttachedB:
				if (attachedTo) {
					transform.localEulerAngles = new Vector3 (270f, GetNewZRotation (attachedTo.eulerAngles.z, 0f), 0f);
				}
				break;
			}
		}
	}
}
