using UnityEngine;
using System.Collections;

public class WaypointAddition : BaseUpgrade
{
	public int maxWaypoints = 2;
	private Movement m;

	void Awake ()
	{
		m = GetComponent<Movement> ();
		if (!(m))
			Destroy (this);
	}

	public override void Upgrade ()
	{
		m.maxTargets = Mathf.Min (m.maxTargets + 1, maxWaypoints);
	}
	
	public override int UpgradeCount ()
	{
		if (m) {
			return m.maxTargets - 1;
		} else {
			return 0;
		}
	}
	
	public override int MaxUpgrades ()
	{
		return maxWaypoints - 1;
	}

	public override string GetUpgradeText ()
	{
		return MaxUpgrades () == UpgradeCount () ? "Fully upgraded" : "Buy upgrade";
	}
}
