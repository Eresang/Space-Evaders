using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadOutShipPanel : MonoBehaviour
{
	private ShipControl affectedShip;
	public Image display;
	public Image selection;

	private Sprite activeSprite, inActiveSprite;

	public void SetShip (ShipControl ship, Sprite image, bool setActive, Sprite active, Sprite inActive)
	{
		this.gameObject.SetActive (setActive);
		affectedShip = ship;
		display.sprite = image;
		activeSprite = active;
		inActiveSprite = inActive;
	}

	public void SetActiveRoster ()
	{
		if (affectedShip) {
			affectedShip.gameObject.SetActive (!affectedShip.gameObject.activeSelf);
		}
		SetSelectionImage ();
	}

	public void SetSelectionImage ()
	{
		if ((affectedShip) && (affectedShip.gameObject.activeSelf)) {
			selection.sprite = activeSprite;
		} else {
			selection.sprite = inActiveSprite;
		}
	}
}
