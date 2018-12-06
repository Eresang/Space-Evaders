using UnityEngine;
using System.Collections;
/*public class Predictor : MonoBehaviour
{
	public BoxCollider2D BoxColliderObject;
	public Movement AssociatedMovement;
	private Vector3 topLeft, topRight, btmLeft, btmRight;
	private Vector3 closestPoint;
	private GameObject lastCollider;

	public float timeLimit = 3f;
	public float interval = 1f;
	private float timer = 0f;

	void Update ()
	{
		Debug.DrawLine (closestPoint - Vector3.left, closestPoint - Vector3.right);
	}

	void FixedUpdate ()
	{
		timer += Time.fixedDeltaTime;
		if (timer > interval) {
			timer = 0f;
			if ((BoxColliderObject) && (AssociatedMovement)) {
				float top = BoxColliderObject.offset.y + (BoxColliderObject.size.y / 2f);
				float btm = BoxColliderObject.offset.y - (BoxColliderObject.size.y / 2f);
				float left = BoxColliderObject.offset.x - (BoxColliderObject.size.x / 2f);
				float right = BoxColliderObject.offset.x + (BoxColliderObject.size.x / 2f);
			
				topLeft = transform.TransformPoint (new Vector3 (left, top, 0f));
				topRight = transform.TransformPoint (new Vector3 (right, top, 0f));
				btmLeft = transform.TransformPoint (new Vector3 (left, btm, 0f));
				btmRight = transform.TransformPoint (new Vector3 (right, btm, 0f));

				// Find closest intersection with spaceobjects and spaceships
				FindIntersections ();
			}
		}
	}

	bool PickTime (out Vector3 result, bool a, bool b, Vector3 start, Vector3 endA, Vector3 endB)
	{
		result = Vector3.zero;
		if (a || b) {
			if ((start - endA).sqrMagnitude < (start - endB).sqrMagnitude) {
				result = endA;
			} else {
				result = endB;
			}
			return true;
		} else {
			return false;
		}
	}

	float ShipIntersect (float lastTime, Vector3 ship1, Vector3 ship2, Vector3 dir1, Vector3 dir2, float speed1, float speed2, GameObject otherShip)
	{
		float resultTime = Mathf.Infinity;

		BoxCollider2D b = otherShip.GetComponentInChildren<BoxCollider2D> ();
		if ((b) && (speed1 != 0f)) {

			float timeDistance = speed1 * speed2 * timeLimit + b.offset.y + BoxColliderObject.size.y;

			if ((ship1 - ship2).sqrMagnitude < timeDistance * timeDistance) {

				float top = b.offset.y + (b.size.y / 2f);
				float btm = b.offset.y - (b.size.y / 2f);
				float left = b.offset.x - (b.size.x / 2f);
				float right = b.offset.x + (b.size.x / 2f);
			
				Vector3 ttopLeft = otherShip.transform.TransformPoint (new Vector3 (left, top, 0f));
				Vector3 ttopRight = otherShip.transform.TransformPoint (new Vector3 (right, top, 0f));
				Vector3 tbtmLeft = otherShip.transform.TransformPoint (new Vector3 (left, btm, 0f));
				Vector3 tbtmRight = otherShip.transform.TransformPoint (new Vector3 (right, btm, 0f));

				Vector3 cP1 = Vector3.zero, cP2 = Vector3.zero, cP3 = Vector3.zero, cP4 = Vector3.zero;

				dir1 *= speed1;
				dir2 *= speed2;

				bool intersect1 = LineLineIntersection (out cP1, topLeft, dir1, ttopLeft, dir2);
				bool intersect2 = LineLineIntersection (out cP2, topRight, dir1, ttopRight, dir2);
				bool intersect3 = LineLineIntersection (out cP3, btmLeft, dir1, tbtmLeft, dir2);
				bool intersect4 = LineLineIntersection (out cP4, btmRight, dir1, tbtmRight, dir2);

				ttopLeft = Vector3.zero;
				if (PickTime (out ttopLeft,
				              PickTime (out tbtmLeft, intersect1, intersect2, ship1, cP1, cP2),
				              PickTime (out tbtmRight, intersect3, intersect4, ship1, cP3, cP4), ship1, tbtmLeft, tbtmRight)) {
					resultTime = (ship1 - ttopLeft).magnitude / speed1;
					Debug.Log (resultTime.ToString ());
					closestPoint = ttopLeft;
				}	
			}
		}
		return resultTime;
	}

	void FindIntersections ()
	{
		int c = ShipContainer.GetSpaceshipCount ();
		GameObject m = ShipContainer.GetMainShip ();

		float ist = timeLimit;
		float com;
		lastCollider = null;

		for (int i = 0; i < c; i++) {
			GameObject g = ShipContainer.GetSpaceShip (i);
			if (g != m) {
				com = ShipIntersect (ist, AssociatedMovement.transform.position, g.transform.position, AssociatedMovement.transform.up, g.transform.up, AssociatedMovement.speed, g.GetComponent<Movement> ().speed, g);
				if (com < ist) {
					ist = com;
					lastCollider = g;
				}
			}
		}

		c = SpaceObjectContainer.GetSpaceObjectCount ();
		for (int i = 0; i < c; i++) {
			GameObject g = SpaceObjectContainer.GetSpaceObject (i);
			com = ShipIntersect (ist, AssociatedMovement.transform.position, g.transform.position, AssociatedMovement.transform.up, g.transform.up, AssociatedMovement.speed, g.GetComponent<Movement> ().speed, g);
			if (com < ist) {
				ist = com;
				lastCollider = g;
			}
		}

		//if (lastCollider)
		//	Debug.Log (lastCollider.name);
	}

	bool LineLineIntersection (out Vector3 intersect, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
	{
		intersect = Vector3.zero;
		Vector3 lineVec3 = linePoint2 - linePoint1;
		Vector3 crossVec1and2 = Vector3.Cross (lineVec1, lineVec2);
		Vector3 crossVec3and2 = Vector3.Cross (lineVec3, lineVec2);

		float s = Vector3.Dot (crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
		
		if ((s >= 0.0f) && (s <= 1.0f)) {
			intersect = linePoint1 + (lineVec1 * s);
			return true;
		} else {
			return false;       
		}
	}
}*/