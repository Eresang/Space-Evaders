using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GarageShopOptionControl : MonoBehaviour
{
	public Image icon;
	public Text nameText;
	public Text descriptionText;
	public Image button;
	public Text costText;

	public void SetOptions (OptionButtonSettings settings)
	{
		icon.sprite = settings.images [0];
		nameText.text = settings.texts [0];
		nameText.color = settings.color;
		descriptionText.text = settings.texts [1];
		descriptionText.color = settings.color;
		button.sprite = settings.images [1];
		costText.text = settings.cost.ToString ();
		costText.color = settings.color;
	}
}
