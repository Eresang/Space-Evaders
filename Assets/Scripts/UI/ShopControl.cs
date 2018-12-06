using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShopControl : MonoBehaviour
{
	private CanvasGroup cg;

	public float fadeTime;
	private float timer = 0f;
	
	void Awake ()
	{
		cg = GetComponent<CanvasGroup> ();
		if (!(cg)) {
			Destroy (this);
		}

		// Remove all scrap in view, only ships and effects should be in view after this point
		if (SpaceObjectContainer.lastSpaceObjectContainer) {
			ScrapControl[] sc = SpaceObjectContainer.lastSpaceObjectContainer.GetComponentsInChildren<ScrapControl> ();
			if (sc != null) {
				for (int i = 0; i < sc.Length; i++) {
					sc [i].Destruct ();
				}
			}
		}
	}

	void FixedUpdate ()
	{
		if (timer < fadeTime) {
			timer += Time.fixedDeltaTime;
			cg.alpha = Mathf.Min (1f, timer / fadeTime);
		}
		// Menu at half opacity, pause
		if (cg.alpha >= 0.5f) {
			Global.paused = true;
			ShipContainer.lastContainer.GetComponent<ShipContainer> ().ResetSelection ();
			GetComponentInChildren<UpgradeControl> ().SetShipPositions ();
		}
	}

	void OnDestroy ()
	{
		// Unpause on destroy
		Global.currentGlobalControl.Unpause (0.2f);
	}
}
