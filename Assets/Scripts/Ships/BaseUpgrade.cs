using UnityEngine;
using System.Collections;

public class BaseUpgrade : MonoBehaviour
{
	public virtual void Upgrade ()
	{
	}

	public virtual int UpgradeCount ()
	{
		return -1;
	}

	public virtual int MaxUpgrades ()
	{
		return -1;
	}

	public virtual string GetUpgradeText ()
	{
		return MaxUpgrades () == UpgradeCount () ? "Fully upgraded" : "Buy upgrade";
	}
}
