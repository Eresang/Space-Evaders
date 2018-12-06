using UnityEngine;
using System.Collections;

public class SpeedBoostEnable : BaseUpgrade
{
	public float speedBoostDuration;
	private Movement m;
	
	void Awake ()
	{
		m = GetComponent<Movement> ();
		if (!(m))
			Destroy (this);
	}

	public override void Upgrade ()
	{
		m.speedBoostDuration = speedBoostDuration;
	}
	
	public override int UpgradeCount ()
	{
		return m.speedBoostDuration != 0f ? 1 : 0;
	}
	
	public override int MaxUpgrades ()
	{
		return 1;
	}

	public override string GetUpgradeText ()
	{
		return MaxUpgrades () == UpgradeCount () ? "Fully upgraded" : "Buy upgrade";
	}
}
