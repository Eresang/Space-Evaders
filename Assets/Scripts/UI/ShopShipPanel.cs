using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum ShipShopOptions
{
	Ships = 0,
	Drones = 1,
	Boost = 2,
	Waypoint = 3
}

public class ShopShipPanel : MonoBehaviour
{
	private ShipControl affectedShip;
	public Image display;
	public Image selection;
	public Image upgrade;
	public Text upgradeText;

	public void DestroyShip ()
	{
		ShipContainer.RemoveSpaceShip (affectedShip.gameObject);
		Destroy (affectedShip.gameObject);
	}

	public void SetSelected (Sprite selectionImage)
	{
		selection.sprite = selectionImage;
	}

	public void BuyUpgrade ()
	{
		if (affectedShip) {
			ShipUpgrades u = affectedShip.gameObject.GetComponent<ShipUpgrades> ();
			if (u) {
				u.UpgradeThisShip ();
			}
		}
	}

	public bool UpgradeAvailable ()
	{
		if (affectedShip) {
			ShipUpgrades u = affectedShip.gameObject.GetComponent<ShipUpgrades> ();
			if (u) {
				return u.AmountOfUpgrades () != u.GetMaxUpgrades ();
			} else {
				return false;
			}
		} else {
			return true;
		}
	}

	public void SetShip (ShipControl ship, Sprite image, bool disregardValidity)
	{
		ShipUpgrades u = null;
		if ((ship) || (disregardValidity)) {
			if (ship) {
				u = ship.gameObject.GetComponent<ShipUpgrades> ();
			}
			this.gameObject.SetActive (true);
			affectedShip = ship;
			display.sprite = image;
		} else {
			this.gameObject.SetActive (false);
		}
		if (u) {
			if (upgrade) {
				upgrade.gameObject.SetActive (u.AmountOfUpgrades () == u.GetMaxUpgrades ());
			}
			if (upgradeText) {
				upgradeText.text = u.AmountOfUpgrades ().ToString ();
				upgradeText.gameObject.SetActive (true);
			}
		} else {
			if (upgrade) {
				upgrade.gameObject.SetActive (false);
			}
			if (upgradeText) {
				upgradeText.gameObject.SetActive (false);
			}
		}
	}

	public ShipShopOptions GetOptions ()
	{
		if (affectedShip) {
			return affectedShip.shipShopOptions;
		} else {
			return ShipShopOptions.Ships;
		}
	}
}
