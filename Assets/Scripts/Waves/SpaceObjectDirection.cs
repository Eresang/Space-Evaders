using UnityEngine;
using System.Collections;

public static class SpaceObjectDirection
{
	private static float angle, turnSpeed;
	private static float timer = 0f;

	/// <summary>
	/// Sets the angle.
	/// </summary>
	/// <param name="newAngle">New angle.</param>
	public static void SetAngle (float newAngle)
	{
		angle = newAngle;
		timer = Time.time;
	}

	/// <summary>
	/// Sets the angular speed.
	/// </summary>
	/// <param name="fullCircleTime">Full circle time.</param>
	public static void SetAngularSpeed (float fullCircleTime)
	{
		if (fullCircleTime != 0f) {
			turnSpeed = (Mathf.PI * 2f) / fullCircleTime;
		} else {
			turnSpeed = 0f;
		}
	}

	/// <summary>
	/// Gets the angle.
	/// </summary>
	/// <returns>The angle.</returns>
	public static float GetAngle ()
	{
		float newAngle = ((Time.time - timer) * turnSpeed + angle) % (Mathf.PI * 2f);
		SetAngle (newAngle);
		return newAngle;
	}
}
