using UnityEngine;
using System.Collections;

public class ShipUpgrades : MonoBehaviour
{
	private BaseUpgrade b;

	void Awake ()
	{
		b = GetComponentInChildren<BaseUpgrade> ();
		if (!(b)) {
			b = GetComponent<BaseUpgrade> ();
		}
	}

	public int AmountOfUpgrades ()
	{
		if (b) {
			return b.UpgradeCount ();
		} else {
			return 0;
		}
	}

	public int GetMaxUpgrades ()
	{
		if (b) {
			return b.MaxUpgrades ();
		} else {
			return 0;
		}
	}

	public void UpgradeThisShip ()
	{
		if (b) {
			b.Upgrade ();
		}
	}

	public string UpgradeText ()
	{
		if (b) {
			return b.GetUpgradeText ();
		} else {
			return "Invalid ship";
		}
	}
}
