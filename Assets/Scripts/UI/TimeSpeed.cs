using UnityEngine;
using System.Collections;

public class TimeSpeed : MonoBehaviour
{
	public ShipContainer control;

	public void SpeedUp ()
	{
		Time.timeScale = 3.5f;
		if (control) {
			control.CancelAll ();
		}
	}

	public void SpeedDown ()
	{
		Time.timeScale = 1f;
		if (control) {
			control.CancelAll ();
		}
	}
}
