using UnityEngine;
using System.Collections;

public static class Screen
{
	// View size
	private static Vector3 bottomLeft = new Vector3 (0f, 0f, 0f);
	private static Vector3 topRight = new Vector3 (0f, 0f, 0f);
	private static Vector3 trueBottomLeft = new Vector3 (0f, 0f, 0f);
	private static Vector3 trueTopRight = new Vector3 (0f, 0f, 0f);

	// Allow extra hidden screen and game space
	public static float margin = 0.1f;

	// Allowable spawn range from zero
	private static float gameRadius = 0f;
	private static float gameRadiusSqr = 0f;

	private static float trueGameRadius = 0f;
	private static float trueGameRadiusSqr = 0f;

	// Touch limits
	public static float touchOrPressTime = 0.4f; // if the touch lasts longer than this the touch will not be treated as a touch input but as a press input
	public static float screenDragThresholdSqr = 2f; // % of screen space that needs to be covered before press input becomes a drag input

	public static float GetXBound (bool left)
	{
		return left ? trueBottomLeft.x : trueTopRight.x;
	}

	public static float GetWidth ()
	{
		return Mathf.Abs (trueTopRight.x - trueBottomLeft.x);
	}

	public static float GetYBound (bool top)
	{
		return top ? trueTopRight.y : trueBottomLeft.y;
	}

	public static float GetHeight ()
	{
		return Mathf.Abs (trueTopRight.y - trueBottomLeft.y);
	}

	// Update camera bounds variables
	public static void Update ()
	{
		trueBottomLeft = Camera.main.ViewportToWorldPoint (new Vector3 (0f, 0f, Camera.main.transform.position.z));
		trueTopRight = Camera.main.ViewportToWorldPoint (new Vector3 (1f, 1f, Camera.main.transform.position.z));
		// Sqr(A) = Sqr(B) + Sqr(C)
		trueGameRadiusSqr = (trueTopRight.x - trueBottomLeft.x) * 0.5f;
		trueGameRadiusSqr *= trueGameRadiusSqr;
		trueGameRadius = (trueTopRight.y - trueBottomLeft.y) * 0.5f;
		trueGameRadius *= trueGameRadius;
		trueGameRadiusSqr += trueGameRadius;
		// Sqrt(A)
		trueGameRadius = Mathf.Sqrt (trueGameRadiusSqr);
		// Considering the margin
		bottomLeft = Camera.main.ViewportToWorldPoint (new Vector3 (-margin, -margin, Camera.main.transform.position.z));
		topRight = Camera.main.ViewportToWorldPoint (new Vector3 (margin + 1f, margin + 1f, Camera.main.transform.position.z));
		// Sqr(A) = Sqr(B) + Sqr(C)
		gameRadiusSqr = (topRight.x - bottomLeft.x) * 0.5f;
		gameRadiusSqr *= gameRadiusSqr;
		gameRadius = (topRight.y - bottomLeft.y) * 0.5f;
		gameRadius *= gameRadius;
		gameRadiusSqr += gameRadius;
		// Sqrt(A)
		gameRadius = Mathf.Sqrt (gameRadiusSqr);
	}

	// Get game diameter
	public static float GetDiameter ()
	{
		return gameRadius * 2f;
	}

	// Get radius edge colliding with vector
	public static Vector3 GetRadiusEdge (Vector3 vector)
	{
		float angle = Mathf.Atan2 (vector.y, vector.x);
		return new Vector3 (Mathf.Cos (angle) * gameRadius, Mathf.Sin (angle) * gameRadius, 0f);
	}

	// Get radius edge colliding at angle
	public static Vector3 GetRadiusEdge (float angle)
	{
		return new Vector3 (Mathf.Cos (angle) * gameRadius, Mathf.Sin (angle) * gameRadius, 0f);
	}

	public static bool IsMovingOutsideGameRadius (Vector3 position, Vector3 direction)
	{
		return IsInGameRadius (position) && (Vector3.Dot (Vector3.zero - position, direction) <= 0);
	}

