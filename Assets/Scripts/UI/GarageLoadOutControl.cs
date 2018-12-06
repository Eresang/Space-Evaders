using UnityEngine;
using System.Collections;

public class GarageLoadOutControl : MonoBehaviour
{

	public LoadOutShipPanel[] shipControls;
	public Sprite[] shipImages;
	public Sprite activeSprite;
	public Sprite inActiveSprite;

	public void ChangeShips ()
	{
		int m = ShipContainer.GetSpaceshipCount ();
		for (int i = 0; i < shipControls.Length; i++) {
			if (shipControls [i]) {
				GameObject ship = ShipContainer.GetSpaceShip (i);
				ShipControl sc = null;
				int image = 0;
				if (ship) {
					sc = ship.GetComponent<ShipControl> ();
					image = (int)sc.shipType;
				}
				shipControls [i].SetShip (sc, shipImages [image], i < m, activeSprite, inActiveSprite);
				shipControls [i].SetSelectionImage ();
			}
		}
	}

	// Use this for initialization
	void Start ()
	{
		ChangeShips ();
	}
}
