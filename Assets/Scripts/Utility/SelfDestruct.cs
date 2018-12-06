using UnityEngine;
using System.Collections;

public enum SelfDestructTrigger
{
	Undefined,
	Timed,
	OutOfGameView,
	OutOfGameRadius,
	MovingOutOfGameView,
	MovingOutOfGameRadius
}

public enum DestructionType
{
	Silent,
	ScrapShip,
	Default
}

public class SelfDestruct : MonoBehaviour
{
	public SelfDestructTrigger trigger;
	public DestructionType destructType = DestructionType.Silent;
	public float selfDestructTime;

	private bool waiting = false;
	private float timer;

	public void DestroyAttached ()
	{
		if (!waiting) {
			switch (destructType) {
			case DestructionType.Default:
				Instantiate (Global.defaultExplosion, transform.position, transform.rotation);
				break;
			case DestructionType.ScrapShip:
				Destroy (GetComponent<Collider2D> ());
				Instantiate (Global.defaultScrapShipExplosion, transform.position, transform.rotation);
				break;
			}

			waiting = true;
			Destroy (transform.gameObject, 0.5f);
		}
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		timer += Time.fixedDeltaTime;
		if (!waiting)
			switch (trigger) {
			case SelfDestructTrigger.Timed:
				if (timer > selfDestructTime) {
					DestroyAttached ();
				}
				break;

			case SelfDestructTrigger.OutOfGameView:
				if (!Screen.IsInGameView (transform.position)) {
					DestroyAttached ();
				}
				break;

			case SelfDestructTrigger.OutOfGameRadius:
				if (!Screen.IsInGameRadius (transform.position)) {
					DestroyAttached ();
				}
				break;

			case SelfDestructTrigger.MovingOutOfGameView:
				if (Screen.IsMovingOutsideGameView (transform.position, transform.up)) {
					DestroyAttached ();
				}
				break;
			
			case SelfDestructTrigger.MovingOutOfGameRadius:
				if (Screen.IsMovingOutsideGameRadius (transform.position, transform.up)) {
					DestroyAttached ();
				}
				break;
			}
	}
}