	// Get screen edge colliding with vector
	public static Vector3 GetScreenEdge (Vector3 startPosition, Vector3 vector)
	{
		Vector3 edgePosition = Vector3.zero;
		Vector3 xzOffsets = Vector3.zero;
		
		// Get x offset
		if (vector.y != 0f) {
			xzOffsets.x = startPosition.x - ((vector.x / vector.y) * startPosition.y);
		}
		// Get y offset
		if (vector.x != 0f) {
			xzOffsets.y = startPosition.y - ((vector.y / vector.x) * startPosition.x);
		}
		
		// Get x coordinate
		if (bottomLeft.x != 0f && topRight.x != 0f) {
			if (vector.y == 0f) {
				if (vector.x < 0f) {
					edgePosition.x = bottomLeft.x;
				} else {
					edgePosition.x = topRight.x;
				}
			} else {
				if (vector.y < 0f) {
					// Get intersection point with bottom
					edgePosition.x = (bottomLeft.y - xzOffsets.y) * (vector.x / vector.y);
				} else {
					// Get intersection point with top
					edgePosition.x = (topRight.y - xzOffsets.y) * (vector.x / vector.y);
				}
			}
		}
		
		// Get y coordinate
		if (bottomLeft.y != 0f && topRight.y != 0f) {
			if (vector.x == 0f) {
				if (vector.y < 0f) {
					edgePosition.y = bottomLeft.y;
				} else {
					edgePosition.y = topRight.y;
				}
			} else {
				if (vector.x < 0f) {
					// Get intersection point with left
					edgePosition.y = (bottomLeft.x - xzOffsets.x) * (vector.y / vector.x);
				} else {
					// Get intersection point with right
					edgePosition.y = (topRight.x - xzOffsets.x) * (vector.y / vector.x);
				}
			}
		}
		
		// Clamp within viewing rectangle
		edgePosition.x = Mathf.Clamp (edgePosition.x, bottomLeft.x, topRight.x);
		edgePosition.y = Mathf.Clamp (edgePosition.y, bottomLeft.y, topRight.y);
		return edgePosition;
	}

	public static bool IsInGameView (Vector3 position)
	{
		return (position.x < topRight.x && position.x > bottomLeft.x) && (position.y < topRight.y && position.y > bottomLeft.y);
	}

	public static bool IsInGameViewNoMargin (Vector3 position)
	{
		return (position.x < trueTopRight.x && position.x > trueBottomLeft.x) && (position.y < trueTopRight.y && position.y > trueBottomLeft.y);
	}

	public static Vector3 RandomPositionInView (float margin)
	{
		return new Vector3 (SeededRandom.Range (trueBottomLeft.x + margin, trueTopRight.x - margin), SeededRandom.Range (trueBottomLeft.y + margin, trueTopRight.y - margin), 0f);
	}

	public static bool IsInGameRadius (Vector3 position)
	{
		return (Vector3.zero - position).sqrMagnitude > gameRadiusSqr;
	}

	public static bool IsMovingAwayFromOrigin (Vector3 position, Vector3 direction)
	{
		return Vector3.Dot (Vector3.zero - position, direction) <= 0f;
	}

	public static bool IsMovingOutsideGameViewNoMargin (Vector3 position, Vector3 direction)
	{
		//Debug.Log ((Vector3.zero - position).sqrMagnitude + " > " + gameRadius * gameRadius + "?");
		return !IsInGameViewNoMargin (position) && (Vector3.Dot (Vector3.zero - position, direction) <= 0f);
	}

	public static bool IsMovingOutsideGameView (Vector3 position, Vector3 direction)
	{
		//Debug.Log ((Vector3.zero - position).sqrMagnitude + " > " + gameRadius * gameRadius + "?");
		return !IsInGameView (position) && (Vector3.Dot (Vector3.zero - position, direction) <= 0f);
	}
}
