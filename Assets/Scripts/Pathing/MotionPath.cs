using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Helper class
// Stores position and estimated time of arrival
class PathPoint
{
	public float Timer { get; private set; }
	public Vector3 Position { get; private set; }
	public GameObject Marker { get; set; }
	
	public PathPoint (Vector3 newPosition, float newTimer, GameObject newMarker)
	{
		this.Position = newPosition;
		this.Timer = newTimer;
		this.Marker = newMarker;
	}
}

// Predicts a path when a new target is set
// Updates the path when points along the path are reached
public class MotionPath : MonoBehaviour
{
	// For retrieving speed and rotation
	public Movement movement;
	public Color color;
	public GameObject targetMarkerPrefab;
	private LineRenderer line;

	// Visual
	public bool singleActiveSpan;
	public bool blinkOutsideView;
	private float blinkTimer = 0f;
	private bool blinkUp = false;
	public float blinkTime;

	// Keep track of how long to keep the points
	private LinkedList<PathPoint> linePositions;
	private Vector3 target;
	private Vector3 projectedTarget;
	private float timer;
	private float alpha;
	private bool active = true;
	
	[Range(0.01f, 10f)]
	public float
		calculateStepTime;

	/// <summary>
	/// Stops the pathing.
	/// </summary>
	public void StopPathing ()
	{
		active = false;
		Clear ();
	}

	Vector3 ChangeZ (Vector3 toUpdate)
	{
		return new Vector3 (toUpdate.x, toUpdate.y, transform.position.z);
	}

	void Clear ()
	{
		// Remove old points and markers
		LinkedListNode<PathPoint> point = linePositions.Last;
		while (point != null) {
			if (point.Value.Marker)
				Destroy (point.Value.Marker);
			linePositions.RemoveLast ();
			point = linePositions.Last;
		}
	}

	public void RemoveMarkers ()
	{
		LinkedListNode<PathPoint> point = linePositions.Last;
		while (point != null) {
			if (point.Value.Marker)
				Destroy (point.Value.Marker);
			point = point.Previous;
		}
	}

	// Generate a new line with associated line points
	public void UpdateTarget ()
	{
		Clear ();

		if (movement) {

			Transform simulated = new GameObject ().transform;
			Movement m = simulated.gameObject.AddComponent <Movement> ();
			m.CopyFrom (movement);
			simulated.rotation = movement.gameObject.transform.rotation;
			simulated.position = movement.gameObject.transform.position;
		
			float i = 0f;
			float t = calculateStepTime;

			Vector3 a = m.Target ();
			GameObject marker = null;
		
			while (!m.MoveFixedStep (simulated)) {
			
				i += Time.fixedDeltaTime;
				if (i >= t) {
					if (a != m.Target ()) {
						if (targetMarkerPrefab) {
							marker = Instantiate (targetMarkerPrefab, a, Quaternion.identity) as GameObject;
							marker.transform.parent = Global.currentMarkerContainer.transform;
						}
						a = m.Target ();
						linePositions.AddFirst (new PathPoint (ChangeZ (simulated.position), i, marker));
					} else {
						linePositions.AddFirst (new PathPoint (ChangeZ (simulated.position), i, null));
					}
					t += calculateStepTime;
				}
			}

			projectedTarget = simulated.position;
			projectedTarget += simulated.up * Screen.GetDiameter ();
			target = movement.LastTarget ();

			if ((targetMarkerPrefab) && (linePositions.First != null) && !m.TargetReached ()) {
				marker = Instantiate (targetMarkerPrefab, target, Quaternion.identity) as GameObject;
				marker.transform.parent = Global.currentMarkerContainer.transform;
				linePositions.First.Value.Marker = marker;
			}
		
			// We can destroy this one immediately
			DestroyImmediate (simulated.gameObject);
		}

		Color c = color;
		c.a = 0f;
		SetLineColors (c, c);
		active = true;
		timer = 0f;
		UpdateLine (true);
	}

	// Set line colors externally
	public void SetLineColors (Color start, Color end)
	{
		alpha = start.a;
		if (line) {
			line.SetColors (start, end);
		}
	}

	// Use this for initialization
	void Awake ()
	{
		linePositions = new LinkedList<PathPoint> ();
	}
	
	// Use this for control reference initialization
	void Start ()
	{
		// movement = movement.gameObject.gameObject.GetComponent<Movement> ();
		line = GetComponent<LineRenderer> ();
		if (movement) {
			UpdateTarget ();
		}
	}

	void OnDestroy ()
	{
		Clear ();
	}

	// Remove points reached
	void UpdateLine (bool forceRefresh)
	{
		if ((line) && (movement)) {
			// Get highest valid position
			int before = linePositions.Count;
		
			if (before == 0) {
				if (movement.stopOnTarget && movement.TargetReached ()) {
					line.SetVertexCount (0);
				} else {
					line.SetVertexCount (2);
					line.SetPosition (0, ChangeZ (movement.gameObject.transform.position));
					line.SetPosition (1, ChangeZ (target));
				}
			} else {
				// Remove old points
				LinkedListNode<PathPoint> point = linePositions.Last;
				while (point != null && point.Value.Timer < timer) {
					Destroy (linePositions.Last.Value.Marker);
					linePositions.RemoveLast ();
					point = linePositions.Last;
				}
			
				// Don't change if no points were removed
				if (before != linePositions.Count || forceRefresh) {
					before = linePositions.Count;
					line.SetVertexCount (before + 2);
				
					// Set first point
					line.SetPosition (0, ChangeZ (movement.gameObject.transform.position));
					// Set middle point(s)
					point = linePositions.Last;
					int i = 1;
					while (point != null) {
						line.SetPosition (i, point.Value.Position);
						i++;
						point = point.Previous;
					}
					// Set last point
					line.SetPosition (before + 1, ChangeZ (target));
				}
			}

			if (!movement.stopOnTarget) {
				line.SetPosition (before + 1, ChangeZ (projectedTarget));
			}
		}
	}

	// Update is called once per 'physics frame'
	void FixedUpdate ()
	{
		if (!Global.paused) {
			// Update line positions
			if (movement.IsSpeedBoostActive ()) {
				timer += Time.fixedDeltaTime * movement.speedBoostMultiplier;
			} else {
				timer += Time.fixedDeltaTime;
			}
			UpdateLine (false);

			Color c = color;
			if (!active) {
				alpha = Mathf.Max (0f, alpha - Time.fixedDeltaTime);
				if (singleActiveSpan && alpha <= 0f) {
					Destroy (this.gameObject);
				}
			} else {
				// Perform a fade-out if line is not active or blink under certain conditions
				if ((blinkOutsideView) && blinkTime > 0f) {
					if (!Screen.IsInGameViewNoMargin (transform.position)) {
						if (blinkUp) {
							blinkTimer = Mathf.Min (blinkTimer + Time.fixedDeltaTime, blinkTime);
							blinkUp = blinkTimer != blinkTime;
						} else {
							blinkTimer = Mathf.Max (blinkTimer - Time.fixedDeltaTime, 0f);
							blinkUp = blinkTimer == 0f;
						}
						alpha = blinkTimer / blinkTime;
					} else {
						active = false;
					}
				} else {
					alpha = Mathf.Lerp (alpha, c.a, Time.fixedDeltaTime);
				}
			}

			c.a = alpha;
			SetLineColors (c, c);
		}
	}
}
