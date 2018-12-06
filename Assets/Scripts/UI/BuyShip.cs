using UnityEngine;
using System.Collections;

public class BuyShip : MonoBehaviour
{
	public GameObject shipPrefab;

	public void TryBuyShip ()
	{
		// if enough resources, ...
		if (shipPrefab) {
			GameObject newShip = Instantiate (shipPrefab) as GameObject;
			if (ShipContainer.lastContainer) {
				newShip.transform.parent = ShipContainer.lastContainer.transform;
				if (UpgradeControl.lastUpgradeControl) {
					newShip.transform.position = UpgradeControl.lastUpgradeControl.spawnLocations [UpgradeControl.lastUpgradeControl.selectedShip - 1];
					UpgradeControl.lastUpgradeControl.UpdateUI ();
				}
			}
		}
	}
}
