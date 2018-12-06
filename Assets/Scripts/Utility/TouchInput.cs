using UnityEngine;
using System.Collections.Generic;

public class TouchInput : MonoBehaviour
{
	// Touch variables
	private bool isTouching;
	private bool cancelTouch = false;
	private Vector3 firstTouch, lastTouch, tapTouch;
	private float firstTime, lastTime;
	private float doubleTapTime = 0.0f;
	private float timer = -1f;

	// Function to be overridden
	public virtual void OnTouchStart (Vector3 start)
	{
	}

	// Function to be overridden
	public virtual void OnDoubleTap (Vector3 position)
	{
	}

	// Function to be overridden
	public virtual void OnTap (Vector3 position)
	{
	}

	// Function to be overridden
	public virtual void OnPress (Vector3 position)
	{
	}

	// Function to be overridden
	public virtual void OnTouchEnd (Vector3 start, Vector3 end)
	{
	}

	// Function to be overridden
	public virtual void OnTouchCancel ()
	{
	}

	// Function to be overridden
	public virtual bool AskDelayTouch (Vector3 position)
	{
		return true;
	}

	public void CancelAll ()
	{
		cancelTouch = true;
		isTouching = false;
		OnTouchCancel ();
		timer = -1f;
	}
	
	void OnTouch (Vector3 position, bool touch)
	{
		if (touch) {
			if (timer > 0f) {
				if ((position - tapTouch).sqrMagnitude < Screen.screenDragThresholdSqr) {
					OnDoubleTap (position);
					timer = -1f;
				} else {
					OnTap (position);
				}
			} else if (AskDelayTouch (position)) {
				tapTouch = position;
				timer = doubleTapTime;
			} else {
				OnTap (position);
			}
		} else {
			if (timer >= 0f && timer - Time.deltaTime < 0f) {
				OnTap (tapTouch);
			}
			timer -= Time.deltaTime;
		}
	}

	// Update is called once per frame
	void Update ()
	{
		// Right mouse cancels left mouse click-drag-release
		if (isTouching && Input.GetMouseButtonDown (1)) {
			CancelAll ();
		}
		
		// Get directions from left mouse click-drag-release
		if (Input.GetMouseButton (0)) {
			if (!isTouching && !cancelTouch) {
				isTouching = true;
				firstTouch = Input.mousePosition;
				firstTime = Time.time;
				OnTouchStart (Camera.main.ScreenToWorldPoint (firstTouch));
			}
		} else {
			cancelTouch = false;
			if (isTouching) {
				isTouching = false;
				lastTouch = Input.mousePosition;
				lastTime = Time.time;
				if ((Camera.main.ScreenToViewportPoint (lastTouch) - Camera.main.ScreenToViewportPoint (firstTouch)).sqrMagnitude > Screen.screenDragThresholdSqr) {
					// Screen distance covered is long enough for drag input
					OnTouchEnd (Camera.main.ScreenToWorldPoint (firstTouch), Camera.main.ScreenToWorldPoint (lastTouch));
				} else {
					if (lastTime - firstTime < Screen.touchOrPressTime) {
						OnTouch (Camera.main.ScreenToWorldPoint (lastTouch), true);
					} else {
						OnPress (Camera.main.ScreenToWorldPoint (lastTouch));
					}
				}
			} else {
				OnTouch (Camera.main.ScreenToWorldPoint (lastTouch), false);
			}
		}
	}
}
