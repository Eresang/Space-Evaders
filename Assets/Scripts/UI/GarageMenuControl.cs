using UnityEngine;
using System.Collections;

public class GarageMenuControl : MonoBehaviour
{
	void Awake ()
	{
		Global.paused = true;
		if (ShipContainer.lastContainer)
			ShipContainer.lastContainer.GetComponent<ShipContainer> ().ResetSelection ();
	}
	
	void OnDestroy ()
	{
		// Unpause on destroy
		Global.currentGlobalControl.Unpause (0.2f);
	}

	public void ExitShop ()
	{
		Destroy (this.gameObject);
	}

	public void ExitToMenu ()
	{
		Application.LoadLevelAsync ("Menu");
	}
}
